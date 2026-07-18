using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选项 UI 控制器 —— 管理对话分支选项按钮的显示
/// 
/// 职责：
/// 1. 监听 DialogManager 事件
/// 2. 当对话节点有分支选项时，生成并显示选项按钮
/// 3. 当节点切换到无选项节点或对话结束时，隐藏选项面板
/// 4. 处理选项按钮点击，将选择传回 DialogManager
/// 5. 播放点击音效（共用 DialogUIController 的 AudioSource）
/// 
/// 使用方式：
///   挂载到 e_ChoicePanel 节点上
///   在 Inspector 中拖拽绑定选项按钮引用
/// 
/// 设计说明：
///   采用预放 4 个按钮 + 显示/隐藏的方式（而非动态创建/销毁），
///   对于 demo 项目更简洁，也更容易理解
/// </summary>
public class ChoiceUIController : MonoBehaviour
{
    #region UI 元素引用

    [Header("── 预放置的选项按钮（最多 4 个） ──")]
    [Tooltip("按钮数组，在 Inspector 中拖入 btn_choice_0 ~ btn_choice_3")]
    [SerializeField] private Button[] choiceButtons = new Button[4];

    [Header("── 按钮上的文本 ──")]
    [Tooltip("文本数组，对应每个按钮上的 Text 子节点")]
    [SerializeField] private Text[] choiceTexts = new Text[4];

    #endregion

    #region 内部状态

    private DialogManager _manager;

    /// <summary>点击音效（通过 PlayClipAtPoint 播放，不依赖自身 AudioSource）</summary>
    private AudioClip _sfxClick;

    #endregion

    #region 生命周期

    private void Start()
    {
        _manager = DialogManager.Instance;
        if (_manager == null) return;

        // 订阅事件
        _manager.OnNodeChanged += HandleNodeChanged;
        _manager.OnChoicesAvailable += HandleChoicesAvailable;
        _manager.OnDialogEnded += HandleDialogEnded;

        // 给每个按钮绑定点击事件
        // 使用 lambda 闭包捕获索引，这样每个按钮知道自己是"第几个选项"
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
            {
                int index = i; // 必须用局部变量捕获，否则 lambda 里的 i 会是最终值
                choiceButtons[i].onClick.AddListener(() => OnChoiceClicked(index));
            }
        }

        // 获取音频组件（从父级 DialogUIController 共用）
        InitAudio();

        // 初始隐藏
        SetPanelVisible(false);
    }

    private void OnDestroy()
    {
        if (_manager != null)
        {
            _manager.OnNodeChanged -= HandleNodeChanged;
            _manager.OnChoicesAvailable -= HandleChoicesAvailable;
            _manager.OnDialogEnded -= HandleDialogEnded;
        }

        // 清理按钮事件
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
                choiceButtons[i].onClick.RemoveAllListeners();
        }
    }

    #endregion

    #region 音效

    /// <summary>
    /// 初始化音效资源
    /// 只加载 AudioClip，不需要 AudioSource——
    /// 因为 e_ChoicePanel 会被 SetActive(false) 关闭，
    /// 挂在它上面的 AudioSource 会被立即停止，音效来不及播放。
    /// 改用 AudioSource.PlayClipAtPoint，创建临时 AudioSource 播放，不受 SetActive 影响。
    /// </summary>
    private void InitAudio()
    {
        // 尝试从 DialogUIController 获取已加载的音效
        var dialogUI = GetComponentInParent<DialogUIController>(true);
        if (dialogUI != null)
        {
            _sfxClick = dialogUI.SharedClickSFX;
        }

        // 兜底：自行加载
        if (_sfxClick == null)
        {
            _sfxClick = Resources.Load<AudioClip>("Audio/sfx_click");
        }
    }

    /// <summary>
    /// 播放点击音效
    /// 使用 AudioSource.PlayClipAtPoint 而非 PlayOneShot，
    /// 因为点击后 e_ChoicePanel 会立即 SetActive(false)，
    /// 挂在自身的 AudioSource 会被 Unity 立即停止。
    /// PlayClipAtPoint 会创建一个临时 AudioSource，播完自动销毁，不受面板开关影响。
    /// </summary>
    private void PlayClickSound()
    {
        if (_manager == null || !_manager.Config.enableClickSound || _sfxClick == null)
            return;
        AudioSource.PlayClipAtPoint(_sfxClick, Camera.main.transform.position, _manager.Config.clickSoundVolume);
    }

    #endregion

    #region 事件处理

    /// <summary>
    /// 节点切换 → 先隐藏选项面板
    /// 如果新节点有选项，会紧接着触发 OnChoicesAvailable
    /// </summary>
    private void HandleNodeChanged(DialogNode node)
    {
        SetPanelVisible(false);
    }

    /// <summary>
    /// 分支选项可用 → 显示选项按钮
    /// 根据实际选项数量，显示相应数量的按钮，多余的隐藏
    /// </summary>
    private void HandleChoicesAvailable(List<ChoiceData> choices)
    {
        // 设置每个按钮的文本和可见性
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null) continue;

            if (i < choices.Count)
            {
                // 该索引有对应选项 → 显示并设置文本
                choiceButtons[i].gameObject.SetActive(true);
                if (choiceTexts[i] != null)
                    choiceTexts[i].text = choices[i].choiceText;
            }
            else
            {
                // 多余的按钮 → 隐藏
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        // 显示选项面板
        SetPanelVisible(true);
    }

    /// <summary>对话结束 → 隐藏选项面板</summary>
    private void HandleDialogEnded()
    {
        SetPanelVisible(false);
    }

    #endregion

    #region 按钮点击

    /// <summary>
    /// 选项按钮被点击
    /// </summary>
    /// <param name="index">选项索引（0~3）</param>
    private void OnChoiceClicked(int index)
    {
        Debug.Log($"[Dialog] 玩家点击了选项 {index}");

        // 播放点击音效
        PlayClickSound();

        // 通知 DialogManager 处理分支跳转
        _manager.SelectChoice(index);

        // 选择完毕后隐藏选项面板
        SetPanelVisible(false);
    }

    #endregion

    #region 面板显示/隐藏

    private void SetPanelVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    #endregion
}

using UnityEngine;

/// <summary>
/// 演示场景控制器 —— Demo 的入口脚本
/// 
/// 职责：
/// 1. 场景启动时自动或手动触发一段对话
/// 2. 提供按键触发对话的方式（方便演示）
/// 3. 对话结束后可以重新触发（方便反复测试）
/// 
/// 使用方式：
///   挂载到场景中任意空 GameObject 上
///   在 Inspector 中设置要播放的对话 ID
/// 
/// 设计说明：
///   这个脚本只是为了演示对话系统的使用方式
///   在实际游戏中，对话的触发可能来自：
///   - NPC 交互（碰撞/点击触发）
///   - 剧情触发点
///   - UI 按钮
///   - 任务系统
/// </summary>
public class DemoSceneController : MonoBehaviour
{
    [Header("── 演示配置 ──")]

    [Tooltip("要播放的对话 ID\n对应 Resources/Config/ 下的 JSON 文件名\n例如 dialog_001")]
    [SerializeField] private string dialogId = "dialog_001";

    [Tooltip("场景启动时是否自动开始对话\n勾选 = 场景打开后自动播放\n不勾选 = 按空格键手动触发")]
    [SerializeField] private bool autoStartOnPlay = true;

    [Tooltip("触发对话的按键（autoStart 关闭时有效）")]
    [SerializeField] private KeyCode triggerKey = KeyCode.Space;

    [Tooltip("对话面板的预制体引用\n如果场景中已经有 DialogPanel，留空即可")]
    [SerializeField] private GameObject dialogPanelPrefab;

    /// <summary>对话面板实例</summary>
    private GameObject _dialogPanelInstance;

    private void Start()
    {
        // 确保场景中有 DialogManager
        if (DialogManager.Instance == null)
        {
            // 如果场景中没有，自动创建一个
            GameObject managerGo = new GameObject("DialogManager");
            managerGo.AddComponent<DialogManager>();
            managerGo.AddComponent<DialogConfig>();
            Debug.Log("[Dialog] 自动创建了 DialogManager");
        }

        // 如果提供了 Prefab 且场景中还没有实例化，则实例化
        if (dialogPanelPrefab != null && _dialogPanelInstance == null)
        {
            _dialogPanelInstance = Instantiate(dialogPanelPrefab);
            Debug.Log("[Dialog] 实例化了 DialogPanel Prefab");
        }

        // 监听对话结束事件，用于重新启用触发
        DialogManager.Instance.OnDialogEnded += OnDialogEnded;

        // 自动启动
        if (autoStartOnPlay)
        {
            // 延迟一帧启动，确保所有 UI 组件的 Start() 已执行完毕
            Invoke(nameof(TriggerDialog), 0.1f);
        }
    }

    private void Update()
    {
        // 手动触发（对话未进行时，按指定键开始对话）
        if (!autoStartOnPlay && Input.GetKeyDown(triggerKey))
        {
            if (!DialogManager.Instance.IsDialogActive)
            {
                TriggerDialog();
            }
        }
    }

    /// <summary>触发对话</summary>
    private void TriggerDialog()
    {
        Debug.Log($"[Dialog] 触发对话：{dialogId}");
        DialogManager.Instance.StartDialog(dialogId);
    }

    /// <summary>对话结束回调</summary>
    private void OnDialogEnded()
    {
        Debug.Log("[Dialog] 对话已结束。按空格键可重新触发。");
        // 对话结束后允许用空格重新触发
        autoStartOnPlay = false;
    }

    private void OnDestroy()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnDialogEnded -= OnDialogEnded;
        }
    }
}

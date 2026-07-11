using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务系统 Toast 提示 UI 组件
/// 
/// 提供轻量级的操作反馈提示（如"任务已接取"、"目标完成"）。
/// 显示后自动淡出消失。
/// 
/// 使用方式：
///   QuestToastUI.Instance.ShowToast("任务已接取！");
/// 
/// 挂载在 QuestPanel 的 e_Toast 子节点上。
/// </summary>
public class QuestToastUI : MonoBehaviour
{
    /// <summary>全局实例</summary>
    private static QuestToastUI _instance;
    public static QuestToastUI Instance => _instance;

    /// <summary>控制整体透明度的 CanvasGroup</summary>
    private CanvasGroup _canvasGroup;

    /// <summary>提示文字组件</summary>
    private Text _toastText;

    /// <summary>当前正在执行的淡出协程</summary>
    private Coroutine _currentCoroutine;

    private void Awake()
    {
        _instance = this;

        // 获取或添加 CanvasGroup
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 查找文本组件
        Transform txtTf = transform.Find("txt_toast");
        if (txtTf != null)
            _toastText = txtTf.GetComponent<Text>();

        // 加载并设置背景 Sprite
        Image bgImage = GetComponent<Image>();
        if (bgImage != null)
        {
            Sprite toastBg = Resources.Load<Sprite>("UI/bg_toast");
            if (toastBg != null)
            {
                bgImage.sprite = toastBg;
                bgImage.type = Image.Type.Sliced;
            }
        }

        // 初始隐藏
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 兜底：如果 Awake 时 txt_toast 还没创建，Start 时再找一次
    /// （当 QuestUIController 先创建子节点再 AddComponent 时，Awake 就能找到；
    ///   这里作为防御性编程保留）
    /// </summary>
    private void Start()
    {
        if (_toastText == null)
        {
            Transform txtTf = transform.Find("txt_toast");
            if (txtTf != null)
                _toastText = txtTf.GetComponent<Text>();
        }
    }

    /// <summary>
    /// 显示 Toast 提示
    /// </summary>
    /// <param name="message">提示文字</param>
    /// <param name="duration">显示时长（秒），-1 使用配置默认值</param>
    public void ShowToast(string message, float duration = -1f)
    {
        if (_toastText != null)
            _toastText.text = message;

        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        float showDuration = duration > 0 ? duration :
            (QuestManager.Instance?.Config?.ToastDuration ?? 1.5f);
        float fadeDuration = QuestManager.Instance?.Config?.ToastFadeDuration ?? 0.3f;

        _currentCoroutine = StartCoroutine(ToastRoutine(showDuration, fadeDuration));
    }

    /// <summary>Toast 显示 → 等待 → 淡出</summary>
    private IEnumerator ToastRoutine(float showDuration, float fadeDuration)
    {
        _canvasGroup.alpha = 1;
        yield return new WaitForSeconds(showDuration);

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = 1 - (elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0;
        _currentCoroutine = null;
    }
}

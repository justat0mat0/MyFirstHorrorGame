using UnityEngine;

/// <summary>
/// 对话系统 —— 配置参数
/// 
/// 挂载到 DialogManager 同一个 GameObject 上
/// 所有参数都可在 Inspector 面板中实时调整
/// 所有参数都有合理的默认值，不需要额外配置即可正常运行
/// </summary>
public class DialogConfig : MonoBehaviour
{
    [Header("打字机效果")]
    [Tooltip("每秒显示的字符数。越大 = 打字越快。建议 20~50")]
    [Range(5, 100)]
    public int charsPerSecond = 30;

    [Tooltip("打字完成后，继续箭头闪烁的间隔时间（秒）")]
    [Range(0.1f, 1f)]
    public float arrowBlinkInterval = 0.5f;

    [Header("音效")]
    [Tooltip("是否启用打字音效（Resources/Audio/sfx_typing）")]
    public bool enableTypingSound = true;

    [Tooltip("是否启用点击音效（推进对话 + 选择选项，Resources/Audio/sfx_click）")]
    public bool enableClickSound = true;

    [Tooltip("打字音效的音量（0~1）")]
    [Range(0f, 1f)]
    public float typingSoundVolume = 0.3f;

    [Tooltip("点击音效的音量（0~1）")]
    [Range(0f, 1f)]
    public float clickSoundVolume = 0.5f;

    [Header("背景音乐")]
    [Tooltip("是否启用对话 BGM（Resources/Audio/bgm_dialog）")]
    public bool enableBGM = true;

    [Tooltip("BGM 音量（0~1）")]
    [Range(0f, 1f)]
    public float bgmVolume = 0.3f;

    [Header("对话设置")]
    [Tooltip("默认加载的对话 ID（对应 Resources/Config/ 下的 JSON 文件名，不含扩展名）")]
    public string defaultDialogId = "dialog_001";

    [Tooltip("对话面板弹出/关闭的渐变时长（秒）。设为 0 则无动画")]
    [Range(0f, 0.5f)]
    public float panelFadeDuration = 0.2f;

    /// <summary>
    /// 计算单个字符的显示间隔时间（秒）
    /// 例如：charsPerSecond = 30 → 每个字间隔 0.033 秒
    /// </summary>
    public float CharInterval => 1f / Mathf.Max(charsPerSecond, 1);
}

using UnityEngine;

/// <summary>
/// 任务系统配置参数
/// 
/// 挂载在 QuestManager 同一个 GameObject 上。
/// 通过 Inspector 调整参数，运行时通过 QuestManager.Instance.Config 访问。
/// 
/// 参数说明：
///   ToastDuration     — 提示信息显示多久
///   EnableSfx         — 是否播放音效
///   TrackerMaxCount   — 屏幕侧边最多同时追踪几个任务
/// </summary>
public class QuestConfig : MonoBehaviour
{
    [Header("── UI 参数 ──")]

    [Tooltip("Toast 提示显示时长（秒）")]
    [SerializeField] private float _toastDuration = 1.5f;

    [Tooltip("Toast 淡出时长（秒）")]
    [SerializeField] private float _toastFadeDuration = 0.3f;

    [Tooltip("屏幕侧边最多同时追踪的任务数")]
    [SerializeField] private int _trackerMaxCount = 3;

    [Header("── 音效开关 ──")]

    [Tooltip("是否启用音效")]
    [SerializeField] private bool _enableSfx = true;

    // ── 公开属性（只读） ──

    /// <summary>Toast 显示时长</summary>
    public float ToastDuration => _toastDuration;

    /// <summary>Toast 淡出时长</summary>
    public float ToastFadeDuration => _toastFadeDuration;

    /// <summary>最大追踪任务数</summary>
    public int TrackerMaxCount => _trackerMaxCount;

    /// <summary>是否启用音效</summary>
    public bool EnableSfx => _enableSfx;
}

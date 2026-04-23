using UnityEngine;

namespace VICTORCom
{
    /// <summary>
    /// 在两种颜色之间做平滑 ping-pong，作用于 <see cref="SpriteRenderer.color"/>。
    /// 挂到与 SpriteRenderer 同级的物体上即可；多场景可复用。
    /// 可选在对话或解谜交互 UI 打开时暂停闪动并固定为稳定色。
    /// </summary>
    public class SpriteColorPingPong : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer target;

        [SerializeField] private Color colorDim = new Color(0.55f, 0.55f, 0.55f, 1f);
        [SerializeField] private Color colorBright = Color.white;

        [Tooltip("完整明暗来回一圈所需秒数（暗→亮→暗）")]
        [Min(0.01f)]
        [SerializeField] private float cycleDuration = 2f;

        [Tooltip("勾选后使用 unscaledTime，时间缩放为 0 时仍会变化")]
        [SerializeField] private bool useUnscaledTime;

        [Header("与 UI 共存（可选）")]
        [Tooltip("对话播放中暂停 ping-pong，避免对话时背后仍闪烁")]
        [SerializeField] private bool pauseWhileDialoguePlaying;

        [Tooltip("对话进行中使用的固定颜色（可与 Sprite 初始色一致）")]
        [SerializeField] private Color stableColorWhileDialoguePaused = Color.white;

        [Tooltip("解谜交互面板播放中暂停 ping-pong")]
        [SerializeField] private bool pauseWhilePuzzleInteractPlaying;

        [Tooltip("解谜交互进行中使用的固定颜色")]
        [SerializeField] private Color stableColorWhilePuzzlePaused = Color.white;

        [Tooltip("禁用时是否恢复进入播放前的颜色")]
        [SerializeField] private bool restoreColorOnDisable = true;

        private Color _colorOnEnable;

        private void Awake()
        {
            if (target == null)
                target = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (target != null)
                _colorOnEnable = target.color;
        }

        private void OnDisable()
        {
            if (restoreColorOnDisable && target != null)
                target.color = _colorOnEnable;
        }

        private void Update()
        {
            if (target == null)
                return;

            if (TryApplyPausedStableColor())
                return;

            float tSrc = useUnscaledTime ? Time.unscaledTime : Time.time;
            float period = Mathf.Max(0.01f, cycleDuration);
            float t = Mathf.PingPong(tSrc, period) / period;
            target.color = Color.Lerp(colorDim, colorBright, t);
        }

        /// <summary>若应因全局 UI 暂停闪动，则写入稳定色并返回 true。</summary>
        private bool TryApplyPausedStableColor()
        {
            if (pauseWhileDialoguePlaying &&
                DialogueUIController.Instance != null &&
                DialogueUIController.Instance.IsPlaying)
            {
                target.color = stableColorWhileDialoguePaused;
                return true;
            }

            if (pauseWhilePuzzleInteractPlaying &&
                PuzzleInteractUIController.Instance != null &&
                PuzzleInteractUIController.Instance.IsPlaying)
            {
                target.color = stableColorWhilePuzzlePaused;
                return true;
            }

            return false;
        }
    }
}

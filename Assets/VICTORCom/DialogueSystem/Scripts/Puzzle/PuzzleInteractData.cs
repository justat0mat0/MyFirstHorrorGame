using UnityEngine;

namespace VICTORCom
{
    /// <summary>ObjPaint 插图如何适配布局框（每种交互资源可单独配置）。</summary>
    public enum IllustrationLayoutMode
    {
        /// <summary>按 Sprite 原始宽高比在框内缩放，不变形（推荐）。</summary>
        ContainSpriteAspect = 0,
        /// <summary>拉伸填满 Rect，可能变形。</summary>
        Stretch = 1,
        /// <summary>用自定义宽高比（宽/高）驱动 AspectRatioFitter，再在其中保持 Sprite 比例。</summary>
        FrameCustomAspect = 2
    }

    [System.Serializable]
    public class PuzzleInteractLine
    {
        [TextArea(2, 6)]
        public string text;

        [Header("本句事件 ID（可选，订阅 PuzzleInteractRuntimeEvents）")]
        public string lineBeginEventId;

        public string lineCompleteEventId;
    }

    [CreateAssetMenu(fileName = "PuzzleInteract", menuName = "DialogueInteractTool/PuzzleInteractData", order = 1)]
    public class PuzzleInteractData : ScriptableObject
    {
        [Tooltip("对应 UI 上标题（如物体名）")]
        public string title = string.Empty;

        [Tooltip("对应 ObjPaint 等处的插图，可为空")]
        public Sprite illustration;

        [Header("插图比例（ObjPaint）")]
        [Tooltip("Contain：不变形；Stretch：拉满；FrameCustom：用下方宽高比约束外框")]
        public IllustrationLayoutMode illustrationLayout = IllustrationLayoutMode.ContainSpriteAspect;

        [Tooltip("仅 FrameCustomAspect：外框宽高比 = 宽/高（如 16:9≈1.78，1:1=1）")]
        [Min(0.01f)]
        public float illustrationCustomAspectRatio = 1f;

        public PuzzleInteractLine[] lines;

        [Header("整段事件 ID（可选）")]
        public string interactStartEventId;

        public string interactEndEventId;

        [Header("打字音效（可选，覆盖控制器默认）")]
        public AudioClip typingSoundOverride;
    }
}

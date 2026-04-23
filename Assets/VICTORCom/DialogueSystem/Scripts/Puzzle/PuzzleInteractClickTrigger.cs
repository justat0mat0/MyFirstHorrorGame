using UnityEngine;

namespace VICTORCom
{
    /// <summary>
    /// 鼠标点击本物体（需 Collider2D）时，将 <see cref="PuzzleInteractData"/> 交给
    /// <see cref="PuzzleInteractUIController"/> 显示。若配置了 <see cref="singleInteract"/> 则优先使用；
    /// 否则按 <see cref="CurrentStage"/> 从 <see cref="interactByStage"/> 取值。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PuzzleInteractClickTrigger : MonoBehaviour
    {
        [Tooltip("若设置，则忽略阶段数组，始终播放此资源")]
        [SerializeField] private PuzzleInteractData singleInteract;

        [Tooltip("下标 = 阶段：Element 0 对应阶段 0。与 singleInteract 二选一（single 优先）")]
        [SerializeField] private PuzzleInteractData[] interactByStage;

        public int CurrentStage;

        [Tooltip("若已有解谜交互面板在播放，则忽略本次点击")]
        [SerializeField] private bool skipIfAlreadyPlaying = true;

        private void OnMouseDown()
        {
            if (skipIfAlreadyPlaying &&
                PuzzleInteractUIController.Instance != null &&
                PuzzleInteractUIController.Instance.IsPlaying)
                return;

            PuzzleInteractData data = ResolveData();
            if (data == null)
            {
                Debug.LogWarning("PuzzleInteractClickTrigger: 未解析到 PuzzleInteractData。", this);
                return;
            }

            if (PuzzleInteractUIController.Instance == null)
            {
                Debug.LogWarning("PuzzleInteractClickTrigger: PuzzleInteractUIController 不存在。", this);
                return;
            }

            PuzzleInteractUIController.Instance.StartInteraction(data);
        }

        private PuzzleInteractData ResolveData()
        {
            if (singleInteract != null)
                return singleInteract;

            int stage = CurrentStage;
            if (interactByStage == null || stage < 0 || stage >= interactByStage.Length)
            {
                Debug.LogWarning($"PuzzleInteractClickTrigger: 当前阶段 {stage} 超出 interactByStage 配置范围。", this);
                return null;
            }

            PuzzleInteractData data = interactByStage[stage];
            if (data == null)
                Debug.LogWarning($"PuzzleInteractClickTrigger: 阶段 {stage} 未配置 PuzzleInteractData。", this);
            return data;
        }
    }
}

using UnityEngine;

namespace VICTORCom
{
    /// <summary>
    /// 全局阶段：
    /// 不同阶段可由场景内触发器配合不同的 DialogueData。
    /// 负责根据当前 Stage 选择并启动对话。
    /// 
    /// 剧情事件请交给 StoryEventController 处理。
    /// </summary>
    public class DialogueStageManager : MonoBehaviour
    {

        public static DialogueStageManager Instance { get; private set; }


        [Tooltip("当前阶段（从 0 开始）。对话触发器会用它选择 dialogueByStage 的下标。")]
        [SerializeField] private int currentStage;


        [Tooltip("下标 = 阶段：Element 0 对应阶段 0，依此类推。某阶段不需要对话可留空。")]
        [SerializeField] private DialogueData[] dialogueByStage;


        [Tooltip("若已有对话在播放，则忽略本次点击")]
        [SerializeField] private bool skipIfDialogueAlreadyPlaying = true;



        public int CurrentStage
        {
            get => currentStage;
            set => currentStage = value;
        }





        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }





        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }





        public void TriggerDialogue()
        {

            if (skipIfDialogueAlreadyPlaying &&
                DialogueUIController.Instance != null &&
                DialogueUIController.Instance.IsPlaying)
            {
                return;
            }



            int stage = Instance.CurrentStage;



            if (dialogueByStage == null ||
                stage < 0 ||
                stage >= dialogueByStage.Length)
            {
                Debug.LogWarning(
                    $"DialogueStageManager: 当前阶段 {stage} 超出 dialogueByStage 配置范围。",
                    this
                );

                return;
            }




            DialogueData data = dialogueByStage[stage];



            if (data == null)
            {
                Debug.LogWarning(
                    $"DialogueStageManager: 阶段 {stage} 未配置 DialogueData。",
                    this
                );

                return;
            }




            if (DialogueUIController.Instance == null)
            {
                Debug.LogWarning(
                    "DialogueStageManager: DialogueUIController 不存在。",
                    this
                );

                return;
            }



            DialogueUIController.Instance.StartDialogue(data);

        }





        /// <summary>
        /// 将阶段设为指定值
        /// </summary>
        public void SetStage(int stage)
        {
            currentStage = stage;
        }





        /// <summary>
        /// 阶段 +1
        /// </summary>
        public void AdvanceStage()
        {
            currentStage++;
        }


    }
}
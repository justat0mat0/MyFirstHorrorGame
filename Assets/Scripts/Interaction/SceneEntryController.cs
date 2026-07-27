using UnityEngine;
using Cinemachine;

namespace VICTORCom
{
    public class SceneEntryController : MonoBehaviour
    {

        [Header("对应Camera")]
        public CinemachineVirtualCamera targetCamera;


        [Header("进入时播放的对话")]
        public DialogueData entryDialogue;


        [Header("只播放一次")]
        public bool playOnce = true;


        private bool hasPlayed = false;



        private void Update()
        {

            if (hasPlayed && playOnce)
                return;



            if (targetCamera == null)
                return;



            // 当前Camera是否正在使用
            if (targetCamera.Priority >= 10)
            {
                PlayEntryDialogue();
            }

        }





        private void PlayEntryDialogue()
        {

            if (entryDialogue == null)
                return;


            if (DialogueUIController.Instance == null)
                return;


            if (DialogueUIController.Instance.IsPlaying)
                return;



            DialogueUIController.Instance.StartDialogue(
                entryDialogue
            );


            hasPlayed = true;


            Debug.Log(
                "进入场景，播放Entry Dialogue"
            );

        }

    }
}
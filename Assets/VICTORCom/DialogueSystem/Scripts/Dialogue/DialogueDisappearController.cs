using UnityEngine;
using VICTORCom;


public class DialogueDisappearController : MonoBehaviour
{

    [Header("对话结束事件ID")]
    public string disappearEventID;



    private void OnEnable()
    {

        DialogueRuntimeEvents.EventRaised += OnDialogueEvent;

    }




    private void OnDisable()
    {

        DialogueRuntimeEvents.EventRaised -= OnDialogueEvent;

    }





    private void OnDialogueEvent(
        string eventID,
        DialogueData data,
        int lineIndex)
    {


        if (eventID == disappearEventID)
        {

            gameObject.SetActive(false);

        }

    }

}
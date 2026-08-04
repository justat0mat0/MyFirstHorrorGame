using UnityEngine;


public class WorkStartPopupTrigger : MonoBehaviour
{

    public WorkStartController workStartController;



    public void TriggerWorkStart()
    {

        Debug.Log(
            "触发工作开始Popup"
        );



        if (workStartController != null)
        {

            workStartController.StartWorkSequence();

        }
        else
        {

            Debug.LogWarning(
                "没有绑定 WorkStartController"
            );

        }


    }


}
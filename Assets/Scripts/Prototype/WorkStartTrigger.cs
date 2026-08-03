using UnityEngine;


public class WorkStartTrigger : MonoBehaviour
{

    public WorkStartController workController;


    private void Start()
    {

        Debug.Log(
            "WorkStartTrigger Start"
        );


        if (workController != null)
        {

            workController.StartWorkSequence();

        }
        else
        {

            Debug.LogWarning(
                "WorkController is NULL"
            );

        }


    }


}
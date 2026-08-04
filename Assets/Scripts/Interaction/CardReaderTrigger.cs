using UnityEngine;


public class CardReaderTrigger : MonoBehaviour
{

    [Header("员工卡")]
    public GameObject employeeCard;


    [Header("刷卡完成控制")]
    public WorkReadyController workReadyController;


    [Header("检测距离")]
    public float checkDistance = 0.5f;



    private bool finished = false;



    private void Update()
    {

        if (finished)
            return;


        if (employeeCard == null)
            return;



        float distance =
            Vector3.Distance(
                employeeCard.transform.position,
                transform.position
            );



        if (distance <= checkDistance)
        {

            FinishCardCheck();

        }

    }





    private void FinishCardCheck()
    {

        finished = true;


        Debug.Log(
            "员工卡刷卡成功"
        );



        if (workReadyController != null)
        {

            workReadyController.CardChecked();

        }


    }

}
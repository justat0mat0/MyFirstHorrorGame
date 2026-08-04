using System.Collections;
using UnityEngine;
using VICTORCom;


public class EmployeeCardEventListener : MonoBehaviour
{

    [Header("员工卡物体")]
    public GameObject internCard;


    [Header("出现设置")]
    public float appearDelay = 0.2f;


    private void OnEnable()
    {
        DialogueRuntimeEvents.EventRaised += OnDialogueEvent;
    }


    private void OnDisable()
    {
        DialogueRuntimeEvents.EventRaised -= OnDialogueEvent;
    }




    private void Start()
    {
        if (internCard != null)
        {
            internCard.SetActive(false);
        }
    }




    private void OnDialogueEvent(
        string eventId,
        DialogueData context,
        int lineIndex
    )
    {

        if (eventId != "get_staff_card")
            return;



        Debug.Log(
            "EmployeeCardEventListener 收到: "
            + eventId
        );



        StartCoroutine(
            ShowCard()
        );



        if (EmployeeCardManager.Instance != null)
        {

            EmployeeCardManager.Instance
                .ObtainInternCard();

        }

    }






    private IEnumerator ShowCard()
    {

        yield return new WaitForSeconds(
            appearDelay
        );


        if (internCard == null)
        {

            Debug.LogWarning(
                "EmployeeCardEventListener: InternCard没有绑定"
            );

            yield break;

        }



        Debug.Log(
            "显示 InternCard"
        );


        internCard.SetActive(true);



        Debug.Log(
            "InternCard active = "
            + internCard.activeSelf
        );

    }

}
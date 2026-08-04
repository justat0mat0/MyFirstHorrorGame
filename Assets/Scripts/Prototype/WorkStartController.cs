using System.Collections;
using UnityEngine;
using DG.Tweening;


public class WorkStartController : MonoBehaviour
{

    [Header("Start Popup")]
    public CanvasGroup workStartPopup;



    [Header("Employee Card")]
    public GameObject employeeCard;

    public GameObject cardReader;



    [Header("Timing")]
    public float popupFadeTime = 0.5f;

    public float popupShowTime = 2f;




    private bool started = false;

    private bool canUseCard = false;





    private void Start()
    {

        Debug.Log(
            "WorkStartController Ready"
        );



        if (employeeCard != null)
        {

            employeeCard.SetActive(false);

        }



        if (cardReader != null)
        {

            cardReader.SetActive(false);

        }


    }








    // WorkStartTrigger调用
    public void StartWorkSequence()
    {

        if (started)
            return;



        started = true;



        Debug.Log(
            "开始营业流程"
        );



        StartCoroutine(
            StartSequence()
        );


    }







    private IEnumerator StartSequence()
    {


        ShowWorkPopup();



        yield return new WaitForSeconds(
            popupShowTime + popupFadeTime * 2
        );



        canUseCard = true;



        Debug.Log(
            "等待玩家按K刷卡"
        );


    }









    private void ShowWorkPopup()
    {

        Debug.Log(
            "显示刷卡提示Popup"
        );



        if (workStartPopup == null)
        {

            Debug.LogWarning(
                "WorkStartPopup没有绑定"
            );

            return;

        }





        // 防止之前状态残留
        workStartPopup.DOKill();



        workStartPopup.gameObject.SetActive(true);



        workStartPopup.alpha = 0f;




        workStartPopup.DOFade(
            1f,
            popupFadeTime
        );




        DOVirtual.DelayedCall(
            popupShowTime,
            () =>
            {

                if (workStartPopup == null)
                    return;



                workStartPopup.DOFade(
                    0f,
                    popupFadeTime
                )
                .OnComplete(() =>
                {

                    workStartPopup.gameObject.SetActive(false);


                });


            });


    }









    private void Update()
    {

        if (!canUseCard)
            return;



        if (Input.GetKeyDown(KeyCode.K))
        {

            Debug.Log(
                "玩家按下K"
            );


            ShowEmployeeCard();

        }


    }









    private void ShowEmployeeCard()
    {

        Debug.Log(
            "显示Work员工卡"
        );



        if (employeeCard == null)
        {

            Debug.LogError(
                "employeeCard没有绑定"
            );

            return;

        }




        employeeCard.SetActive(true);



        Debug.Log(
            "卡active:"
            + employeeCard.activeSelf
        );





        if (cardReader != null)
        {

            cardReader.SetActive(true);


            Debug.Log(
                "刷卡机active:"
                + cardReader.activeSelf
            );

        }


    }


}
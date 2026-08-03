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

        if (employeeCard != null)
            employeeCard.SetActive(false);


        if (cardReader != null)
            cardReader.SetActive(false);


    }



    public void StartWorkSequence()
    {

        if (started)
            return;


        started = true;


        StartCoroutine(StartSequence());

    }



    private IEnumerator StartSequence()
    {

        ShowWorkPopup();


        yield return new WaitForSeconds(
            popupShowTime + popupFadeTime * 2
        );


        // Popup结束后等待刷卡
        canUseCard = true;


        Debug.Log("等待玩家按K刷卡");


    }





    private void ShowWorkPopup()
    {

        if (workStartPopup == null)
            return;


        workStartPopup.gameObject.SetActive(true);

        workStartPopup.alpha = 0;


        workStartPopup.DOFade(
            1,
            popupFadeTime
        );


        DOVirtual.DelayedCall(
            popupShowTime,
            () => {


                workStartPopup.DOFade(
                    0,
                    popupFadeTime
                )
                .OnComplete(() => {

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

            if (EmployeeCardManager.Instance != null &&
               EmployeeCardManager.Instance.HasInternCard())
            {

                ShowEmployeeCard();

            }
            else
            {

                Debug.Log("玩家还没有员工卡");

            }

        }


    }





    private void ShowEmployeeCard()
    {

        Debug.Log("显示员工卡");


        if (employeeCard != null)
            employeeCard.SetActive(true);


        if (cardReader != null)
            cardReader.SetActive(true);


    }


}
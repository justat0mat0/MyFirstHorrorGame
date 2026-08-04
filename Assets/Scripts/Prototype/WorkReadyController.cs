using UnityEngine;
using DG.Tweening;


public class WorkReadyController : MonoBehaviour
{

    [Header("准备完成 Popup")]
    public CanvasGroup readyPopup;



    [Header("员工卡")]
    public GameObject employeeCard;



    [Header("刷卡机")]
    public GameObject cardReader;



    [Header("Business Start")]
    public WorkBusinessStartController businessController;



    [Header("音效")]
    public AudioSource audioSource;

    public AudioClip cardSound;



    [Header("Popup Timing")]
    public float fadeTime = 0.5f;

    public float showTime = 2f;



    private bool finished = false;




    public void CardChecked()
    {

        if (finished)
            return;


        finished = true;



        Debug.Log(
            "员工卡刷卡成功"
        );



        // 播放刷卡音
        if (audioSource != null &&
            cardSound != null)
        {

            audioSource.PlayOneShot(
                cardSound
            );

        }





        // 隐藏员工卡
        if (employeeCard != null)
        {

            employeeCard.SetActive(false);

        }



        // 隐藏刷卡机
        if (cardReader != null)
        {

            cardReader.SetActive(false);

        }





        ShowReadyPopup();

    }







    private void ShowReadyPopup()
    {

        if (readyPopup == null)
        {

            StartBusiness();

            return;

        }



        readyPopup.gameObject.SetActive(true);


        readyPopup.alpha = 0;



        readyPopup.DOFade(
            1,
            fadeTime
        );




        DOVirtual.DelayedCall(
            showTime,
            () =>
            {

                readyPopup.DOFade(
                    0,
                    fadeTime
                )
                .OnComplete(() =>
                {

                    readyPopup.gameObject.SetActive(false);



                    StartBusiness();


                });


            });


    }






    private void StartBusiness()
    {

        Debug.Log(
            "通知正式开始营业"
        );



        if (businessController != null)
        {

            businessController.StartBusiness();

        }
        else
        {

            Debug.LogWarning(
                "WorkReadyController: 没有绑定 WorkBusinessStartController"
            );

        }


    }


}
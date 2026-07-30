using System.Collections;
using UnityEngine;


public class Stage2EndController : MonoBehaviour
{

    [Header("Stage")]
    public GameObject stage2Play;
    public GameObject stage2End;




    [Header("Report Flow")]
    public GameObject performancePopup;

    public ReportPrinterEffect printerEffect;

    public ReportConfirmController confirmController;




    [Header("UI 音效")]
    public AudioSource uiAudio;

    public AudioClip popupSound;




    [Header("Timing")]
    public float popupDelay = 1f;

    public float printerDelay = 0.2f;




    private bool waitingConfirm = false;

    private bool hasShownEnd = false;





    private void Start()
    {

        // 注册打印完成事件
        if (printerEffect != null)
        {
            printerEffect.OnPrintComplete += OnPrinterFinished;
        }




        if (stage2End != null)
        {
            stage2End.SetActive(false);
        }




        if (performancePopup != null)
        {
            performancePopup.SetActive(false);
        }

    }









    private void Update()
    {

        // 测试
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowEnd();
        }




        if (waitingConfirm &&
            Input.GetMouseButtonDown(0))
        {
            ConfirmPopup();
        }

    }









    public void ShowEnd()
    {

        if (hasShownEnd)
            return;



        hasShownEnd = true;




        if (stage2Play != null)
        {
            stage2Play.SetActive(false);
        }





        if (stage2End != null)
        {

            stage2End.SetActive(true);


            StartCoroutine(
                StartReportFlow()
            );

        }

    }









    private IEnumerator StartReportFlow()
    {

        yield return new WaitForSeconds(
            popupDelay
        );




        if (performancePopup != null)
        {

            performancePopup.SetActive(true);

        }




        if (uiAudio != null &&
            popupSound != null)
        {

            uiAudio.PlayOneShot(
                popupSound
            );

        }




        waitingConfirm = true;

    }









    private void ConfirmPopup()
    {

        if (!waitingConfirm)
            return;




        waitingConfirm = false;




        if (performancePopup != null)
        {

            performancePopup.SetActive(false);

        }




        StartCoroutine(
            StartPrinter()
        );

    }









    private IEnumerator StartPrinter()
    {

        yield return new WaitForSeconds(
            printerDelay
        );




        if (printerEffect != null)
        {

            printerEffect.PlayPrint();

        }

    }









    private void OnPrinterFinished()
    {

        Debug.Log(
            "Stage2EndController: 打印完成"
        );




        if (confirmController != null)
        {

            confirmController.ShowConfirmButton();

        }

    }









    private void OnDestroy()
    {

        if (printerEffect != null)
        {

            printerEffect.OnPrintComplete -= OnPrinterFinished;

        }

    }


}
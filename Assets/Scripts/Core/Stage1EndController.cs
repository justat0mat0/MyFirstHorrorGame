using System.Collections;
using UnityEngine;

public class Stage1EndController : MonoBehaviour
{
    [Header("Stage")]
    public GameObject stage1Play;
    public GameObject stage1End;


    [Header("Report Flow")]
    public GameObject performancePopup;
    public ReportPrinterEffect printerEffect;


    [Header("Timing")]
    public float popupDelay = 1f;


    private bool waitingConfirm = false;



    void Start()
    {
        // 初始隐藏结算界面
        if (stage1End != null)
        {
            stage1End.SetActive(false);
        }


        // 初始隐藏绩效提示框
        if (performancePopup != null)
        {
            performancePopup.SetActive(false);
        }
    }



    void Update()
    {
        // 测试进入Stage1End
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowEnd();
        }


        // 等待玩家确认提示
        if (waitingConfirm && Input.GetMouseButtonDown(0))
        {
            ConfirmReport();
        }
    }



    public void ShowEnd()
    {
        // 关闭工作阶段
        if (stage1Play != null)
        {
            stage1Play.SetActive(false);
        }


        // 打开结算阶段
        if (stage1End != null)
        {
            stage1End.SetActive(true);

            StartCoroutine(StartReportFlow());
        }
    }



    IEnumerator StartReportFlow()
    {
        // 等待VCam切换稳定
        yield return new WaitForSeconds(popupDelay);


        // 显示绩效提示框
        if (performancePopup != null)
        {
            performancePopup.SetActive(true);
        }


        // 开始等待玩家点击
        waitingConfirm = true;
    }



    public void ConfirmReport()
    {
        // 防止重复触发
        if (!waitingConfirm)
            return;


        waitingConfirm = false;


        // 隐藏提示框
        if (performancePopup != null)
        {
            performancePopup.SetActive(false);
        }


        // 开始打印
        if (printerEffect != null)
        {
            printerEffect.PlayPrint();
        }
    }
}
using System;
using System.Collections;
using UnityEngine;

public class ReportPrinterEffect : MonoBehaviour
{
    [Header("纸张物件")]
    public Transform paperRoot;

    public SpriteRenderer blankPaper;
    public SpriteRenderer completePaper;


    [Header("打印位置")]
    public Vector3 startPosition;
    public Vector3 endPosition;


    [Header("动画时间")]
    public float moveDuration = 2f;


    [Header("测试播放")]
    public bool playOnStart = false;


    private bool isPrinting = false;


    // 打印完成事件
    public Action OnPrintComplete;



    private void Start()
    {
        // 初始隐藏报告纸
        HidePaper();


        if (playOnStart)
        {
            PlayPrint();
        }
    }



    public void PlayPrint()
    {
        // 防止重复打印
        if (isPrinting)
            return;


        Debug.Log("ReportPrinterEffect: 开始打印");

        StartCoroutine(PrintRoutine());
    }



    private IEnumerator PrintRoutine()
    {
        isPrinting = true;


        // 设置初始位置
        if (paperRoot != null)
        {
            paperRoot.localPosition = startPosition;
        }


        // 打印开始时显示白纸
        SetAlpha(blankPaper, 1f);
        SetAlpha(completePaper, 0f);



        float timer = 0f;


        while (timer < moveDuration)
        {
            timer += Time.deltaTime;


            float t = timer / moveDuration;



            // 纸张移动
            if (paperRoot != null)
            {
                paperRoot.localPosition =
                    Vector3.Lerp(
                        startPosition,
                        endPosition,
                        t
                    );
            }



            // 白纸逐渐消失
            SetAlpha(
                blankPaper,
                1f - t
            );


            // 完整报告逐渐出现
            SetAlpha(
                completePaper,
                t
            );



            yield return null;
        }



        // 最终状态
        if (paperRoot != null)
        {
            paperRoot.localPosition = endPosition;
        }


        SetAlpha(blankPaper, 0f);
        SetAlpha(completePaper, 1f);



        isPrinting = false;


        Debug.Log("ReportPrinterEffect: 打印完成");


        // 通知外部流程
        OnPrintComplete?.Invoke();
    }



    // 初始隐藏纸张
    private void HidePaper()
    {
        SetAlpha(blankPaper, 0f);
        SetAlpha(completePaper, 0f);
    }



    private void SetAlpha(
        SpriteRenderer renderer,
        float alpha
    )
    {
        if (renderer == null)
            return;


        Color color = renderer.color;

        color.a = alpha;

        renderer.color = color;
    }
}
using System;
using System.Collections;
using UnityEngine;

public class ReportPrinterEffect : MonoBehaviour
{
    [Header("打印机整体")]
    public GameObject printerGroup;

    public float printerFadeDuration = 0.5f;



    [Header("纸张物件")]
    public Transform paperRoot;

    public SpriteRenderer blankPaper;
    public SpriteRenderer completePaper;



    [Header("打印遮罩")]
    public GameObject paperMask;



    [Header("打印声音")]
    public AudioSource printerAudio;

    public AudioClip printingSound;



    [Header("打印位置")]
    public Vector3 startPosition;

    public Vector3 endPosition;



    [Header("动画时间")]
    public float printerWaitTime = 0.8f;

    public float moveDuration = 2f;



    [Header("测试播放")]
    public bool playOnStart = false;



    private bool isPrinting = false;



    public Action OnPrintComplete;



    private void Start()
    {
        HidePaper();


        if (printerGroup != null)
        {
            printerGroup.SetActive(false);
        }


        if (playOnStart)
        {
            PlayPrint();
        }
    }





    public void PlayPrint()
    {
        if (isPrinting)
            return;


        StartCoroutine(
            PrinterRoutine()
        );
    }





    private IEnumerator PrinterRoutine()
    {
        isPrinting = true;



        // 打开整体
        if (printerGroup != null)
        {
            printerGroup.SetActive(true);
        }



        // 等待出现
        yield return new WaitForSeconds(
            printerFadeDuration
        );



        // 打印口打开
        if (paperMask != null)
        {
            paperMask.SetActive(true);
        }



        yield return new WaitForSeconds(
            printerWaitTime
        );



        // 出纸
        yield return StartCoroutine(
            PrintRoutine()
        );



        // 整体关闭
        if (printerGroup != null)
        {
            printerGroup.SetActive(false);
        }



        isPrinting = false;



        OnPrintComplete?.Invoke();
    }





    private IEnumerator PrintRoutine()
    {
        if (paperRoot != null)
        {
            paperRoot.localPosition =
                startPosition;
        }



        SetAlpha(
            blankPaper,
            1f
        );


        SetAlpha(
            completePaper,
            0f
        );



        // 纸移动开始
        StartPrinterSound();



        float timer = 0f;



        while (timer < moveDuration)
        {
            timer += Time.deltaTime;


            float t =
                timer / moveDuration;



            if (paperRoot != null)
            {
                paperRoot.localPosition =
                    Vector3.Lerp(
                        startPosition,
                        endPosition,
                        t
                    );
            }



            SetAlpha(
                blankPaper,
                1f - t
            );


            SetAlpha(
                completePaper,
                t
            );



            yield return null;
        }



        if (paperRoot != null)
        {
            paperRoot.localPosition =
                endPosition;
        }



        SetAlpha(
            blankPaper,
            0f
        );


        SetAlpha(
            completePaper,
            1f
        );



        // 纸停止 → 声音停止
        StopPrinterSound();
    }





    private void StartPrinterSound()
    {
        if (printerAudio != null &&
           printingSound != null)
        {
            printerAudio.clip =
                printingSound;


            printerAudio.loop = true;


            printerAudio.Play();


            Debug.Log(
                "打印声音开始"
            );
        }
    }





    private void StopPrinterSound()
    {
        if (printerAudio != null)
        {
            printerAudio.Stop();

            printerAudio.loop = false;


            Debug.Log(
                "打印声音停止"
            );
        }
    }





    private void HidePaper()
    {
        SetAlpha(
            blankPaper,
            0f
        );


        SetAlpha(
            completePaper,
            0f
        );
    }





    private void SetAlpha(
        SpriteRenderer renderer,
        float alpha
    )
    {
        if (renderer == null)
            return;


        Color color =
            renderer.color;


        color.a =
            alpha;


        renderer.color =
            color;
    }
}
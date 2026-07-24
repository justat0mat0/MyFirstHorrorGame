using System.Collections;
using UnityEngine;

public class ReportConfirmController : MonoBehaviour
{
    [Header("确认按钮")]
    public GameObject confirmButton;



    [Header("盖章")]
    public SpriteRenderer stampRenderer;



    [Header("退出按钮")]
    public GameObject exitButton;



    [Header("Stamp 音效")]
    public AudioSource stampAudio;

    public AudioClip stampSound;



    [Header("Stamp 动画")]
    public float stampDuration = 0.2f;


    // 最终显示大小
    public float stampFinalScale = 0.6f;


    // 出现时放大倍率
    public float stampStartMultiplier = 1.3f;



    private void Start()
    {
        // 初始隐藏确认按钮
        if (confirmButton != null)
        {
            confirmButton.SetActive(false);
        }


        // 初始隐藏退出按钮
        if (exitButton != null)
        {
            exitButton.SetActive(false);
        }


        // 初始隐藏印章
        if (stampRenderer != null)
        {
            SetAlpha(
                stampRenderer,
                0f
            );


            stampRenderer.transform.localScale =
                Vector3.one * stampFinalScale;
        }
    }





    // 给 Stage1EndController 调用
    public void ShowConfirmButton()
    {
        if (confirmButton != null)
        {
            confirmButton.SetActive(true);
        }
    }





    // Button OnClick 调用
    public void ConfirmReport()
    {
        if (confirmButton != null)
        {
            confirmButton.SetActive(false);
        }


        StartCoroutine(
            StampRoutine()
        );
    }





    private IEnumerator StampRoutine()
    {
        if (stampRenderer != null)
        {
            Transform stampTransform =
                stampRenderer.transform;



            Vector3 startScale =
                Vector3.one *
                (stampFinalScale *
                 stampStartMultiplier);



            Vector3 endScale =
                Vector3.one *
                stampFinalScale;



            // 初始状态
            stampTransform.localScale =
                startScale;


            SetAlpha(
                stampRenderer,
                0f
            );



            float timer = 0f;



            while (timer < stampDuration)
            {
                timer += Time.deltaTime;


                float t =
                    timer /
                    stampDuration;



                SetAlpha(
                    stampRenderer,
                    t
                );



                stampTransform.localScale =
                    Vector3.Lerp(
                        startScale,
                        endScale,
                        t
                    );



                yield return null;
            }



            SetAlpha(
                stampRenderer,
                1f
            );


            stampTransform.localScale =
                endScale;
        }



        // 播放盖章音效
        if (stampAudio != null &&
           stampSound != null)
        {
            stampAudio.PlayOneShot(
                stampSound
            );
        }



        // 等待反馈
        yield return new WaitForSeconds(
            0.5f
        );



        // 显示退出按钮
        if (exitButton != null)
        {
            exitButton.SetActive(true);
        }
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
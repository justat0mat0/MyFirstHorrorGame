using System.Collections;
using UnityEngine;


public class VerificationWindowController : MonoBehaviour
{

    [Header("审核窗口")]
    public CanvasGroup[] windows;


    [Header("窗口出现间隔")]
    public float delay = 0.2f;


    [Header("淡入时间")]
    public float fadeDuration = 0.3f;


    private bool isOpening = false;



    public void PlayOpenAnimation()
    {

        if (isOpening)
            return;


        isOpening = true;



        // 每次开始前重置窗口状态
        foreach (CanvasGroup window in windows)
        {

            if (window == null)
                continue;


            window.alpha = 0;

            window.gameObject.SetActive(false);

        }



        StartCoroutine(OpenSequence());

    }



    private IEnumerator OpenSequence()
    {

        foreach (CanvasGroup window in windows)
        {

            if (window == null)
                continue;


            window.gameObject.SetActive(true);


            float timer = 0;


            while (timer < fadeDuration)
            {

                timer += Time.deltaTime;


                window.alpha =
                    Mathf.Lerp(
                        0,
                        1,
                        timer / fadeDuration
                    );


                yield return null;

            }


            window.alpha = 1;


            VerificationTextController textController =
                window.GetComponentInChildren<VerificationTextController>();


            if (textController != null)
            {
                textController.StartTyping();
            }


            yield return new WaitForSeconds(delay);

        }


        isOpening = false;

    }

}
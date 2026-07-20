using System.Collections;
using UnityEngine;
using Cinemachine;

public class MainMenuController : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineVirtualCamera stage1Cam;
    public CinemachineVirtualCamera stage2Cam;
    public CinemachineVirtualCamera settingsCam;
    public CanvasGroup settingsCanvas;

    [Header("Title Move")]
    public RectTransform titleCanvas;
    public RectTransform titleStart;
    public RectTransform titleEnd;
    public float titleMoveDuration = 1.5f;



    [Header("Press Any Button Fade")]
    public CanvasGroup pressGroup;
    public float pressFadeDuration = 1f;



    [Header("Menu Group Fade")]
    public CanvasGroup menuGroup;
    public float menuFadeDelay = 1.8f;
    public float menuFadeDuration = 1.2f;



    [Header("Settings")]
    public bool startWithMenuHidden = true;



    private bool started = false;



    void Awake()
    {
        // Title初始位置
        if (titleCanvas != null && titleStart != null)
        {
            titleCanvas.position = titleStart.position;
        }


        // Menu初始隐藏
        if (startWithMenuHidden && menuGroup != null)
        {
            menuGroup.alpha = 0;
            menuGroup.interactable = false;
            menuGroup.blocksRaycasts = false;
        }


        // Press显示
        if (pressGroup != null)
        {
            pressGroup.alpha = 1;
        }


        // 防止Stage1一开始抢镜
        if (stage1Cam != null)
        {
            stage1Cam.Priority = 0;
        }
    }



    void Update()
    {
        if (!started && Input.anyKeyDown)
        {
            started = true;



            // Camera切换
            if (stage1Cam != null)
            {
                stage1Cam.Priority = 20;
            }



            // Title移动
            if (titleCanvas != null && titleEnd != null)
            {
                StartCoroutine(MoveTitle());
            }



            // Press消失
            if (pressGroup != null)
            {
                StartCoroutine(FadeOutPress());
            }



            // Menu出现
            if (menuGroup != null)
            {
                StartCoroutine(FadeInMenu());
            }
        }
    }




    IEnumerator MoveTitle()
    {
        float timer = 0;


        Vector3 startPos = titleCanvas.position;
        Vector3 endPos = titleEnd.position;



        while (timer < titleMoveDuration)
        {
            timer += Time.deltaTime;


            float t = timer / titleMoveDuration;


            titleCanvas.position =
                Vector3.Lerp(
                    startPos,
                    endPos,
                    t
                );


            yield return null;
        }


        titleCanvas.position = endPos;
    }




    IEnumerator FadeOutPress()
    {
        float timer = 0;



        while (timer < pressFadeDuration)
        {
            timer += Time.deltaTime;


            pressGroup.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    timer / pressFadeDuration
                );


            yield return null;
        }


        pressGroup.alpha = 0;
    }





    IEnumerator FadeInMenu()
    {
        yield return new WaitForSeconds(menuFadeDelay);



        float timer = 0;


        menuGroup.interactable = true;
        menuGroup.blocksRaycasts = true;



        while (timer < menuFadeDuration)
        {
            timer += Time.deltaTime;


            menuGroup.alpha =
                Mathf.Lerp(
                    0,
                    1,
                    timer / menuFadeDuration
                );


            yield return null;
        }


        menuGroup.alpha = 1;
    }
    public void EnterStage2()
    {
        if (stage2Cam != null)
        {
            stage2Cam.Priority = 20;
        }
    }
    public void EnterSettings()
    {
        if (settingsCam != null)
        {
            settingsCam.Priority = 30;
        }


        if (settingsCanvas != null)
        {
            settingsCanvas.alpha = 1;
            settingsCanvas.interactable = true;
            settingsCanvas.blocksRaycasts = true;
        }


        // 关闭环境音
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAmbient();
        }
    }
    public void ExitSettings()
    {
        if (settingsCam != null)
        {
            settingsCam.Priority = 0;
        }

        if (stage2Cam != null)
        {
            stage2Cam.Priority = 20;
        }

        if (settingsCanvas != null)
        {
            settingsCanvas.alpha = 0;
            settingsCanvas.interactable = false;
            settingsCanvas.blocksRaycasts = false;
        }
    }
}
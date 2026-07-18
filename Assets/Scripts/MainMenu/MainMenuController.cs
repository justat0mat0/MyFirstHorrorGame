using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{

    public Camera mainCamera;

    public Transform stage0CamPos;
    public Transform stage1CamPos;


    public CanvasGroup pressCanvas;

    public GameObject stage0Canvas;
    public GameObject stage1Canvas;


    public float moveDuration = 2f;


    bool started = false;



    void Start()
    {
        mainCamera.transform.position = stage0CamPos.position;
        mainCamera.transform.rotation = stage0CamPos.rotation;

        stage0Canvas.SetActive(true);
        stage1Canvas.SetActive(false);
    }



    void Update()
    {

        if (!started && Input.anyKeyDown)
        {
            started = true;

            StartCoroutine(StartMenu());
        }

    }



    IEnumerator StartMenu()
    {

        // Press fade
        yield return StartCoroutine(FadeOutPress());


        // 镜头移动
        yield return StartCoroutine(MoveCamera());


        stage0Canvas.SetActive(false);
        stage1Canvas.SetActive(true);

    }



    IEnumerator FadeOutPress()
    {

        float time = 0;


        while (time < 1)
        {
            time += Time.deltaTime;

            pressCanvas.alpha = 1 - time;

            yield return null;
        }

        pressCanvas.alpha = 0;

    }



    IEnumerator MoveCamera()
    {

        float time = 0;


        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;


        while (time < moveDuration)
        {

            time += Time.deltaTime;

            float t = time / moveDuration;


            // 平滑曲线
            t = Mathf.SmoothStep(0, 1, t);


            mainCamera.transform.position =
                Vector3.Lerp(
                    startPos,
                    stage1CamPos.position,
                    t
                );


            mainCamera.transform.rotation =
                Quaternion.Lerp(
                    startRot,
                    stage1CamPos.rotation,
                    t
                );


            yield return null;

        }


    }

}
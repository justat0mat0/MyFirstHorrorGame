using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Stage3EnterController : MonoBehaviour
{

    [Header("Stage3 Title")]
    public CanvasGroup normalTitle;
    public CanvasGroup gothicTitle;


    [Header("Title Fade Settings")]
    public float titleFadeOutDuration = 0.125f;
    public float titleChangeDelay = 0.05f;
    public float titleFadeInDuration = 0.75f;



    [Header("Button Change")]
    public CanvasGroup enterText;
    public CanvasGroup exploreText;


    public float buttonFadeOutDuration = 0.125f;
    public float buttonChangeDelay = 0.05f;
    public float buttonFadeInDuration = 0.3f;



    [Header("Menu Exit")]
    public CanvasGroup menuGroup;


    public float menuFadeDelay = 2f;
    public float menuFadeDuration = 1f;



    [Header("Scene Change")]
    public float sceneLoadDelay = 0.5f;
    public string sceneName = "SampleScene";



    private bool triggered = false;



    public void Enter()
    {
        if (triggered)
            return;


        triggered = true;


        StartCoroutine(Stage3Sequence());
    }





    IEnumerator Stage3Sequence()
    {

        // Title变化
        StartCoroutine(ChangeTitle());


        // ENTER -> EXPLORE
        StartCoroutine(ChangeButton());



        // 等待玩家看到变化
        yield return new WaitForSeconds(menuFadeDelay);



        // Menu消失
        yield return StartCoroutine(FadeOutMenu());



        // 进入Scene前等待
        yield return new WaitForSeconds(sceneLoadDelay);



        SceneManager.LoadScene(sceneName);

    }





    IEnumerator ChangeTitle()
    {

        float timer = 0;



        // Normal Title Fade Out

        while (timer < titleFadeOutDuration)
        {
            timer += Time.deltaTime;


            normalTitle.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    timer / titleFadeOutDuration
                );


            yield return null;
        }


        normalTitle.alpha = 0;



        yield return new WaitForSeconds(titleChangeDelay);



        timer = 0;



        // Gothic Title Fade In

        while (timer < titleFadeInDuration)
        {
            timer += Time.deltaTime;


            gothicTitle.alpha =
                Mathf.Lerp(
                    0,
                    1,
                    timer / titleFadeInDuration
                );


            yield return null;
        }


        gothicTitle.alpha = 1;

    }






    IEnumerator ChangeButton()
    {

        float timer = 0;



        // ENTER Fade Out

        while (timer < buttonFadeOutDuration)
        {
            timer += Time.deltaTime;


            enterText.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    timer / buttonFadeOutDuration
                );


            yield return null;
        }


        enterText.alpha = 0;



        yield return new WaitForSeconds(buttonChangeDelay);



        timer = 0;



        // EXPLORE Fade In

        while (timer < buttonFadeInDuration)
        {
            timer += Time.deltaTime;


            exploreText.alpha =
                Mathf.Lerp(
                    0,
                    1,
                    timer / buttonFadeInDuration
                );


            yield return null;
        }


        exploreText.alpha = 1;

    }






    IEnumerator FadeOutMenu()
    {

        float timer = 0;



        while (timer < menuFadeDuration)
        {
            timer += Time.deltaTime;


            menuGroup.alpha =
                Mathf.Lerp(
                    1,
                    0,
                    timer / menuFadeDuration
                );


            yield return null;
        }


        menuGroup.alpha = 0;


        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;

    }

}
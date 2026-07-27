using System.Collections;
using UnityEngine;


public class ItemFadeOut : MonoBehaviour
{

    public float fadeDuration = 0.5f;


    private SpriteRenderer spriteRenderer;



    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    public void FadeOut()
    {

        StartCoroutine(FadeRoutine());

    }



    private IEnumerator FadeRoutine()
    {

        float timer = 0f;


        Color startColor = spriteRenderer.color;



        while (timer < fadeDuration)
        {

            timer += Time.deltaTime;


            float alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    timer / fadeDuration
                );


            spriteRenderer.color =
                new Color(
                    startColor.r,
                    startColor.g,
                    startColor.b,
                    alpha
                );


            yield return null;

        }



        spriteRenderer.color =
            new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                0f
            );


        gameObject.SetActive(false);

    }

}
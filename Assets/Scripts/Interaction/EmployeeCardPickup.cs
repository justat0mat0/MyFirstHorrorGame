using System.Collections;
using UnityEngine;

public class EmployeeCardPickup : MonoBehaviour
{

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;


    private SpriteRenderer spriteRenderer;


    private bool clicked = false;



    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();


        if (spriteRenderer == null)
        {
            Debug.LogWarning(
                "EmployeeCardPickup: 没有SpriteRenderer"
            );
        }

    }





    private void OnMouseDown()
    {

        if (clicked)
            return;


        clicked = true;


        Debug.Log(
            "点击员工卡"
        );


        StartCoroutine(
            FadeOut()
        );

    }





    private IEnumerator FadeOut()
    {

        if (spriteRenderer == null)
        {

            gameObject.SetActive(false);

            yield break;

        }



        float timer = 0f;


        Color startColor =
            spriteRenderer.color;



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



        Debug.Log(
            "员工卡已隐藏"
        );



        gameObject.SetActive(false);

    }

}
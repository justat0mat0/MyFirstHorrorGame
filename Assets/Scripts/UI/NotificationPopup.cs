using UnityEngine;
using DG.Tweening;


public class NotificationPopup : MonoBehaviour
{

    public static NotificationPopup Instance;



    [Header("弹窗图片")]
    public GameObject popupObject;



    [Header("动画设置")]
    [Tooltip("弹窗出现动画时间")]
    public float appearDuration = 0.3f;


    [Tooltip("弹窗显示多久后开始Fade")]
    public float showDuration = 2f;


    [Tooltip("Fade淡出持续时间")]
    public float fadeDuration = 0.5f;



    [Header("大小设置")]
    [Tooltip("弹窗最终显示大小")]
    public float targetScale = 0.5f;



    private SpriteRenderer spriteRenderer;



    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }



        if (popupObject != null)
        {

            spriteRenderer =
                popupObject.GetComponent<SpriteRenderer>();


            popupObject.SetActive(false);

        }
        else
        {

            Debug.LogWarning(
                "NotificationPopup: popupObject没有绑定",
                this
            );

        }

    }







    public void Show()
    {

        Debug.Log(
            "NotificationPopup Show 被调用"
        );



        if (popupObject == null)
        {

            Debug.LogWarning(
                "NotificationPopup: popupObject为空"
            );

            return;

        }




        popupObject.SetActive(true);



        transform.DOKill();



        //恢复透明度
        if (spriteRenderer != null)
        {

            Color color = spriteRenderer.color;

            color.a = 1f;

            spriteRenderer.color = color;

        }




        transform.localScale = Vector3.zero;



        transform.DOScale(
            targetScale,
            appearDuration
        )
        .SetEase(Ease.OutBack);






        DOVirtual.DelayedCall(
            showDuration,
            () =>
            {

                if (spriteRenderer == null)
                    return;



                spriteRenderer
                    .DOFade(
                        0f,
                        fadeDuration
                    )
                    .OnComplete(() =>
                    {

                        popupObject.SetActive(false);



                        Color reset =
                            spriteRenderer.color;


                        reset.a = 1f;


                        spriteRenderer.color =
                            reset;


                    });


            });


    }


}
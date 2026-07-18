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



    private SpriteRenderer spriteRenderer;



    private void Awake()
    {
        Instance = this;


        spriteRenderer = popupObject.GetComponent<SpriteRenderer>();


        popupObject.SetActive(false);
    }



    public static void Show()
    {
        if (Instance == null)
        {
            Debug.LogWarning("没有找到NotificationPopup");
            return;
        }


        Instance.ShowPopup();
    }



    private void ShowPopup()
    {
        popupObject.SetActive(true);


        //防止重复播放动画
        transform.DOKill();


        //恢复透明度
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;



        //出现动画
        transform.localScale = Vector3.zero;


        transform.DOScale(
            1f,
            appearDuration
        )
        .SetEase(Ease.OutBack);



        //停留后Fade
        DOVirtual.DelayedCall(
            showDuration,
            () =>
            {
                spriteRenderer
                    .DOFade(
                        0f,
                        fadeDuration
                    )
                    .OnComplete(() =>
                    {
                        popupObject.SetActive(false);


                        //方便下一次显示
                        Color reset = spriteRenderer.color;
                        reset.a = 1f;
                        spriteRenderer.color = reset;
                    });
            }
        );
    }
}
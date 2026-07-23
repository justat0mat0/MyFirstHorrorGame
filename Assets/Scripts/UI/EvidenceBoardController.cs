using UnityEngine;
using DG.Tweening;

public class EvidenceBoardController : MonoBehaviour
{
    [Header("Board")]
    public GameObject smallBoard;
    public GameObject bigBoard;


    [Header("Animation")]
    public float openDuration = 0.5f;
    public float closeDuration = 0.3f;


    [Header("Scale")]
    [Tooltip("BigBoard 初始缩放")]
    public float startScale = 0.3f;

    [Tooltip("BigBoard 打开后的缩放")]
    public float endScale = 2.5f;


    private bool isOpen = false;


    void Start()
    {
        // 初始状态
        if (smallBoard != null)
            smallBoard.SetActive(true);

        if (bigBoard != null)
        {
            bigBoard.SetActive(false);
            bigBoard.transform.localScale = Vector3.one * startScale;
        }
    }


    // 点击小调查板
    public void OpenBoard()
    {
        if (isOpen) return;

        isOpen = true;


        // 小板隐藏
        if (smallBoard != null)
            smallBoard.SetActive(false);


        // 大板显示
        if (bigBoard != null)
        {
            bigBoard.SetActive(true);

            // 防止上一次动画影响
            bigBoard.transform.DOKill();


            // 从小放大
            bigBoard.transform.localScale =
                Vector3.one * startScale;


            bigBoard.transform.DOScale(
                Vector3.one * endScale,
                openDuration
            )
            .SetEase(Ease.OutBack);
        }
    }



    // 关闭调查板（以后可以用）
    public void CloseBoard()
    {
        if (!isOpen) return;

        isOpen = false;


        if (bigBoard != null)
        {
            bigBoard.transform.DOKill();

            bigBoard.transform.DOScale(
                Vector3.one * startScale,
                closeDuration
            )
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                bigBoard.SetActive(false);

                if (smallBoard != null)
                    smallBoard.SetActive(true);
            });
        }
    }
}
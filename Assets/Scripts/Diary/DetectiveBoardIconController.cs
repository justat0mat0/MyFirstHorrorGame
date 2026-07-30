using UnityEngine;


public class DetectiveBoardIconController : MonoBehaviour
{

    private CanvasGroup canvasGroup;



    private void Awake()
    {

        canvasGroup = GetComponent<CanvasGroup>();


        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

    }





    private void Start()
    {

        Debug.Log(
            "DetectiveBoardIconController启动"
        );



        if (DiaryManager.Instance != null)
        {

            DiaryManager.Instance.OnDiaryUpdated += ShowIcon;


            Debug.Log(
                "成功订阅Diary事件"
            );



            // 如果之前已经收集过碎片
            if (DiaryManager.Instance.GetCollectedCount() > 0)
            {
                ShowIcon();
            }
            else
            {
                HideIcon();
            }

        }

    }





    private void ShowIcon()
    {

        Debug.Log(
            "收到日记更新，显示侦探板Icon"
        );


        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

    }





    private void HideIcon()
    {

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

    }





    private void OnDestroy()
    {

        if (DiaryManager.Instance != null)
        {
            DiaryManager.Instance.OnDiaryUpdated -= ShowIcon;
        }

    }

}
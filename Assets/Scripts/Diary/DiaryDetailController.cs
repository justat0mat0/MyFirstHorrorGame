using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class DiaryDetailController : MonoBehaviour
{

    public static DiaryDetailController Instance;



    [Header("详情面板")]
    public GameObject panel;



    [Header("日记内容")]
    public Image diaryImage;

    public TMP_Text titleText;

    public TMP_Text descriptionText;





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



        // 初始关闭详情
        if (panel != null)
        {
            panel.SetActive(false);
        }

    }








    /// <summary>
    /// 显示日记详情
    /// </summary>
    public void ShowDiary(
        Sprite image,
        string title,
        string description
    )
    {

        Debug.Log(
            "打开日记详情：" + title
        );



        if (panel != null)
        {
            panel.SetActive(true);
        }




        if (diaryImage != null)
        {
            diaryImage.sprite = image;
        }




        if (titleText != null)
        {
            titleText.text = title;
        }




        if (descriptionText != null)
        {
            descriptionText.text = description;
        }


    }








    /// <summary>
    /// 关闭日记详情
    /// </summary>
    public void CloseDetail()
    {

        Debug.Log(
            "关闭日记详情"
        );



        if (panel != null)
        {
            panel.SetActive(false);
        }



        // 清空内容（避免下次打开残留）
        if (diaryImage != null)
        {
            diaryImage.sprite = null;
        }



        if (titleText != null)
        {
            titleText.text = "";
        }



        if (descriptionText != null)
        {
            descriptionText.text = "";
        }


    }



}
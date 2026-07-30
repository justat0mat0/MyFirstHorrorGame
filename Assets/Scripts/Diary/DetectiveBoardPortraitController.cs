using UnityEngine;


public class DetectiveBoardPortraitController : MonoBehaviour
{

    [Header("人物碎片（对应日记碎片ID）")]
    public GameObject[] portraitParts;



    private void Start()
    {

        if (DiaryManager.Instance != null)
        {

            DiaryManager.Instance.OnDiaryUpdated += RefreshPortrait;


            // 防止重新打开侦探板没有刷新
            RefreshPortrait();

        }

    }





    private void OnDestroy()
    {

        if (DiaryManager.Instance != null)
        {

            DiaryManager.Instance.OnDiaryUpdated -= RefreshPortrait;

        }

    }





    private void RefreshPortrait()
    {

        if (DiaryManager.Instance == null)
            return;



        for (int i = 0; i < portraitParts.Length; i++)
        {

            if (portraitParts[i] == null)
                continue;



            bool unlocked =
                DiaryManager.Instance.HasFragment(i);



            portraitParts[i]
                .SetActive(unlocked);

        }

    }

}
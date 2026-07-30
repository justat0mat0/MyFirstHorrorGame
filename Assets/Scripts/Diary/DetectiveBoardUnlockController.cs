using UnityEngine;


public class DetectiveBoardUnlockController : MonoBehaviour
{

    [Header("首次获得线索后解锁的人物区域")]
    public GameObject characterBoard;



    private void Start()
    {

        if (DiaryManager.Instance != null)
        {

            DiaryManager.Instance.OnDiaryUpdated += CheckUnlock;


            // 防止打开侦探板时没有刷新
            CheckUnlock();

        }

    }





    private void OnDestroy()
    {

        if (DiaryManager.Instance != null)
        {

            DiaryManager.Instance.OnDiaryUpdated -= CheckUnlock;

        }

    }





    private void CheckUnlock()
    {

        if (DiaryManager.Instance == null)
            return;



        bool unlocked =
            DiaryManager.Instance.GetCollectedCount() > 0;



        if (characterBoard != null)
        {

            characterBoard.SetActive(unlocked);

        }

    }

}
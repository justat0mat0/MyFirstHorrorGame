using UnityEngine;


public class DiaryBoardController : MonoBehaviour
{

    public static DiaryBoardController Instance;



    [Header("碎片显示")]
    public GameObject[] fragmentImages;





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


    }







    private void Start()
    {

        Debug.Log(
            "DiaryBoardController启动"
        );


        // 初始刷新一次
        RefreshBoard();

    }








    private void OnEnable()
    {

        // 每次打开侦探板自动刷新
        if (DiaryManager.Instance != null)
        {
            RefreshBoard();
        }

    }








    /// <summary>
    /// 打开侦探板
    /// </summary>
    public void OpenBoard()
    {

        Debug.Log(
            "打开侦探板"
        );


        gameObject.SetActive(true);



        RefreshBoard();


    }









    /// <summary>
    /// 刷新碎片显示状态
    /// </summary>
    public void RefreshBoard()
    {

        Debug.Log(
            "刷新侦探板碎片"
        );



        if (DiaryManager.Instance == null)
        {

            Debug.LogWarning(
                "没有找到DiaryManager"
            );

            return;

        }






        for (int i = 0; i < fragmentImages.Length; i++)
        {

            if (fragmentImages[i] == null)
                continue;




            bool unlocked =
                DiaryManager.Instance.HasFragment(i);




            fragmentImages[i]
                .SetActive(unlocked);




            Debug.Log(
                "碎片 " + (i + 1) + " 状态：" + unlocked
            );


        }


    }









    /// <summary>
    /// 关闭侦探板
    /// </summary>
    public void CloseBoard()
    {

        Debug.Log(
            "关闭侦探板"
        );



        // 关闭详情页
        if (DiaryDetailController.Instance != null)
        {

            DiaryDetailController.Instance.CloseDetail();

        }



        gameObject.SetActive(false);


    }


}
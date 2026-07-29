using UnityEngine;


public class DiaryBoardController : MonoBehaviour
{

    public static DiaryBoardController Instance;


    [Header("ËéÆ¬ÏÔÊ¾")]
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

        Debug.Log("DiaryBoardControllerÆô¶¯");

    }






    /// <summary>
    /// ´ò¿ªÕìÌ½°å
    /// </summary>
    public void OpenBoard()
    {

        Debug.Log("´ò¿ªÕìÌ½°å");


        gameObject.SetActive(true);


        RefreshBoard();


    }






    /// <summary>
    /// Ë¢ĞÂËéÆ¬
    /// </summary>
    public void RefreshBoard()
    {

        Debug.Log("Ë¢ĞÂÕìÌ½°åËéÆ¬");



        if (DiaryManager.Instance == null)
        {

            Debug.LogWarning(
                "Ã»ÓĞÕÒµ½DiaryManager"
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
                "ËéÆ¬ " + i + " ×´Ì¬£º" + unlocked
            );


        }


    }






    /// <summary>
    /// ¹Ø±ÕÕìÌ½°å
    /// </summary>
    public void CloseBoard()
    {

        Debug.Log("¹Ø±ÕÕìÌ½°å");


        gameObject.SetActive(false);


    }


}
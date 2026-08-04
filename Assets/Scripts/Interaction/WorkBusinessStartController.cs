using UnityEngine;


public class WorkBusinessStartController : MonoBehaviour
{

    [Header("侦探板小提示")]
    public GameObject smallBoard;



    [Header("Document Entry")]
    public DocumentEntryController documentEntry;



    private bool started = false;





    public void StartBusiness()
    {

        if (started)
            return;



        started = true;



        Debug.Log(
            "正式开始营业"
        );




        // 开启小侦探板提示
        if (smallBoard != null)
        {

            smallBoard.SetActive(true);


            Debug.Log(
                "SmallBoard出现"
            );

        }
        else
        {

            Debug.LogWarning(
                "没有绑定 SmallBoard"
            );

        }






        // 开启文件并播放滑入
        if (documentEntry != null)
        {

            documentEntry.gameObject.SetActive(true);


            documentEntry.PlayEntryAnimation();


            Debug.Log(
                "DocumentEntry启动"
            );

        }
        else
        {

            Debug.LogWarning(
                "没有绑定 DocumentEntryController"
            );

        }



    }


}
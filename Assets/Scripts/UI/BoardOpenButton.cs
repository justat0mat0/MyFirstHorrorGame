using UnityEngine;

public class BoardOpenButton : MonoBehaviour
{

    [Header("侦探板")]
    public GameObject smallBoard;
    public GameObject bigBoard;


    private bool isOpen = false;



    private void Start()
    {
        Debug.Log("BoardOpenButton Start");

        if (bigBoard != null)
        {
            bigBoard.SetActive(false);
            Debug.Log("BigBoard 初始关闭");
        }
        else
        {
            Debug.Log("BigBoard 没有绑定");
        }
    }



    private void OnMouseDown()
    {

        Debug.Log("点击侦探板");


        if (isOpen)
        {
            CloseBoard();
        }
        else
        {
            OpenBoard();
        }

    }



    private void OpenBoard()
    {

        Debug.Log("执行 OpenBoard");


        isOpen = true;


        if (smallBoard != null)
        {
            smallBoard.SetActive(false);
            Debug.Log("SmallBoard关闭");
        }
        else
        {
            Debug.Log("SmallBoard为空");
        }



        if (bigBoard != null)
        {
            bigBoard.SetActive(true);
            Debug.Log("BigBoard开启");
        }
        else
        {
            Debug.Log("BigBoard为空");
        }

    }



    private void CloseBoard()
    {

        Debug.Log("执行 CloseBoard");


        isOpen = false;


        if (bigBoard != null)
        {
            bigBoard.SetActive(false);
        }


        if (smallBoard != null)
        {
            smallBoard.SetActive(true);
        }

    }

}
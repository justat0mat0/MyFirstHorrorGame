using UnityEngine;
using DG.Tweening;


public class DocumentEntryController : MonoBehaviour
{

    [Header("展开文件")]
    public GameObject menuUnfolded;

    public GameObject allergyUnfolded;



    [Header("移动设置")]
    public float moveDuration = 0.8f;



    private bool arrived = false;




    private void Start()
    {

        if (menuUnfolded != null)
            menuUnfolded.SetActive(false);



        if (allergyUnfolded != null)
            allergyUnfolded.SetActive(false);

    }







    public void PlayEntryAnimation()
    {

        arrived = false;



        // 初始化展开状态
        if (menuUnfolded != null)
            menuUnfolded.SetActive(false);



        if (allergyUnfolded != null)
            allergyUnfolded.SetActive(false);





        // 当前Inspector位置作为起点
        Vector3 startPosition =
            transform.position;



        // 获取摄像机中心位置
        Vector3 targetPosition =
            Camera.main.transform.position;



        // 2D游戏保持Z轴不变
        targetPosition.z =
            startPosition.z;




        // 回到起点
        transform.position =
            startPosition;



        // 移动到镜头中心
        transform.DOMove(
            targetPosition,
            moveDuration
        )
        .SetEase(
            Ease.OutQuad
        )
        .OnComplete(() =>
        {

            arrived = true;


            Debug.Log(
                "DocumentEntry移动到中心完成"
            );

        });


    }








    private void OnMouseDown()
    {

        if (!arrived)
            return;



        OpenDocument();

    }







    private void OpenDocument()
    {

        gameObject.SetActive(false);



        if (menuUnfolded != null)
        {

            menuUnfolded.SetActive(true);

        }



        if (allergyUnfolded != null)
        {

            allergyUnfolded.SetActive(true);

        }



        Debug.Log(
            "文件展开"
        );


    }


}
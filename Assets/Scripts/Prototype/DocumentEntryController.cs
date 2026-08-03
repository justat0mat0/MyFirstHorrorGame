using UnityEngine;
using DG.Tweening;

public class DocumentEntryController : MonoBehaviour
{

    [Header("展开文件")]
    public GameObject menuUnfolded;
    public GameObject allergyUnfolded;


    [Header("滑入设置")]
    public Vector3 moveOffset = new Vector3(0, -3f, 0);

    public float moveDuration = 0.8f;


    private bool arrived = false;



    private void Start()
    {

        if (menuUnfolded != null)
            menuUnfolded.SetActive(false);


        if (allergyUnfolded != null)
            allergyUnfolded.SetActive(false);


        // 记录当前位置
        Vector3 targetPosition = transform.position + moveOffset;


        transform.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                arrived = true;
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
            menuUnfolded.SetActive(true);


        if (allergyUnfolded != null)
            allergyUnfolded.SetActive(true);

    }

}
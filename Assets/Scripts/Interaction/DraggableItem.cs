using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    [Header("拖动设置")]
    public bool canDrag = true;


    private bool isDragging = false;

    private Vector3 offset;

    private Camera mainCamera;


    private Vector3 originalPosition;



    private void Start()
    {
        mainCamera = Camera.main;

        originalPosition = transform.position;
    }



    private void OnMouseDown()
    {
        if (!canDrag)
            return;


        isDragging = true;


        Vector3 mouseWorld =
            mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );


        mouseWorld.z = transform.position.z;


        offset =
            transform.position - mouseWorld;


        Debug.Log(
            "开始拖动物品: " + gameObject.name
        );
    }



    private void OnMouseDrag()
    {
        if (!isDragging)
            return;


        Vector3 mouseWorld =
            mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );


        mouseWorld.z = transform.position.z;


        transform.position =
            mouseWorld + offset;
    }



    private void OnMouseUp()
    {
        if (!isDragging)
            return;


        isDragging = false;


        Debug.Log(
            "放下物品: " + gameObject.name
        );
    }



    // 如果以后需要取消拖动回原位
    public void ReturnToOriginalPosition()
    {
        transform.position =
            originalPosition;
    }
}
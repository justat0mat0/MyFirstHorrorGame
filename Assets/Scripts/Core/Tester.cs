using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour, IDragHandler, IPointerClickHandler,IBeginDragHandler
{
    private Camera mainCam;
    private float zOffset; // 防止物体乱飞
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录物体到相机的距离，保证转换时 Z 轴正确
        zOffset = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = eventData.position;
        mousePos.z = zOffset;
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
        transform.position = worldPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

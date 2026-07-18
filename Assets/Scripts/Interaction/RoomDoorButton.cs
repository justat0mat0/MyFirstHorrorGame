using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomDoorButton : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetRoom;
    [SerializeReference]
    private CinemachineVirtualCamera targetCine;
    // Start is called before the first frame update
    void Start()
    {
        targetCine = targetRoom.transform.Find("CamPos").transform.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 调用 CameraManager 切换相机
        CameraController.Instance.SwitchToCamera(targetCine);
    }
}

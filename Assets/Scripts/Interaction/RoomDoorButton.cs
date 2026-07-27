using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;


public class RoomDoorButton : MonoBehaviour, IPointerClickHandler
{

    public GameObject targetRoom;


    private CinemachineVirtualCamera targetCine;



    private void Start()
    {
        FindCamera();
    }



    private void FindCamera()
    {

        if (targetRoom == null)
        {
            return;
        }



        Transform camPos =
            targetRoom.transform.Find("CamPos");



        if (camPos == null)
        {
            return;
        }



        targetCine =
            camPos.GetComponentInChildren<CinemachineVirtualCamera>();

    }





    public void OnPointerClick(PointerEventData eventData)
    {

        if (targetCine == null)
        {
            FindCamera();
        }



        if (targetCine == null)
        {
            return;
        }



        CameraController.Instance.SwitchToCamera(
            targetCine
        );

    }

}
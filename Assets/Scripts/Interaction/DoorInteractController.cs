using UnityEngine;

public class DoorInteractController : MonoBehaviour
{

    [Header("是否需要员工权限")]
    public bool requireEmployeePermission = true;


    [Header("进入房间")]
    public RoomDoorButton roomDoorButton;



    private void OnMouseDown()
    {
        Debug.Log("DoorHitbox 被点击");

        TryEnterRoom();
    }



    public void TryEnterRoom()
    {

        Debug.Log("尝试进入房间");


        if (requireEmployeePermission)
        {

            if (EmployeePermissionController.Instance == null)
            {
                Debug.LogWarning(
                    "找不到 EmployeePermissionController"
                );

                return;
            }



            bool canEnter =
                EmployeePermissionController.Instance.CanEnterStaffArea();



            Debug.Log(
                "员工区域权限：" + canEnter
            );



            if (!canEnter)
            {
                Debug.Log(
                    "权限不足，无法进入"
                );

                return;
            }

        }



        EnterRoom();

    }





    private void EnterRoom()
    {

        Debug.Log(
            "权限通过，进入房间"
        );


        if (roomDoorButton != null)
        {

            roomDoorButton.OnPointerClick(null);

        }
        else
        {

            Debug.LogWarning(
                "DoorInteractController 没有绑定 RoomDoorButton"
            );

        }

    }

}
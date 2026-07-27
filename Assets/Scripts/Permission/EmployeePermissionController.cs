using UnityEngine;


public class EmployeePermissionController : MonoBehaviour
{

    public static EmployeePermissionController Instance;



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
        Debug.Log("Permission系统启动");
    }

    /// <summary>
    /// 是否拥有正式员工权限
    /// </summary>
    public bool HasEmployeePermission()
    {

        if (InventoryController.Instance == null)
        {

            Debug.LogWarning(
                "没有找到 InventoryController"
            );


            return false;

        }



        return InventoryController.Instance.HasEmployeeCard();

    }








    /// <summary>
    /// 是否可以进入员工区域
    /// </summary>
    public bool CanEnterStaffArea()
    {

        bool result = HasEmployeePermission();



        Debug.Log(
            "员工区域权限：" + result
        );


        return result;

    }








    /// <summary>
    /// 是否可以开始工作
    /// </summary>
    public bool CanStartWork()
    {

        bool result = HasEmployeePermission();



        Debug.Log(
            "工作权限：" + result
        );


        return result;

    }


}
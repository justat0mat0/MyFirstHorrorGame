using System.Collections;
using UnityEngine;


public class EmployeeVerificationController : MonoBehaviour
{

    public VerificationWindowController windowController;


    public float verificationDelay = 3f;



    public void StartVerification()
    {

        Debug.Log("开始身份验证");


        gameObject.SetActive(true);



        if (windowController == null)
        {

            Debug.LogError("没有绑定窗口控制器");

            return;

        }



        StartCoroutine(VerificationProcess());

    }




    private IEnumerator VerificationProcess()
    {

        windowController.PlayOpenAnimation();



        //等待审核流程完成
        yield return new WaitForSeconds(verificationDelay);



        VerificationSuccess();

    }





    private void VerificationSuccess()
    {

        Debug.Log("身份核验成功");



        //显示成功Popup
        if (NotificationPopup.Instance != null)
        {

            NotificationPopup.Instance.Show();

        }
        else
        {

            Debug.LogWarning(
                "没有找到NotificationPopup"
            );

        }




        //确认员工权限
        if (EmployeePermissionController.Instance != null)
        {

            bool permission =
                EmployeePermissionController.Instance
                .CanEnterStaffArea();



            Debug.Log(
                "身份权限确认：" + permission
            );


        }
        else
        {

            Debug.LogWarning(
                "没有找到EmployeePermissionController"
            );

        }


    }


}
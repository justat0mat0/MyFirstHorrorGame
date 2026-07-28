using UnityEngine;
using UnityEngine.EventSystems;


public class TerminalInteractController : MonoBehaviour, IPointerClickHandler
{

    [Header("身份验证UI控制器")]
    public EmployeeVerificationController verificationController;



    public void OnPointerClick(PointerEventData eventData)
    {

        Debug.Log("员工终端被点击");


        if (verificationController == null)
        {
            Debug.LogError(
                "TerminalInteractController: 没有绑定 EmployeeVerificationController"
            );

            return;
        }


        verificationController.StartVerification();

    }

}
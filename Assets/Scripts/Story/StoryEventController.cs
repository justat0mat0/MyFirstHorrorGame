using UnityEngine;
using VICTORCom;


public class StoryEventController : MonoBehaviour
{

    public static StoryEventController Instance;



    [Header("Stage1 NPC")]
    public GameObject staff1;



    [Header("Notification Popup")]
    public NotificationPopup internCardPopup;
    public NotificationPopup employeeCardPopup;




    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }





    private void OnEnable()
    {
        DialogueRuntimeEvents.EventRaised += OnDialogueRuntimeEvent;
    }





    private void OnDisable()
    {
        DialogueRuntimeEvents.EventRaised -= OnDialogueRuntimeEvent;
    }







    private void OnDialogueRuntimeEvent(
        string eventId,
        DialogueData dialogue,
        int lineIndex)
    {

        switch (eventId)
        {


            //========================
            // Stage1
            //========================


            case "staff1_appear":

                ShowStaff1();

                break;




            case "get_staff_card":

                GetInternCard();

                break;





            //========================
            // Stage2
            //========================


            case "get_employee_card":

                GetEmployeeCard();

                break;



            default:

                break;

        }

    }









    private void ShowStaff1()
    {

        Debug.Log("staff1出现");


        if (staff1 == null)
        {
            Debug.LogWarning(
                "StoryEventController: staff1没有绑定"
            );

            return;
        }



        staff1.SetActive(true);

    }









    private void GetInternCard()
    {

        Debug.Log("获得实习员工证");



        //加入背包

        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.AddInternCard();
        }
        else
        {
            Debug.LogWarning(
                "没有找到InventoryController"
            );
        }





        Debug.Log("准备调用internCardPopup");



        if (internCardPopup != null)
        {

            Debug.Log(
                "internCardPopup存在"
            );


            internCardPopup.Show();

        }
        else
        {

            Debug.LogWarning(
                "internCardPopup为空"
            );

        }

    }









    private void GetEmployeeCard()
    {

        Debug.Log("获得正式员工证");



        //加入背包

        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.AddEmployeeCard();
        }
        else
        {
            Debug.LogWarning(
                "没有找到InventoryController"
            );
        }





        Debug.Log("准备调用employeeCardPopup");



        if (employeeCardPopup != null)
        {

            Debug.Log(
                "employeeCardPopup存在"
            );


            employeeCardPopup.Show();

        }
        else
        {

            Debug.LogWarning(
                "employeeCardPopup为空"
            );

        }


    }



}
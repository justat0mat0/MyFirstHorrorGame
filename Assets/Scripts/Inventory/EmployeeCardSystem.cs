using UnityEngine;


public class EmployeeCardController : MonoBehaviour
{

    public static EmployeeCardController Instance;



    [Header("员工证物件")]
    public GameObject employeeCardObject;



    private bool hasCard = false;





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



        if (employeeCardObject != null)
        {
            employeeCardObject.SetActive(false);
        }

    }







    //获得正式员工证

    public void ObtainCard()
    {

        hasCard = true;


        ShowCard();


        Debug.Log(
            "获得正式员工证"
        );

    }








    //显示员工证

    public void ShowCard()
    {

        if (employeeCardObject == null)
        {

            Debug.LogWarning(
                "EmployeeCardController: 没有绑定员工证物件"
            );

            return;

        }



        employeeCardObject.SetActive(true);


        Debug.Log(
            "显示正式员工证"
        );

    }








    //隐藏员工证

    public void HideCard()
    {

        if (employeeCardObject == null)
            return;



        employeeCardObject.SetActive(false);


        Debug.Log(
            "隐藏正式员工证"
        );

    }







    public bool HasCard()
    {

        return hasCard;

    }


}
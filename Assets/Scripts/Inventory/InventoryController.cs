using UnityEngine;


public class InventoryController : MonoBehaviour
{

    public static InventoryController Instance;



    [Header("测试物品")]
    public GameObject internCard;

    public GameObject employeeCard;




    private bool hasInternCard = false;

    private bool hasEmployeeCard = false;




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






    //=========================
    // 实习员工证
    //=========================


    public void AddInternCard()
    {

        hasInternCard = true;


        Debug.Log(
            "获得实习员工证"
        );


        if (internCard != null)
        {
            internCard.SetActive(true);
        }

    }





    public bool HasInternCard()
    {

        return hasInternCard;

    }








    //=========================
    // 正式员工证
    //=========================


    public void AddEmployeeCard()
    {

        hasEmployeeCard = true;


        Debug.Log(
            "获得正式员工证"
        );



        if (employeeCard != null)
        {
            employeeCard.SetActive(true);
        }

    }





    public bool HasEmployeeCard()
    {

        return hasEmployeeCard;

    }



}
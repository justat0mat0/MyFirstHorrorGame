using UnityEngine;


public class EmployeeCardManager : MonoBehaviour
{

    public static EmployeeCardManager Instance;


    private bool hasInternCard = false;



    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;


        DontDestroyOnLoad(gameObject);

    }





    // 获得员工卡
    public void ObtainInternCard()
    {

        hasInternCard = true;


        Debug.Log("获得 Intern 员工卡");

    }





    // 检查是否拥有员工卡
    public bool HasInternCard()
    {

        return hasInternCard;

    }


}
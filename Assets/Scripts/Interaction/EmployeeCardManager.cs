using UnityEngine;

public class EmployeeCardManager : MonoBehaviour
{

    public static EmployeeCardManager Instance;


    private bool hasInternCard = false;



    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;


        // 必须保证这个物体是Hierarchy根节点
        DontDestroyOnLoad(gameObject);


        Debug.Log("EmployeeCardManager 初始化");

    }





    // 获得实习员工证
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





    // 测试用：查看当前状态
    public void DebugCardState()
    {

        Debug.Log(
            "当前员工卡状态：" + hasInternCard
        );

    }


}
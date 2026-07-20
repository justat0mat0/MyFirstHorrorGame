using UnityEngine;

public class Stage1EndController : MonoBehaviour
{
    public GameObject stage1Play;
    public GameObject stage1End;


    void Start()
    {
        // 初始隐藏结算界面
        if (stage1End != null)
        {
            stage1End.SetActive(false);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowEnd();
        }
    }


    public void ShowEnd()
    {
        if (stage1Play != null)
        {
            stage1Play.SetActive(false);
        }


        if (stage1End != null)
        {
            stage1End.SetActive(true);
        }
    }
}
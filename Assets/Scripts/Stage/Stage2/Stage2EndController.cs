using UnityEngine;

public class Stage2EndController : MonoBehaviour
{
    public GameObject stage2Play;
    public GameObject stage2End;


    void Start()
    {
        // 初始隐藏结算界面
        if (stage2End != null)
        {
            stage2End.SetActive(false);
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
        if (stage2Play != null)
        {
            stage2Play.SetActive(false);
        }


        if (stage2End != null)
        {
            stage2End.SetActive(true);
        }
    }
}
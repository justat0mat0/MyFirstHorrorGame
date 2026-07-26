using UnityEngine;

public class Stage2ReportController : MonoBehaviour
{
    [Header("Stage2 Report")]
    public GameObject reportPanel;


    private void Start()
    {
        if (reportPanel != null)
        {
            reportPanel.SetActive(false);
        }
    }


    public void OpenReport()
    {
        if (reportPanel != null)
        {
            reportPanel.SetActive(true);
        }
    }


    public void CloseReport()
    {
        if (reportPanel != null)
        {
            reportPanel.SetActive(false);
        }
    }
}
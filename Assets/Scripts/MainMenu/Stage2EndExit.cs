using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2EndExit : MonoBehaviour
{
    public GameObject stage2End;


    public void ExitStage2()
    {
        ChapterState.EnterChapter(3);

        SceneManager.LoadScene("MainMenu");
    }
}
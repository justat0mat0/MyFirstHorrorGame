using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1EndExit : MonoBehaviour
{

    public GameObject stage1End;



    public void ExitStage1()
    {

        // 记录进入Stage2
        ChapterState.EnterChapter(2);



        // 回MainMenu Scene
        SceneManager.LoadScene("MainMenu");

    }

}
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Stage1EndExit : MonoBehaviour
{
    public GameObject stage1End;


    public void ExitStage1()
    {
        // 清理场景切换前所有Tween
        DOTween.KillAll();


        // 记录进入Stage2
        ChapterState.EnterChapter(2);



        // 回MainMenu Scene
        SceneManager.LoadScene("MainMenu");
    }
}
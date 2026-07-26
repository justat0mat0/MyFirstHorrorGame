using UnityEngine;

public class ChapterEnterButton : MonoBehaviour
{

    public int chapterNumber = 1;

    public MainMenuButton mainMenuButton;



    public void EnterChapter()
    {

        // 设置当前章节
        ChapterState.EnterChapter(chapterNumber);



        // 进入SampleScene
        if (mainMenuButton != null)
        {
            mainMenuButton.StartGame();
        }

    }

}
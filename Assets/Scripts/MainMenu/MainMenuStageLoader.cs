using UnityEngine;

public class MainMenuStageLoader : MonoBehaviour
{
    public GameObject stage1Menu;
    public GameObject stage2Menu;
    public GameObject stage3Menu;


    void Start()
    {
        stage1Menu.SetActive(false);
        stage2Menu.SetActive(false);
        stage3Menu.SetActive(false);


        if (ChapterState.currentChapter == 1)
        {
            stage1Menu.SetActive(true);
        }


        if (ChapterState.currentChapter == 2)
        {
            stage2Menu.SetActive(true);
        }


        if (ChapterState.currentChapter == 3)
        {
            stage3Menu.SetActive(true);
        }


        EnsureBGM();
    }



    void EnsureBGM()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.EnsureBGM();
        }
    }
}
using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject stage1;
    public GameObject stage2;
    public GameObject stage3;


    void Start()
    {
        LoadCurrentStage();
    }


    public void LoadCurrentStage()
    {
        stage1.SetActive(false);
        stage2.SetActive(false);
        stage3.SetActive(false);


        switch (ChapterState.currentChapter)
        {
            case 1:
                stage1.SetActive(true);
                break;

            case 2:
                stage2.SetActive(true);
                break;

            case 3:
                stage3.SetActive(true);
                break;
        }
    }
}
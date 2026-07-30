using UnityEngine;


public class DiaryFragmentButton : MonoBehaviour
{

    public Sprite diaryImage;


    public string diaryTitle;


    [TextArea(3, 10)]
    public string diaryDescription;




    public void OpenDiary()
    {

        if (DiaryDetailController.Instance != null)
        {

            DiaryDetailController.Instance.ShowDiary(
                diaryImage,
                diaryTitle,
                diaryDescription
            );

        }
        else
        {

            Debug.LogWarning(
                "√ª”–’“µΩDiaryDetailController"
            );

        }

    }

}
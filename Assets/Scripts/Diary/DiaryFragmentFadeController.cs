using UnityEngine;
using VICTORCom;


public class DiaryFragmentFadeController : MonoBehaviour
{

    [Header("对应日记碎片ID")]
    [Tooltip("Diary1=0, Diary2=1, Diary3=2 ...")]
    public int fragmentID;



    private void OnEnable()
    {

        PuzzleInteractRuntimeEvents.EventRaised += OnPuzzleEvent;

    }




    private void OnDisable()
    {

        PuzzleInteractRuntimeEvents.EventRaised -= OnPuzzleEvent;

    }





    private void OnPuzzleEvent(
        string eventId,
        PuzzleInteractData data,
        int lineIndex)
    {

        string targetEvent =
            "collect_diary_" +
            (fragmentID + 1).ToString("00");



        if (eventId == targetEvent)
        {

            gameObject.SetActive(false);

        }

    }

}
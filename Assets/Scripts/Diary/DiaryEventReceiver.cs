using UnityEngine;
using VICTORCom;


public class DiaryEventReceiver : MonoBehaviour
{


    private void OnEnable()
    {

        PuzzleInteractRuntimeEvents.EventRaised
            += OnPuzzleEvent;

    }





    private void OnDisable()
    {

        PuzzleInteractRuntimeEvents.EventRaised
            -= OnPuzzleEvent;

    }





    private void OnPuzzleEvent(
        string eventId,
        PuzzleInteractData data,
        int lineIndex)
    {


        switch (eventId)
        {

            case "collect_diary_01":

                DiaryManager.Instance.CollectFragment(0);

                break;



            case "collect_diary_02":

                DiaryManager.Instance.CollectFragment(1);

                break;



            case "collect_diary_03":

                DiaryManager.Instance.CollectFragment(2);

                break;



            case "collect_diary_04":

                DiaryManager.Instance.CollectFragment(3);

                break;



            case "collect_diary_05":

                DiaryManager.Instance.CollectFragment(4);

                break;



            case "collect_diary_06":

                DiaryManager.Instance.CollectFragment(5);

                break;


        }


    }


}
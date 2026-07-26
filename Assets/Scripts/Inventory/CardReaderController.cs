using UnityEngine;
using UnityEngine.Events;


public class CardReaderController : MonoBehaviour
{


    [Header("工作状态")]
    public bool isWorking = false;



    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip cardSound;



    [Header("事件")]
    public UnityEvent onClockIn;
    public UnityEvent onClockOut;





    private void Start()
    {
        Debug.Log(
            "CardReaderController Ready"
        );
    }







    // 点击刷卡机
    private void OnMouseDown()
    {

        SwipeCard();

    }








    public void SwipeCard()
    {

        //检查有没有正式员工证

        if (EmployeeCardController.Instance == null)
        {
            Debug.LogWarning(
                "没有员工证系统"
            );

            return;
        }




        if (!EmployeeCardController.Instance.HasCard())
        {

            Debug.Log(
                "没有正式员工证，无法刷卡"
            );

            return;

        }






        if (!isWorking)
        {

            ClockIn();

        }
        else
        {

            ClockOut();

        }


    }









    private void ClockIn()
    {

        isWorking = true;


        PlayCardSound();


        Debug.Log(
            "员工签到成功"
        );


        onClockIn?.Invoke();


    }









    private void ClockOut()
    {

        isWorking = false;


        PlayCardSound();


        Debug.Log(
            "员工签退成功"
        );


        onClockOut?.Invoke();


    }









    private void PlayCardSound()
    {

        if (audioSource != null &&
           cardSound != null)
        {

            audioSource.PlayOneShot(cardSound);

        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EmployeeCard"))
        {
            Debug.Log("员工证进入刷卡区域");
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EmployeeCard"))
        {
            Debug.Log("员工证离开刷卡区域");
        }
    }


}
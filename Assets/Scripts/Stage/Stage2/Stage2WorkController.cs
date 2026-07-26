using UnityEngine;
using UnityEngine.Events;


public class Stage2WorkController : MonoBehaviour
{
    [Header("工作状态")]
    public bool isWorking = false;


    [Header("工作事件")]
    public UnityEvent onWorkStart;
    public UnityEvent onWorkEnd;



    private void Start()
    {
        Debug.Log("Stage2WorkController Ready");
    }



    // 员工签到后调用
    public void StartWork()
    {
        if (isWorking)
            return;


        isWorking = true;


        Debug.Log("Stage2 工作开始");


        onWorkStart?.Invoke();
    }



    // 员工签退后调用
    public void EndWork()
    {
        if (!isWorking)
            return;


        isWorking = false;


        Debug.Log("Stage2 工作结束");


        onWorkEnd?.Invoke();
    }



    // 给其他系统查询
    public bool IsWorking()
    {
        return isWorking;
    }
}
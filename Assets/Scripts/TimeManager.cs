using UnityEngine;

public class TimeManager : MonoBehaviour
{

    //营业总时间
    public float workTime = 60f;


    //当前剩余时间
    private float timer;


    //是否正在营业
    public bool isWorking = false;



    //游戏开始时执行一次
    void Start()
    {
        StartWork();
    }
    void Update()
    {

        //如果正在营业
        if (isWorking)
        {

            //时间减少
            timer -= Time.deltaTime;


            //显示剩余时间
            Debug.Log("剩余时间：" + timer);



            //时间结束
            if (timer <= 0)
            {
                EndWork();
            }

        }

    }



    //开始营业
    public void StartWork()
    {

        isWorking = true;

        timer = workTime;


        Debug.Log("开始营业");

    }



    //结束营业
    void EndWork()
    {

        isWorking = false;


        Debug.Log("下班了");

    }

}
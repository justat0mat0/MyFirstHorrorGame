using System;
using UnityEngine;


public class DiaryManager : MonoBehaviour
{

    public static DiaryManager Instance;



    [Header("日记碎片数量")]
    public int fragmentCount = 6;



    private bool[] collectedFragments;



    // 日记更新事件
    public event Action OnDiaryUpdated;





    private void Awake()
    {

        if (Instance == null)
        {

            Instance = this;

        }
        else
        {

            Destroy(gameObject);
            return;

        }



        collectedFragments =
            new bool[fragmentCount];

    }








    /// <summary>
    /// 收集日记碎片
    /// </summary>
    public void CollectFragment(int id)
    {

        if (id < 0 || id >= collectedFragments.Length)
        {

            Debug.LogWarning(
                "日记碎片ID错误：" + id
            );

            return;

        }





        if (collectedFragments[id])
        {

            Debug.Log(
                "该日记碎片已经收集：" + id
            );

            return;

        }





        collectedFragments[id] = true;



        Debug.Log(
            "获得日记碎片：" + (id + 1)
        );



        // 通知其他系统（日记板、icon等）
        OnDiaryUpdated?.Invoke();

    }








    /// <summary>
    /// 是否已经收集某碎片
    /// </summary>
    public bool HasFragment(int id)
    {

        if (id < 0 || id >= collectedFragments.Length)
            return false;



        return collectedFragments[id];

    }








    /// <summary>
    /// 当前收集数量
    /// </summary>
    public int GetCollectedCount()
    {

        int count = 0;



        foreach (bool collected in collectedFragments)
        {

            if (collected)
                count++;

        }



        return count;

    }








    /// <summary>
    /// 是否收集完成
    /// </summary>
    public bool AllFragmentsCollected()
    {

        return GetCollectedCount() >= fragmentCount;

    }


}
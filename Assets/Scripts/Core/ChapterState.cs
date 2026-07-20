using UnityEngine;

public static class ChapterState
{
    // 当前章节
    // 1 = Stage1
    // 2 = Stage2
    // 3 = Stage3
    public static int currentChapter = 1;



    // =========================
    // 进入指定章节
    // =========================

    public static void EnterChapter(int chapter)
    {
        currentChapter = chapter;

        Debug.Log("Current Chapter: " + currentChapter);
    }



    // =========================
    // 完成章节并进入下一章
    // =========================

    public static bool CompleteChapter(int chapter)
    {
        // 防止错误章节推进
        if (currentChapter != chapter)
        {
            Debug.Log(
                "Chapter complete failed. Current: "
                + currentChapter
                + " Tried: "
                + chapter
            );

            return false;
        }


        currentChapter++;


        Debug.Log(
            "Chapter completed. Next Chapter: "
            + currentChapter
        );


        return true;
    }



    // =========================
    // 是否是当前章节
    // =========================

    public static bool IsCurrentChapter(int chapter)
    {
        return currentChapter == chapter;
    }



    // =========================
    // 游戏结束判断
    // =========================

    public static bool IsGameFinished()
    {
        // 目前3章
        return currentChapter > 3;
    }
}
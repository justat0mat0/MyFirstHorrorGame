using System;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Demo 项目日志记录器
/// 
/// 功能：
/// - 每次 Play 时清空旧日志，重新记录
/// - 生成 2 个日志文件（在项目根目录 DemoLogs/ 下）：
///   1. DemoLog.log       → 干净版（仅日志正文，方便快速浏览）
///   2. DemoLogStack.log  → 完整版（正文 + 调用栈，方便定位问题）
/// 
/// 使用方式：
///   自动初始化，无需手动调用。Play 即生效。
/// 
/// 排查技巧：
///   运行后打开 DemoLogs/DemoLog.log，搜索 [Dialog] 等标签
///   快速定位某个 demo 模块的运行流程和报错
/// </summary>
public static class DemoLogWriter
{
    private static string s_LogFilePath;
    private static string s_LogFilePathWithStack;
    private static bool s_Initialized;
    private static readonly object s_WriteLock = new object();

    /// <summary>
    /// 初始化日志系统（Play 时自动调用）
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        if (s_Initialized) return;

        // 日志目录 = 项目根目录/DemoLogs/
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        string logDirectory = Path.Combine(projectRoot, "DemoLogs");

        // 确保目录存在
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // 清理旧的日志文件
        CleanupOldLogFiles(logDirectory);

        // 固定路径
        s_LogFilePath = Path.Combine(logDirectory, "DemoLog.log");
        s_LogFilePathWithStack = Path.Combine(logDirectory, "DemoLogStack.log");

        // 创建新日志文件
        try
        {
            string header = $"=== AstroDemos Start: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n\n";
            File.WriteAllText(s_LogFilePath, header);
            File.WriteAllText(s_LogFilePathWithStack, header);
        }
        catch (Exception e)
        {
            Debug.LogError($"[DemoLog] 无法创建日志文件: {e.Message}");
            return;
        }

        // 监听 Unity 日志事件（包括子线程的日志）
        Application.logMessageReceivedThreaded += OnLogMessageReceived;

        // 自动注册退出事件
        Application.quitting += OnApplicationQuitting;

        s_Initialized = true;
        Debug.Log($"[DemoLog] 日志已启动 → {logDirectory}");
    }

    /// <summary>
    /// 游戏退出时自动调用
    /// </summary>
    private static void OnApplicationQuitting()
    {
        Shutdown();
    }

    /// <summary>
    /// 关闭日志系统
    /// </summary>
    public static void Shutdown()
    {
        if (!s_Initialized) return;

        Application.logMessageReceivedThreaded -= OnLogMessageReceived;
        s_Initialized = false;
    }

    /// <summary>
    /// 接收 Unity 日志消息，写入两个文件
    /// </summary>
    private static void OnLogMessageReceived(string logString, string stackTrace, LogType type)
    {
        string prefix = type switch
        {
            LogType.Error => "[ERROR] ",
            LogType.Warning => "[WARN] ",
            LogType.Exception => "[EXCEPTION] ",
            LogType.Assert => "[ASSERT] ",
            _ => ""
        };

        lock (s_WriteLock)
        {
            try
            {
                // 干净版：只保留正文
                File.AppendAllText(s_LogFilePath, $"{prefix}{logString}\n");

                // 带堆栈版：保留正文 + 完整堆栈
                if (string.IsNullOrEmpty(stackTrace))
                {
                    File.AppendAllText(s_LogFilePathWithStack, $"{prefix}{logString}\n\n");
                }
                else
                {
                    File.AppendAllText(s_LogFilePathWithStack, $"{prefix}{logString}\n{stackTrace}\n");
                }
            }
            catch
            {
                // 忽略写入错误，避免递归
            }
        }
    }

    /// <summary>获取干净日志文件路径</summary>
    public static string GetLogFilePath() => s_LogFilePath;

    /// <summary>获取带堆栈日志文件路径</summary>
    public static string GetLogFilePathWithStack() => s_LogFilePathWithStack;

    /// <summary>
    /// 清理旧的日志文件
    /// </summary>
    private static void CleanupOldLogFiles(string directory)
    {
        try
        {
            if (!Directory.Exists(directory)) return;

            foreach (var file in Directory.GetFiles(directory, "DemoLog*.log"))
            {
                try { File.Delete(file); }
                catch { /* 忽略 */ }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[DemoLog] 清理旧日志失败: {e.Message}");
        }
    }
}

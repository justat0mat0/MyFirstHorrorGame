using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Demo 日志工具菜单
/// 
/// 提供 Tools 菜单快捷入口：
/// - 打开日志文件夹（项目根目录/DemoLogs/）
/// - 打开干净日志（无堆栈）
/// - 打开完整日志（带堆栈）
/// - 复制日志路径到剪贴板
/// </summary>
public static class DemoLogTool
{
    /// <summary>
    /// 获取日志目录路径（项目根目录/DemoLogs/）
    /// </summary>
    private static string GetLogDirectory()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        return Path.Combine(projectRoot, "DemoLogs");
    }

    [MenuItem("Tools/Demo 日志/打开日志文件夹", priority = 100)]
    public static void OpenLogFolder()
    {
        string logDir = GetLogDirectory();
        if (Directory.Exists(logDir))
        {
            OpenDirectory(logDir);
        }
        else
        {
            EditorUtility.DisplayDialog("提示", $"日志目录不存在:\n{logDir}\n\n请先 Play 运行一次生成日志。", "确定");
        }
    }

    [MenuItem("Tools/Demo 日志/打开干净日志 (无堆栈)", priority = 101)]
    public static void OpenCleanLog()
    {
        OpenLogFile("DemoLog.log");
    }

    [MenuItem("Tools/Demo 日志/打开完整日志 (带堆栈)", priority = 102)]
    public static void OpenLogWithStack()
    {
        OpenLogFile("DemoLogStack.log");
    }

    [MenuItem("Tools/Demo 日志/复制日志路径到剪贴板", priority = 200)]
    public static void CopyLogPathToClipboard()
    {
        string logDir = GetLogDirectory();
        GUIUtility.systemCopyBuffer = logDir;
        UnityEngine.Debug.Log($"[DemoLog] 日志路径已复制: {logDir}");
    }

    private static void OpenLogFile(string fileName)
    {
        string logDir = GetLogDirectory();
        string filePath = Path.Combine(logDir, fileName);

        if (!File.Exists(filePath))
        {
            EditorUtility.DisplayDialog("提示",
                $"未找到日志文件:\n{filePath}\n\n请先 Play 运行一次生成日志。", "确定");
            return;
        }

        OpenFileWithDefaultApp(filePath);
        UnityEngine.Debug.Log($"[DemoLog] 打开日志: {filePath}");
    }

    private static void OpenDirectory(string directoryPath)
    {
        try
        {
#if UNITY_EDITOR_WIN
            Process.Start("explorer.exe", $"\"{directoryPath}\"");
#elif UNITY_EDITOR_OSX
            Process.Start("open", $"\"{directoryPath}\"");
#else
            Process.Start("xdg-open", $"\"{directoryPath}\"");
#endif
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[DemoLog] 打开目录失败: {e.Message}");
        }
    }

    private static void OpenFileWithDefaultApp(string filePath)
    {
        try
        {
#if UNITY_EDITOR_WIN
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });
#elif UNITY_EDITOR_OSX
            Process.Start("open", $"\"{filePath}\"");
#else
            Process.Start("xdg-open", $"\"{filePath}\"");
#endif
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[DemoLog] 打开文件失败: {e.Message}");
            EditorUtility.RevealInFinder(filePath);
        }
    }
}

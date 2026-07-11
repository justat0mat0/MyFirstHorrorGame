using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// 一键创建任务系统演示场景
/// 
/// 菜单路径：AstroDemos/Create Quest Demo Scene
/// MCP 可调用：execute_menu_item("AstroDemos/Create Quest Demo Scene")
/// 
/// 创建内容：
/// - Main Camera（深蓝色背景）
/// - QuestDemoController（会在运行时自动创建 QuestManager、Canvas、EventSystem）
/// 
/// 保存路径：Assets/Demo_QuestSystem/Scenes/DemoScene_Quest.unity
/// </summary>
public class QuestSceneBuilder
{
    private const string SCENE_PATH = "Assets/Demo_QuestSystem/Scenes/DemoScene_Quest.unity";

    [MenuItem("AstroDemos/Create Quest Demo Scene")]
    public static void CreateScene()
    {
        // 创建新场景
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── 1. Main Camera ──
        GameObject cameraGo = new GameObject("Main Camera");
        cameraGo.tag = "MainCamera";
        Camera cam = cameraGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.05f, 0.07f, 0.12f, 1f); // 深蓝色背景
        cam.orthographic = false;
        cameraGo.AddComponent<AudioListener>();

        // ── 2. QuestDemoController ──
        // 运行时会自动创建 QuestManager、QuestPanel Canvas、EventSystem
        GameObject controllerGo = new GameObject("QuestDemoController");
        controllerGo.AddComponent<QuestDemoController>();

        // ── 3. 保存场景 ──
        // 确保目录存在
        string dir = System.IO.Path.GetDirectoryName(SCENE_PATH);
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        EditorSceneManager.SaveScene(scene, SCENE_PATH);
        AssetDatabase.Refresh();

        // 添加到 Build Settings
        AddSceneToBuildSettings(SCENE_PATH);

        Debug.Log($"[Quest] ✅ 场景创建完成：{SCENE_PATH}");
    }

    /// <summary>将场景添加到 Build Settings（如果不在列表中）</summary>
    private static void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(
            EditorBuildSettings.scenes);

        // 检查是否已存在
        foreach (var s in scenes)
        {
            if (s.path == scenePath)
            {
                Debug.Log($"[Quest] 场景已在 Build Settings 中");
                return;
            }
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log($"[Quest] ✅ 场景已添加到 Build Settings");
    }
}

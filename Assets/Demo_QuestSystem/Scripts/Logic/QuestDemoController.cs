using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务系统 2D 演示场景控制器 —— Demo 的入口脚本
/// 
/// 职责：
/// 1. 自动创建完整的 2D 游戏场景（玩家、NPC、可拾取物品）
/// 2. 初始化任务系统和 UI（任务面板常驻显示）
/// 3. 管理游戏 HUD（金币、操作提示）
/// 
/// 场景布局（暗黑奇幻俯视角 2D 村庄）：
///   中上方 = 村长（任务起点）
///   左侧   = 药师 + 散落的草药
///   右侧   = 建筑师 + 散落的木材
///   四周   = 暗色森林 + 荧光蘑菇 + 灯笼 + 栅栏 + 水井
/// 
/// 操作说明：WASD 移动 | E 交互
/// </summary>
public class QuestDemoController : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("[Quest] === QuestDemoController.Start 开始 ===");

        // ── 1. 确保有 QuestManager ──
        if (QuestManager.Instance == null)
        {
            GameObject managerGo = new GameObject("QuestManager");
            managerGo.AddComponent<QuestConfig>();   // 先加 Config，QuestManager.Awake 才能找到
            managerGo.AddComponent<QuestManager>();
            Debug.Log("[Quest] 自动创建了 QuestManager");
        }

        // ── 2. 确保有 EventSystem ──
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("[Quest] 自动创建了 EventSystem");
        }

        // ── 3. 构建 2D 世界 ──
        Build2DWorld();

        // ── 4. 确保有 QuestPanel Canvas（任务 UI 常驻显示） ──
        EnsureQuestPanel();

        // ── 5. 创建 HUD ──
        CreateHUD();

        // ── 6. 初始化任务系统 ──
        QuestManager.Instance.Initialize();

        // ── 7. 设置相机 ──
        SetupCamera();

        Debug.Log("[Quest] === QuestDemoController.Start 完成 ===");
        Debug.Log("[Quest] 操作说明：WASD 移动 | E 与 NPC 交互 | 走近物品自动拾取");
    }

    #region 构建 2D 世界

    /// <summary>
    /// 构建 2D 游戏场景（暗黑奇幻版）
    /// 包含：暗色地面、玩家、3 个 NPC（带姓名板）、建筑、可拾取物品、
    ///       丰富装饰（树木、蘑菇、灯笼、石头、花朵、栅栏、水井）
    /// 暗黑风格：深色基调 + 发光窗户 + 荧光蘑菇 + 暖色灯笼
    /// </summary>
    private void Build2DWorld()
    {
        GameObject worldRoot = new GameObject("=== World ===");

        // ── 地面（暗色地面底层） ──
        CreateGround(worldRoot.transform);

        // ── 石板小路（连接各建筑） ──
        CreatePath(worldRoot.transform, "Path_Main", new Vector3(0, 1.5f, 0), new Vector2(1.2f, 6f));
        CreatePath(worldRoot.transform, "Path_Left", new Vector3(-2.5f, 0.5f, 0), new Vector2(4f, 0.8f));
        CreatePath(worldRoot.transform, "Path_Right", new Vector3(2.5f, 0.5f, 0), new Vector2(4f, 0.8f));

        // ── 玩家 ──
        CreatePlayer(worldRoot.transform);

        // ── NPC：村长（中上方，站在房子前面） ──
        CreateNPC(worldRoot.transform, "NPC_VillageChief", "village_chief", "村长",
                  new Vector3(0, 3f, 0), "Sprites/spr_npc_chief");

        // ── NPC：药师（左侧） ──
        CreateNPC(worldRoot.transform, "NPC_Herbalist", "herbalist", "药师",
                  new Vector3(-4.5f, -0.5f, 0), "Sprites/spr_npc_herbalist");

        // ── NPC：建筑师（右侧） ──
        CreateNPC(worldRoot.transform, "NPC_Builder", "builder", "建筑师",
                  new Vector3(4.5f, -0.5f, 0), "Sprites/spr_npc_builder");

        // ── 建筑物 ──
        CreateBuilding(worldRoot.transform, "House_Chief", new Vector3(0, 5f, 0), "Sprites/spr_house_chief");
        CreateBuilding(worldRoot.transform, "House_Herbalist", new Vector3(-4.5f, 2f, 0), "Sprites/spr_house_herbalist");
        CreateBuilding(worldRoot.transform, "House_Builder", new Vector3(4.5f, 2f, 0), "Sprites/spr_house_builder");

        // ── 可拾取物品：草药（药师附近散落） ──
        CreateCollectible(worldRoot.transform, "Herb_1", "herb", "草药",
                          new Vector3(-6.5f, -1.5f, 0), "Sprites/spr_herb");
        CreateCollectible(worldRoot.transform, "Herb_2", "herb", "草药",
                          new Vector3(-5.5f, -3f, 0), "Sprites/spr_herb");
        CreateCollectible(worldRoot.transform, "Herb_3", "herb", "草药",
                          new Vector3(-7.5f, 0.5f, 0), "Sprites/spr_herb");
        CreateCollectible(worldRoot.transform, "Herb_4", "herb", "草药",
                          new Vector3(-3.5f, -2.5f, 0), "Sprites/spr_herb");

        // ── 可拾取物品：木材（建筑师附近散落） ──
        CreateCollectible(worldRoot.transform, "Wood_1", "wood", "木材",
                          new Vector3(6.5f, -1.5f, 0), "Sprites/spr_wood");
        CreateCollectible(worldRoot.transform, "Wood_2", "wood", "木材",
                          new Vector3(5.5f, -3f, 0), "Sprites/spr_wood");
        CreateCollectible(worldRoot.transform, "Wood_3", "wood", "木材",
                          new Vector3(7.5f, 0.5f, 0), "Sprites/spr_wood");
        CreateCollectible(worldRoot.transform, "Wood_4", "wood", "木材",
                          new Vector3(3.5f, -2.5f, 0), "Sprites/spr_wood");
        CreateCollectible(worldRoot.transform, "Wood_5", "wood", "木材",
                          new Vector3(6f, -4f, 0), "Sprites/spr_wood");

        // ── 树木（暗黑森林，环绕村庄，营造氛围） ──
        float[][] treePosArr = {
            new[] { -9f, 7f },     new[] { 9f, 7f },       // 顶角
            new[] { -4f, 8f },     new[] { 4f, 8f },       // 顶部
            new[] { 0f, 9f },                               // 顶部中央
            new[] { -10f, 3f },    new[] { 10f, 3f },      // 两侧上
            new[] { -10f, -1f },   new[] { 10f, -1f },     // 两侧中
            new[] { -9f, -4f },    new[] { 9f, -4f },      // 两侧下
            new[] { -6f, -6f },    new[] { 6f, -6f },      // 底部两侧
        };
        for (int i = 0; i < treePosArr.Length; i++)
        {
            float treeScale = 0.7f + (i % 3) * 0.1f;
            CreateDecoration(worldRoot.transform, $"Tree_{i}",
                new Vector3(treePosArr[i][0], treePosArr[i][1], 0),
                "Sprites/spr_tree", treeScale, 4);
        }

        // ── 荧光蘑菇（暗黑氛围关键元素） ──
        CreateDecoration(worldRoot.transform, "Mushroom_0", new Vector3(-7f, 1f, 0), "Sprites/spr_mushroom", 0.7f, 3);
        CreateDecoration(worldRoot.transform, "Mushroom_1", new Vector3(7f, -2f, 0), "Sprites/spr_mushroom", 0.6f, 3);
        CreateDecoration(worldRoot.transform, "Mushroom_2", new Vector3(-2f, -5f, 0), "Sprites/spr_mushroom", 0.55f, 3);
        CreateDecoration(worldRoot.transform, "Mushroom_3", new Vector3(3f, 6.5f, 0), "Sprites/spr_mushroom", 0.65f, 3);

        // ── 灯笼（照亮小路，暖色点缀） ──
        CreateDecoration(worldRoot.transform, "Lantern_0", new Vector3(-1.5f, 1.5f, 0), "Sprites/spr_lantern", 0.7f, 5);
        CreateDecoration(worldRoot.transform, "Lantern_1", new Vector3(1.5f, 1.5f, 0), "Sprites/spr_lantern", 0.7f, 5);
        CreateDecoration(worldRoot.transform, "Lantern_2", new Vector3(-3f, -1.5f, 0), "Sprites/spr_lantern", 0.6f, 5);
        CreateDecoration(worldRoot.transform, "Lantern_3", new Vector3(3f, -1.5f, 0), "Sprites/spr_lantern", 0.6f, 5);

        // ── 栅栏（村庄边界感） ──
        CreateDecoration(worldRoot.transform, "Fence_0", new Vector3(-3f, -3.5f, 0), "Sprites/spr_fence_h", 0.8f, 1);
        CreateDecoration(worldRoot.transform, "Fence_1", new Vector3(3f, -3.5f, 0), "Sprites/spr_fence_h", 0.8f, 1);

        // ── 水井（村庄中心装饰） ──
        CreateDecoration(worldRoot.transform, "Well_0", new Vector3(2f, 3.5f, 0), "Sprites/spr_well", 0.75f, 3);

        // ── 花朵（暗色神秘花，点缀） ──
        CreateDecoration(worldRoot.transform, "Flower_0", new Vector3(-5f, 2f, 0), "Sprites/spr_flower_1", 0.6f, 1);
        CreateDecoration(worldRoot.transform, "Flower_1", new Vector3(4f, -3f, 0), "Sprites/spr_flower_2", 0.6f, 1);
        CreateDecoration(worldRoot.transform, "Flower_2", new Vector3(-2f, -4.5f, 0), "Sprites/spr_flower_1", 0.55f, 1);
        CreateDecoration(worldRoot.transform, "Flower_3", new Vector3(6.5f, 4f, 0), "Sprites/spr_flower_2", 0.55f, 1);
        CreateDecoration(worldRoot.transform, "Flower_4", new Vector3(-8f, -2f, 0), "Sprites/spr_flower_1", 0.5f, 1);
        CreateDecoration(worldRoot.transform, "Flower_5", new Vector3(8f, 5f, 0), "Sprites/spr_flower_2", 0.5f, 1);

        // ── 石头（散落各处，增加自然感） ──
        CreateDecoration(worldRoot.transform, "Rock_0", new Vector3(-7f, -3f, 0), "Sprites/spr_rock_1", 0.7f, 1);
        CreateDecoration(worldRoot.transform, "Rock_1", new Vector3(8f, 1f, 0), "Sprites/spr_rock_1", 0.6f, 1);
        CreateDecoration(worldRoot.transform, "Rock_2", new Vector3(-3f, 6f, 0), "Sprites/spr_rock_1", 0.5f, 1);

        Debug.Log("[Quest] 2D 暗黑奇幻世界构建完成");
    }

    /// <summary>创建地面背景（多层叠加，更精致的草地效果）</summary>
    private void CreateGround(Transform parent)
    {
        // ── 底色层 ──
        GameObject ground = new GameObject("Ground");
        ground.transform.SetParent(parent, false);
        ground.transform.position = Vector3.zero;
        SpriteRenderer sr = ground.AddComponent<SpriteRenderer>();
        Sprite grassSpr = Resources.Load<Sprite>("Sprites/spr_tile_grass");
        if (grassSpr == null) Debug.LogError("[Quest] ❌ 加载失败: Sprites/spr_tile_grass");
        sr.sprite = grassSpr;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(30, 22);
        sr.sortingOrder = -100;
    }

    /// <summary>创建泥土小路（使用 Sliced 模式，更安全的拉伸方式）</summary>
    private void CreatePath(Transform parent, string goName, Vector3 position, Vector2 size)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite pathSpr = Resources.Load<Sprite>("Sprites/spr_path");
        if (pathSpr == null)
        {
            Debug.LogError("[Quest] ❌ 加载失败: Sprites/spr_path");
            return;
        }
        sr.sprite = pathSpr;
        // 使用 Simple 模式 + localScale 来拉伸路径（避免 Tiled 模式的兼容问题）
        sr.drawMode = SpriteDrawMode.Simple;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        sr.sortingOrder = -90;
    }

    /// <summary>创建玩家</summary>
    private void CreatePlayer(Transform parent)
    {
        GameObject go = new GameObject("Player");
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(0, 0, 0);
        go.tag = "Player";

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite playerSpr = Resources.Load<Sprite>("Sprites/spr_player");
        if (playerSpr == null) Debug.LogError("[Quest] ❌ 玩家精灵加载失败: Sprites/spr_player");
        sr.sprite = playerSpr;
        sr.sortingOrder = 8;  // 玩家在所有物体之上

        go.AddComponent<PlayerController2D>();

        Debug.Log("[Quest] 玩家创建完成 (WASD移动, E交互)");
    }

    /// <summary>创建 NPC（带精致姓名板）</summary>
    private void CreateNPC(Transform parent, string goName, string npcId, string npcName,
                           Vector3 position, string spritePath)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        go.transform.position = position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite npcSpr = Resources.Load<Sprite>(spritePath);
        if (npcSpr == null) Debug.LogError($"[Quest] ❌ NPC精灵加载失败: {spritePath}");
        sr.sprite = npcSpr;
        sr.sortingOrder = 6;  // NPC 在建筑(2)、树木(4)之上

        NPCController npc = go.AddComponent<NPCController>();
        SetPrivateField(npc, "_npcId", npcId);
        SetPrivateField(npc, "_npcName", npcName);

        // ── 精致姓名板（带背景图 + 文字） ──
        CreateNameLabel(go.transform, npcName, new Vector3(0, -0.95f, 0));
    }

    /// <summary>创建建筑物</summary>
    private void CreateBuilding(Transform parent, string goName, Vector3 position, string spritePath)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        go.transform.position = position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite buildSpr = Resources.Load<Sprite>(spritePath);
        if (buildSpr == null)
            Debug.LogError($"[Quest] ❌ 建筑精灵加载失败: {spritePath}");
        else
            Debug.Log($"[Quest] ✅ 建筑精灵加载成功: {spritePath} ({buildSpr.rect.width}x{buildSpr.rect.height})");
        sr.sprite = buildSpr;
        // 提高 sortingOrder 确保在草地和小路之上（但低于角色）
        sr.sortingOrder = 2;
        go.transform.localScale = new Vector3(1.8f, 1.8f, 1);

        // 建筑碰撞体
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1.0f, 0.6f);
        col.offset = new Vector2(0, -0.15f);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    /// <summary>创建可拾取物品</summary>
    private void CreateCollectible(Transform parent, string goName, string itemId, string itemName,
                                   Vector3 position, string spritePath)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        go.transform.position = position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite itemSpr = Resources.Load<Sprite>(spritePath);
        if (itemSpr == null) Debug.LogWarning($"[Quest] ⚠️ 物品精灵加载失败: {spritePath}");
        sr.sprite = itemSpr;
        sr.sortingOrder = 3;  // 物品在地面装饰(1)之上

        CollectibleItem item = go.AddComponent<CollectibleItem>();
        SetPrivateField(item, "_itemId", itemId);
        SetPrivateField(item, "_itemName", itemName);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    /// <summary>创建装饰物</summary>
    private void CreateDecoration(Transform parent, string goName, Vector3 position,
                                  string spritePath, float scale = 1f, int sortingOrder = 2)
    {
        GameObject go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = new Vector3(scale, scale, 1);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        Sprite decSpr = Resources.Load<Sprite>(spritePath);
        if (decSpr == null) Debug.LogWarning($"[Quest] ⚠️ 装饰精灵加载失败: {spritePath}");
        sr.sprite = decSpr;
        sr.sortingOrder = sortingOrder;

        // 树木碰撞体
        if (spritePath.Contains("tree"))
        {
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.3f, 0.3f);
            col.offset = new Vector2(0, -0.2f);

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    /// <summary>创建精致姓名板（背景精灵 + 文字叠加）</summary>
    private void CreateNameLabel(Transform parent, string text, Vector3 offset)
    {
        GameObject labelGo = new GameObject("NameLabel");
        labelGo.transform.SetParent(parent, false);
        labelGo.transform.localPosition = offset;

        // ── 背景精灵（圆角半透明面板） ──
        GameObject bgGo = new GameObject("NameplateBG");
        bgGo.transform.SetParent(labelGo.transform, false);
        SpriteRenderer bgSr = bgGo.AddComponent<SpriteRenderer>();
        Sprite npBg = Resources.Load<Sprite>("Sprites/spr_nameplate_bg");
        if (npBg == null) Debug.LogWarning("[Quest] ⚠️ 姓名板背景加载失败: Sprites/spr_nameplate_bg");
        bgSr.sprite = npBg;
        bgSr.sortingOrder = 20;
        bgGo.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // ── 文字（叠加在背景上方） ──
        GameObject textGo = new GameObject("NameplateText");
        textGo.transform.SetParent(labelGo.transform, false);
        textGo.transform.localPosition = new Vector3(0, 0.01f, -0.01f);
        TextMesh tm = textGo.AddComponent<TextMesh>();
        tm.text = text;
        tm.characterSize = 0.07f;
        tm.fontSize = 64;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 1f, 0.95f, 0.95f);
        tm.fontStyle = FontStyle.Bold;

        MeshRenderer mr = textGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 21;
    }

    #endregion

    #region UI

    /// <summary>确保场景中有 QuestPanel Canvas（常驻显示）</summary>
    private void EnsureQuestPanel()
    {
        if (FindObjectOfType<QuestUIController>() != null)
        {
            Debug.Log("[Quest] QuestPanel 已存在");
            return;
        }

        GameObject canvasGo = new GameObject("QuestPanel");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();
        canvasGo.AddComponent<QuestUIController>();

        Debug.Log("[Quest] 自动创建了 QuestPanel Canvas（常驻显示）");
    }

    /// <summary>创建游戏 HUD（金币、操作提示）</summary>
    private void CreateHUD()
    {
        GameObject hudGo = new GameObject("HUD_Canvas");
        Canvas canvas = hudGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5;

        CanvasScaler scaler = hudGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        hudGo.AddComponent<GraphicRaycaster>();
        hudGo.AddComponent<GameHUD>();
    }

    /// <summary>设置相机</summary>
    private void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        cam.orthographic = true;
        // 适中的视野大小，让角色和建筑在竖屏视频中也看得清
        cam.orthographicSize = 5.5f;
        // 暗黑主题：极深的暗绿黑色，与暗色地面自然过渡
        cam.backgroundColor = new Color(0.06f, 0.08f, 0.05f);

        if (cam.GetComponent<CameraFollow2D>() == null)
            cam.gameObject.AddComponent<CameraFollow2D>();
    }

    #endregion

    #region 工具方法

    /// <summary>通过反射设置 SerializeField 私有字段</summary>
    private void SetPrivateField<T>(Component comp, string fieldName, T value)
    {
        var field = comp.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(comp, value);
        else
            Debug.LogWarning($"[Quest] 字段 {fieldName} 在 {comp.GetType().Name} 中未找到");
    }

    #endregion
}

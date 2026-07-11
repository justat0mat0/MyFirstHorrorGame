using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务系统 UI 主控制器 —— 常驻右侧面板
/// 
/// 职责：
/// 1. 在屏幕右侧始终显示任务面板（无需按键触发）
/// 2. 展示所有可见任务：名称、状态、目标进度、奖励
/// 3. 提供「接取」「领取奖励」按钮
/// 4. 播放音效 + Toast 提示
/// 
/// 设计亮点：
/// - 常驻显示，玩家一眼看清所有任务进度
/// - 所有 UI 在代码中自动构建，无需 Prefab
/// - 3 个任务分别展示「可接取 → 进行中 → 已完成」全生命周期
/// 
/// 挂载在 QuestPanel Canvas 上。
/// </summary>
public class QuestUIController : MonoBehaviour
{
    #region 常量

    // 侧边栏布局参数（大尺寸，竖屏手机看视频也清晰）
    private const float SIDEBAR_WIDTH = 420f;
    private const float CARD_WIDTH = 390f;
    private const float CARD_SPACING = 18f;
    private const float TITLE_HEIGHT = 58f;

    #endregion

    #region 状态

    /// <summary>任务卡片容器</summary>
    private Transform _cardContainer;

    /// <summary>已创建的卡片对象</summary>
    private List<GameObject> _cards = new List<GameObject>();

    /// <summary>Toast UI</summary>
    private QuestToastUI _toastUI;

    /// <summary>是否已订阅 QuestManager 事件</summary>
    private bool _subscribedToManager;

    // ── 音效 ──
    private AudioClip _sfxClick;
    private AudioClip _sfxAccept;
    private AudioClip _sfxComplete;
    private AudioClip _sfxClaim;
    private AudioSource _audioSource;

    #endregion

    #region 生命周期

    private void Awake()
    {
        BuildSidebar();
        LoadAudio();

        // ⚠️ 事件订阅必须放在 Awake 中！
        // 因为 QuestManager 在 QuestDemoController.Start() 的 step 1 创建（Awake 设置 _instance），
        // 而 QuestUIController 在 step 4 创建。此时 QuestManager.Instance 已有效。
        // 如果放在 Start()，会导致 QuestManager.Initialize()（step 6）触发的 OnQuestDataRefreshed
        // 无法被 QuestUIController 接收（因为 Start 在下一帧才执行）。
        var mgr = QuestManager.Instance;
        if (mgr != null)
        {
            mgr.OnQuestDataRefreshed += RefreshAll;
            mgr.OnQuestStatusChanged += OnStatusChanged;
            mgr.OnObjectiveProgressUpdated += OnProgressUpdated;
            _subscribedToManager = true;
            Debug.Log("[Quest] QuestUIController.Awake: 已订阅 QuestManager 事件");
        }
        else
        {
            Debug.LogWarning("[Quest] QuestUIController.Awake: QuestManager.Instance 为 null，将在 Start 中重试订阅");
        }
    }

    private void Start()
    {
        // 补救：如果 Awake 时 QuestManager 还不存在，在 Start 中再次尝试订阅
        if (!_subscribedToManager)
        {
            var mgr = QuestManager.Instance;
            if (mgr != null)
            {
                mgr.OnQuestDataRefreshed += RefreshAll;
                mgr.OnQuestStatusChanged += OnStatusChanged;
                mgr.OnObjectiveProgressUpdated += OnProgressUpdated;
                _subscribedToManager = true;
                Debug.Log("[Quest] QuestUIController.Start: 补救订阅 QuestManager 事件");
            }
        }

        // 安全网：延迟首次刷新（以防 Initialize 在 Start 之后才被调用）
        Invoke(nameof(RefreshAll), 0.2f);
    }

    private void OnDestroy()
    {
        var mgr = QuestManager.Instance;
        if (mgr != null)
        {
            mgr.OnQuestDataRefreshed -= RefreshAll;
            mgr.OnQuestStatusChanged -= OnStatusChanged;
            mgr.OnObjectiveProgressUpdated -= OnProgressUpdated;
        }
    }

    #endregion

    #region 构建 UI

    /// <summary>
    /// 构建常驻右侧边栏
    /// 
    /// 层级：
    /// QuestPanel (Canvas)
    ///   ├── e_Sidebar (右侧半透明面板)
    ///   │   ├── txt_sidebar_title ("📋 任 务")
    ///   │   └── e_ScrollArea
    ///   │       └── e_CardContainer (VerticalLayoutGroup)
    ///   │           ├── card_quest_001
    ///   │           ├── card_quest_002
    ///   │           └── card_quest_003
    ///   └── e_Toast
    /// </summary>
    private void BuildSidebar()
    {
        Transform root = transform;

        // ── 侧边栏容器（右侧，拉伸到屏幕高度） ──
        GameObject sidebar = new GameObject("e_Sidebar");
        sidebar.transform.SetParent(root, false);
        RectTransform sidebarRect = sidebar.AddComponent<RectTransform>();
        sidebarRect.anchorMin = new Vector2(1, 0);
        sidebarRect.anchorMax = new Vector2(1, 1);
        sidebarRect.pivot = new Vector2(1, 1);
        sidebarRect.offsetMin = new Vector2(-SIDEBAR_WIDTH, 0);   // left edge
        sidebarRect.offsetMax = new Vector2(0, 0);                 // right edge flush

        Image sidebarBg = sidebar.AddComponent<Image>();
        sidebarBg.color = new Color(0.05f, 0.07f, 0.12f, 0.88f);
        sidebarBg.raycastTarget = false;

        // ── 标题 ──
        GameObject titleGo = new GameObject("txt_sidebar_title");
        titleGo.transform.SetParent(sidebar.transform, false);
        RectTransform titleRect = titleGo.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.offsetMin = new Vector2(0, -TITLE_HEIGHT);
        titleRect.offsetMax = Vector2.zero;

        // 标题背景
        Image titleBg = titleGo.AddComponent<Image>();
        titleBg.color = new Color(0.08f, 0.12f, 0.2f, 0.9f);
        titleBg.raycastTarget = false;

        // 标题文字
        GameObject titleTextGo = new GameObject("txt");
        titleTextGo.transform.SetParent(titleGo.transform, false);
        RectTransform ttRect = titleTextGo.AddComponent<RectTransform>();
        ttRect.anchorMin = Vector2.zero;
        ttRect.anchorMax = Vector2.one;
        ttRect.offsetMin = new Vector2(15, 0);
        ttRect.offsetMax = new Vector2(-10, 0);
        Text titleText = titleTextGo.AddComponent<Text>();
        titleText.text = "📋 任 务";
        titleText.fontSize = 34;
        titleText.color = new Color(0.92f, 0.95f, 1f);
        titleText.alignment = TextAnchor.MiddleLeft;
        titleText.fontStyle = FontStyle.Bold;
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // ── 滚动区域 ──
        GameObject scrollArea = new GameObject("e_ScrollArea");
        scrollArea.transform.SetParent(sidebar.transform, false);
        RectTransform scrollRect = scrollArea.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.pivot = new Vector2(0.5f, 1);
        scrollRect.offsetMin = new Vector2(5, 5);
        scrollRect.offsetMax = new Vector2(-5, -TITLE_HEIGHT - 5);

        scrollArea.AddComponent<Image>().color = new Color(0, 0, 0, 0); // 透明
        scrollArea.AddComponent<RectMask2D>();

        // 卡片容器（VerticalLayoutGroup）
        GameObject container = new GameObject("e_CardContainer");
        container.transform.SetParent(scrollArea.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(0.5f, 1);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        VerticalLayoutGroup vlg = container.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = CARD_SPACING;
        vlg.padding = new RectOffset(8, 8, 8, 8);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;    // ← 必须 true，VLG 才会读取 LayoutElement.preferredHeight
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false; // ← 不要均分高度，每张卡片按自身 preferredHeight

        ContentSizeFitter csf = container.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // ScrollRect
        ScrollRect sr = scrollArea.AddComponent<ScrollRect>();
        sr.content = containerRect;
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Clamped;
        sr.scrollSensitivity = 25f;

        _cardContainer = container.transform;

        // ── Toast 通知条（屏幕中上方，接取/完成任务时短暂显示） ──
        GameObject toast = new GameObject("e_Toast");
        toast.transform.SetParent(root, false);
        RectTransform toastRect = toast.AddComponent<RectTransform>();
        toastRect.anchorMin = new Vector2(0.5f, 1);
        toastRect.anchorMax = new Vector2(0.5f, 1);
        toastRect.pivot = new Vector2(0.5f, 1);
        // 居中偏左（避开右侧边栏），距顶部 60px
        toastRect.anchoredPosition = new Vector2(-SIDEBAR_WIDTH / 2, -60);
        toastRect.sizeDelta = new Vector2(480, 56);

        Image toastBg = toast.AddComponent<Image>();
        toastBg.color = new Color(0.12f, 0.18f, 0.28f, 0.95f);
        toastBg.raycastTarget = false;

        // ⚠️ 必须先创建 txt_toast 子节点，再挂 QuestToastUI
        // 因为 AddComponent 会立即触发 Awake()，Awake 里会 Find("txt_toast")
        GameObject toastTextGo = new GameObject("txt_toast");
        toastTextGo.transform.SetParent(toast.transform, false);
        RectTransform toastTextRect = toastTextGo.AddComponent<RectTransform>();
        toastTextRect.anchorMin = Vector2.zero;
        toastTextRect.anchorMax = Vector2.one;
        toastTextRect.offsetMin = new Vector2(12, 0);
        toastTextRect.offsetMax = new Vector2(-12, 0);
        Text toastText = toastTextGo.AddComponent<Text>();
        toastText.text = "";
        toastText.fontSize = 28;
        toastText.color = new Color(1f, 0.95f, 0.8f);
        toastText.alignment = TextAnchor.MiddleCenter;
        toastText.fontStyle = FontStyle.Bold;
        toastText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        _toastUI = toast.AddComponent<QuestToastUI>();

        Debug.Log("[Quest] ✅ 常驻右侧任务面板构建完成");
    }

    /// <summary>加载音效</summary>
    private void LoadAudio()
    {
        _sfxClick = Resources.Load<AudioClip>("Audio/sfx_click");
        _sfxAccept = Resources.Load<AudioClip>("Audio/sfx_accept");
        _sfxComplete = Resources.Load<AudioClip>("Audio/sfx_complete");
        _sfxClaim = Resources.Load<AudioClip>("Audio/sfx_claim");

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    #endregion

    #region 刷新

    /// <summary>全量刷新所有任务卡片</summary>
    private void RefreshAll()
    {
        // 清理旧卡片
        foreach (var go in _cards)
            if (go != null) Destroy(go);
        _cards.Clear();

        if (_cardContainer == null)
        {
            Debug.LogWarning("[Quest] RefreshAll: _cardContainer 为 null，跳过刷新");
            return;
        }

        var mgr = QuestManager.Instance;
        if (mgr == null)
        {
            Debug.LogWarning("[Quest] RefreshAll: QuestManager.Instance 为 null，跳过刷新");
            return;
        }
        if (mgr.QuestDb == null)
        {
            Debug.LogWarning("[Quest] RefreshAll: QuestDb 为 null（Initialize 尚未调用？），跳过刷新");
            return;
        }

        var quests = mgr.GetAllVisibleQuests();
        Debug.Log($"[Quest] RefreshAll: 找到 {quests.Count} 个可见任务，开始创建卡片");

        foreach (var def in quests)
        {
            var rt = mgr.GetRuntimeData(def.questId);
            if (rt == null) continue;

            GameObject card = CreateQuestCard(def, rt);
            card.transform.SetParent(_cardContainer, false);
            _cards.Add(card);
        }

        Debug.Log($"[Quest] RefreshAll: 共创建 {_cards.Count} 张任务卡片");

        // 强制布局系统立即重新计算（解决动态创建卡片后不显示的问题）
        Canvas.ForceUpdateCanvases();
        RectTransform containerRT = _cardContainer.GetComponent<RectTransform>();
        if (containerRT != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRT);
    }

    #endregion

    #region 任务卡片

    /// <summary>
    /// 创建一张任务卡片（精简版，去掉描述文字，大字号，观众一眼看清）
    /// 
    /// 布局：
    /// ┌──────────────────────────────┐
    /// │ ┃ 任务名称            状态标签 │
    /// │ ┃                            │
    /// │ ┃  ▸ 目标                     │
    /// │ ┃  ✓ 目标1              1/1   │
    /// │ ┃  □ 目标2              0/3   │
    /// │ ┃                            │
    /// │ ┃  ▸ 奖励                     │
    /// │ ┃  💰 100 金币  ⭐ 20 经验    │
    /// │ ┃         [ 接 取 任 务 ]      │
    /// └──────────────────────────────┘
    /// </summary>
    private GameObject CreateQuestCard(QuestDefinition def, QuestRuntimeData rt)
    {
        // ── 计算动态高度（大字号 + 宽松间距） ──
        float height = 0;
        height += 48;  // 标题行（名称 + 状态）
        height += 8;   // 标题下方间距
        height += 32;  // "▸ 目标"
        height += def.objectives.Count * 36; // 每个目标行
        height += 10;  // 目标与奖励间距
        height += 32;  // "▸ 奖励"
        height += 36;  // 奖励内容
        if (rt.status == QuestStatus.Available || rt.status == QuestStatus.Completed)
            height += 56; // 按钮（加大）
        height += 18;  // 底部 padding

        // ── 卡片容器 ──
        GameObject card = new GameObject($"card_{def.questId}");
        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(0, height);

        // LayoutElement 让 VerticalLayoutGroup 正确计算此卡片的高度
        LayoutElement le = card.AddComponent<LayoutElement>();
        le.preferredHeight = height;
        le.minHeight = height;

        Image cardBg = card.AddComponent<Image>();
        cardBg.color = GetCardBgColor(rt.status);
        cardBg.raycastTarget = false;

        // ── 左侧状态色条（加宽到5px） ──
        GameObject bar = new GameObject("status_bar");
        bar.transform.SetParent(card.transform, false);
        RectTransform barRect = bar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0, 0);
        barRect.anchorMax = new Vector2(0, 1);
        barRect.pivot = new Vector2(0, 0.5f);
        barRect.offsetMin = new Vector2(0, 4);
        barRect.offsetMax = new Vector2(5, -4);
        bar.AddComponent<Image>().color = GetStatusBarColor(rt.status);

        // ── 内容区 ──
        float yOffset = -8f;
        float leftPad = 16f;

        // 任务名称（大号粗体，最低 28px）
        CreateCardText(card.transform, "txt_name", def.questName,
            leftPad, yOffset, CARD_WIDTH - 130, 40, 28, Color.white, TextAnchor.MiddleLeft, FontStyle.Bold);

        // 状态标签（右上角）
        string statusStr = GetStatusText(rt.status);
        Color statusColor = GetStatusColor(rt.status);
        CreateCardText(card.transform, "txt_status", statusStr,
            CARD_WIDTH - 120, yOffset, 108, 40, 24, statusColor, TextAnchor.MiddleRight, FontStyle.Bold);

        yOffset -= 48;

        // ── 目标标题 ──
        CreateCardText(card.transform, "txt_obj_label", "▸ 目标",
            leftPad, yOffset, 200, 30, 22,
            new Color(0.5f, 0.75f, 0.95f), TextAnchor.MiddleLeft, FontStyle.Bold);

        yOffset -= 32;

        // ── 各目标（大字号，每行留足高度） ──
        for (int i = 0; i < def.objectives.Count; i++)
        {
            var obj = def.objectives[i];
            int cur = (i < rt.progress.Count) ? rt.progress[i] : 0;
            bool done = cur >= obj.targetCount;

            string icon = done ? "✓" : "□";
            Color iconColor = done ? new Color(0.3f, 0.9f, 0.4f) : new Color(0.55f, 0.6f, 0.65f);

            // 图标（24px）
            CreateCardText(card.transform, $"txt_obj_icon_{i}", icon,
                leftPad + 4, yOffset, 28, 32, 24, iconColor, TextAnchor.MiddleCenter);

            // 目标描述 + 进度（24px，足够大）
            string progressStr = obj.targetCount > 1 ? $"  {cur}/{obj.targetCount}" : "";
            Color descColor = done ? new Color(0.45f, 0.45f, 0.45f) : new Color(0.88f, 0.88f, 0.88f);
            CreateCardText(card.transform, $"txt_obj_desc_{i}", obj.description + progressStr,
                leftPad + 34, yOffset, CARD_WIDTH - 60, 32, 24, descColor, TextAnchor.MiddleLeft,
                done ? FontStyle.Italic : FontStyle.Normal);

            yOffset -= 36;
        }

        yOffset -= 10; // 目标与奖励间距

        // ── 奖励标题 ──
        CreateCardText(card.transform, "txt_rwd_label", "▸ 奖励",
            leftPad, yOffset, 200, 30, 22,
            new Color(0.95f, 0.8f, 0.35f), TextAnchor.MiddleLeft, FontStyle.Bold);

        yOffset -= 32;

        // ── 奖励内容（24px，金色高亮） ──
        string rewardStr = $"💰 {def.rewards.gold} 金币";
        if (def.rewards.exp > 0) rewardStr += $"    ⭐ {def.rewards.exp} 经验";
        CreateCardText(card.transform, "txt_rwd", rewardStr,
            leftPad + 4, yOffset, CARD_WIDTH - 30, 32, 24,
            new Color(1f, 0.9f, 0.5f), TextAnchor.MiddleLeft);

        yOffset -= 36;

        // ── 操作按钮（加大，手机也能看清） ──
        if (rt.status == QuestStatus.Available)
        {
            CreateActionButton(card.transform, "btn_accept", "接 取 任 务",
                new Color(0.15f, 0.5f, 0.25f, 0.95f), yOffset, def.questId, true);
        }
        else if (rt.status == QuestStatus.Completed)
        {
            CreateActionButton(card.transform, "btn_claim", "领 取 奖 励",
                new Color(0.65f, 0.45f, 0.1f, 0.95f), yOffset, def.questId, false);
        }

        return card;
    }

    /// <summary>在卡片上创建文字</summary>
    private void CreateCardText(Transform parent, string name, string text,
        float x, float y, float width, float height, int fontSize,
        Color color, TextAnchor alignment, FontStyle style = FontStyle.Normal)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);

        Text txt = go.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = alignment;
        txt.fontStyle = style;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.horizontalOverflow = HorizontalWrapMode.Wrap;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        txt.raycastTarget = false;
    }

    /// <summary>创建操作按钮（接取/领取）</summary>
    private void CreateActionButton(Transform parent, string name, string label,
        Color bgColor, float yOffset, string questId, bool isAccept)
    {
        GameObject btnGo = new GameObject(name);
        btnGo.transform.SetParent(parent, false);
        RectTransform btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 1);
        btnRect.anchorMax = new Vector2(0.5f, 1);
        btnRect.pivot = new Vector2(0.5f, 1);
        btnRect.anchoredPosition = new Vector2(0, yOffset);
        btnRect.sizeDelta = new Vector2(210, 46);

        Image btnBg = btnGo.AddComponent<Image>();
        btnBg.color = bgColor;
        btnBg.raycastTarget = true;

        Button btn = btnGo.AddComponent<Button>();
        string qId = questId;
        if (isAccept)
        {
            btn.onClick.AddListener(() =>
            {
                PlaySfx(_sfxClick);
                QuestManager.Instance?.AcceptQuest(qId);
            });
        }
        else
        {
            btn.onClick.AddListener(() =>
            {
                PlaySfx(_sfxClick);
                QuestManager.Instance?.ClaimReward(qId);
            });
        }

        // 按钮文字（加大到24px）
        GameObject txtGo = new GameObject("txt");
        txtGo.transform.SetParent(btnGo.transform, false);
        RectTransform txtRect = txtGo.AddComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;
        Text txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.fontSize = 24;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.raycastTarget = false;
    }

    #endregion

    #region 事件回调

    private void OnStatusChanged(string questId, QuestStatus newStatus)
    {
        switch (newStatus)
        {
            case QuestStatus.InProgress:
                PlaySfx(_sfxAccept);
                ShowToast("✅ 任务已接取！");
                break;
            case QuestStatus.Completed:
                PlaySfx(_sfxComplete);
                ShowToast("🎉 任务目标全部达成！");
                break;
            case QuestStatus.Claimed:
                PlaySfx(_sfxClaim);
                ShowToast("🎁 奖励已领取！");
                break;
        }
        // 状态变化后全量刷新卡片
        RefreshAll();
    }

    private void OnProgressUpdated(string questId, int objIndex, int current, int target)
    {
        RefreshAll();
    }

    #endregion

    #region 辅助

    private string GetStatusText(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.Available: return "可接取";
            case QuestStatus.InProgress: return "进行中";
            case QuestStatus.Completed: return "已完成";
            case QuestStatus.Claimed: return "✓ 已领取";
            default: return "";
        }
    }

    private Color GetStatusColor(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.Available: return new Color(0.4f, 0.9f, 0.4f);
            case QuestStatus.InProgress: return new Color(1f, 0.85f, 0.3f);
            case QuestStatus.Completed: return new Color(0.3f, 0.85f, 1f);
            case QuestStatus.Claimed: return new Color(0.45f, 0.45f, 0.45f);
            default: return Color.white;
        }
    }

    private Color GetCardBgColor(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.Available: return new Color(0.1f, 0.16f, 0.12f, 0.85f);
            case QuestStatus.InProgress: return new Color(0.14f, 0.14f, 0.08f, 0.85f);
            case QuestStatus.Completed: return new Color(0.08f, 0.14f, 0.18f, 0.85f);
            case QuestStatus.Claimed: return new Color(0.08f, 0.08f, 0.08f, 0.65f);
            default: return new Color(0.1f, 0.1f, 0.1f, 0.8f);
        }
    }

    private Color GetStatusBarColor(QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.Available: return new Color(0.3f, 0.85f, 0.3f);
            case QuestStatus.InProgress: return new Color(1f, 0.75f, 0.1f);
            case QuestStatus.Completed: return new Color(0.2f, 0.7f, 1f);
            case QuestStatus.Claimed: return new Color(0.35f, 0.35f, 0.35f);
            default: return Color.gray;
        }
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || _audioSource == null) return;
        var config = QuestManager.Instance?.Config;
        if (config != null && !config.EnableSfx) return;
        _audioSource.PlayOneShot(clip);
    }

    private void ShowToast(string message)
    {
        if (_toastUI != null)
            _toastUI.ShowToast(message);
    }

    #endregion
}

using UnityEngine;

/// <summary>
/// NPC 控制器
/// 
/// 职责：
/// 1. 作为任务发布者，头顶显示任务指示器（!/?/✓）
/// 2. 玩家按 E 交互时：接取/提交/领取任务
/// 3. 在玩家接近时显示交互提示
/// 
/// 每个 NPC 通过 giverNpcId 与 quest_database.json 中的任务关联。
/// 挂载在 NPC GameObject 上，需要 BoxCollider2D (isTrigger)。
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class NPCController : MonoBehaviour, IInteractable
{
    [Header("── NPC 配置 ──")]
    [Tooltip("NPC 的唯一标识符（对应 quest_database.json 中的 giverNpcId / targetId）")]
    [SerializeField] private string _npcId;
    [Tooltip("NPC 显示名称")]
    [SerializeField] private string _npcName = "NPC";

    [Header("── 指示器 ──")]
    [Tooltip("指示器显示在 NPC 头顶的偏移")]
    [SerializeField] private Vector2 _indicatorOffset = new Vector2(0, 0.8f);

    // ── 运行时 ──
    private SpriteRenderer _indicatorRenderer;
    private Sprite _sprAvailable;
    private Sprite _sprProgress;
    private Sprite _sprComplete;
    private bool _playerInRange;

    // ── 交互提示（带精灵背景） ──
    private GameObject _promptGo;

    // ── 交互提示偏移 ──
    [Header("── 姓名板 ──")]
    [Tooltip("交互提示与 NPC 中心的 Y 偏移")]
    [SerializeField] private float _promptOffsetY = -1.35f;

    public string InteractableName => _npcName;
    public string NpcId => _npcId;

    private void Start()
    {
        // 加载指示器精灵
        _sprAvailable = Resources.Load<Sprite>("Sprites/spr_indicator_available");
        _sprProgress = Resources.Load<Sprite>("Sprites/spr_indicator_progress");
        _sprComplete = Resources.Load<Sprite>("Sprites/spr_indicator_complete");

        // 创建指示器
        CreateIndicator();

        // 创建交互提示
        CreatePrompt();

        // 配置 Collider
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2f, 2f);

        // 订阅任务事件
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestDataRefreshed += RefreshIndicator;
            QuestManager.Instance.OnQuestStatusChanged += (_, __) => RefreshIndicator();
            QuestManager.Instance.OnObjectiveProgressUpdated += (_, __, ___, ____) => RefreshIndicator();
        }

        RefreshIndicator();
    }

    private void CreateIndicator()
    {
        GameObject go = new GameObject("QuestIndicator");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = _indicatorOffset;
        _indicatorRenderer = go.AddComponent<SpriteRenderer>();
        _indicatorRenderer.sortingOrder = 10;
        go.SetActive(false);
    }

    private void CreatePrompt()
    {
        _promptGo = new GameObject("InteractPrompt");
        _promptGo.transform.SetParent(transform, false);
        _promptGo.transform.localPosition = new Vector3(0, _promptOffsetY, 0);

        // ── 背景精灵（精致的半透明面板） ──
        GameObject bgGo = new GameObject("PromptBG");
        bgGo.transform.SetParent(_promptGo.transform, false);
        SpriteRenderer bgSr = bgGo.AddComponent<SpriteRenderer>();
        bgSr.sprite = Resources.Load<Sprite>("Sprites/spr_prompt_bg");
        bgSr.sortingOrder = 22;
        // 缩放到合适的世界尺寸（180×44px / 100ppu * 0.55 ≈ 0.99×0.24 世界单位）
        bgGo.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

        // ── 文字（叠加在背景上方） ──
        GameObject textGo = new GameObject("PromptText");
        textGo.transform.SetParent(_promptGo.transform, false);
        textGo.transform.localPosition = new Vector3(0, 0.01f, -0.01f);
        TextMesh tm = textGo.AddComponent<TextMesh>();
        tm.text = "按 E 交互";
        tm.characterSize = 0.065f;
        tm.fontSize = 64;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 1f, 0.75f, 0.95f);
        tm.fontStyle = FontStyle.Bold;

        MeshRenderer mr = textGo.GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder = 23;

        _promptGo.SetActive(false);
    }

    /// <summary>刷新头顶任务指示器</summary>
    public void RefreshIndicator()
    {
        var mgr = QuestManager.Instance;
        if (mgr == null || _indicatorRenderer == null) return;

        IndicatorState state = GetIndicatorState(mgr);

        switch (state)
        {
            case IndicatorState.Available:
                _indicatorRenderer.sprite = _sprAvailable;
                _indicatorRenderer.gameObject.SetActive(true);
                break;
            case IndicatorState.InProgress:
                _indicatorRenderer.sprite = _sprProgress;
                _indicatorRenderer.gameObject.SetActive(true);
                break;
            case IndicatorState.Complete:
                _indicatorRenderer.sprite = _sprComplete;
                _indicatorRenderer.gameObject.SetActive(true);
                break;
            default:
                _indicatorRenderer.gameObject.SetActive(false);
                break;
        }
    }

    private IndicatorState GetIndicatorState(QuestManager mgr)
    {
        // 优先：有可在此 NPC 领取奖励的已完成任务
        var completedQuests = mgr.GetQuestsByStatus(QuestStatus.Completed);
        if (completedQuests != null)
        {
            foreach (var q in completedQuests)
            {
                if (q.giverNpcId == _npcId)
                    return IndicatorState.Complete;
            }
        }

        // 其次：有任务需要与这个 NPC 对话（进行中目标）
        var activeQuests = mgr.GetActiveQuests();
        if (activeQuests != null)
        {
            foreach (var q in activeQuests)
            {
                var rt = mgr.GetRuntimeData(q.questId);
                if (rt == null) continue;
                for (int i = 0; i < q.objectives.Count; i++)
                {
                    var obj = q.objectives[i];
                    if (obj.Type == ObjectiveType.Talk && obj.targetId == _npcId)
                    {
                        int progress = (i < rt.progress.Count) ? rt.progress[i] : 0;
                        if (progress < obj.targetCount)
                            return IndicatorState.InProgress;
                    }
                }
            }
        }

        // 最后：这个 NPC 有可接取的任务
        var availableQuests = mgr.GetQuestsByStatus(QuestStatus.Available);
        if (availableQuests != null)
        {
            foreach (var q in availableQuests)
            {
                if (q.giverNpcId == _npcId)
                    return IndicatorState.Available;
            }
        }

        return IndicatorState.None;
    }

    #region IInteractable

    public bool CanInteract() => true;

    public void OnInteract(PlayerController2D player)
    {
        var mgr = QuestManager.Instance;
        if (mgr == null) return;

        // 1. 先尝试领取奖励（此 NPC 发布的已完成任务）
        var completedQuests = mgr.GetQuestsByStatus(QuestStatus.Completed);
        if (completedQuests != null)
        {
            foreach (var q in completedQuests)
            {
                if (q.giverNpcId == _npcId)
                {
                    mgr.ClaimReward(q.questId);
                    Debug.Log($"[Quest] 在 {_npcName} 处领取了任务 [{q.questName}] 的奖励");
                    RefreshIndicator();
                    return;
                }
            }
        }

        // 2. 上报 Talk 事件（推进正在进行的任务目标）
        mgr.ReportEvent(ObjectiveType.Talk, _npcId, 1);

        // 3. 尝试接取此 NPC 的新任务
        var availableQuests = mgr.GetQuestsByStatus(QuestStatus.Available);
        if (availableQuests != null)
        {
            foreach (var q in availableQuests)
            {
                if (q.giverNpcId == _npcId)
                {
                    mgr.AcceptQuest(q.questId);
                    Debug.Log($"[Quest] 在 {_npcName} 处接取了任务 [{q.questName}]");
                    break;
                }
            }
        }

        RefreshIndicator();
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            if (_promptGo != null) _promptGo.SetActive(true);

            var player = other.GetComponent<PlayerController2D>();
            if (player != null) player.SetNearbyInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            if (_promptGo != null) _promptGo.SetActive(false);

            var player = other.GetComponent<PlayerController2D>();
            if (player != null) player.ClearNearbyInteractable(this);
        }
    }

    #endregion

    private enum IndicatorState { None, Available, InProgress, Complete }
}

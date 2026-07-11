using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务管理器 —— 整个任务系统的核心
/// 
/// 职责：
/// 1. 从 JSON 加载任务数据库
/// 2. 管理所有任务的运行时状态（解锁、接取、进度、完成、领奖）
/// 3. 提供游戏事件上报接口（收集、对话、到达）
/// 4. 通过 C# 事件通知 UI 层更新显示
/// 
/// 使用方式：
///   QuestManager.Instance.AcceptQuest("quest_001");
///   QuestManager.Instance.ReportEvent(ObjectiveType.Collect, "ore_iron", 1);
/// 
/// 设计原则：
/// - 单例模式，全局唯一
/// - 纯逻辑层，不直接操作任何 UI
/// - 通过事件与 UI 层解耦
/// </summary>
public class QuestManager : MonoBehaviour
{
    #region 单例

    private static QuestManager _instance;

    /// <summary>全局单例访问点</summary>
    public static QuestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<QuestManager>();
            }
            return _instance;
        }
    }

    #endregion

    #region 事件（UI 层通过订阅这些事件来更新显示）

    /// <summary>
    /// 任务状态变化时触发（接取、完成、领奖等）
    /// 参数：变化的任务ID、新状态
    /// </summary>
    public event Action<string, QuestStatus> OnQuestStatusChanged;

    /// <summary>
    /// 任务目标进度更新时触发
    /// 参数：任务ID、目标索引、当前进度、目标数量
    /// </summary>
    public event Action<string, int, int, int> OnObjectiveProgressUpdated;

    /// <summary>
    /// 全局刷新事件（任务接取/完成后，通知UI全量刷新）
    /// </summary>
    public event Action OnQuestDataRefreshed;

    #endregion

    #region 数据

    /// <summary>任务数据库（从 JSON 加载的任务定义）</summary>
    private QuestDatabase _questDb;

    /// <summary>所有任务的运行时状态（key = questId）</summary>
    private Dictionary<string, QuestRuntimeData> _runtimeData = new Dictionary<string, QuestRuntimeData>();

    /// <summary>任务配置参数（Inspector 可调）</summary>
    public QuestConfig Config { get; private set; }

    /// <summary>获取任务数据库（只读）</summary>
    public QuestDatabase QuestDb => _questDb;

    #endregion

    #region 生命周期

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        Config = GetComponent<QuestConfig>();
        if (Config == null)
        {
            Debug.LogWarning("[Quest] 未找到 QuestConfig，已自动添加默认配置");
            Config = gameObject.AddComponent<QuestConfig>();
        }
    }

    #endregion

    #region 初始化

    /// <summary>
    /// 初始化任务系统：加载任务数据库，创建运行时状态
    /// 由 QuestDemoController.Start() 调用
    /// </summary>
    public void Initialize()
    {
        _questDb = LoadQuestDatabase();
        if (_questDb == null || _questDb.quests == null)
        {
            Debug.LogError("[Quest] 任务数据库加载失败");
            return;
        }

        _runtimeData.Clear();
        foreach (var def in _questDb.quests)
        {
            var rt = new QuestRuntimeData(def.questId, def.objectives.Count);

            if (!string.IsNullOrEmpty(def.prerequisiteQuestId))
                rt.status = QuestStatus.Locked;
            else
                rt.status = QuestStatus.Available;

            _runtimeData[def.questId] = rt;
        }

        Debug.Log($"[Quest] 初始化完成，共 {_questDb.quests.Count} 个任务");

        // 通知所有订阅者（如 QuestUIController）数据已就绪，可以刷新 UI
        OnQuestDataRefreshed?.Invoke();
    }

    #endregion

    #region 任务操作

    /// <summary>接取任务（前提：状态必须是 Available）</summary>
    public bool AcceptQuest(string questId)
    {
        if (!_runtimeData.TryGetValue(questId, out var rt))
        {
            Debug.LogWarning($"[Quest] 接取失败：任务 {questId} 不存在");
            return false;
        }

        if (rt.status != QuestStatus.Available)
        {
            Debug.LogWarning($"[Quest] 接取失败：任务 {questId} 状态为 {rt.status}，不可接取");
            return false;
        }

        rt.status = QuestStatus.InProgress;
        Debug.Log($"[Quest] ✅ 接取任务：{questId}");

        OnQuestStatusChanged?.Invoke(questId, QuestStatus.InProgress);
        OnQuestDataRefreshed?.Invoke();
        return true;
    }

    /// <summary>领取已完成任务的奖励（前提：状态必须是 Completed）</summary>
    public bool ClaimReward(string questId)
    {
        if (!_runtimeData.TryGetValue(questId, out var rt))
        {
            Debug.LogWarning($"[Quest] 领取失败：任务 {questId} 不存在");
            return false;
        }

        if (rt.status != QuestStatus.Completed)
        {
            Debug.LogWarning($"[Quest] 领取失败：任务 {questId} 状态为 {rt.status}，不可领取");
            return false;
        }

        var def = _questDb.GetQuestById(questId);
        if (def?.rewards != null)
        {
            string rewardStr = $"金币 +{def.rewards.gold}";
            if (def.rewards.exp > 0) rewardStr += $"，经验 +{def.rewards.exp}";
            if (def.rewards.items != null)
            {
                foreach (var item in def.rewards.items)
                    rewardStr += $"，{item.itemName} x{item.quantity}";
            }
            Debug.Log($"[Quest] 🎁 领取奖励：{rewardStr}");

            // 给玩家发放金币奖励
            if (def.rewards.gold > 0 && PlayerController2D.Instance != null)
            {
                PlayerController2D.Instance.AddGold(def.rewards.gold);
            }
        }

        rt.status = QuestStatus.Claimed;
        Debug.Log($"[Quest] ✅ 任务完成并领取奖励：{questId}");

        UnlockDependentQuests(questId);

        OnQuestStatusChanged?.Invoke(questId, QuestStatus.Claimed);
        OnQuestDataRefreshed?.Invoke();
        return true;
    }

    /// <summary>
    /// 上报游戏事件，自动推进相关任务的目标进度
    /// 
    /// 这是任务系统与游戏逻辑的对接点。
    /// 在实际游戏中，各系统（拾取、NPC交互等）调用此方法通知任务系统。
    /// </summary>
    public void ReportEvent(ObjectiveType eventType, string targetId, int count = 1)
    {
        Debug.Log($"[Quest] 📡 上报事件：{eventType} / {targetId} x{count}");

        foreach (var pair in _runtimeData)
        {
            if (pair.Value.status != QuestStatus.InProgress) continue;

            var def = _questDb.GetQuestById(pair.Key);
            if (def == null) continue;

            bool anyUpdated = false;

            for (int i = 0; i < def.objectives.Count; i++)
            {
                var obj = def.objectives[i];

                if (obj.Type == eventType &&
                    obj.targetId == targetId &&
                    pair.Value.progress[i] < obj.targetCount)
                {
                    int oldProgress = pair.Value.progress[i];
                    pair.Value.progress[i] = Mathf.Min(pair.Value.progress[i] + count, obj.targetCount);
                    int newProgress = pair.Value.progress[i];

                    if (newProgress != oldProgress)
                    {
                        anyUpdated = true;
                        Debug.Log($"[Quest] 📈 {pair.Key} 目标[{i}] 进度：{newProgress}/{obj.targetCount}" +
                                  (newProgress >= obj.targetCount ? " ✓" : ""));

                        OnObjectiveProgressUpdated?.Invoke(pair.Key, i, newProgress, obj.targetCount);
                    }
                }
            }

            if (anyUpdated && AreAllObjectivesComplete(def, pair.Value))
            {
                pair.Value.status = QuestStatus.Completed;
                Debug.Log($"[Quest] 🎉 任务所有目标达成：{pair.Key}");
                OnQuestStatusChanged?.Invoke(pair.Key, QuestStatus.Completed);
            }
        }

        OnQuestDataRefreshed?.Invoke();
    }

    /// <summary>重置所有任务状态（demo 用，方便反复测试）</summary>
    public void ResetAllQuests()
    {
        Debug.Log("[Quest] 🔄 重置所有任务");
        Initialize();
        OnQuestDataRefreshed?.Invoke();
    }

    #endregion

    #region 查询方法

    public QuestRuntimeData GetRuntimeData(string questId)
    {
        _runtimeData.TryGetValue(questId, out var rt);
        return rt;
    }

    public List<QuestDefinition> GetQuestsByStatus(QuestStatus status)
    {
        var result = new List<QuestDefinition>();
        foreach (var pair in _runtimeData)
        {
            if (pair.Value.status == status)
            {
                var def = _questDb.GetQuestById(pair.Key);
                if (def != null) result.Add(def);
            }
        }
        return result;
    }

    public List<QuestDefinition> GetQuestsByCategory(QuestCategory category)
    {
        var result = new List<QuestDefinition>();
        foreach (var def in _questDb.quests)
        {
            if (def.Category == category)
            {
                var rt = GetRuntimeData(def.questId);
                if (rt != null && rt.status != QuestStatus.Locked)
                    result.Add(def);
            }
        }
        return result;
    }

    public List<QuestDefinition> GetAllVisibleQuests()
    {
        var result = new List<QuestDefinition>();
        foreach (var def in _questDb.quests)
        {
            var rt = GetRuntimeData(def.questId);
            if (rt != null && rt.status != QuestStatus.Locked)
                result.Add(def);
        }
        return result;
    }

    public List<QuestDefinition> GetActiveQuests()
    {
        var result = new List<QuestDefinition>();
        foreach (var pair in _runtimeData)
        {
            if (pair.Value.status == QuestStatus.InProgress)
            {
                var def = _questDb.GetQuestById(pair.Key);
                if (def != null) result.Add(def);
            }
        }
        return result;
    }

    #endregion

    #region 内部方法

    private bool AreAllObjectivesComplete(QuestDefinition def, QuestRuntimeData rt)
    {
        for (int i = 0; i < def.objectives.Count; i++)
        {
            if (rt.progress[i] < def.objectives[i].targetCount)
                return false;
        }
        return true;
    }

    private void UnlockDependentQuests(string completedQuestId)
    {
        foreach (var def in _questDb.quests)
        {
            if (def.prerequisiteQuestId == completedQuestId)
            {
                var rt = GetRuntimeData(def.questId);
                if (rt != null && rt.status == QuestStatus.Locked)
                {
                    rt.status = QuestStatus.Available;
                    Debug.Log($"[Quest] 🔓 解锁新任务：{def.questName}（{def.questId}）");
                    OnQuestStatusChanged?.Invoke(def.questId, QuestStatus.Available);
                }
            }
        }
    }

    private QuestDatabase LoadQuestDatabase()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Config/quest_database");
        if (jsonFile == null)
        {
            Debug.LogError("[Quest] 文件不存在：Resources/Config/quest_database.json");
            return null;
        }

        try
        {
            QuestDatabase db = JsonUtility.FromJson<QuestDatabase>(jsonFile.text);
            Debug.Log($"[Quest] 成功加载任务数据库（{db.quests.Count} 个任务）");
            return db;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Quest] JSON 解析失败：{e.Message}");
            return null;
        }
    }

    #endregion
}

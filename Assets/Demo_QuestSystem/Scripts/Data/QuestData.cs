using System;
using System.Collections.Generic;

/// <summary>
/// 任务系统 —— 数据定义
/// 
/// 包含以下核心数据类：
/// - QuestDatabase：所有任务定义的集合（对应 quest_database.json）
/// - QuestDefinition：单个任务的完整定义（目标、奖励、前置条件等）
/// - QuestObjective：任务目标（收集物品、消灭敌人、与NPC对话、到达地点）
/// - QuestReward：任务奖励（金币 + 物品列表）
/// - QuestRewardItem：奖励中的单个物品条目
/// - QuestRuntimeData：运行时任务状态数据（进度跟踪）
/// 
/// 所有数据通过 JSON 配置文件加载，修改任务内容不需要改代码
/// </summary>

/// <summary>任务分类枚举</summary>
public enum QuestCategory
{
    Main = 0,    // 主线任务
    Side = 1     // 支线任务
}

/// <summary>任务状态枚举</summary>
public enum QuestStatus
{
    /// <summary>未解锁（前置任务未完成）</summary>
    Locked = 0,

    /// <summary>可接取（前置条件已满足，但玩家还没接）</summary>
    Available = 1,

    /// <summary>进行中（已接取，目标未全部完成）</summary>
    InProgress = 2,

    /// <summary>已完成（所有目标达成，等待领取奖励）</summary>
    Completed = 3,

    /// <summary>已领取（奖励已领取，任务归档）</summary>
    Claimed = 4
}

/// <summary>
/// 任务目标类型枚举
/// 每种类型对应不同的完成判定逻辑
/// </summary>
public enum ObjectiveType
{
    /// <summary>收集物品（如"收集 3 个铁矿石"）</summary>
    Collect = 0,

    /// <summary>消灭敌人（如"消灭 5 只史莱姆"）</summary>
    Kill = 1,

    /// <summary>与NPC对话（如"与村长对话"）</summary>
    Talk = 2,

    /// <summary>到达地点（如"前往矿洞入口"）</summary>
    Reach = 3
}

/// <summary>
/// 单个任务目标定义
/// 
/// 每个任务可包含多个目标，全部完成后任务进入"已完成"状态
/// targetId 用于匹配游戏事件（如物品ID、怪物ID、NPC名称、地点名称）
/// </summary>
[Serializable]
public class QuestObjective
{
    /// <summary>目标类型（收集/消灭/对话/到达）</summary>
    public int type;

    /// <summary>
    /// 目标关联ID
    /// - Collect: 物品ID（如 "ore_iron"）
    /// - Kill:    敌人ID（如 "slime"）
    /// - Talk:    NPC ID（如 "village_chief"）
    /// - Reach:   地点ID（如 "mine_entrance"）
    /// </summary>
    public string targetId;

    /// <summary>需要达成的数量（如收集3个、消灭5只）</summary>
    public int targetCount;

    /// <summary>目标描述文字（显示在UI上），如 "收集铁矿石 x3"</summary>
    public string description;

    /// <summary>获取目标类型枚举</summary>
    public ObjectiveType Type => (ObjectiveType)type;
}

/// <summary>
/// 奖励中的单个物品条目
/// </summary>
[Serializable]
public class QuestRewardItem
{
    /// <summary>物品ID</summary>
    public string itemId;

    /// <summary>物品名称（用于显示，避免运行时查表）</summary>
    public string itemName;

    /// <summary>奖励数量</summary>
    public int quantity;
}

/// <summary>
/// 任务奖励定义
/// 包含金币奖励和物品奖励列表
/// </summary>
[Serializable]
public class QuestReward
{
    /// <summary>金币奖励</summary>
    public int gold;

    /// <summary>经验奖励</summary>
    public int exp;

    /// <summary>物品奖励列表</summary>
    public List<QuestRewardItem> items;
}

/// <summary>
/// 单个任务的完整定义（JSON → 反序列化）
/// 
/// 包含：基本信息、目标列表、奖励、前置任务链
/// 通过 questId 被系统引用
/// </summary>
[Serializable]
public class QuestDefinition
{
    /// <summary>任务唯一ID，如 "quest_001"</summary>
    public string questId;

    /// <summary>任务名称，如 "初出茅庐"</summary>
    public string questName;

    /// <summary>任务描述（显示在详情面板）</summary>
    public string description;

    /// <summary>任务分类（0=主线, 1=支线）</summary>
    public int category;

    /// <summary>任务目标列表（全部完成才算任务完成）</summary>
    public List<QuestObjective> objectives;

    /// <summary>任务奖励</summary>
    public QuestReward rewards;

    /// <summary>
    /// 前置任务ID（空字符串 = 无前置，直接可接取）
    /// 前置任务必须处于 Claimed 状态，本任务才解锁
    /// </summary>
    public string prerequisiteQuestId;

    /// <summary>
    /// 发布此任务的 NPC ID
    /// 用于确定哪个 NPC 头顶显示 "!" 并可以接取/提交任务
    /// </summary>
    public string giverNpcId;

    /// <summary>获取任务分类枚举</summary>
    public QuestCategory Category => (QuestCategory)category;
}

/// <summary>
/// 任务数据库（所有任务定义的集合）
/// 对应 quest_database.json
/// </summary>
[Serializable]
public class QuestDatabase
{
    /// <summary>所有任务定义列表</summary>
    public List<QuestDefinition> quests;

    /// <summary>按 questId 快速查找的缓存字典</summary>
    [NonSerialized] private Dictionary<string, QuestDefinition> _lookup;

    /// <summary>
    /// 根据 ID 查找任务定义
    /// 首次调用时构建查找字典，后续 O(1) 查找
    /// </summary>
    public QuestDefinition GetQuestById(string questId)
    {
        if (_lookup == null)
        {
            _lookup = new Dictionary<string, QuestDefinition>();
            if (quests != null)
            {
                foreach (var q in quests)
                    _lookup[q.questId] = q;
            }
        }

        _lookup.TryGetValue(questId, out QuestDefinition def);
        return def;
    }
}

/// <summary>
/// 单个任务的运行时状态数据
/// 
/// 跟踪玩家对每个任务的进度（各目标的当前完成数）
/// 这个类不从 JSON 加载，在运行时根据 QuestDefinition 创建
/// </summary>
[Serializable]
public class QuestRuntimeData
{
    /// <summary>任务ID，关联 QuestDefinition</summary>
    public string questId;

    /// <summary>当前状态</summary>
    public QuestStatus status;

    /// <summary>
    /// 各目标的当前进度（索引对应 QuestDefinition.objectives 列表）
    /// 例如 objectives[0].targetCount = 3，则 progress[0] 从 0 增长到 3
    /// </summary>
    public List<int> progress;

    public QuestRuntimeData(string questId, int objectiveCount)
    {
        this.questId = questId;
        this.status = QuestStatus.Available;
        this.progress = new List<int>();
        for (int i = 0; i < objectiveCount; i++)
            this.progress.Add(0);
    }
}

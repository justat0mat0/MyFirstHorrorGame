using System;
using System.Collections.Generic;

/// <summary>
/// 对话系统 —— 数据定义
/// 
/// 包含三个核心数据类：
/// - DialogData：一段完整的对话（对应一个 JSON 文件）
/// - DialogNode：单个对话节点（一句话）
/// - ChoiceData：分支选项（玩家可选择的选项）
/// 
/// 所有数据通过 JSON 配置文件加载，修改对话内容不需要改代码
/// </summary>

/// <summary>
/// 一段完整的对话，包含多个对话节点
/// 对应一个 JSON 配置文件（如 dialog_001.json）
/// </summary>
[Serializable]
public class DialogData
{
    /// <summary>对话唯一标识，如 "dialog_001"</summary>
    public string dialogId;

    /// <summary>该对话包含的所有节点列表</summary>
    public List<DialogNode> nodes;

    /// <summary>
    /// 根据 nodeId 查找对话节点
    /// 使用线性查找，对话节点数量通常很少（几个~几十个），无需优化
    /// </summary>
    /// <param name="nodeId">要查找的节点 ID</param>
    /// <returns>找到的节点，未找到返回 null</returns>
    public DialogNode GetNodeById(int nodeId)
    {
        if (nodes == null) return null;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].nodeId == nodeId)
                return nodes[i];
        }

        return null;
    }
}

/// <summary>
/// 单个对话节点 —— 对话中的一句话
/// 
/// 包含：说话人信息（名字、头像、位置）、对话文本、跳转逻辑
/// 跳转逻辑分两种：
///   1. 无分支：按 nextNodeId 顺序推进
///   2. 有分支：显示 choices 列表，由玩家选择后跳转
/// </summary>
[Serializable]
public class DialogNode
{
    /// <summary>节点 ID，对话内唯一编号，从 0 开始</summary>
    public int nodeId;

    /// <summary>说话人名字，如 "勇者"、"村长"</summary>
    public string speakerName;

    /// <summary>
    /// 说话人头像资源名（不含路径和扩展名）
    /// 如 "avatar_player"，运行时从 Resources/UI/ 加载
    /// </summary>
    public string speakerAvatar;

    /// <summary>
    /// 头像显示位置
    /// "left" = 左侧（通常是主角）
    /// "right" = 右侧（通常是 NPC）
    /// </summary>
    public string position;

    /// <summary>对话文本内容，支持换行（\n）</summary>
    public string content;

    /// <summary>
    /// 下一个节点的 ID
    /// -1 表示对话结束
    /// 当 choices 不为空时，此字段被忽略（由玩家选择决定跳转目标）
    /// </summary>
    public int nextNodeId;

    /// <summary>
    /// 分支选项列表
    /// 为 null 或空列表表示无分支，直接按 nextNodeId 推进
    /// </summary>
    public List<ChoiceData> choices;

    /// <summary>是否有分支选项</summary>
    public bool HasChoices => choices != null && choices.Count > 0;

    /// <summary>是否是对话的结束节点（无分支 且 nextNodeId == -1）</summary>
    public bool IsEndNode => !HasChoices && nextNodeId == -1;
}

/// <summary>
/// 分支选项数据
/// 显示为一个按钮，玩家点击后跳转到指定的目标节点
/// </summary>
[Serializable]
public class ChoiceData
{
    /// <summary>选项按钮上显示的文字，如 "当然！交给我吧！"</summary>
    public string choiceText;

    /// <summary>选择后跳转到的目标节点 ID</summary>
    public int targetNodeId;
}

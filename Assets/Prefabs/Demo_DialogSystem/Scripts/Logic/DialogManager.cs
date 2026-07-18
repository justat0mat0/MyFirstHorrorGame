using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话管理器 —— 整个对话系统的核心
/// 
/// 职责：
/// 1. 从 JSON 加载对话数据
/// 2. 管理对话流程（开始 → 逐句推进 → 分支处理 → 结束）
/// 3. 通过 C# 事件通知 UI 层更新显示
/// 
/// 使用方式：
///   DialogManager.Instance.StartDialog("dialog_001");
/// 
/// 设计原则：
/// - 单例模式，全局唯一
/// - 纯逻辑层，不直接操作任何 UI
/// - 通过事件与 UI 层解耦，方便替换 UI 实现
/// </summary>
public class DialogManager : MonoBehaviour
{
    #region 单例

    private static DialogManager _instance;

    /// <summary>全局单例访问点</summary>
    public static DialogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogManager>();
                if (_instance == null)
                    Debug.LogError("[Dialog] 场景中没有 DialogManager，请添加到场景中");
            }
            return _instance;
        }
    }

    #endregion

    #region 事件（UI 层通过订阅这些事件来更新显示）

    /// <summary>对话开始时触发</summary>
    public event Action OnDialogStarted;

    /// <summary>
    /// 切换到新的对话节点时触发
    /// 参数：当前节点数据（包含说话人、内容、头像等信息）
    /// </summary>
    public event Action<DialogNode> OnNodeChanged;

    /// <summary>
    /// 当前节点有分支选项时触发
    /// 参数：可选择的选项列表
    /// UI 层收到后应显示选项按钮
    /// </summary>
    public event Action<List<ChoiceData>> OnChoicesAvailable;

    /// <summary>对话结束时触发（最后一个节点播放完毕或手动结束）</summary>
    public event Action OnDialogEnded;

    #endregion

    #region 状态

    /// <summary>当前加载的对话数据</summary>
    private DialogData _currentDialog;

    /// <summary>当前正在显示的对话节点</summary>
    private DialogNode _currentNode;

    /// <summary>对话是否正在进行中</summary>
    public bool IsDialogActive { get; private set; }

    /// <summary>获取当前节点数据（只读）</summary>
    public DialogNode CurrentNode => _currentNode;

    /// <summary>获取当前对话数据（只读，用于 UI 层预加载头像等）</summary>
    public DialogData CurrentDialog => _currentDialog;

    /// <summary>对话配置参数（Inspector 可调）</summary>
    public DialogConfig Config { get; private set; }

    #endregion

    #region 生命周期

    private void Awake()
    {
        // 单例保护：如果已有实例，销毁自己
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // 获取同 GameObject 上的配置组件，没有则自动添加
        Config = GetComponent<DialogConfig>();
        if (Config == null)
        {
            Debug.LogWarning("[Dialog] 未找到 DialogConfig，已自动添加默认配置");
            Config = gameObject.AddComponent<DialogConfig>();
        }
    }

    #endregion

    #region 公开方法（外部调用接口）

    /// <summary>
    /// 开始一段对话
    /// </summary>
    /// <param name="dialogId">
    /// 对话 ID，对应 Resources/Config/ 下的 JSON 文件名（不含扩展名）
    /// 例如传入 "dialog_001"，会加载 Resources/Config/dialog_001.json
    /// </param>
    public void StartDialog(string dialogId)
    {
        // 防止重复启动
        if (IsDialogActive)
        {
            Debug.LogWarning($"[Dialog] 当前已有对话在进行中，无法启动 {dialogId}");
            return;
        }

        // 加载 JSON 对话数据
        DialogData dialog = LoadDialogData(dialogId);
        if (dialog == null)
        {
            Debug.LogError($"[Dialog] 加载对话数据失败：{dialogId}");
            return;
        }

        _currentDialog = dialog;
        IsDialogActive = true;

        Debug.Log($"[Dialog] ▶ 开始对话：{dialogId}（共 {dialog.nodes.Count} 个节点）");

        // 通知 UI 层：对话开始了
        OnDialogStarted?.Invoke();

        // 跳转到第一个节点（nodeId = 0）
        MoveToNode(0);
    }

    /// <summary>
    /// 推进到下一个节点（玩家点击"继续"时由 UI 层调用）
    /// </summary>
    /// <returns>true = 成功推进到下一节点，false = 对话结束或当前有未选择的分支</returns>
    public bool AdvanceToNext()
    {
        if (!IsDialogActive || _currentNode == null)
            return false;

        // 当前节点有分支选项 → 不允许直接推进，必须通过 SelectChoice() 选择
        if (_currentNode.HasChoices)
        {
            Debug.Log("[Dialog] 当前节点有分支选项，请先选择");
            return false;
        }

        // 当前节点是结束节点 → 结束对话
        if (_currentNode.IsEndNode)
        {
            EndDialog();
            return false;
        }

        // 正常推进到下一个节点
        MoveToNode(_currentNode.nextNodeId);
        return true;
    }

    /// <summary>
    /// 玩家选择了某个分支选项（由选项按钮的点击回调调用）
    /// </summary>
    /// <param name="choiceIndex">选项索引，从 0 开始</param>
    public void SelectChoice(int choiceIndex)
    {
        if (!IsDialogActive || _currentNode == null || !_currentNode.HasChoices)
        {
            Debug.LogWarning("[Dialog] SelectChoice 无效：当前没有可选分支");
            return;
        }

        if (choiceIndex < 0 || choiceIndex >= _currentNode.choices.Count)
        {
            Debug.LogError($"[Dialog] 无效选项索引：{choiceIndex}（范围 0~{_currentNode.choices.Count - 1}）");
            return;
        }

        ChoiceData choice = _currentNode.choices[choiceIndex];
        Debug.Log($"[Dialog] 玩家选择：「{choice.choiceText}」→ 跳转节点 {choice.targetNodeId}");

        // 跳转到选项指定的目标节点
        MoveToNode(choice.targetNodeId);
    }

    #endregion

    #region 内部方法

    /// <summary>
    /// 跳转到指定 ID 的对话节点
    /// 这是对话流转的核心方法
    /// </summary>
    private void MoveToNode(int nodeId)
    {
        DialogNode node = _currentDialog.GetNodeById(nodeId);
        if (node == null)
        {
            Debug.LogError($"[Dialog] 找不到节点 ID={nodeId}，对话数据可能有误");
            EndDialog();
            return;
        }

        _currentNode = node;

        // 记录每个节点切换，方便调试对话流程
        string textPreview = node.content.Length > 20 ? node.content.Substring(0, 20) + "..." : node.content;
        string choiceInfo = node.HasChoices ? $"（有{node.choices.Count}个选项）" : node.IsEndNode ? "（结束节点）" : "";
        Debug.Log($"[Dialog] → 节点{node.nodeId}｜{node.speakerName}：{textPreview}{choiceInfo}");

        // 通知 UI 层：节点切换了，请更新显示
        OnNodeChanged?.Invoke(node);

        // 如果该节点有分支选项，额外通知 UI 层显示选项按钮
        if (node.HasChoices)
        {
            OnChoicesAvailable?.Invoke(node.choices);
        }
    }

    /// <summary>
    /// 结束当前对话，清理状态
    /// </summary>
    private void EndDialog()
    {
        Debug.Log($"[Dialog] ■ 对话结束：{_currentDialog?.dialogId}");

        IsDialogActive = false;
        _currentDialog = null;
        _currentNode = null;

        // 通知 UI 层：对话结束了，请关闭面板
        OnDialogEnded?.Invoke();
    }

    /// <summary>
    /// 从 Resources 文件夹加载对话 JSON 数据
    /// 加载路径：Resources/Config/{dialogId}.json
    /// </summary>
    private DialogData LoadDialogData(string dialogId)
    {
        string path = $"Config/{dialogId}";
        TextAsset jsonFile = Resources.Load<TextAsset>(path);

        if (jsonFile == null)
        {
            Debug.LogError($"[Dialog] 文件不存在：Resources/{path}.json");
            return null;
        }

        try
        {
            DialogData data = JsonUtility.FromJson<DialogData>(jsonFile.text);
            Debug.Log($"[Dialog] 成功加载：{data.dialogId}（{data.nodes.Count} 个节点）");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Dialog] JSON 解析失败：{e.Message}");
            return null;
        }
    }

    #endregion
}

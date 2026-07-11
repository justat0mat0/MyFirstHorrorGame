/// <summary>
/// 可交互对象接口
/// 
/// 所有能被玩家按 E 交互的物体都实现此接口：
/// NPC、可拾取物品等。
/// </summary>
public interface IInteractable
{
    /// <summary>交互对象的名称（用于 log 和 UI 提示）</summary>
    string InteractableName { get; }

    /// <summary>当前是否可交互</summary>
    bool CanInteract();

    /// <summary>执行交互</summary>
    void OnInteract(PlayerController2D player);
}

using UnityEngine;

/// <summary>
/// 区域触发器
/// 
/// 职责：
/// 1. 玩家进入区域时上报 Reach 事件
/// 2. 显示区域名称标签
/// 
/// 挂载在地标 GameObject 上。
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class AreaTrigger : MonoBehaviour
{
    [Header("── 区域配置 ──")]
    [Tooltip("区域标识符（对应 quest 中的 targetId，如 mine_entrance / forest_edge）")]
    [SerializeField] private string _areaId = "mine_entrance";
    [Tooltip("区域显示名称")]
    [SerializeField] private string _areaName = "矿洞入口";
    [Tooltip("触发器大小")]
    [SerializeField] private Vector2 _triggerSize = new Vector2(3f, 3f);

    // ── 运行时 ──
    private bool _triggered;

    private void Awake()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = _triggerSize;
    }

    private void Start()
    {
        // 创建区域名称标签
        CreateLabel();
    }

    private void CreateLabel()
    {
        GameObject labelGo = new GameObject("AreaLabel");
        labelGo.transform.SetParent(transform, false);
        labelGo.transform.localPosition = new Vector3(0, 1.5f, 0);

        TextMesh tm = labelGo.AddComponent<TextMesh>();
        tm.text = _areaName;
        tm.characterSize = 0.15f;
        tm.fontSize = 48;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(1f, 1f, 1f, 0.8f);

        // 简单背景（用子物体的 SpriteRenderer）
        // 这里简化，仅用文字
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 上报到达事件
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ReportEvent(ObjectiveType.Reach, _areaId, 1);
        }

        Debug.Log($"[Quest] 到达区域：{_areaName} (id={_areaId})");

        if (QuestToastUI.Instance != null)
        {
            QuestToastUI.Instance.ShowToast($"到达 {_areaName}");
        }
    }
}

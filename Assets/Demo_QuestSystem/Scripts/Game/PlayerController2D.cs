using UnityEngine;

/// <summary>
/// 2D 玩家控制器
/// 
/// 职责：
/// 1. WASD / 方向键移动（俯视角 2D）
/// 2. 按 E 键与附近 NPC 交互
/// 3. 管理金币
/// 4. 控制 SpriteRenderer 朝向
/// 
/// 挂载在 Player GameObject 上，需要 Rigidbody2D + BoxCollider2D。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    #region 单例

    private static PlayerController2D _instance;
    public static PlayerController2D Instance => _instance;

    #endregion

    #region 配置

    [Header("── 移动 ──")]
    [Tooltip("移动速度（单位/秒）")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("── 交互 ──")]
    [Tooltip("交互检测半径")]
    [SerializeField] private float _interactRadius = 1.5f;

    [Header("── 属性 ──")]
    [SerializeField] private int _gold = 0;

    #endregion

    #region 状态

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private bool _inputLocked; // UI 打开时锁定输入

    /// <summary>当前附近可交互的对象</summary>
    private IInteractable _nearbyInteractable;

    public int Gold => _gold;
    public bool InputLocked { get => _inputLocked; set => _inputLocked = value; }

    // 事件
    public event System.Action OnStatsChanged;

    #endregion

    #region 生命周期

    private void Awake()
    {
        _instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // 配置 Rigidbody2D
        _rb.gravityScale = 0;          // 俯视角无重力
        _rb.freezeRotation = true;      // 不旋转
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        if (_inputLocked) return;

        // ── 交互 ──
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void FixedUpdate()
    {
        if (_inputLocked)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        // ── 移动 ──
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(h, v).normalized * _moveSpeed;
        _rb.velocity = move;

        // 翻转 Sprite
        if (h != 0 && _spriteRenderer != null)
        {
            _spriteRenderer.flipX = h < 0;
        }
    }

    #endregion

    #region 交互

    /// <summary>检测附近可交互对象并交互</summary>
    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _interactRadius);
        float minDist = float.MaxValue;
        IInteractable closest = null;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = interactable;
                }
            }
        }

        if (closest != null)
        {
            Debug.Log($"[Quest] 玩家与 {closest.InteractableName} 交互");
            closest.OnInteract(this);
        }
    }

    /// <summary>设置当前附近的可交互对象（由 trigger 回调设置）</summary>
    public void SetNearbyInteractable(IInteractable interactable)
    {
        _nearbyInteractable = interactable;
    }

    public void ClearNearbyInteractable(IInteractable interactable)
    {
        if (_nearbyInteractable == interactable)
            _nearbyInteractable = null;
    }

    #endregion

    #region 经济

    /// <summary>增加金币</summary>
    public void AddGold(int amount)
    {
        _gold += amount;
        Debug.Log($"[Quest] 获得 {amount} 金币，当前: {_gold}");
        OnStatsChanged?.Invoke();
    }

    /// <summary>消耗金币</summary>
    public bool SpendGold(int amount)
    {
        if (_gold < amount) return false;
        _gold -= amount;
        OnStatsChanged?.Invoke();
        return true;
    }

    #endregion

    #region 调试

    private void OnDrawGizmosSelected()
    {
        // 交互范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }

    #endregion
}

using UnityEngine;

/// <summary>
/// 可拾取物品
/// 
/// 职责：
/// 1. 玩家走近时自动拾取（Trigger 碰撞）
/// 2. 拾取后上报 Collect 事件给 QuestManager
/// 3. 一段时间后自动重生（Demo 循环）
/// 
/// 挂载在 Item GameObject 上，需要 BoxCollider2D(isTrigger)。
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class CollectibleItem : MonoBehaviour
{
    [Header("── 物品配置 ──")]
    [Tooltip("物品标识符（对应 quest 中的 targetId，如 ore_iron / herb / wood）")]
    [SerializeField] private string _itemId = "ore_iron";
    [Tooltip("显示名称")]
    [SerializeField] private string _itemName = "铁矿石";

    [Header("── 重生 ──")]
    [Tooltip("拾取后重生时间（秒）")]
    [SerializeField] private float _respawnTime = 8f;

    // ── 运行时 ──
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;
    private bool _collected;
    private float _respawnTimer;

    // ── 音效 ──
    private AudioClip _sfxPickup;
    private AudioSource _audioSource;

    public string ItemId => _itemId;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
        _collider.size = new Vector2(0.6f, 0.6f);

        // 加载拾取音效
        _sfxPickup = Resources.Load<AudioClip>("Audio/sfx_pickup");
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (!_collected) return;

        _respawnTimer -= Time.deltaTime;
        if (_respawnTimer <= 0)
        {
            Respawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        Collect();
    }

    private void Collect()
    {
        _collected = true;
        _respawnTimer = _respawnTime;

        // 播放拾取音效
        if (_sfxPickup != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_sfxPickup);
        }

        // 隐藏
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = false;
        _collider.enabled = false;

        // 上报收集事件
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ReportEvent(ObjectiveType.Collect, _itemId, 1);
        }

        Debug.Log($"[Quest] 拾取了 {_itemName} (id={_itemId})");

        // Toast
        if (QuestToastUI.Instance != null)
        {
            QuestToastUI.Instance.ShowToast($"拾取了 {_itemName}");
        }
    }

    private void Respawn()
    {
        _collected = false;
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = true;
        _collider.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 对话 UI 控制器 —— 管理对话面板的视觉呈现
/// 
/// 职责：
/// 1. 监听 DialogManager 事件，更新 UI 显示
/// 2. 打字机效果（逐字显示文本）
/// 3. 说话人头像和名字切换（左右高亮）
/// 4. 处理玩家点击（跳过打字 / 推进对话）
/// 5. 继续箭头的显示/隐藏/闪烁
/// 6. 播放打字音效和点击音效
/// 
/// 使用方式：
///   挂载到 DialogPanel 预制体的根节点上
///   在 Inspector 中拖拽绑定各 UI 元素引用
/// </summary>
public class DialogUIController : MonoBehaviour, IPointerClickHandler
{
    #region UI 元素引用（在 Inspector 中拖拽绑定）

    [Header("── 对话框 ──")]
    [Tooltip("说话人名字")]
    [SerializeField] private Text txtSpeakerName;

    [Tooltip("对话内容文本（打字机效果作用于此）")]
    [SerializeField] private Text txtDialogContent;

    [Tooltip("继续提示箭头（打字完成后显示，闪烁提示玩家点击）")]
    [SerializeField] private Image imgContinueArrow;

    [Header("── 头像 ──")]
    [Tooltip("左侧头像（通常是主角）")]
    [SerializeField] private Image imgAvatarLeft;

    [Tooltip("右侧头像（通常是 NPC）")]
    [SerializeField] private Image imgAvatarRight;

    [Header("── 面板容器 ──")]
    [Tooltip("对话框容器（e_DialogBox），用于整体显示/隐藏")]
    [SerializeField] private GameObject dialogBoxContainer;

    [Tooltip("半透明遮罩（可选，对话时压暗背景）")]
    [SerializeField] private Image imgMask;

    #endregion

    #region 内部状态

    /// <summary>打字机协程引用</summary>
    private Coroutine _typingCoroutine;

    /// <summary>箭头闪烁协程引用</summary>
    private Coroutine _arrowBlinkCoroutine;

    /// <summary>当前完整的对话文本（打字机的目标文本）</summary>
    private string _fullText = "";

    /// <summary>打字机是否正在运行</summary>
    private bool _isTyping;

    /// <summary>当前节点是否有分支选项</summary>
    private bool _currentNodeHasChoices;

    /// <summary>DialogManager 引用缓存</summary>
    private DialogManager _manager;

    /// <summary>音频播放器（用于 SFX）</summary>
    private AudioSource _audioSource;

    /// <summary>BGM 专用播放器（独立于 SFX，互不干扰）</summary>
    private AudioSource _bgmSource;

    /// <summary>打字音效</summary>
    private AudioClip _sfxTyping;

    /// <summary>点击音效</summary>
    private AudioClip _sfxClick;

    /// <summary>对话背景音乐</summary>
    private AudioClip _bgmClip;

    #endregion

    #region 生命周期

    private void Start()
    {
        _manager = DialogManager.Instance;
        if (_manager == null) return;

        // 订阅 DialogManager 的事件
        _manager.OnDialogStarted += HandleDialogStarted;
        _manager.OnNodeChanged += HandleNodeChanged;
        _manager.OnChoicesAvailable += HandleChoicesAvailable;
        _manager.OnDialogEnded += HandleDialogEnded;

        // 初始化音频
        InitAudio();

        // 初始状态：隐藏对话面板
        SetPanelVisible(false);
    }

    private void OnDestroy()
    {
        // 取消订阅，防止内存泄漏
        if (_manager != null)
        {
            _manager.OnDialogStarted -= HandleDialogStarted;
            _manager.OnNodeChanged -= HandleNodeChanged;
            _manager.OnChoicesAvailable -= HandleChoicesAvailable;
            _manager.OnDialogEnded -= HandleDialogEnded;
        }
    }

    #endregion

    #region 音效

    /// <summary>
    /// 初始化音频组件和音效资源
    /// AudioSource 挂载在 DialogPanel 根节点上
    /// 音效从 Resources/Audio/ 自动加载，无需手动拖拽
    /// </summary>
    private void InitAudio()
    {
        // ─── SFX AudioSource ───
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.spatialBlend = 0f;
        _audioSource.playOnAwake = false;

        // ─── BGM AudioSource（独立于 SFX，避免 PlayOneShot 互相干扰）───
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.spatialBlend = 0f;
        _bgmSource.playOnAwake = false;
        _bgmSource.loop = true; // BGM 循环播放

        // ─── 加载音效资源 ───
        _sfxTyping = Resources.Load<AudioClip>("Audio/sfx_typing");
        _sfxClick = Resources.Load<AudioClip>("Audio/sfx_click");
        _bgmClip = Resources.Load<AudioClip>("Audio/bgm_dialog");

        if (_sfxTyping == null)
            Debug.LogWarning("[Dialog] 未找到打字音效：Resources/Audio/sfx_typing.wav");
        if (_sfxClick == null)
            Debug.LogWarning("[Dialog] 未找到点击音效：Resources/Audio/sfx_click.wav");
        if (_bgmClip == null)
            Debug.LogWarning("[Dialog] 未找到 BGM：Resources/Audio/bgm_dialog（可选，放入后自动生效）");

        // 打印当前音效配置，方便排查
        var cfg = _manager.Config;
        Debug.Log($"[Dialog] 音效初始化完成 | typing={cfg.enableTypingSound}(vol={cfg.typingSoundVolume}) click={cfg.enableClickSound}(vol={cfg.clickSoundVolume}) bgm={cfg.enableBGM}(vol={cfg.bgmVolume}) | sfx_typing={(_sfxTyping != null ? "OK" : "MISS")} sfx_click={(_sfxClick != null ? "OK" : "MISS")} bgm={(_bgmClip != null ? "OK" : "MISS")}");
    }

    /// <summary>播放打字音效（每个字触发一次）</summary>
    private void PlayTypingSound()
    {
        if (!_manager.Config.enableTypingSound || _sfxTyping == null || _audioSource == null)
            return;
        _audioSource.PlayOneShot(_sfxTyping, _manager.Config.typingSoundVolume);
    }

    /// <summary>播放点击音效（推进对话时触发）</summary>
    private void PlayClickSound()
    {
        if (!_manager.Config.enableClickSound || _sfxClick == null || _audioSource == null)
            return;
        _audioSource.PlayOneShot(_sfxClick, _manager.Config.clickSoundVolume);
    }

    /// <summary>获取 AudioSource（供 ChoiceUIController 共用）</summary>
    public AudioSource SharedAudioSource => _audioSource;

    /// <summary>获取点击音效（供 ChoiceUIController 共用）</summary>
    public AudioClip SharedClickSFX => _sfxClick;

    /// <summary>开始播放对话 BGM（循环）</summary>
    private void StartBGM()
    {
        if (!_manager.Config.enableBGM || _bgmClip == null || _bgmSource == null)
            return;

        _bgmSource.clip = _bgmClip;
        _bgmSource.volume = _manager.Config.bgmVolume;
        _bgmSource.Play();
        Debug.Log("[Dialog] BGM 开始播放");
    }

    /// <summary>停止对话 BGM</summary>
    private void StopBGM()
    {
        if (_bgmSource != null && _bgmSource.isPlaying)
        {
            _bgmSource.Stop();
            Debug.Log("[Dialog] BGM 停止播放");
        }
    }

    #endregion

    #region 事件处理（响应 DialogManager 的状态变化）

    /// <summary>对话开始 → 显示面板 + 预加载头像 + 播放 BGM</summary>
    private void HandleDialogStarted()
    {
        SetPanelVisible(true);
        PreloadAvatars();
        StartBGM();
    }

    /// <summary>
    /// 预加载所有角色头像
    /// 在对话开始时扫描所有节点，为左右两侧分别加载第一个出现的头像，
    /// 避免第一句话时某一侧头像为空白
    /// </summary>
    private void PreloadAvatars()
    {
        DialogData dialog = _manager.CurrentDialog;
        if (dialog == null || dialog.nodes == null) return;

        bool leftSet = false, rightSet = false;

        foreach (var node in dialog.nodes)
        {
            // 两侧都设置好了就不用继续扫描
            if (leftSet && rightSet) break;
            if (string.IsNullOrEmpty(node.speakerAvatar)) continue;

            Sprite sprite = Resources.Load<Sprite>($"UI/{node.speakerAvatar}");
            if (sprite == null) continue;

            if (node.position == "left" && !leftSet && imgAvatarLeft != null)
            {
                imgAvatarLeft.sprite = sprite;
                leftSet = true;
                Debug.Log($"[Dialog] 预加载左侧头像：{node.speakerAvatar}");
            }
            else if (node.position == "right" && !rightSet && imgAvatarRight != null)
            {
                imgAvatarRight.sprite = sprite;
                rightSet = true;
                Debug.Log($"[Dialog] 预加载右侧头像：{node.speakerAvatar}");
            }
        }
    }

    /// <summary>
    /// 节点切换 → 更新所有 UI 元素
    /// 这是最核心的方法：每当 DialogManager 切换到新节点时调用
    /// </summary>
    private void HandleNodeChanged(DialogNode node)
    {
        _currentNodeHasChoices = node.HasChoices;

        // 1. 更新说话人名字
        if (txtSpeakerName != null)
            txtSpeakerName.text = node.speakerName ?? "";

        // 2. 更新头像
        UpdateAvatar(node);

        // 3. 隐藏继续箭头（等打字完成后再显示）
        SetContinueArrowVisible(false);

        // 4. 启动打字机效果
        StartTyping(node.content);
    }

    /// <summary>分支选项可用 → 标记状态（选项按钮的显示由 ChoiceUIController 负责）</summary>
    private void HandleChoicesAvailable(List<ChoiceData> choices)
    {
        _currentNodeHasChoices = true;
        // 有选项时不显示继续箭头（等玩家选择）
    }

    /// <summary>对话结束 → 停止 BGM + 隐藏面板</summary>
    private void HandleDialogEnded()
    {
        StopAllEffects();
        StopBGM();
        SetPanelVisible(false);
    }

    #endregion

    #region 打字机效果

    /// <summary>开始打字机效果</summary>
    private void StartTyping(string text)
    {
        StopAllEffects();
        _fullText = text ?? "";
        _typingCoroutine = StartCoroutine(TypeText(_fullText));
    }

    /// <summary>
    /// 打字机协程：逐字显示文本
    /// 每次显示一个字符，间隔由 DialogConfig.CharInterval 控制
    /// 同时播放打字音效（如果启用）
    /// </summary>
    private IEnumerator TypeText(string text)
    {
        _isTyping = true;
        txtDialogContent.text = "";

        // 获取每个字符的间隔时间
        float charInterval = _manager.Config.CharInterval;

        for (int i = 0; i < text.Length; i++)
        {
            // 逐字追加（使用 Substring 避免频繁字符串拼接）
            txtDialogContent.text = text.Substring(0, i + 1);

            // 播放打字音效（跳过空格和标点，减少密集感）
            char c = text[i];
            if (!char.IsWhiteSpace(c) && c != '，' && c != '。' && c != '、' && c != '！' && c != '？')
            {
                PlayTypingSound();
            }

            yield return new WaitForSeconds(charInterval);
        }

        // 打字完成
        OnTypingComplete();
    }

    /// <summary>跳过打字机，立即显示全文</summary>
    private void SkipTyping()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        txtDialogContent.text = _fullText;
        OnTypingComplete();
    }

    /// <summary>
    /// 打字机完成后的处理
    /// - 如果没有分支选项 → 显示继续箭头（提示玩家点击继续）
    /// - 如果有分支选项 → 不显示箭头（等玩家选择选项）
    /// </summary>
    private void OnTypingComplete()
    {
        _isTyping = false;
        _typingCoroutine = null;

        if (!_currentNodeHasChoices)
        {
            SetContinueArrowVisible(true);
            StartArrowBlink();
        }
    }

    /// <summary>停止所有打字机和箭头动画效果</summary>
    private void StopAllEffects()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
            _typingCoroutine = null;
        }
        StopArrowBlink();
        _isTyping = false;
    }

    #endregion

    #region 头像切换

    /// <summary>
    /// 更新头像显示
    /// 
    /// 逻辑：
    /// - 根据 node.position 决定头像在左还是右
    /// - 当前说话人的头像高亮（白色），另一侧变暗（灰色半透明）
    /// - 尝试从 Resources/UI/ 加载头像 Sprite，找不到则保持原样
    /// </summary>
    private void UpdateAvatar(DialogNode node)
    {
        // 尝试加载头像图片
        Sprite avatarSprite = null;
        if (!string.IsNullOrEmpty(node.speakerAvatar))
        {
            avatarSprite = Resources.Load<Sprite>($"UI/{node.speakerAvatar}");
            // 找不到也没关系，会显示白色方块（占位图状态）
        }

        bool isLeft = node.position == "left";

        // 设置头像图片（只在加载成功时替换）
        if (isLeft && imgAvatarLeft != null && avatarSprite != null)
            imgAvatarLeft.sprite = avatarSprite;
        else if (!isLeft && imgAvatarRight != null && avatarSprite != null)
            imgAvatarRight.sprite = avatarSprite;

        // 高亮当前说话人，暗化另一侧
        Color activeColor = Color.white;                              // 正常显示
        Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);     // 灰色半透明

        if (imgAvatarLeft != null)
            imgAvatarLeft.color = isLeft ? activeColor : inactiveColor;

        if (imgAvatarRight != null)
            imgAvatarRight.color = isLeft ? inactiveColor : activeColor;
    }

    #endregion

    #region 继续箭头

    /// <summary>设置继续箭头的显示/隐藏</summary>
    private void SetContinueArrowVisible(bool visible)
    {
        if (imgContinueArrow != null)
            imgContinueArrow.gameObject.SetActive(visible);
    }

    /// <summary>开始箭头闪烁动画</summary>
    private void StartArrowBlink()
    {
        StopArrowBlink();
        _arrowBlinkCoroutine = StartCoroutine(ArrowBlinkLoop());
    }

    /// <summary>停止箭头闪烁</summary>
    private void StopArrowBlink()
    {
        if (_arrowBlinkCoroutine != null)
        {
            StopCoroutine(_arrowBlinkCoroutine);
            _arrowBlinkCoroutine = null;
        }
    }

    /// <summary>
    /// 箭头闪烁协程
    /// 通过切换 Image.enabled 实现闪烁效果
    /// </summary>
    private IEnumerator ArrowBlinkLoop()
    {
        float interval = _manager.Config.arrowBlinkInterval;
        while (true)
        {
            if (imgContinueArrow != null)
                imgContinueArrow.enabled = !imgContinueArrow.enabled;
            yield return new WaitForSeconds(interval);
        }
    }

    #endregion

    #region 面板显示/隐藏

    /// <summary>设置整个对话面板的可见性</summary>
    private void SetPanelVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    #endregion

    #region 点击处理

    /// <summary>
    /// 处理玩家点击对话面板（实现 IPointerClickHandler 接口）
    /// 
    /// 点击逻辑（三种情况）：
    /// 1. 打字机正在运行 → 跳过打字，立即显示全文
    /// 2. 全文已显示，且没有分支选项 → 推进到下一句
    /// 3. 全文已显示，但有分支选项 → 不响应（等待玩家点击选项按钮）
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_manager == null || !_manager.IsDialogActive)
            return;

        if (_isTyping)
        {
            // 情况 1：正在打字 → 跳过，立即显示全文
            SkipTyping();
            PlayClickSound();
        }
        else if (!_currentNodeHasChoices)
        {
            // 情况 2：全文已显示，无分支 → 推进到下一句
            PlayClickSound();
            _manager.AdvanceToNext();
        }
        // 情况 3：有分支选项 → 不做任何事，等待选项按钮点击
    }

    #endregion
}

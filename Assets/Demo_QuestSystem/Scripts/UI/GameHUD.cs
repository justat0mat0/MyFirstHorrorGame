using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏 HUD 界面
/// 
/// 职责：
/// 1. 显示玩家金币数
/// 2. 显示操作提示（WASD移动 / E交互）
/// 
/// 挂载在 HUD_Canvas 上，独立于任务面板。
/// </summary>
public class GameHUD : MonoBehaviour
{
    private Text _txtGold;
    private Text _txtTips;

    private void Awake()
    {
        BuildHUD();
    }

    private void Start()
    {
        // 延迟订阅（等 PlayerController2D 创建好）
        Invoke(nameof(SubscribeEvents), 0.2f);
        Invoke(nameof(RefreshAll), 0.3f);
    }

    private void SubscribeEvents()
    {
        var player = PlayerController2D.Instance;
        if (player != null)
        {
            player.OnStatsChanged += RefreshAll;
        }
    }

    private void BuildHUD()
    {
        // ── 左上角：金币（大号面板，竖屏视频也清晰） ──
        GameObject statsPanel = CreatePanel("StatsPanel", transform,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20),
            new Vector2(320, 60), new Color(0.05f, 0.08f, 0.12f, 0.82f));

        _txtGold = CreateHUDText("txt_gold", statsPanel.transform,
            new Vector2(15, -10), new Vector2(290, 44),
            "💰 金币: 0", 30, new Color(1f, 0.85f, 0.3f));
        _txtGold.fontStyle = FontStyle.Bold;

        // ── 底部居中：操作提示（加大字号和面板） ──
        GameObject tipsPanel = CreatePanel("TipsPanel", transform,
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 20),
            new Vector2(620, 50), new Color(0.05f, 0.08f, 0.12f, 0.7f));

        _txtTips = CreateHUDText("txt_tips", tipsPanel.transform,
            new Vector2(0, 0), new Vector2(620, 50),
            "WASD 移动  |  E 与 NPC 交互  |  走近物品自动拾取", 24,
            new Color(0.7f, 0.75f, 0.8f));
        _txtTips.alignment = TextAnchor.MiddleCenter;
        RectTransform tipsRect = _txtTips.GetComponent<RectTransform>();
        tipsRect.anchorMin = Vector2.zero;
        tipsRect.anchorMax = Vector2.one;
        tipsRect.offsetMin = Vector2.zero;
        tipsRect.offsetMax = Vector2.zero;
    }

    private void RefreshAll()
    {
        var player = PlayerController2D.Instance;
        if (player == null) return;

        if (_txtGold != null)
            _txtGold.text = $"💰 金币: {player.Gold}";
    }

    private GameObject CreatePanel(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, Color bgColor)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = anchorMin;
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        Image bg = go.AddComponent<Image>();
        bg.color = bgColor;
        bg.raycastTarget = false;

        return go;
    }

    private Text CreateHUDText(string name, Transform parent,
        Vector2 localPos, Vector2 size, string text, int fontSize, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = localPos;
        rect.sizeDelta = size;

        Text txt = go.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = fontSize;
        txt.color = color;
        txt.alignment = TextAnchor.MiddleLeft;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.raycastTarget = false;

        return txt;
    }

    private void OnDestroy()
    {
        var player = PlayerController2D.Instance;
        if (player != null)
            player.OnStatsChanged -= RefreshAll;
    }
}

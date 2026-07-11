using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;

/// <summary>
/// UI 层级结构打印工具
/// 
/// 使用方式：
///   在 Hierarchy 窗口选中任意 GameObject，
///   右键 → Print Hierarchy（简洁版）
///   右键 → Print Hierarchy Detail（详细版，含尺寸、位置、组件标签）
/// 
/// 输出会打印到 Console，并自动复制到剪贴板
/// 适合复制给 AI 分析 UI 结构
/// </summary>
public class DemoHierarchyPrinter
{
    [MenuItem("GameObject/Print Hierarchy", false, 0)]
    static void PrintHierarchy()
    {
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null)
        {
            Debug.LogError("[HierarchyPrinter] 请先选择一个 GameObject！");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"--- Structure of '{selectedObj.name}' ---");
        Traverse(selectedObj.transform, "", sb, true);

        Debug.Log(sb.ToString());
        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log("[HierarchyPrinter] 层级结构已复制到剪贴板！");
    }

    [MenuItem("GameObject/Print Hierarchy Detail", false, 1)]
    static void PrintHierarchyDetail()
    {
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null)
        {
            Debug.LogError("[HierarchyPrinter] 请先选择一个 GameObject！");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"--- Detail of '{selectedObj.name}' ---");
        TraverseDetail(selectedObj.transform, "", sb, true, 0);

        Debug.Log(sb.ToString());
        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log("[HierarchyPrinter] 详细层级已复制到剪贴板！");
    }

    /// <summary>简洁版遍历：只输出名称</summary>
    static void Traverse(Transform current, string indent, StringBuilder sb, bool isLast)
    {
        string marker = isLast ? "└── " : "├── ";
        sb.AppendLine(indent + marker + current.name);
        string nextIndent = indent + (isLast ? "    " : "│   ");

        for (int i = 0; i < current.childCount; i++)
        {
            bool isLastChild = (i == current.childCount - 1);
            Traverse(current.GetChild(i), nextIndent, sb, isLastChild);
        }
    }

    /// <summary>
    /// 详细版遍历：输出名称、尺寸、位置、关键组件标签
    /// 最大深度 8 层，避免超大层级卡死
    /// </summary>
    static void TraverseDetail(Transform current, string indent, StringBuilder sb, bool isLast, int depth)
    {
        string marker = isLast ? "└── " : "├── ";

        var rt = current as RectTransform;
        string info = current.name;

        // 标记 inactive
        if (!current.gameObject.activeSelf) info += " [INACTIVE]";

        // RectTransform 信息
        if (rt != null)
        {
            var rect = rt.rect;
            info += $" ({rect.width:F0}x{rect.height:F0})";
            info += $" pos=({rt.anchoredPosition.x:F0},{rt.anchoredPosition.y:F0})";
        }

        // 关键组件标签
        var tags = new System.Collections.Generic.List<string>();

        if (current.GetComponent<Canvas>() != null) tags.Add("Canvas");
        if (current.GetComponent<Image>() != null) tags.Add("Img");
        if (current.GetComponent<Text>() != null) tags.Add("Txt");
        if (current.GetComponent<Button>() != null) tags.Add("Btn");
        if (current.GetComponent<ScrollRect>() != null) tags.Add("Scroll");
        if (current.GetComponent<GridLayoutGroup>() != null) tags.Add("Grid");
        if (current.GetComponent<HorizontalLayoutGroup>() != null) tags.Add("HLayout");
        if (current.GetComponent<VerticalLayoutGroup>() != null) tags.Add("VLayout");
        if (current.GetComponent<ContentSizeFitter>() != null) tags.Add("CSF");
        if (current.GetComponent<Mask>() != null || current.GetComponent<RectMask2D>() != null) tags.Add("Mask");
        if (current.GetComponent<LayoutElement>() != null)
        {
            var le = current.GetComponent<LayoutElement>();
            if (le.ignoreLayout) tags.Add("IgnoreLayout");
        }
        if (current.GetComponent<CanvasGroup>() != null)
        {
            var cg = current.GetComponent<CanvasGroup>();
            tags.Add($"CG(a={cg.alpha:F1})");
        }
        if (current.GetComponent<Toggle>() != null) tags.Add("Toggle");
        if (current.GetComponent<InputField>() != null) tags.Add("Input");
        if (current.GetComponent<Slider>() != null) tags.Add("Slider");

        if (tags.Count > 0) info += " [" + string.Join(",", tags) + "]";

        sb.AppendLine(indent + marker + info);
        string nextIndent = indent + (isLast ? "    " : "│   ");

        // 最大深度限制
        if (depth >= 8) return;

        for (int i = 0; i < current.childCount; i++)
        {
            bool isLastChild = (i == current.childCount - 1);
            TraverseDetail(current.GetChild(i), nextIndent, sb, isLastChild, depth + 1);
        }
    }

    [MenuItem("GameObject/Print Hierarchy", true)]
    static bool ValidateSelection() => Selection.activeGameObject != null;

    [MenuItem("GameObject/Print Hierarchy Detail", true)]
    static bool ValidateSelectionDetail() => Selection.activeGameObject != null;
}

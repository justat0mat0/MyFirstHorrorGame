using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    /// <summary>0 = 左侧角色，1 = 右侧角色</summary>
    [Range(0, 1)]
    public int speakerIndex;

    [TextArea(2, 6)]
    public string text;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "HorrorGame/对话数据", order = 0)]
public class DialogueData : ScriptableObject
{
    public Sprite portraitLeft;
    public Sprite portraitRight;

    public string nameLeft = "角色A";
    public string nameRight = "角色B";

    public DialogueLine[] lines;

    [Header("打字音效（可选，覆盖 DialogueUIController 上的默认音效）")]
    [Tooltip("不为空时，本段对话使用此音效作为打字声")]
    public AudioClip typingSoundOverride;
}

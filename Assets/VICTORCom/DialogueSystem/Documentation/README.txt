[快速工具 - 对话交互]：极简叙事交互与对话系统
ProDialogue & Environment Interaction Framework
一句话简介： 专为叙事向游戏设计的一站式解决方案，涵盖深度对话、物件检查及环境交互，即插即用，无代码冲突。

🌟 核心特性 (Key Features)
完全独立运行： 代码逻辑高度解耦，不依赖第三方库，不与项目原有逻辑冲突。

双重叙事维度： 同时支持 标准对话系统 与 环境/物件交互系统。

极速部署： 专为 Game Jam 与学生项目优化，3 分钟即可完成从零到一的交互配置。

高自由度事件钩子： 每一个对话节点均支持自定义事件触发，方便程序开发者通过代码扩展复杂功能。

自适应 UI： 预设 UI 完美适配多种分辨率，支持一键替换预制件以匹配不同美术风格。

🚀 快速开始 (Quick Start)
1. 环境准备
确保项目中已安装 TextMesh Pro (Unity 官方包)。

将插件文件夹拖入项目的 Assets 目录。

2. 三步配置
全局管理： 在场景中拖入 DialogueManager 预制件（处理 UI 与输入逻辑）。

物件挂载： 选中你的场景物体（如 NPC 或线索道具），挂载 DialogueInteractable 或 ObjectExaminer 脚本。

配置内容： 在 Inspector 面板中填入对话文字，并在 Scene 窗口中拖动调整 Interaction Radius (绿色球体范围)。

3. 运行测试
按下 Play，控制玩家靠近物体，按下交互键（默认 E）即可开启对话。

🛠️ 进阶：事件触发机制 (For Developers)
对于有编程基础的用户，你可以利用内置的 UnityEvent 在对话特定位置执行代码：

// 示例：在对话结束时打开密室大门
public void OnDialogueEnd() {
    doorAnimator.SetTrigger("Open");
    Debug.Log("对话结束，触发后续剧情逻辑！");
}
在 Inspector 面板中，只需将该函数拖入组件的 OnComplete 事件槽位即可。

📂 目录结构 (Folder Structure)
Prefabs/ - 核心管理对象与多种 UI 预制件。

Scripts/ - 逻辑核心，包含触发器、管理器与数据结构。

Demo/ - 包含一个完整的“调查解谜”示例场景（建议优先查看）。

Resources/ - 预设的 UI 素材与配置文件。

🎮 适用项目 (Use Cases)
叙事冒险 / 侦探解谜： 线索收集与逻辑推演。

走廊叙事 / 恐怖游戏： 环境独白与氛围塑造。

RPG / 独立游戏： 轻量化且稳健的交互基础。

Game Jam / 毕业设计： 以极低的时间成本获得极高的交互完成度。

⚠️ 技术说明与支持 (Support)
测试环境： Unity 2022.3 LTS (由于仅涉及 UI 与基础逻辑，向上/向下兼容性极佳)。

渲染管线： 完美支持 Built-in、URP 及 HDRP。

联系开发者： 如果你在使用过程中遇到任何问题，或有功能定制需求，请通过以下方式联系，我会在第一时间回复：

Email: [你的邮箱]

Social: [你的小红书/GitHub/B站链接]
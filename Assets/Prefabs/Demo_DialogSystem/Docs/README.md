# 🎮 对话系统 Demo — Dialog System

> 一个开箱即用的 Unity 对话系统模块，支持分支对话、打字机效果、头像切换，纯 JSON 配置驱动。

---

## ✨ 你能学到什么

| 核心能力 | 说明 |
|---------|------|
| **JSON 数据驱动架构** | 不改一行代码就能编辑所有对话内容，理解数据与逻辑分离的工程思想 |
| **分支对话系统** | 玩家选择影响后续对话走向，学会树状对话流的设计与实现 |
| **打字机效果** | 逐字显示 + 点击跳过，掌握协程动画的实用写法 |
| **头像与说话人切换** | 根据配置自动切换左右头像和说话人名字，理解 UI 状态管理 |
| **三层代码架构** | Data 层 / Logic 层 / UI 层完全分离，面试常考的 MVC 设计落地 |
| **事件驱动通信** | Manager 通过 C# 事件通知 UI 更新，低耦合高扩展 |

---

## 📦 包含内容

```
Demo_DialogSystem/
  ├── Scripts/
  │   ├── Data/          → 数据结构定义（DialogData, DialogNode, ChoiceData）
  │   ├── Logic/         → 对话管理器（加载、推进、分支、结束回调）
  │   ├── UI/            → UI 控制（打字机、头像切换、选项面板）
  │   └── Config/        → 可调参数（打字速度等）
  ├── Resources/
  │   ├── UI/            → UI 图片资源
  │   ├── Audio/         → 音效 + BGM 资源
  │   └── Config/        → JSON 对话配置文件
  ├── Prefabs/           → 对话面板 + 选项按钮预制体
  └── Scenes/            → 演示场景（打开即可运行）
```

---

## 🎯 功能一览

### 核心功能（全部已实现）
- ✅ **JSON 配置加载** — 从 JSON 文件读取对话数据，运行时解析
- ✅ **对话触发** — 调用 `DialogManager.Instance.StartDialog("dialog_001")` 一行代码启动
- ✅ **逐句推进** — 点击推进，到末尾自动结束
- ✅ **打字机效果** — 逐字显示，可配置速度，点击立即显示全文
- ✅ **说话人 & 头像** — 显示名字和头像，支持左右站位
- ✅ **分支选项** — 2~4 个选项按钮，玩家选择影响对话走向
- ✅ **结束回调** — 对话结束时触发事件，方便外部系统监听

### 音效 & BGM（已集成）
- 🔊 **打字音效** — 每个字显示时播放短促"嗒"声（标点和空格自动跳过）
- 🔊 **点击音效** — 推进对话和选择选项时播放升调"叮"声
- 🎵 **背景音乐** — 对话开始自动播放循环 BGM，对话结束自动停止
- 🔊 所有音效/BGM 可在 Inspector 中一键开关，音量可调

### 扩展接口（已预留）
- 🔌 表情/立绘切换接口
- 🔌 对话日志（回看历史对话）
- 🔌 条件分支（根据游戏状态显示/隐藏选项）
- 🔌 自动播放模式

---

## 🔧 环境要求

| 项目 | 要求 |
|------|------|
| **Unity 版本** | **2021.3 LTS** 或更新版本（推荐 2022.3 LTS） |
| **渲染管线** | 任意（Built-in / URP / HDRP 均可，本 demo 是纯 UI + 逻辑，不依赖渲染管线） |
| **项目模板** | 任意（2D / 3D / Universal 3D 都行） |
| **平台** | 全平台（PC / Mobile / WebGL 等） |

> 💡 本 demo 不使用任何第三方插件，不依赖特定渲染管线，导入即用。

---

## 🚀 快速开始

### 1. 导入 .unitypackage

1. 打开你的 Unity 项目（已有项目或新建项目均可）
2. 菜单栏 `Assets → Import Package → Custom Package...`
3. 选择 `Demo_DialogSystem.unitypackage`，点 **打开**
4. 弹出的导入窗口中，确保所有文件都勾选，点 **Import**
5. 等待导入完成，Project 面板中出现 `Demo_DialogSystem/` 文件夹即为成功

### 2. 运行演示

1. 在 Project 面板中进入 `Demo_DialogSystem/Scenes/`
2. 双击 **`DemoScene_Dialog`** 打开演示场景
3. 点击 **▶ Play** 运行
4. **操作方式**：
   - 点击屏幕任意位置 → 推进对话（打字中点击 = 立即显示全文）
   - 出现选项时 → 点击选项按钮选择分支
   - 对话结束后 → 按 **空格键** 可重新触发对话

> ⚠️ 如果运行后看不到对话，请检查 Game 视图的分辨率是否设为 **1920×1080**（Free Aspect 也可以，但 16:9 显示效果最佳）。

### 3. 自定义对话内容

编辑 `Resources/Config/dialog_001.json`，按以下格式添加/修改对话节点：

```json
{
  "nodeId": 0,
  "speakerName": "村长",
  "speakerAvatar": "avatar_npc_a",
  "position": "right",
  "content": "你好，欢迎来到我们的村庄。",
  "nextNodeId": 1,
  "choices": null
}
```

需要分支？给 `choices` 填上选项数组：

```json
"choices": [
  { "choiceText": "好的！", "targetNodeId": 3 },
  { "choiceText": "再见", "targetNodeId": 4 }
]
```

### 4. 添加新角色

1. 准备一张头像 PNG（建议 128×128 或 200×200）
2. 放到 `Resources/UI/` 目录，命名如 `avatar_npc_b.png`
3. 确保图片 Inspector 中 Texture Type 设为 `Sprite (2D and UI)`
4. 在 JSON 配置中使用 `"speakerAvatar": "avatar_npc_b"`
5. 运行即可看到新角色

### 5. 替换音效 / BGM

| 文件 | 路径 | 说明 |
|------|------|------|
| 打字音效 | `Resources/Audio/sfx_typing.wav` | 替换为你自己的短促音效（建议 < 50ms） |
| 点击音效 | `Resources/Audio/sfx_click.wav` | 替换为你自己的 UI 点击音效 |
| 背景音乐 | `Resources/Audio/bgm_dialog.wav` | 替换为你自己的 BGM（循环播放） |

> 文件名必须保持一致，代码通过文件名自动加载。也可以在 Inspector 中关闭不需要的音效。

---

## 🏗️ 代码架构

```
                  ┌──────────────────┐
                  │   JSON 配置文件    │  ← 数据源
                  └────────┬─────────┘
                           │ Resources.Load
                  ┌────────▼─────────┐
                  │   DialogData     │  ← 数据层（纯数据类）
                  │   DialogNode     │
                  │   ChoiceData     │
                  └────────┬─────────┘
                           │
                  ┌────────▼─────────┐
                  │  DialogManager   │  ← 逻辑层（单例管理器）
                  │  - StartDialog() │
                  │  - NextNode()    │     事件通知
                  │  - SelectChoice()│  ──────────────┐
                  └──────────────────┘                │
                                                      │
                  ┌───────────────────────────────────▼──┐
                  │            UI 层                       │
                  │  DialogUIController  （对话面板）        │
                  │  ChoiceUIController  （选项面板）        │
                  └────────────────────────────────────────┘
```

**设计思路**：
- **数据层**只管"是什么"（纯数据类，无 Unity 依赖）
- **逻辑层**只管"怎么走"（对话流程控制、状态机）
- **UI 层**只管"怎么显示"（接收事件、更新画面）
- 三层通过 **C# 事件 (Action)** 通信，改 UI 不影响逻辑，改逻辑不影响 UI

---

## ⚙️ 可调参数

在 `DialogConfig.cs` 中可以修改：

| 参数 | 默认值 | 说明 |
|------|--------|------|
| `TypingSpeed` | 0.05f | 每个字的显示间隔（秒），越小越快 |
| `enableTypingSound` | true | 是否启用打字音效 |
| `enableClickSound` | true | 是否启用点击音效 |
| `typingSoundVolume` | 0.3 | 打字音效音量（0~1） |
| `clickSoundVolume` | 0.5 | 点击音效音量（0~1） |
| `enableBGM` | true | 是否启用对话 BGM |
| `bgmVolume` | 0.3 | BGM 音量（0~1） |

---

## 📐 适用场景

- **RPG** — NPC 对话、任务接取
- **AVG / 视觉小说** — 剧情推进
- **独立游戏** — 过场剧情、教程引导
- **面试准备** — 展示你对对话系统架构的理解

---

## ❓ 常见问题

**Q：我的项目不是 URP，能用吗？**
A：可以。对话系统是纯 UI + 逻辑，不依赖渲染管线。导入后直接可用。

**Q：如何增加更多对话？**
A：在 `Resources/Config/` 下新建 JSON 文件（如 `dialog_002.json`），然后调用 `DialogManager.Instance.StartDialog("dialog_002")` 即可。

**Q：如何接入任务系统？**
A：监听 `DialogManager.OnDialogEnded` 事件，在回调中判断对话 ID 并触发任务逻辑。

**Q：每个类里的方法都有注释吗？**
A：是的。**每个 public 方法都有 summary 注释，关键算法有行内注释**，打开代码就能看懂。

---

## 📊 代码概览

| 脚本 | 行数 | 职责 |
|------|------|------|
| `DialogData.cs` | ~40 行 | 数据结构定义 + JSON 反序列化 |
| `DialogManager.cs` | ~120 行 | 对话流程管理（加载、推进、分支、结束） |
| `DialogUIController.cs` | ~200 行 | 打字机效果、头像切换、面板显示隐藏 |
| `ChoiceUIController.cs` | ~80 行 | 选项按钮生成与点击处理 |
| `DialogConfig.cs` | ~50 行 | 可调参数（打字速度、音效开关、音量等） |
| **合计** | **~600 行** | 注释详尽，结构清晰，含音效集成 |

---

> 💬 如有问题或建议，欢迎联系。
> 
> 🔗 更多游戏模块 Demo，请访问我的展示站。

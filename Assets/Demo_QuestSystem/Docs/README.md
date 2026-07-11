# 任务系统 Demo（Quest System）

> Unity 任务系统完整实现 —— JSON 驱动 + 2D 村庄场景 + NPC 交互 + 物品收集 + 进度追踪

## 功能一览

- ✅ **JSON 配置驱动**：任务数据全部在 JSON 中定义，修改任务不改代码
- ✅ **任务状态机**：未解锁 → 可接取 → 进行中 → 已完成 → 已领取
- ✅ **多种目标类型**：收集物品、与 NPC 对话、到达指定地点
- ✅ **任务链系统**：完成前置任务后自动解锁后续任务
- ✅ **2D 村庄场景**：可移动的玩家角色 + 3 个 NPC + 建筑 + 装饰物
- ✅ **NPC 交互系统**：走近 NPC 按 E 交互，接取/提交任务
- ✅ **物品拾取**：场景中散落的草药和木材，走近自动拾取
- ✅ **实时任务面板**：右侧始终显示任务列表，实时更新进度
- ✅ **奖励系统**：金币 + 经验奖励，完成后一键领取
- ✅ **Toast 提示**：任务状态变化时弹出提示动画
- ✅ **注释详尽**：每个类、每个方法都有详细中文注释

## 快速开始

1. 导入 `.unitypackage` 到你的 Unity 项目（推荐 Universal 3D 模板）
2. 打开场景 `Demo_QuestSystem/Scenes/DemoScene_Quest`
3. 点击 Play 运行
4. WASD 移动角色，走近 NPC 按 E 交互

## 操作说明

| 按键 | 功能 |
|------|------|
| W/A/S/D | 移动角色 |
| E | 与 NPC 交互（接取/提交任务） |

## 任务流程示例

```
1. 移动到村长旁边 → 按 E → 接取"拜访村长"任务 → 自动完成（对话即完成）
2. 移动到药师旁边 → 按 E → 接取"采集草药"任务
3. 到地图左侧拾取 3 个草药 → 回到药师 → 按 E 提交 → 领取奖励
4. 移动到建筑师旁边 → 按 E → 接取"帮助建筑师"任务
5. 到地图右侧拾取 4 个木材 → 回到建筑师 → 按 E 提交 → 领取奖励
```

## 工程结构

```
Demo_QuestSystem/
  Scripts/
    Data/QuestData.cs              ← 数据结构定义（任务、目标、奖励）
    Logic/QuestManager.cs          ← 任务管理器（核心逻辑）
    Logic/QuestDemoController.cs   ← Demo 入口（构建 2D 世界 + 初始化）
    UI/QuestUIController.cs        ← 任务面板 UI（始终显示的侧边栏）
    UI/QuestToastUI.cs             ← Toast 提示动画
    UI/GameHUD.cs                  ← 游戏 HUD（金币显示）
    Config/QuestConfig.cs          ← 配置参数
    Game/PlayerController2D.cs     ← 玩家移动控制
    Game/NPCController.cs          ← NPC 交互逻辑
    Game/CollectibleItem.cs        ← 可拾取物品
    Game/AreaTrigger.cs            ← 区域触发器
    Game/CameraFollow2D.cs         ← 相机跟随
    Game/IInteractable.cs          ← 交互接口
  Resources/
    Config/quest_database.json     ← 任务数据库
    Sprites/                       ← 2D 精灵资源
    UI/                            ← UI 图片资源
    Audio/                         ← 音效资源
  Scenes/DemoScene_Quest           ← 演示场景
```

## 如何自定义任务

编辑 `Resources/Config/quest_database.json`，按格式添加新任务即可：

```json
{
  "questId": "quest_new",
  "questName": "你的任务名",
  "description": "任务描述",
  "giverNpcId": "village_chief",
  "objectives": [
    { "type": 0, "targetId": "herb", "targetCount": 3, "description": "收集草药 x3" }
  ],
  "rewards": { "gold": 100, "exp": 50, "items": [] },
  "prerequisiteQuestId": ""
}
```

目标类型：`0=收集` `2=对话` `3=到达`

## 在你的游戏中使用

```csharp
// 初始化
QuestManager.Instance.Initialize();

// 上报游戏事件（自动推进匹配的任务目标）
QuestManager.Instance.ReportEvent(ObjectiveType.Collect, "herb", 1);
QuestManager.Instance.ReportEvent(ObjectiveType.Talk, "village_chief", 1);
QuestManager.Instance.ReportEvent(ObjectiveType.Reach, "village_square", 1);
```

## 联系作者

B站：晚上做游戏

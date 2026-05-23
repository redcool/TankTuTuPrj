# TankTuTu 项目文档

## 项目概述

**项目名称**: TankTuTu (坦克图图)
**项目类型**: Unity 3D Roguelike 俯视角射击游戏
**Unity版本**: 2022.3.62f2
**目标平台**: Steam

## 代码规范

### 1. MonoBehaviour 放置规则
- **MonoBehaviour 子类禁止放置在 Editor 目录**
- Editor 目录仅放置真正的编辑器工具类（如 MenuItem、EditorWindow 等）

### 2. Editor 脚本 MenuItem 规划
- **根菜单**: `铁皮突突`
- **分类**: `创建UI` / `创建数据` / `创建敌人` 等
- **示例**: `铁皮突突/创建UI/创建 HUD Canvas Prefab`

### 5. Prefab 创建流程

1. **编写 Editor 代码** - 在 `Assets/Scripts/Editor/` 目录创建 Creator 脚本
2. **编译通过** - 确保代码无编译错误
3. **执行 MenuItem** - 使用 `unityMCP_execute_menu_item` 调用菜单
4. **Prefab 生成** - 自动保存到 `Assets/Resources/Prefabs/UI/`

#### 示例：创建 HUDCanvas
```csharp
// 1. 编写 Editor 代码 (HUDCanvasCreator.cs)
[MenuItem("铁皮突突/创建UI/创建 HUD Canvas Prefab")]
public static void CreateHUDCanvas() { ... }

// 2. 编译通过后执行
unityMCP_execute_menu_item(menu_path="铁皮突突/创建UI/创建 HUD Canvas Prefab")
```

### 6. 当前 Prefab 状态

| 界面 | Prefab | 状态 |
|------|--------|------|
| HUDCanvas | HUDCanvas.prefab | ✅ 已创建 |
| StartMenuCanvas | StartMenuCanvas.prefab | ✅ 已存在 |
| CharacterSelectCanvas | CharacterSelectCanvas.prefab | ✅ 已存在 |
```
Assets/Scripts/
├── Runtime/           # 游戏运行时代码 (MonoBehaviour)
│   ├── Controller/    # 控制器
│   ├── View/         # 界面
│   └── ValueObject/  # 数据类
└── Editor/           # 编辑器工具 (纯Editor代码)
    └── [MenuItem("铁皮突突/...")]  # 使用 static 方法
```

### 4. Editor 脚本规范
- 使用 `[MenuItem("铁皮突突/...")]` 特性
- 使用 `static` 方法
- 放在 `Assets/Scripts/Editor/` 目录
- 使用 `#if UNITY_EDITOR` 包裹

## 核心玩法

类似土豆兄弟(Brotato)的俯视角射击游戏，区别在于：
- **主角是一辆战车**，而非人物
- 战车有 **6个武器槽位**，购买武器后自动安装

### 核心循环

```
开始界面 → 选择角色 → 选择武器 → 选择难度 
    → 关卡战斗(60秒) → 关卡结算 
    → 商场购买(武器/道具) → 下一关
```

### 游戏流程

1. **新游戏** → 选择角色界面 → 选择武器 → 选择难度 → 关卡开始
2. **继续游戏** → 读取存档 → 继续关卡
3. **战斗过程**: 玩家控制战车移动，武器自动瞄准怪物并射击
4. **资源收集**: 怪物击败后掉落能量块，靠近自动收集
5. **精英/Boss**: 击败后掉落宝箱
6. **关卡结算**: 60秒结束或所有玩家死亡后弹出
7. **商场界面**: 购买武器和道具，相同武器可合并升级

## 战车系统

### 15种属性

| 属性 | 说明 | MVP基准值 |
|------|------|-----------|
| 最大生命值 | 战车的耐久上限 | 300 (100实际HP) |
| 生命回复 | 每秒恢复生命值 | 0.5 HP/s |
| 生命窃取 | 攻击时恢复生命 | 2% |
| 百分比伤害 | 全局伤害加成 | 0% |
| 远程伤害 | 远程武器伤害加成 | 0% |
| 近战伤害 | 近战武器伤害加成 | 0% |
| 元素伤害 | 元素武器伤害加成 | 0% |
| 工程学 | 炮台/陷阱类武器加成 | 0% |
| 攻速 | 武器攻击速度加成 | +5% |
| 暴击率 | 暴击概率 | 5% |
| 范围 | 武器攻击范围 | 5 |
| 护甲 | 减少受到的伤害 | 0 |
| 闪避 | 闪避攻击的概率 | 0% |
| 移速 | 战车移动速度 | 3 |
| 幸运 | 提升稀有物品掉落率 | 0% |
| 收获 | 提升资源获取量 | x1.0 |

### 价值换算参考

- 1点移速 = 3价值 ≈ 10金币
- 1点最大生命 = 3价值 ≈ 30金币
- 1%攻速 = 6价值 ≈ 20金币

## 武器系统

### 武器槽位
- 战车有 **6个武器槽位** (Slot_0 ~ Slot_5)
- 购买武器后自动安装到第一个空闲槽位
- 槽位已满时提示"武器槽位已满"

### 自动瞄准
- 所有武器自动寻找范围内最近敌人
- 每把武器独立冷却、独立射击
- 无目标时保持待命状态

### MVP武器
- 铁球
- 枪械
- 激光
- 火焰

## 敌人系统

### 小怪
- **海狸(Beaver)** - 普通小怪
- **奶牛(Cow)** - 普通小怪
- 击败后掉落 1-3 个能量块

### Boss
- **大象(Elephant)** - Boss敌人
- 击败后掉落 1个宝箱

### 波次系统
- 按波次生成怪物
- 敌人追踪最近玩家并攻击

## 资源系统

### 能量块
- 击败普通怪物掉落
- 玩家靠近(距离<1.5米)自动收集

### 宝箱
- 击败Boss掉落
- 打开后可触发抽卡

## 关卡系统

- **时长**: 60秒倒计时
- **结算**: 计时结束或所有玩家死亡
- **通过条件**: 至少1名玩家存活
- **失败条件**: 所有玩家战车死亡

## 商城系统

- 购买武器和道具
- 商品刷新机制
- 相同武器可合并升级

## 抽卡系统

### 概率
- N(普通): 70%
- R(稀有): 25%
- SR(超稀有): 4%
- SSR(超级稀有): 1%

### 幸运值修正
- SR概率 + (幸运值 × 1)%
- SSR概率 + (幸运值 × 0.5)%

### 抽卡时机
- 关卡结算时
- 宝箱开启

## 升级系统

- 打怪获取经验
- 升级后关卡结算时可抽卡选属性

## 存档系统

- 本地JSON存储
- SteamCloud同步(预留)

## 多人系统

- **最多4人同屏**
- 使用InputSystem输入系统
- 各自独立的数据管理
- 手柄断开处理

## 技术规格

- **引擎**: Unity 2022.3.62f2
- **视角**: 3D 45度俯视角
- **渲染**: URP (Universal Render Pipeline)
- **输入**: Unity Input System
- **发布**: Steam

## 资源目录

```
Assets/Resources/
├── PrefabFinal/
│   └── PlayerTank.prefab          # 主角战车
├── Prefabs/
│   ├── UI/                        # 界面预制体
│   ├── Cars/suv.prefab           # SUV战车
│   ├── Monsters/                 # 怪物
│   │   ├── Common/               # 普通怪
│   │   └── Boss/                 # Boss
│   ├── Weapons/                  # 武器
│   ├── Bullet/                   # 子弹
│   └── Items/                    # 道具
└── ScriptableObjects/
    ├── Characters/                # 角色数据
    └── EnergyDrop/               # 能量块数据
```

## 项目状态

### 已完成
- ✅ URP渲染管线配置
- ✅ InputSystem输入配置 (Assets/Input/TankTuTu.inputactions)
- ✅ FollowCamera 45度跟随相机脚本 (已添加到 Main Camera)
- ✅ MVC架构代码 (44个C#脚本)
- ✅ 核心系统脚本
- ✅ ShopView.cs 商城界面脚本
- ✅ GachaView.cs 抽卡界面脚本

### 待完成
- ⏳ UI界面完整连接 (View脚本绑定到Prefab)
- ⏳ Prefab组件验证

## UI布局参考 (Brotato)

### 角色选择界面 (Character Select)
- **布局**: 左/中 卡片网格 + 右侧详情面板 + 底部按钮
- **结构**:
  - 中央/左侧: 角色卡片网格 (ScrollView, 可滚动)
  - 右侧: 角色详情面板 (属性、武器选择)
  - 底部: 开始游戏/返回按钮

### 开始界面 (Start Menu)
- **布局**: 居中按钮 + 背景
- **结构**:
  - 居中: 游戏标题
  - 大按钮: 新游戏、继续游戏

### 游戏流程UI
1. **开始界面** (StartMenuCanvas) → 点击开始
2. **角色选择界面** (CharacterSelectCanvas) → 选择角色 → 选武器 → 开始
3. **战斗界面** (HUDView) → 60秒倒计时
4. **结算界面** (ResultView) → 显示统计数据
5. **商城界面** (ShopView) → 购买武器/道具
6. **抽卡界面** (GachaView) → 升级抽卡

## UI 界面制作计划

### 制作流程
1. 检查每个界面 Prefab 的完整性
2. 绑定对应的 View 控制脚本
3. 添加缺失的 UI 元素
4. 放置到 `Assets/Resources/Prefabs/UI/` 目录
5. 创建 UIManager 统一管理界面加载

### 界面清单

| 界面 | Prefab | 脚本 | 状态 |
|------|--------|------|------|
| 开始菜单 | StartMenuCanvas.prefab | StartView | ✅ 已绑定 |
| 角色选择 | CharacterSelectCanvas.prefab | CharacterSelectView + CharacterDetailPanel | ✅ 已绑定 |
| 战斗HUD | 缺失 | HUDView | ❌ 需要创建Prefab |
| 商城 | 缺失 | ShopView | ❌ 需要创建Prefab |
| 抽卡 | 缺失 | GachaView | ❌ 需要创建Prefab |
| 结算 | 缺失 | ResultView | ❌ 需要创建Prefab |

### 界面加载流程

```
StartView.Show() 
    → CharacterSelectView.Show() 
        → 选择角色后 LoadScene("Level_0") 
            → Level场景加载
            → HUDView (战斗界面) - 需要手动添加
            → 关卡结束 → ResultView.Show() - 需要创建Prefab
                → ShopView.Show() - 需要创建Prefab
                    → GachaView.Show() - 需要创建Prefab
                        → 下一关
```

### 界面制作流程 (Unity MCP)

1. **拼界面** - 使用 Unity MCP 创建 UI 元素 (UGUI 默认素材)
2. **写代码** - 完善 View 脚本的响应逻辑
3. **测试** - 在场景中测试功能
4. **绑定** - 将脚本绑定到 Prefab
5. **保存** - Prefab 放置到 `Assets/Resources/Prefabs/UI/`

### Unity MCP UI 创建限制

由于 Unity MCP 对 Prefab 编辑的限制较多，以下 Prefab **需要手动在 Unity 编辑器中创建**：

| Prefab | 需要的元素 | 手动创建步骤 |
|--------|-----------|-------------|
| HUDCanvas | Canvas + HUDView + TimerText + HealthText + ResourceText + WaveText | 见下方步骤1 |
| ShopCanvas | Canvas + ShopView + ItemGrid + Buttons | 见下方步骤2 |
| GachaCanvas | Canvas + GachaView + PullButton + ResultPanel | 见下方步骤3 |
| ResultCanvas | Canvas + ResultView + StatsPanel + Buttons | 见下方步骤4 |

### 手动创建 Prefab 步骤

#### 步骤1: 创建 HUDCanvas.prefab
1. 在 Hierarchy 右键 → UI → Canvas → 命名为 "HUDCanvas"
2. 添加组件: CanvasScaler, GraphicRaycaster
3. 添加脚本: HUDView (在 Scripts 中搜索添加)
4. 创建 4 个子对象:
   - TimerText (右上角, Text) - 显示倒计时
   - HealthText (左上角, Text) - 显示生命值
   - ResourceText (左下角, Text) - 显示资源
   - WaveText (右侧中间, Text) - 显示波次
5. 将 HUDView 的 4 个 Text 字段拖拽到对应子对象
6. 将整个 Canvas 拖到 `Assets/Resources/Prefabs/UI/` 保存为 Prefab

#### 步骤2-4: 类似步骤1，分别创建 ShopCanvas, GachaCanvas, ResultCanvas

### 现有 View 脚本对应的 Prefab 需要创建

| 脚本 | 需要的 Prefab 元素 |
|------|-------------------|
| HUDView | TimerText, HealthText, ResourceText, WaveText |
| ShopView | ItemGrid, RefreshButton, ConfirmButton, ResourceText |
| GachaView | PullButton, ResultPanel, ResultGrid, CloseButton |
| ResultView | StatsPanel, ContinueButton, RestartButton |

- **Scene**: GameStart.unity
- **RootCanvas** 下有:
  - StartMenuCanvas (开始界面 - Prefab已存在，绑定StartView脚本)
  - CharacterSelectCanvas (角色选择 - Prefab已存在，绑定CharacterSelectView脚本)

### StartMenuCanvas 结构
```
StartMenuCanvas
├── Background (Image)
├── TitleText (Text)
└── ButtonPanel (VerticalLayoutGroup)
    ├── StartButton
    ├── ContinueButton
    ├── ConfirmButton (空)
    └── BackButton (空)
```

### CharacterSelectCanvas 结构
```
CharacterSelectCanvas (CharacterSelectView脚本)
├── Background (Image)
├── TitleText (Text)
├── CharacterGrid (ScrollRect)
│   ├── Viewport
│   │   └── Content (GridLayoutGroup) ← 角色卡片容器
│   ├── Scrollbar Horizontal
│   └── Scrollbar Vertical
├── ButtonPanel (HorizontalLayoutGroup)
└── StatsPanel
    └── StatsText ← 角色详情显示
```

### 现有角色数据 (ScriptableObjects)
位置: `Assets/Resources/ScriptableObjects/Characters/`
- WellRounded.asset (均衡)
- Brawler.asset (斗士)
- Ranger.asset (游侠)
- Engineer.asset (工程师)
- Lucky.asset (幸运)

每个角色包含:
- 角色名称、图标、描述
- 属性加成 (HP/移速/攻速/暴击/护甲/范围/幸运/收获)
- 初始武器路径
- 解锁条件
- 特殊能力

### 需要完善
1. ⏳ CharacterSelectCanvas 按钮绑定 (ConfirmButton, BackButton) - 已完成代码
2. ⏳ StatsPanel 详情面板 - 需要添加 CharacterDetailPanel 组件和对应子元素
   - IconImage (Image)
   - NameText (Text)
   - StatsText (Text) - 已有
   - AbilityText (Text)
   - WeaponsText (Text)
   - EmptyHint (GameObject)

## 今日任务

- [x] FollowCamera.cs 创建并添加到 Main Camera
- [x] ShopView.cs 创建
- [x] GachaView.cs 创建
- [x] CharacterSelectView 按钮绑定代码已完善
- [x] CharacterDetailPanel 组件已添加到 Prefab
- [ ] StatsPanel UI 结构完善 (需要手动添加子元素)
- [ ] 测试运行

遵循 `docs/Unity + OpenCode 代码规范.md`:
- 命名: PascalCase (类/方法) + camelCase (变量)
- 结构: Controller/View/ValueObject 分离
- 注释: 类注释 + 公共方法注释
- 序列化: [SerializeField] + [Header] + [Tooltip]

---

## 2026-04-09 更新

### 1. 玩家选择详情面板 (PlayerSelectedDetailView)

**路径**: `Assets/Scripts/Runtime/View/PlayerSelectedDetailView.cs`

功能: 显示角色、武器、难度的详情

**Prefab**: `Assets/Resources/Prefabs/UI/PlayerSelectedDetailCanvas.prefab`

**结构**:
```
PlayerSelectedDetailCanvas
├── MainContainer (VerticalLayoutGroup)
│   ├── CharacterSection (角色详情)
│   │   ├── Icon (Image)
│   │   ├── NameText (Text)
│   │   ├── TypeText (Text)
│   │   └── DescriptionText (Text)
│   ├── WeaponSection (武器详情)
│   │   ├── Icon (Image)
│   │   ├── NameText (Text)
│   │   ├── TypeText (Text)
│   │   └── DescriptionText (Text)
│   └── DifficultySection (难度详情)
│       ├── Icon (Image)
│       ├── NameText (Text)
│       ├── TypeText (Text)
│       └── DescriptionText (Text)
```

### 2. ValueObject 属性封装

为方便访问，为 ScriptableObject 添加了 Properties:

**CharacterDataSO.cs**:
```csharp
public string CharacterName => characterName;
public Sprite Icon => icon;
public string Description => description;
public int MaxHpBonus => maxHpBonus;
public float SpeedBonusPercent => speedBonusPercent;
// ... 其他属性
```

**WeaponDataSO.cs**:
```csharp
public string WeaponId => _weaponId;
public string WeaponName => _weaponName;
public WeaponType WeaponType => _weaponType;
public float Damage => _damage;
public float AttackSpeed => _attackSpeed;
public float Range => _range;
public int Level => _level;
public int MaxLevel => _maxLevel;
// ... 其他属性
```

### 3. 选择系统 (Selection System)

#### SelectionController.cs
路径: `Assets/Scripts/Runtime/View/SelectionController.cs`

通用选择控制器，支持:
- WASD / 方向键
- 手柄左摇杆
- 鼠标悬停检测
- 确认: Enter / 鼠标左键 / 手柄A
- 取消: Escape / 手柄B

#### SelectionItem.cs
路径: `Assets/Scripts/Runtime/View/SelectionItem.cs`

选择项高亮组件，显示选中框和缩放效果

#### 三个选择控制器

| 控制器 | 路径 | 功能 |
|--------|------|------|
| CharacterSelectionController | Runtime/View/ | 角色选择 |
| WeaponSelectionController | Runtime/View/ | 武器选择 |
| DifficultySelectionController | Runtime/View/ | 难度选择 |

**Prefab**: `Assets/Resources/Prefabs/UI/SelectionCanvas.prefab`

### 4. 角色卡片交互 (CharacterCard)

路径: `Assets/Scripts/Runtime/View/CharacterCard.cs`

实现了:
- **悬停事件** (`IPointerEnterHandler`): 鼠标移动到角色图标上 → 显示详情
- **点击事件**: 点击确认选择角色

### 5. 角色选择流程修改 (CharacterSelectView)

修改:
- 移除 `_confirmButton` 确认按钮
- 点击角色卡片 → 直接记录角色 → 进入武器选择

### 6. 玩家战局存档 (PlayerBattleSaveSO)

路径: `Assets/Scripts/Runtime/ValueObject/ScriptableObjects/PlayerBattleSaveSO.cs`

存储内容:
- 角色数据 (_selectedCharacter, _characterId)
- 武器数据 (_startingWeapon, _purchasedWeapons)
- 道具数据 (_startingItems, _purchasedItems)
- 难度 (_difficulty, _difficultyName)
- 战局状态 (_currentWave, _elapsedTime, _currentResources, _killCount)
- 玩家状态 (_currentHp, _maxHp)

方法:
- `SetSelectedCharacter()` - 设置角色
- `SetStartingWeapon()` - 设置初始武器
- `SetDifficulty()` - 设置难度
- `AddPurchasedWeapon()` - 添加购买的武器
- `AddPurchasedItem()` - 添加购买的道具
- `UpdateBattleState()` - 更新战局状态
- `ClearBattleData()` - 清除所有数据

---

## 完整的游戏选择流程

```
1. GameStart场景
   └── StartMenuCanvas (StartView)
       └── 点击开始 → CharacterSelectCanvas (CharacterSelectView)

2. 角色选择 (CharacterSelectView)
   └── 鼠标悬停 → 显示角色详情 (PlayerSelectedDetailView)
   └── 点击角色 → 保存到 PlayerBattleSaveSO
   └── 进入武器选择 → SelectionCanvas (WeaponPanel)

3. 武器选择 (WeaponSelectionController)
   └── 移动选择框 → 显示武器详情
   └── 确认 → 保存武器 → 进入难度选择

4. 难度选择 (DifficultySelectionController)
   └── 移动选择框 → 显示难度详情
   └── 确认 → 保存难度 → 加载 Level_0
```

---

## Editor 脚本列表 (MenuItem)

| 脚本 | 菜单路径 | 功能 |
|------|----------|------|
| HUDCanvasCreator | 铁皮突突/创建UI/创建 HUD Canvas Prefab | 创建HUD界面 |
| PlayerSelectedDetailCanvasCreator | 铁皮突突/创建UI/创建玩家选择详情 Canvas Prefab | 创建详情界面 |
| SimpleSelectionCanvasCreator | 铁皮突突/创建UI/创建选择流程 Canvas | 创建选择界面 |

---

## 当前 Prefab 状态

| 界面 | Prefab | 状态 |
|------|--------|------|
| HUDCanvas | HUDCanvas.prefab | ✅ |
| StartMenuCanvas | StartMenuCanvas.prefab | ✅ |
| CharacterSelectCanvas | CharacterSelectCanvas.prefab | ✅ |
| CharacterCardPrefab | CharacterCardPrefab.prefab | ✅ |
| PlayerSelectedDetailCanvas | PlayerSelectedDetailCanvas.prefab | ✅ |
| SelectionCanvas | SelectionCanvas.prefab | ✅ |
| WeaponCardPrefab | WeaponCardPrefab.prefab | ✅ |
| WeaponSelectionCanvas | WeaponSelectionCanvas.prefab | ✅ |

---

## 2026-04-09 追加更新: 战车武器类型重新设计

### 1. WeaponType 枚举更新

**文件**: `Assets/Scripts/Runtime/ValueObject/WeaponDataValue.cs`

```csharp
public enum WeaponType
{
    MainCannon,     // 主炮 - 高伤害 单发
    Howitzer,       // 榴弹炮 - 范围伤害
    Cannon,         // 加农炮 - 均衡输出
    Gatling,        // 机关炮 - 快速连射
    Missile,        // 导弹 - 高精度追踪
    Rocket,         // 火箭弹 - 弹幕覆盖
    Tesla,          // 电磁炮 - 链式伤害
    Laser           // 激光炮 - 持续伤害
}
```

### 2. 武器数据 SO 重新生成

**文件**: `Assets/Scripts/Editor/CreateDefaultWeapons.cs`

**生成路径**: `Assets/Resources/ScriptableObjects/Weapons/`

| 武器 | 类型 | 伤害 | 攻速 | 范围 | 价格 | 特性 |
|------|------|------|------|------|------|------|
| 主炮 | MainCannon | 40 | 0.8 | 15 | 0 (默认) | 高伤害单发 |
| 榴弹炮 | Howitzer | 30 | 0.5 | 12 | 200 | 范围伤害 |
| 加农炮 | Cannon | 25 | 1.0 | 14 | 150 | 均衡输出 |
| 机关炮 | Gatling | 8 | 5.0 | 8 | 180 | 快速连射 |
| 导弹 | Missile | 50 | 0.4 | 18 | 350 | 追踪锁定 |
| 火箭弹 | Rocket | 20 | 0.6 | 10 | 280 | 弹幕覆盖 |
| 电磁炮 | Tesla | 35 | 0.7 | 12 | 320 | 链式传导 |
| 激光炮 | Laser | 15 | 1.5 | 14 | 250 | 持续穿透 |
| 穿甲弹 | Cannon | 45 | 0.6 | 16 | 300 | 高穿深 |
| 燃烧弹 | Howitzer | 25 | 0.5 | 11 | 220 | 持续燃烧 |

### 3. 武器卡片颜色映射

**文件**: `Assets/Scripts/Runtime/View/WeaponCard.cs`

```csharp
// 根据武器类型显示不同颜色
MainCannon/Cannon: 红色 (1, 0.4, 0.4)     // 火炮
Howitzer/Rocket:  橙色 (1, 0.6, 0.2)     // 榴弹/火箭
Gatling:          黄色 (1, 1, 0.4)       // 机关炮
Missile:          绿色 (0.4, 1, 0.4)     // 导弹
Tesla:            蓝色 (0.4, 0.8, 1)     // 电磁
Laser:            紫色 (0.8, 0.4, 1)     // 激光
```

### 4. UI Prefab 更新

| Prefab | 路径 |
|--------|------|
| WeaponCardPrefab | `Assets/Resources/Prefabs/UI/WeaponCardPrefab.prefab` |
| WeaponSelectionCanvas | `Assets/Resources/Prefabs/UI/WeaponSelectionCanvas.prefab` |
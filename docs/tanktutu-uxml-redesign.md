# TankTuTu (坦克突突) UI 全面重构方案

> 基于 UXML (UI Toolkit) 的完整 UI 系统重设计
> 生成时间: 2026-05-22
> 状态: 设计方案（待实现）

---

## 📚 关联文档

| 文档 | 说明 |
|------|------|
| `docs/tanktutu-data-architecture.md` | 完整数据架构设计：10种战车、5大类18种武器、道具/存档/设置 |
| `docs/tanktutu-uxml-redesign.md` | (本文) UI 重构方案 |

**前置阅读**: 本文的 UI 界面依赖于数据架构文档中定义的 SO 数据结构。
建议先阅读 `tanktutu-data-architecture.md` 了解战车/武器的属性设计。

---

## 一、背景与目标

### 1.1 现状

| 项目 | 值 |
|------|-----|
| Unity 版本 | 2022.3.62f2 |
| 当前 UI 系统 | uGUI (Canvas/Image/Text/Button) |
| 字体 | LegacyRuntime.ttf (默认) |
| UI 资源 | 无 - 全部使用纯色块占位 |
| 脚本数量 (UI) | ~20 个 View/Controller 脚本 |
| 预制体数量 (UI) | 9 个 Prefab (Resources/Prefabs/UI/) |
| 编辑方式 | Editor 脚本程序化创建 (5 个 Creator) |

### 1.2 核心问题

1. **uGUI 限制** - 缺乏 CSS 式样式系统，样式重复，难以统一维护
2. **无设计系统** - 颜色/字体/间距无统一 Token
3. **纯色块 UI** - 没有任何精灵图、纹理、图标资源
4. **两套选择系统** - 旧 SelectionController + 新事件式 View 并存
5. **纯文字 HUD** - 无血条、能量条、进度条等视觉元素
6. **无动效** - 界面切换、按钮反馈、数值变化均无动画
7. **程序化创建** - Creator 脚本使用硬编码坐标，难以调整

### 1.3 重构目标

```
彻底删除旧 uGUI 代码和 Prefab，使用 UI Toolkit (UXML+USS+C#) 全面重建
```

| 维度 | 旧系统 | 新系统 |
|------|--------|--------|
| UI 框架 | uGUI (Canvas/Image/Text) | UI Toolkit (UIDocument/UXML/USS) |
| 样式 | C# 硬编码 | USS 样式表 (类 CSS) |
| 结构 | Prefab 层级 | UXML 声明式结构 |
| 布局 | Anchor/Pivot | Flexbox |
| 字体 | LegacyRuntime.ttf | 自定义 TMP Font |
| 资源 | 无 | ComfyUI 生成 |
| 动效 | 无 | USS transitions + C# 动画 |
| 设计模式 | 混合 (View + Controller) | MVP (HUD) / MVVM (选择界面) |

---

## 二、设计系统

### 2.1 主题定义

```
游戏名:  坦克突突 (TankTuTu)
风格:    军事装甲 + 废土工业
参考:    Brotato UI 布局 + 战地风格视觉
```

### 2.2 色彩系统

```css
/* 主色调 - 军事装甲 */
--color-primary:       #4A7C59;   /* 军绿色 */
--color-primary-dark:  #2D4F38;   /* 深军绿 */
--color-primary-light: #6BA37A;   /* 浅军绿 */

/* 金属色 */
--color-metal:         #8B8B8B;   /* 金属灰 */
--color-metal-dark:    #4A4A4A;   /* 深金属 */
--color-metal-light:   #B8B8B8;   /* 亮金属 */
--color-gold:          #C9A84C;   /* 暗金 */

/* 功能色 */
--color-danger:        #FF4444;   /* 危险红 */
--color-warning:       #FF6B35;   /* 警示橙 */
--color-success:       #4CAF50;   /* 成功绿 */
--color-info:          #00D4FF;   /* 科技蓝 */

/* 背景色 */
--color-bg-dark:       #0D0D0D;   /* 最深背景 */
--color-bg-panel:      #1A1A1A;   /* 面板背景 */
--color-bg-card:       #262626;   /* 卡片背景 */
--color-bg-overlay:    rgba(0,0,0,0.8); /* 遮罩 */

/* 文字色 */
--color-text-primary:  #FFFFFF;   /* 主文字 */
--color-text-secondary:#B0B0B0;  /* 次要文字 */
--color-text-disabled: #666666;  /* 禁用文字 */

/* 稀有度色 */
--rarity-n:            #9E9E9E;   /* 普通 - 灰 */
--rarity-r:            #4FC3F7;   /* 稀有 - 蓝 */
--rarity-sr:           #CE93D8;   /* 超稀有 - 紫 */
--rarity-ssr:          #FFD54F;   /* 传说 - 金 */
```

### 2.3 字体系统

```
标题字体: Exo 2 / Rajdhani (军事风格无衬线)
正文字体: Noto Sans SC (清晰中文)
数字字体: Rajdhani (等宽数字,适合HUD)

字号层级:
  --font-display:   48px  (大标题)
  --font-heading:   32px  (界面标题)
  --font-subtitle:  24px  (区块标题)
  --font-body:      18px  (正文)
  --font-small:     14px  (辅助文字)
  --font-tiny:      12px  (标注)
```

### 2.4 间距系统

```css
/* 4px 基准递增 */
--space-xs:  4px;
--space-sm:  8px;
--space-md:  16px;
--space-lg:  24px;
--space-xl:  32px;
--space-2xl: 48px;
--space-3xl: 64px;
```

### 2.5 圆角系统

```css
--radius-sm:  4px;   /* 小元素 */
--radius-md:  8px;   /* 卡片 */
--radius-lg:  12px;  /* 面板 */
--radius-xl:  16px;  /* 弹窗 */
--radius-full: 50%;  /* 圆形 */
```

---

## 三、界面清单与数据流

### 3.1 完整界面清单

| ID | 界面名 | 场景 | 模式 | 优先级 |
|----|--------|------|------|--------|
| S01 | StartMenu | GameStart | MVP | P0 |
| S02 | CharacterSelect | GameStart | MVP | P0 |
| S03 | WeaponSelect | GameStart | MVP | P0 |
| S04 | DifficultySelect | GameStart | MVP | P0 |
| S05 | PlayerDetailPreview | GameStart | MVP | P0 |
| S06 | SettingsPanel | GameStart | MVVM | P1 |
| S07 | HUD | Level_0 | MVP | P0 |
| S08 | ResultScreen | Level_0 | MVP | P0 |
| S09 | ShopScreen | Level_0 | MVP | P0 |
| S10 | GachaScreen | Level_0 | MVP | P1 |
| S11 | PauseMenu | Level_0 | MVP | P1 |
| S12 | WeaponUpgrade | Level_0 | MVP | P1 |

### 3.2 游戏流程 (完整)

```
Scene: GameStart.unity
┌─────────────────────────────────────────────────────────────────┐
│  S01 StartMenu                                                  │
│  ┌───────────────────────────┐                                  │
│  │    [坦克突突 LOGO]        │                                  │
│  │    █ 新游戏               │  ──→ S02 CharacterSelect        │
│  │    █ 继续游戏             │  ──→ (加载存档→Level_0)          │
│  │    █ 设置                 │  ──→ S06 SettingsPanel           │
│  │    █ 退出                 │                                  │
│  └───────────────────────────┘                                  │
└─────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│  S02 CharacterSelect  (10种战车, 可滚动)  (+ DetailPreview)    │
│  ┌──────┬──────┬──────┬──────┬──────┐  ┌──────────────────┐   │
│  │ MBT  │ SCOUT│ JEEP │ APC  │ IFV  │  │ [角色详情]        │   │
│  ├──────┼──────┼──────┼──────┼──────┤  │ 名称             │   │
│  │ TD   │ SPG  │ AA   │FLAME │ ENG  │  │ 属性列表          │   │
│  └──────┴──────┴──────┴──────┴──────┘  │ 特殊能力          │   │
│  (← → 横向滚动查看更多)                  │ 车辆类型标签       │   │
│  [← 返回]                               │ [确定]            │   │
│                                         └──────────────────┘   │
│  ──→ S03 WeaponSelect                                          │
└─────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│  S03 WeaponSelect  (5大类18种, 带分类Tab)  (+ DetailPreview)   │
│  ┌──────────────────────────────────────────────┐               │
│  │ [主炮] [机枪] [导弹] [喷射] [近战]  ← 分类Tab  │               │
│  ├──────┬──────┬──────┬──────┬──────┬──────┤      │               │
│  │ 加农 │ 榴弹 │ 穿甲 │ 迫击 │ 轻机 │ 重机 │      │               │
│  │ 25/1 │ 30/05│ 45/06│ 35/03│ 5/4  │12/25│      │               │
│  ├──────┼──────┼──────┼──────┼──────┼──────┤      │               │
│  │机关  │霰弹  │火箭  │追踪  │巡航  │火焰  │      │               │
│  │ 8/5  │ 6×6  │20/06│50/04│80/02│15/s  │      │               │
│  ├──────┼──────┼──────┼──────┼──────┼──────┤      │               │
│  │冷冻  │水炮  │酸液  │电锯  │斩刀  │震荡  │      │               │
│  │10/s  │20/15│12/s  │30/s  │60/08│40/05│      │               │
│  └──────┴──────┴──────┴──────┴──────┴──────┘      │               │
│  [← 返回]  ──→ S04 DifficultySelect                            │
└─────────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│  S04 DifficultySelect                                           │
│  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐       │
│  │新手   │ │简单   │ │普通   │ │困难   │ │专家   │ │大师   │       │
│  │★☆☆☆☆│ │★★☆☆☆│ │★★★☆☆│ │★★★★☆│ │★★★★★│ │★★★★★│       │
│  └──────┘ └──────┘ └──────┘ └──────┘ └──────┘ └──────┘       │
│                                 [开始战斗 ▸]                     │
└─────────────────────────────────────────────────────────────────┘
         │ (LoadScene Level_0)
         ▼
┌─────────────────────────────────────────────────────────────────┐
│  Level_0                                                        │
│                                                                │
│  ┌──────────────────────────────────────────────────┐           │
│  │ S07 HUD                                          │           │
│  │ ▓▓▓▓▓▓▓▓▓▓▓▓▓░░░  HP 280/300  ⏱ 00:45  🌊 3/10 │           │
│  │                                                    │           │
│  │               [游戏世界 - 战车战斗]                  │           │
│  │                                                    │           │
│  │  💎 156  ⬆ Lv.3                                   │           │
│  │                                                    │           │
│  │  ┌──┐ ┌──┐ ┌──┐ ┌──┐ ┌──┐ ┌──┐  [武器槽位]       │           │
│  │  │W1│ │W2│ │W3│ │W4│ │W5│ │W6│                    │           │
│  │  └──┘ └──┘ └──┘ └──┘ └──┘ └──┘                    │           │
│  └──────────────────────────────────────────────────┘           │
│         │ (timeout / all die)                                    │
│         ▼                                                        │
│  ┌──────────────────────────────────────────────────┐           │
│  │ S08 ResultScreen                                  │           │
│  │  🏆 关卡完成！                                    │           │
│  │  用时: 01:23  击杀: 47  获得: 💎234               │           │
│  │  ⭐⭐⭐ 评价: S                                  │           │
│  │  [下一关 ▸]  [返回菜单]                            │           │
│  └──────────────────────────────────────────────────┘           │
│         │ (下一关)                                               │
│         ▼                                                        │
│  ┌──────────────────────────────────────────────────┐           │
│  │ S09 ShopScreen                                    │           │
│  │  ┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐             │           │
│  │  │道具│ │武器│ │道具│ │武器│ │道具│   💎 156     │           │
│  │  └────┘ └────┘ └────┘ └────┘ └────┘             │           │
│  │  [刷新 💎20]                    [开始战斗 ▸]      │           │
│  └──────────────────────────────────────────────────┘           │
│         │ (或 Gacha)                                             │
│         ▼                                                        │
│  ┌──────────────────────────────────────────────────┐           │
│  │ S10 GachaScreen                                   │           │
│  │  ╔══════════════════════════════╗                 │           │
│  │  ║      🎰 抽卡结果             ║                 │           │
│  │  ║  [SSR] [R] [SR] [N] [R]    ║                 │           │
│  │  ║  传说！  稀有 ...            ║                 │           │
│  │  ╚══════════════════════════════╝                 │           │
│  │  [抽卡 💎50]                    [关闭]            │           │
│  └──────────────────────────────────────────────────┘           │
│         │ → 循环 (下一波) → HUD → ...                            │
└─────────────────────────────────────────────────────────────────┘
```

### 3.3 数据流架构

```
                    ┌─────────────────────────────┐
                    │     GameManager (单例)        │
                    │  - 玩家状态                   │
                    │  - 资源管理                   │
                    │  - 游戏流程控制               │
                    └──────────┬──────────────────┘
                               │
          ┌────────────────────┼────────────────────┐
          │                    │                     │
          ▼                    ▼                     ▼
   ┌──────────┐       ┌──────────────┐      ┌──────────────┐
   │ SaveMgr  │       │ LevelManager │      │ EnemySpawner │
   └──────────┘       └──────┬───────┘      └──────────────┘
                             │
          ┌──────────────────┼──────────────────┐
          │                  │                   │
          ▼                  ▼                   ▼
   ┌────────────┐   ┌──────────────┐    ┌──────────────┐
   │ TankCtrl   │   │ HUD Presenter │   │ ShopMgr      │
   └────────────┘   └──────┬───────┘    └──────┬───────┘
                           │                   │
          ┌────────────────┼───────────────────┘
          │                │
          ▼                ▼
   ┌────────────┐   ┌──────────────┐
   │ UIDocument │   │ MonoBehavior │
   │ (UXML)     │   │ (MVP View)   │
   └────────────┘   └──────────────┘
```

---

## 四、技术架构

### 4.1 项目结构

```
Assets/
├── UI/                               # UI Toolkit 目录
│   ├── StartMenu/                    # 开始菜单
│   │   ├── StartMenu.uxml
│   │   ├── StartMenu.uss
│   │   └── StartMenuPresenter.cs
│   ├── CharacterSelect/              # 角色选择
│   │   ├── CharacterSelect.uxml
│   │   ├── CharacterSelect.uss
│   │   ├── CharacterCard.uxml        # 角色卡片模板
│   │   ├── CharacterCard.uss
│   │   └── CharacterSelectPresenter.cs
│   ├── WeaponSelect/                 # 武器选择
│   │   ├── WeaponSelect.uxml
│   │   ├── WeaponSelect.uss
│   │   ├── WeaponCard.uxml           # 武器卡片模板
│   │   ├── WeaponCard.uss
│   │   └── WeaponSelectPresenter.cs
│   ├── DifficultySelect/             # 难度选择
│   │   ├── DifficultySelect.uxml
│   │   ├── DifficultySelect.uss
│   │   └── DifficultySelectPresenter.cs
│   ├── HUD/                          # 战斗 HUD
│   │   ├── HUD.uxml
│   │   ├── HUD.uss
│   │   └── HUDPresenter.cs
│   ├── Result/                       # 结算界面
│   │   ├── Result.uxml
│   │   ├── Result.uss
│   │   └── ResultPresenter.cs
│   ├── Shop/                         # 商城
│   │   ├── Shop.uxml
│   │   ├── Shop.uss
│   │   └── ShopPresenter.cs
│   ├── Gacha/                        # 抽卡
│   │   ├── Gacha.uxml
│   │   ├── Gacha.uss
│   │   └── GachaPresenter.cs
│   ├── Common/                       # 公共组件
│   │   ├── Common.uss               # 全局样式
│   │   ├── Buttons.uss              # 按钮样式
│   │   ├── Cards.uss                # 卡片样式
│   │   ├── ProgressBars.uss         # 进度条样式
│   │   ├── Panel.uxml               # 通用面板模板
│   │   └── Modal.uxml               # 弹窗模板
│   └── Fonts/                        # 字体资源
│       ├── Exo2-Bold.ottf
│       ├── Rajdhani-Regular.ottf
│       └── NotoSansSC-Regular.otf
│
├── Scripts/
│   ├── Runtime/
│   │   ├── UI/                       # UI Presenters (新)
│   │   │   ├── StartMenuPresenter.cs
│   │   │   ├── CharacterSelectPresenter.cs
│   │   │   ├── WeaponSelectPresenter.cs
│   │   │   ├── DifficultySelectPresenter.cs
│   │   │   ├── HUDPresenter.cs
│   │   │   ├── ResultPresenter.cs
│   │   │   ├── ShopPresenter.cs
│   │   │   ├── GachaPresenter.cs
│   │   │   ├── UIStateMachine.cs      # UI 状态机
│   │   │   └── UIServiceLocator.cs    # UI 服务定位器
│   │   ├── Controller/               # (保留)
│   │   ├── ValueObject/              # (保留)
│   │   └── Manager/                 # (保留)
│   └── Editor/
│       ├── UXMLAssetPostprocessor.cs  # 自动处理 UXML 引用
│       └── ...
```

### 4.2 MVP 模式实现 (选择)

```
Unity 2022.3 不支持 MVVM 数据绑定，统一使用 MVP 模式

Presenter (MonoBehaviour) ←→ UIDocument (UXML + USS)
     │                              │
     │  Query UI elements           │  Load UXML
     │  Bind events                 │  Apply USS styles
     │  Update UI on data change    │
     ▼                              ▼
  GameManager                    Panel.uxml
  /Controller                    Panel.uss
```

```csharp
// MVP 模式模板
public class XxxPresenter : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    
    // UI element references
    private Label _titleLabel;
    private Button _confirmButton;
    
    private void Awake()
    {
        var root = _uiDocument.rootVisualElement;
        _titleLabel = root.Q<Label>("title");
        _confirmButton = root.Q<Button>("confirm-btn");
        
        _confirmButton.clicked += OnConfirmClicked;
    }
    
    private void OnConfirmClicked() { /* ... */ }
    
    public void Show()  { _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex; }
    public void Hide()  { _uiDocument.rootVisualElement.style.display = DisplayStyle.None; }
}
```

### 4.3 UI 状态机 (UIFlowManager)

```csharp
// 管理界面切换流程
public class UIFlowManager : MonoBehaviour
{
    public enum UIState { StartMenu, CharacterSelect, WeaponSelect, 
                          DifficultySelect, Battle, Result, Shop, Gacha, Pause }
    
    private UIState _currentState;
    private Stack<UIState> _history = new();
    
    // 每个状态对应一个 UIDocument
    private Dictionary<UIState, UIDocument> _uiDocuments;
    
    public void TransitionTo(UIState newState) { /* hide all, show one + animation */ }
    public void GoBack() { /* pop history, transition */ }
}
```

---

## 五、资源生成计划 (ComfyUI)

### 5.1 所需资源清单

| 资源 | 数量 | 用途 | 优先级 |
|------|------|------|--------|
| 按钮背景 (3状态) | 3组×3 | normal/hover/pressed | P0 |
| 面板边框 (平铺) | 4 | 九宫格边框纹理 | P0 |
| 难度等级图标 | 6 | 6个难度等级 | P0 |
| 武器类型图标 | 18 | 每种武器一个图标 | P0 |
| 角色头像 | 10 | 10种战车角色 | P0 |
| HUD 图标集 | 5 | 生命/能量/时间/波次/金币 | P0 |
| 背景纹理 (迷彩) | 2 | 主菜单/面板背景 | P1 |
| 武器稀有度光效 | 4 | N/R/SR/SSR 背景光 | P1 |
| 坦克装饰边框 | 2 | 武器槽位边框 | P1 |
| 进度条纹理 | 3 | 血量/能量/经验条 | P0 |
| 评分星级图标 | 5 | 结算界面评价 | P1 |

### 5.2 生成工作流

```
ComfyUI 工作流:
  1. 使用 Flux / SDXL 模型
  2. Prompt: "military tank icon, flat design, dark theme, game UI, [具体要求]"
  3. 后处理: Resize → Quantize → 导入 Unity
  
下载模型推荐:
  - 图标生成: FLUX.1-schnell (快速)
  - 纹理生成: SDXL 1.0 (高质量)
  - LoRA: 如有军事风格 LoRA 可加载
```

---

## 六、实施路线图

### Phase 1: 基础 (P0)
```
[ ] 1.1 创建 UI 目录结构和 Common.uss 全局样式
[ ] 1.2 实现 StartMenu + UIFlowManager
[ ] 1.3 实现 CharacterSelect + CharacterCard 模板
[ ] 1.4 实现 WeaponSelect + WeaponCard 模板
[ ] 1.5 实现 DifficultySelect
[ ] 1.6 实现 GameStart 场景对接
```

### Phase 2: 战斗 (P0)
```
[ ] 2.1 实现 HUD (血量条/计时器/资源/波次)
[ ] 2.2 实现武器槽位显示
[ ] 2.3 实现 ResultScreen
[ ] 2.4 实现 ShopScreen
[ ] 2.5 实现 Level_0 场景对接
```

### Phase 3: 增强 (P1)
```
[ ] 3.1 ComfyUI 资源生成与导入
[ ] 3.2 USS 样式打磨 (军事主题)
[ ] 3.3 动效系统 (界面切换/按钮/HUD数值)
[ ] 3.4 GachaScreen 抽卡动效
[ ] 3.5 SettingsPanel + PauseMenu
```

### Phase 4: 清理 (P1)
```
[ ] 4.1 删除旧 uGUI Prefab
[ ] 4.2 删除旧 View/Controller 脚本
[ ] 4.3 删除旧 Editor Creator 脚本
[ ] 4.4 更新 SceneInitializer
[ ] 4.5 全面测试
```

---

## 七、UI 元素详细规格

### 7.1 StartMenu.uxml

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement name="start-menu">
        <!-- 全屏背景 -->
        <ui:VisualElement name="background" />
        
        <!-- 遮罩层 -->
        <ui:VisualElement name="overlay" />
        
        <!-- 中央内容 -->
        <ui:VisualElement name="center-content">
            <!-- 游戏 LOGO -->
            <ui:VisualElement name="logo-area">
                <ui:Label name="game-title" text="坦克突突" />
                <ui:Label name="game-subtitle" text="TANK TUTU" />
            </ui:VisualElement>
            
            <!-- 按钮组 -->
            <ui:VisualElement name="button-panel">
                <ui:Button name="new-game-btn" text="新游戏" class="menu-button primary" />
                <ui:Button name="continue-btn" text="继续游戏" class="menu-button" />
                <ui:Button name="settings-btn" text="设置" class="menu-button" />
                <ui:Button name="quit-btn" text="退出" class="menu-button" />
            </ui:VisualElement>
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

### 7.2 HUD.uxml

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement name="hud-container">
        <!-- 左上: 血量 + 等级 -->
        <ui:VisualElement name="top-left">
            <ui:VisualElement name="health-section">
                <ui:Image name="health-icon" />
                <ui:ProgressBar name="health-bar" class="bar-health" />
                <ui:Label name="health-text" />
            </ui:VisualElement>
            <ui:Label name="level-text" />
        </ui:VisualElement>
        
        <!-- 右上: 计时 + 波次 -->
        <ui:VisualElement name="top-right">
            <ui:VisualElement name="timer-section">
                <ui:Image name="timer-icon" />
                <ui:Label name="timer-text" />
            </ui:VisualElement>
            <ui:VisualElement name="wave-section">
                <ui:Label name="wave-text" />
                <ui:ProgressBar name="wave-bar" class="bar-wave" />
            </ui:VisualElement>
        </ui:VisualElement>
        
        <!-- 左下: 资源 -->
        <ui:VisualElement name="bottom-left">
            <ui:Image name="resource-icon" />
            <ui:Label name="resource-text" />
        </ui:VisualElement>
        
        <!-- 底部: 武器槽位 -->
        <ui:VisualElement name="weapon-slots">
            <ui:VisualElement name="slot-0" class="weapon-slot" />
            <ui:VisualElement name="slot-1" class="weapon-slot" />
            <ui:VisualElement name="slot-2" class="weapon-slot" />
            <ui:VisualElement name="slot-3" class="weapon-slot" />
            <ui:VisualElement name="slot-4" class="weapon-slot" />
            <ui:VisualElement name="slot-5" class="weapon-slot" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

### 7.3 Common.uss (部分)

```css
/* 全局重置 */
* {
    unity-font-definition: url("project://database/Assets/UI/Fonts/NotoSansSC-Regular.otf");
    font-size: 18px;
    color: #FFFFFF;
    -unity-text-align: middle-center;
}

/* 按钮系统 */
.menu-button {
    background-color: #2D4F38;
    border-color: #4A7C59;
    border-width: 2px;
    border-radius: 8px;
    padding: 12px 32px;
    font-size: 24px;
    transition: background-color 0.15s, scale 0.1s;
}

.menu-button:hover {
    background-color: #4A7C59;
}

.menu-button:active {
    background-color: #6BA37A;
    scale: 0.97;
}

.menu-button.primary {
    background-color: #4A7C59;
    border-color: #C9A84C;
}

/* 进度条系统 */
.bar-health {
    height: 16px;
    background-color: #4A4A4A;
    border-radius: 8px;
}

.bar-health .unity-progress-bar__progress {
    background-color: #FF4444;
    border-radius: 8px;
}

.bar-health .unity-progress-bar__background {
    background-color: #333333;
    border-radius: 8px;
}

/* 卡片系统 */
.character-card {
    background-color: #262626;
    border-color: #4A4A4A;
    border-width: 2px;
    border-radius: 12px;
    padding: 8px;
    transition: border-color 0.15s, scale 0.15s;
}

.character-card:hover {
    border-color: #4A7C59;
    scale: 1.05;
}

.character-card:selected {
    border-color: #C9A84C;
}

/* 面板系统 */
.panel {
    background-color: rgba(13, 13, 13, 0.95);
    border-color: #4A4A4A;
    border-width: 1px;
    border-radius: 12px;
    padding: 24px;
}
```

---

## 八、关键决策记录

### 8.1 为什么选择 UI Toolkit 而非 uGUI

| 对比项 | uGUI | UI Toolkit | 结论 |
|--------|------|------------|------|
| 样式系统 | C# 硬编码 / Prefab | USS (CSS 子集) | ✅ UITK |
| 布局 | Anchor/Pivot | Flexbox | ✅ UITK |
| 可维护性 | 差 (Prefab 二进制) | 好 (UXML 纯文本) | ✅ UITK |
| 性能 | Canvas 批量合批 | 原生绘制 | ✅ UITK |
| 数据绑定 | 无原生支持 | 2023.2+ 支持 | ⚠️ 2022.3 需 MVP |
| 学习成本 | 低 | 中 | ⚠️ 可以接受 |
| 工具链 | Prefab Editor | UI Builder | ⚠️ 需要安装 |

### 8.2 为什么选择 MVP 而非 MVVM

Unity 2022.3 不支持 `dataSource` 数据绑定（需要 2023.2+），所以选择 MVP 模式：
- Presenter 直接操作 UXML 元素
- 事件驱动更新
- 无绑定反射开销，性能更好

### 8.3 旧代码清理策略

```
保留:
  - Assets/Scripts/Runtime/Controller/   (核心游戏逻辑)
  - Assets/Scripts/Runtime/ValueObject/   (数据模型)
  - Assets/Scripts/Runtime/Manager/       (管理器)

删除:
  - Assets/Scripts/Runtime/View/          (全部重写为 UI Toolkit Presenter)
  - Assets/Scripts/Runtime/Creator/       (如有)
  - Assets/Resources/Prefabs/UI/          (所有 uGUI Prefab)
  - Assets/Scripts/Editor/*Creator.cs     (旧的程序化 UI 创建脚本)
  - Assets/Scripts/Runtime/Model/         (如有)
```

**注意**: 删除前需确认 `SceneInitializer.cs` 等脚本不再引用旧 View 脚本

---

## 九、下一步

### 立即开始

1. ✅ 完成本文档 (设计方案)
2. [ ] 创建 `Assets/UI/` 目录结构
3. [ ] 下载字体资源 (Exo2, Rajdhani, NotoSansSC)
4. [ ] 编写 Common.uss 全局样式
5. [ ] 实现 StartMenu.uxml + Presenter
6. [ ] 实现 UIFlowManager 状态机
7. [ ] 接入 GameStart 场景

### 工具准备

```bash
# 字体下载 (可通过 Unity Package Manager 或手动导入)
# 使用 opencode-unity-uixml 生成器辅助创建 UXML

cd C:\Users\Admin\.agents\skills\opencode-unity-uixml
python unity_uixml_generator.py --name StartMenu --desc "开始菜单" --pattern mvp
```

# 战车射击游戏 MVP - 探索思考

## 探索背景

这是一个全新的 Unity 项目（AutoPrj 下已有 AutoUnityPrj）。
用户想要开发类似 Sibling Shooter 的俯视角射击游戏。
不同点：主角是战车，有 6 个武器槽。

## 核心需求可视化

```
游戏主循环：
┌─────────────────────────────────────────────┐
│          开始界面 (Start Screen)             │
│   [新游戏] ──────→ 选择关卡 ─────→ 战斗      │
│   [继续] ──────→ 加载存档                     │
└─────────────────────────────────────────────┘
                    ↓
         ┌────────────────────────────────┐
         │         战斗关卡 (60s)         │
         │  [战车上路]                    │
         │  [6 武器自动瞄准射击]          │
         │  [敌人追击]                    │
         │  [掉落资源/宝箱]               │
         │  [时间到] → 结算               │
         └────────────────────────────────┘
                    ↓
         ┌────────────────────────────────┐
         │         商城界面 (Shop)        │
         │  [展示可用武器/道具]           │
         │  [合并升级武器]               │
         │  [数量叠加 属性叠加]           │
         │  [开始战斗]                    │
         └────────────────────────────────┘
```

## 系统架构思考

### 1. 战车系统 (Tank System)

```
Tank
├── Transform (位置/旋转/缩放)
├── Camera (主显示相机)
├── CharacterController/FixedJointRigidbody (移动)
├── WeaponSlots (6 个槽位)
│   ├── WeaponSlot 0
│   ├── WeaponSlot 1
│   ├── ...
│   └── WeaponSlot 5
├── Inventory (当前携带资源)
└── PlayerController (Input 处理)

Tank (玩家 1)
Tank (玩家 2)
Tank (玩家 3)
Tank (玩家 4)
```

### 2. 武器系统

```
WeaponSlot
├── WeaponData (配置)
│   ├── damage
│   ├── fireRate
│   ├── range
│   ├── projectileSpeed
│   ├── pierceCount
│   └── visualEffect
├── ProjectileSpawn (弹药生成点)
└── AutoAim (自动瞄准器)

WeaponConfig
├── type (机枪/弩弓/霰弹/狙击)
├── ammoType
├── upgradeStats
└── unlockCost
```

### 3. 自动瞄准机制

```
AutoAimSystem
├── Raycast 检测 (前方扇形)
│   ├── 主武器 (最高优先级)
│   ├── 副武器
│   └── 其他武器
├── 角度计算
│   ├── 计算每个敌人相对角度
│   ├── 扇形覆盖 (如 60 度)
│   └── 选择最佳目标
├── 目标分配
│   ├── 最近优先
│   ├── 生命百分比
│   └── 随机轮转
└── 冷却管理
    ├── 武器独立冷却
    └── 避免重复射击同一目标
```

### 4. 资源收集系统

```
PickupSystem
├── CollectionZone (玩家收集范围)
│   ├── radius (如 3-5 单位)
│   └── 自动吸附
├── ResourceItem
│   ├── healthPotion
│   ├── manaPotion
│   ├── gold
│   └── xp
└── PickupTrigger (靠近拾取)
```

### 5. 商城系统

```
ShopSystem
├── MerchantUI
│   ├── WeaponList
│   │   ├── AvailableWeapons
│   │   ├── OwnedWeapons
│   │   ├── MergeStacks
│   │   └── UpgradeButtons
│   └── ItemList
│       ├── Potions
│       ├── Consumables
│       └── Buffs
├── PurchaseLogic
│   ├── CanAfford (资源检查)
│   ├── Purchase (扣除资源)
│   └── Consume (道具消耗)
└── MergeSystem
    ├── GroupByType
    ├── StackCombine (同类型合并)
    └── UpdateUI
```

## 关键技术点待确认

### 待确认问题

1. **战车移动控制**
   - 使用 CharacterController 还是 Rigidbody？
   - 移动方式：WASD/摇杆直接控制？
   - 是否限制移动方向？

2. **武器槽分配**
   - 6 个槽位如何分配？全部自动瞄准还是部分手动？
   - 是否支持武器禁用/隐藏？
   - 槽位顺序是否重要？

3. **自动瞄准范围**
   - 扇形角度（如 45°/60°/90°）？
   - 检测距离（多少单位）？
   - 优先级计算逻辑？

4. **资源拾取**
   - 拾取范围（如 3/5 单位）？
   - 是否自动拾取还是点击确认？
   - 拾取动画？

5. **商城 UI**
   - 武器合并动画？
   - 升级进度条？
   - 道具消耗即时生效？

6. **多玩家同步**
   - Steam 工作进程？
   - 同步频率？
   - 反作弊措施？

7. **关卡设计**
   - 地图大小？
   - 敌人生成方式？
   - Boss 机制设计？
   - 阶段切换点？

### 技术调研点

- [ ] 查找 Unity Input System 多玩家配置示例
- [ ] 研究俯视角相机最优设置（正交 vs 透视）
- [ ] 自动瞄准算法实现（射线检测 vs 角度计算）
- [ ] Steam 平台发布配置
- [ ] 类似游戏的技术实现

## 下一步动作

1. 分析背景任务返回的技术资料
2. 基于资料形成技术架构方案
3. 提出待确认问题清单
4. 等待用户确认或调整方向

## 探索收获

目前已确认：
- ✅ 游戏核心循环清晰
- ✅ 系统架构思路明确
- ⏳ 背景任务正在调研技术方案
- ⏳ 待确认具体实现细节

等待任务完成后再做决策。

# TankTuTu 数据架构设计

> 数据驱动的完整方案：战车角色、武器、道具、存档、设置
> 基于 ScriptableObject + ValueObject 双层架构
> 2026-05-22（v2.0 - 补充战车/武器完整设计）

---

## 零、架构原则

### 双层数据模型

```
┌─────────────────────────────────────────────────────────────────┐
│                  SO 层 (ScriptableObject)                        │
│  设计师在 Inspector 中配置，Assets/Resources 目录下持久化存储      │
│  相当于"数据模板/配置表"                                          │
├─────────────────────────────────────────────────────────────────┤
│                  VO 层 (ValueObject)                             │
│  运行时实例，内存中可变，可序列化                                   │
│  相当于"运行时数据快照"                                            │
└─────────────────────────────────────────────────────────────────┘
```

### SO ↔ VO 映射规则

```
Editor 配置                  运行时
CharacterDataSO ──ToDataValue()──→ TankDataValue (战斗属性)
         └── 组合/引用 TankDataSO 获取基础数值

WeaponDataSO   ──ToDataValue()──→ WeaponDataValue (武器实例)
ItemDataSO     ──ToDataValue()──→ ItemDataValue (道具实例)
```

### TDD 开发流程

```
Step 1: 写架构设计文档（本文档）
Step 2: 写 SO 脚本定义
Step 3: 写 SO → VO 转换单元测试
Step 4: 写 VO 逻辑单元测试（CanAttack/Upgrade/ApplyToTank 等）
Step 5: 整合 SO Editor 生成脚本
Step 6: 集成测试（多个SO加载 → 角色选择 → 战斗全流程）
Step 7: 对接 UI Presenter
```

---

## 一、战车角色数据 (Character/Vehicle) — 10种 MVP

### 1.1 数据结构

```
CharacterDataSO (ScriptableObject)           TankDataVO (ValueObject)
┌──────────────────────┐                    ┌──────────────────────┐
│ 标识                  │   Editor配置       │ 基础血量              │
│ - characterId        │  ──────────→        │ - maxHealth (HP)     │
│ - characterName      │   ToDataValue()    │ - healthRegen (HP/s) │
│ - icon (头像)         │                    │ - lifesteal (%)      │
│ - description        │                    ├──────────────────────┤
│ - vehicleType        │                    │ 伤害属性              │
│  (TANK/JEEP/APC/SPG) │                    │ - percentDamage (%)  │
├──────────────────────┤                    │ - rangedDamage (%)   │
│ 基础属性(bonus)       │                    │ - meleeDamage (%)    │
│ - maxHp Bonus        │                    │ - elementDamage (%)  │
│ - speed Bonus%       │                    │ - engineering (%)    │
│ - attackSpeed Bonus% │                    ├──────────────────────┤
│ - critChance Bonus%  │                    │ 战斗属性              │
│ - armor Bonus        │                    │ - attackSpeed (次/s) │
│ - range Bonus%       │                    │ - critRate (%)       │
│ - dodge Bonus%       │                    │ - range (米)         │
│ - luck Bonus         │                    │ - aimAccuracy (%)    │
│ - harvest Bonus      │                    ├──────────────────────┤
├──────────────────────┤                    │ 防御属性              │
│ 初始配置              │                    │ - armor (减伤值)     │
│ - startingWeaponPaths│                    │ - dodge (%)          │
│ - startingWeaponIds  │                    ├──────────────────────┤
├──────────────────────┤                    │ 移动 & 成长          │
│ 解锁条件              │                    │ - moveSpeed (m/s)    │
│ - isUnlockedDefault  │                    │ - luck               │
│ - unlockCondition    │                    │ - harvest (倍率)     │
│ - unlockRequirement  │                    └──────────────────────┘
├──────────────────────┤
│ 特殊能力              │   Method: ToDataValue() 组合:
│ - specialAbility     │   TankDataSO基础 + CharacterDataSO加成
│ - specialAbilityType │   → 返回最终 TankDataValue
└──────────────────────┘
```

### 1.2 数据来源参考

调研了 World of Tanks (5类)、Armored Warfare (5类)、BattleTanx (12种)、
Tank Wars (5职业)、Broforce 等游戏的战车设计，归纳出以下 10 种差异化角色：

### 1.3 10种战车完整清单

| # | ID | 名称 | 原型参考 | 定位 | HP | 移速 | 攻速 | 伤害 | 范围 | 护甲 | 特殊能力 |
|---|-----|------|---------|------|----|------|------|------|------|------|---------|
| 1 | `mbt` | **主战坦克** | M1 Abrams/Leopard 2 | 重甲肉盾 | ★★★★★ | ★★ | ★★ | ★★★★ | ★★★ | ★★★★★ | 每30s生成护盾 |
| 2 | `scout` | **轻型侦察车** | M3 Bradley/Scout | 高速侦察 | ★★ | ★★★★★ | ★★★★ | ★★ | ★★★ | ★ | 移速+50%持续5s(CD15s) |
| 3 | `jeep` | **突击吉普** | Jeep Willy/Military | 游击骚扰 | ★ | ★★★★★ | ★★★★ | ★★★ | ★ | ★ | 击杀后+40%移速3s |
| 4 | `apc` | **装甲运兵车** | Sd.Kfz.251/Halftrack | 经济支援 | ★★★ | ★★★ | ★★★ | ★★★ | ★★★ | ★★★ | +50%收获，击杀掉落翻倍 |
| 5 | `ifv` | **步兵战车** | CV90/Bradley | 全能均衡 | ★★★ | ★★★★ | ★★★ | ★★★ | ★★★★ | ★★ | 无特殊，全属性+5% |
| 6 | `td` | **歼击战车** | M10 Hellcat/M18 | 远程狙杀 | ★★ | ★★★★ | ★ | ★★★★★ | ★★★★★ | ★ | +30%暴伤，无视30%护甲 |
| 7 | `spg` | **自行火炮** | M109 Paladin/PzH2000 | 范围轰炸 | ★ | ★ | ★ | ★★★★ | ★★★★★★ | ★ | 炮弹范围+50%，移速-30% |
| 8 | `aa` | **自行高炮** | Gepard/M163 VADS | 弹幕压制 | ★★ | ★★★ | ★★★★★ | ★★★ | ★★ | ★★ | 攻速+40%，子弹可穿透+1 |
| 9 | `flame` | **喷火坦克** | Churchill Crocodile | 灼烧专家 | ★★★ | ★★★ | ★★★ | ★★★ | ★ | ★★★ | 火焰伤害+50%，持续3s DoT |
| 10 | `engineer` | **工程支援车** | M1150 ABV | 炮塔搭建 | ★★★★ | ★★ | ★★ | ★★ | ★★★ | ★★★★ | 每20s自动部署炮塔 |

### 1.4 车辆类型标签 (用于UI筛选/分类)

```
VehicleType 枚举:
  TANK     — 主战/歼击 (重型)
  LIGHT    — 侦察/吉普 (轻型)
  APC      — 运兵/工程 (支援)  
  SPG      — 自行火炮/高炮 (远程)
  SPECIAL  — 喷火/特殊 (特种)
```

### 1.5 数值平衡表 (详细属性)

```
注: 基础 = 100HP, 3m/s移速, 1.0攻速, 10伤害, 10范围, 0护甲

ID         HP  regen  lifesteal  spd  atkSpd  dmg  range  crit  armor  dodge  luck  harvest
mbt       180  0.3    0%         2.2  0.7     10    10     5%    5      5%     0     1.0
scout     70   0.3    2%         4.5  1.6     7     9      10%   0      15%    5     1.2
jeep      60   0.5    3%         5.0  1.5     9     6      15%   0      20%    10    1.3
apc       120  0.5    1%         3.0  1.0     8     10     5%    2      5%     15    2.0
ifv       110  0.5    2%         3.5  1.1     10    11     8%    2      8%     5     1.1
td        80   0.2    0%         3.2  0.6     18    18     25%   0      5%     0     1.0
spg       55   0.1    0%         1.5  0.4     15    25     10%   0      0%     0     1.0
aa        75   0.3    1%         3.5  2.2     8     8      5%    1      10%    0     1.1
flame     130  0.8    5%         2.8  1.0     12    4      10%   3      5%     0     1.0
engineer  150  1.0    2%         2.5  0.8     7     9      5%    4      0%     5     1.5
```

### 1.6 初始武器分配

| 战车 | 初始武器 | 说明 |
|------|---------|------|
| 主战坦克 | 加农炮 | 稳定输出 |
| 侦察车 | 轻机枪 | 快速骚扰 |
| 突击吉普 | 霰弹枪 | 近程爆发 |
| 运兵车 | 榴弹炮 | 范围清理 |
| 步战车 | 主炮 | 均衡开局 |
| 歼击车 | 穿甲炮 | 远程打击 |
| 自行火炮 | 巡航导弹 | 范围轰炸 |
| 自行高炮 | 机关炮 | 弹幕压制 |
| 喷火坦克 | 火焰喷射器 | 近战灼烧 |
| 工程支援车 | 主炮 | 标准开局 |

### 1.7 解锁条件

| 战车 | 解锁方式 |
|------|---------|
| 主战坦克 | ✅ 默认 |
| 侦察车 | ✅ 默认 |
| 突击吉普 | ✅ 默认 |
| 步战车 | ✅ 默认 |
| 运兵车 | 通关 简单 难度 |
| 歼击车 | 通关 普通 难度 |
| 自行火炮 | 累计击杀 500 敌人 |
| 自行高炮 | 累计拾取 100 金币 |
| 喷火坦克 | 累计造成 5000 火焰伤害 |
| 工程支援车 | 通关 困难 难度 |

### 1.8 当前已有文件 vs 需要变更

| 文件 | 状态 | 变更 |
|------|------|------|
| `CharacterDataSO.cs` | ⚠️ 需扩展 | 增加 vehicleType, dodge, specialAbilityType 字段 |
| `TankDataSO.cs` | 🔄 整合 | 建议合并到 CharacterDataSO，不再单独使用 |
| `TankDataValue.cs` | ✅ 保留 | 作为运行时 VO |
| `Resources/Characters/*.asset` | 🔄 需重做 | 从5个→10个，重新命名 |
| 角色头像 Sprite | ❌ ComfyUI 生成 | 10个角色头像+缩略图 |

### 1.9 单元测试计划

```
Game.Runtime.Tests.Character
  ✓ TankDataValue_DefaultValues_MatchSpec()
  ✓ TankDataValue_Clamping_HpNeverNegative()
  ✓ TankDataValue_LoadFromSave_RestoresCorrectly()
  ✓ CharacterDataSO_ToDataValue_PropertiesTransfer()
  ✓ CharacterDataSO_StartingWeapons_CanLoad()
  ✓ CharacterDataSO_IsUnlocked_DefaultVsConditional()
  ✓ VehicleType_Enum_10Entries()
```

---

## 二、武器数据 (Weapon) — 5大类 × 3~4种

### 2.1 武器类型层级

```
WeaponCategory (大类)           WeaponType (子类)
┌─────────────────────┐       ┌─────────────────────┐
│ 主炮类 MainCannon   │──────→│ Cannon (加农炮)      │
│                     │       │ Howitzer (榴弹炮)    │
│                     │       │ AP_Piercing (穿甲炮) │
│                     │       │ Mortar (迫击炮)      │
├─────────────────────┤       ├─────────────────────┤
│ 机枪类 MachineGun   │──────→│ LightMG (轻机枪)     │
│                     │       │ HeavyMG (重机枪)     │
│                     │       │ Gatling (机关炮)     │
│                     │       │ Shotgun (霰弹枪)     │
├─────────────────────┤       ├─────────────────────┤
│ 导弹类 Missile      │──────→│ Rocket (火箭弹)      │
│                     │       │ Homing (追踪导弹)    │
│                     │       │ Cruise (巡航导弹)    │
├─────────────────────┤       ├─────────────────────┤
│ 喷射类 Sprayer      │──────→│ Flame (火焰喷射器)   │
│                     │       │ Cryo (冷冻喷射器)    │
│                     │       │ Water (高压水炮)     │
│                     │       │ Acid (酸液喷射器)    │
├─────────────────────┤       ├─────────────────────┤
│ 近战类 Melee        │──────→│ Drill (旋转电锯)     │
│                     │       │ Blade (巨型斩刀)     │
│                     │       │ Hammer (震荡锤)      │
│                     │       │ Lance (冲击钻)       │
└─────────────────────┘       └─────────────────────┘
```

### 2.2 数据结构扩展

```
WeaponDataSO (ScriptableObject)
┌────────────────────────┐
│ 标识                    │
│ - weaponId             │
│ - weaponName           │
│ - icon                 │
│ - description          │
│ - weaponCategory       │  ← 新增: WeaponCategory 枚举
│ - weaponType           │  ← 保留: WeaponType 枚举
├────────────────────────┤
│ 伤害属性                │
│ - damage               │
│ - damageType (PHYSICAL │
│   /FIRE/ICE/ACID/ENERGY)│ ← 新增
│ - attackSpeed          │
│ - range                │
├────────────────────────┤
│ 特殊属性                │
│ - pierce (穿透次数)     │
│ - area (爆炸半径)       │
│ - duration (持续秒)     │
│ - projectileSpeed      │  ← 新增: 弹道速度
│ - projectileCount      │  ← 新增: 每发子弹数(霰弹)
│ - knockback            │  ← 新增: 击退力
├────────────────────────┤
│ 升级属性                │
│ - level                │
│ - maxLevel             │
│ - upgradeCost          │
│ - upgradeDamagePerLevel│  ← 新增: 每级增加伤害
├────────────────────────┤
│ 商业属性                │
│ - price                │
│ - isDefault            │
│ - rarity (COMMON/RARE/ │
│   EPIC/LEGENDARY)      │  ← 新增
└────────────────────────┘
```

### 2.3 完整武器清单 — 18种 MVP

#### 主炮类 (Main Cannon) — 单发高伤，范围爆炸

| ID | 名称 | 伤害 | 攻速 | 范围 | 穿透 | 范围 | 特殊 | 价格 |
|----|------|------|------|------|------|------|------|------|
| `cannon` | 加农炮 | 25 | 1.0 | 14 | 1 | 0.5 | 标准弹道 | 150 |
| `howitzer` | 榴弹炮 | 30 | 0.5 | 12 | 0 | 2.5 | 大范围爆炸 | 200 |
| `ap_shell` | 穿甲炮 | 45 | 0.6 | 16 | 3 | 0 | 高穿透直线 | 300 |
| `mortar` | **迫击炮** 🆕 | 35 | 0.3 | 20 | 0 | 3.0 | 抛物线高抛 | 400 |

#### 机枪类 (Machine Gun) — 高速连射，弹幕压制

| ID | 名称 | 伤害 | 攻速 | 范围 | 穿透 | 子弹数 | 特殊 | 价格 |
|----|------|------|------|------|------|--------|------|------|
| `light_mg` | **轻机枪** 🆕 | 5 | 4.0 | 10 | 0 | 1 | 稳定弹道 | 80 |
| `heavy_mg` | **重机枪** 🆕 | 12 | 2.5 | 12 | 1 | 1 | 高单发伤害 | 150 |
| `gatling` | 机关炮 | 8 | 5.0 | 8 | 0 | 1 | 极高射速 | 180 |
| `shotgun` | **霰弹枪** 🆕 | 6×6 | 1.2 | 6 | 0 | 6 | 扇形散射 | 160 |

#### 导弹类 (Missile) — 高爆发，追踪/范围

| ID | 名称 | 伤害 | 攻速 | 范围 | 穿透 | 范围 | 特殊 | 价格 |
|----|------|------|------|------|------|------|------|------|
| `rocket` | 火箭弹 | 20 | 0.6 | 10 | 0 | 1.5 | 高速直射 | 280 |
| `missile` | 追踪导弹 | 50 | 0.4 | 18 | 0 | 2.0 | 自动追踪 | 350 |
| `cruise` | **巡航导弹** 🆕 | 80 | 0.2 | 25 | 0 | 4.0 | 超大范围延迟爆炸 | 500 |

#### 喷射类 (Sprayer) — 持续伤害，元素特效

| ID | 名称 | 伤害 | 攻速 | 范围 | 伤害类型 | 特殊 | 价格 |
|----|------|------|------|------|---------|------|------|
| `flame` | **火焰喷射器** 🆕 | 15/s | 3.0 | 5 | FIRE | 持续灼烧3s DoT | 200 |
| `cryo` | **冷冻喷射器** 🆕 | 10/s | 3.0 | 6 | ICE | 减速50%持续2s | 220 |
| `water` | **高压水炮** 🆕 | 20 | 1.5 | 7 | PHYSICAL | 强击退+推开 | 180 |
| `acid` | **酸液喷射器** 🆕 | 12/s | 2.5 | 5 | ACID | 减甲50%持续3s | 250 |

#### 近战类 (Melee) — 高风险高回报

| ID | 名称 | 伤害 | 攻速 | 范围 | 穿透 | 特殊 | 价格 |
|----|------|------|------|------|------|------|------|
| `drill` | **旋转电锯** 🆕 | 30/s | 4.0 | 3 | 无限 | 持续近身伤害 | 300 |
| `blade` | **巨型斩刀** 🆕 | 60 | 0.8 | 4 | 3 | 大范围挥砍 | 350 |
| `hammer` | **震荡锤** 🆕 | 40 | 0.5 | 3 | 0 | 范围眩晕1.5s | 400 |
| `lance` | **冲击钻** 🆕 | 25 | 1.5 | 5 | 2 | 突进+伤害 | 250 |

### 2.4 数据类型枚举

```csharp
public enum WeaponCategory
{
    MainCannon,  // 主炮类
    MachineGun,  // 机枪类
    Missile,     // 导弹类
    Sprayer,     // 喷射类
    Melee        // 近战类
}

public enum DamageType
{
    PHYSICAL,  // 物理
    FIRE,      // 火焰
    ICE,       // 冰冻
    ACID,      // 酸液
    ENERGY     // 能量
}

public enum WeaponRarity
{
    COMMON,    // 白色 - 商店常见
    RARE,      // 蓝色 - 商店较少
    EPIC,      // 紫色 - 稀有掉落
    LEGENDARY  // 橙色 - BOSS掉落
}
```

### 2.5 当前已有文件 vs 需要变更

| 文件 | 状态 | 变更 |
|------|------|------|
| `WeaponDataSO.cs` | ⚠️ 需扩展 | 增加 category, damageType, projectileSpeed, knockback, rarity 等字段 |
| `WeaponDataValue.cs` | ⚠️ 需更新 | WeaponType 枚举保留，新增 WeaponCategory, DamageType, WeaponRarity |
| `Resources/Weapons/*.asset` | 🔄 需扩展 | 从10种→18种，新增8个武器 |
| 武器图标 Sprite | ❌ ComfyUI | 18个武器图标 |

### 2.6 单元测试计划

```
Game.Runtime.Tests.Weapon
  ✓ WeaponDataValue_DefaultConstructor_ValidState()
  ✓ WeaponDataValue_ParameterizedConstructor_PropertiesSet()
  ✓ WeaponDataValue_CanAttack_CooldownLogic()
  ✓ WeaponDataValue_Upgrade_IncreasesDamageSpeed()
  ✓ WeaponDataValue_Upgrade_MaxLevelCaps()
  ✓ WeaponDataValue_GetFinalDamage_WithTankBonuses()
  ✓ WeaponDataValue_GetFinalRange_WithRangeBonus()
  ✓ WeaponDataValue_GetDamageTypeText_EnumCoverage()
  ✓ WeaponDataValue_GetRarityColor_RarityMapping()
  ✓ WeaponDataSO_ToDataValue_AllFieldsMatch()
  ✓ WeaponCategory_Enum_5Entries()
  ✓ DamageType_Enum_5Entries()
```

---

## 三、道具数据 (Item)

### 3.1 数据结构

```
ItemDataSO (ScriptableObject)    [新增]       ItemDataValue (VO)
┌────────────────────┐                       ┌────────────────────┐
│ 标识               │                       │ 标识               │
│ - itemId           │  Editor配置           │ - itemId           │
│ - itemName         │  ──────────→          │ - itemName         │
│ - icon             │  ToDataValue()        │ - itemType         │
│ - description      │                       │ - description      │
│ - itemType         │                       ├────────────────────┤
├────────────────────┤                       │ 数值               │
│ 数值               │                       │ - price            │
│ - price            │                       │ - level/maxLevel   │
│ - level            │                       ├────────────────────┤
│ - maxLevel         │                       │ 属性加成           │
├────────────────────┤                       │ - maxHealthBonus   │
│ 属性加成           │                       │ - healthRegenBonus │
│ - maxHealthBonus   │                       │ - damageBonus      │
│ - healthRegenBonus │                       │ - attackSpeedBonus │
│ - damageBonus      │                       │ - moveSpeedBonus   │
│ - attackSpeedBonus │                       │ - critRateBonus    │
│ - moveSpeedBonus   │                       │ - armorBonus       │
│ - critRateBonus    │                       │ - luckBonus        │
│ - armorBonus       │                       │ - harvestBonus     │
│ - luckBonus        │                       ├────────────────────┤
│ - harvestBonus     │                       │ 堆叠               │
├────────────────────┤                       │ - stackCount       │
│ 堆叠               │                       │ - maxStack         │
│ - canStack         │                       │ - canStack         │
│ - maxStack         │                       ├────────────────────┤
├────────────────────┤                       │ 稀有度             │
│ 稀有度             │                       │ - rarity           │
│ - rarity           │                       │                    │
└────────────────────┘                       │ 方法               │
                                             │ - ApplyToTank()    │
                                             │ - RemoveFromTank() │
                                             └────────────────────┘
```

### 3.2 当前状态

| 文件 | 说明 | 状态 |
|------|------|------|
| `ItemDataValue.cs` | 运行时 VO + 静态工厂方法 | ✅ 完整(需改造) |
| `ItemDataSO.cs` | SO 定义 | ❌ 缺失（需新建） |
| `Resources/Items/` | Item SO 实例 | ❌ 缺失 |

**当前问题**: `ItemDataValue.cs` 中直接内置了 `CreateHeart()` 等硬编码预设，
需要改为 `ItemDataSO` + Resources 加载的方式，与武器/角色保持一致。

### 3.3 道具清单 (MVP)

#### Passive（被动道具 — 常驻加成）

| ID | 名称 | 效果 | 价格 | 稀有度 |
|----|------|------|------|--------|
| `heart` | 生命之心 | +20 最大生命 | 50 | COMMON |
| `guard` | 铁壁护甲 | +3 护甲 | 80 | COMMON |
| `boots` | 敏捷之靴 | +5% 攻速 | 80 | COMMON |
| `bracer` | 力量护腕 | +10% 伤害 | 100 | RARE |
| `coin` | 幸运硬币 | +5 幸运 | 150 | RARE |
| `ring` | 丰收戒指 | +10% 收获 | 200 | RARE |
| `cloak` | 幽灵披风 | +10% 闪避 | 180 | RARE |
| `scope` | 精准瞄具 | +15% 范围 | 220 | EPIC |

#### Consumable（消耗品 — 一次性效果）

| ID | 名称 | 效果 | 价格 | 稀有度 |
|----|------|------|------|--------|
| `repair_kit` | 维修包 | 回复 50 HP | 30 | COMMON |
| `shield_pot` | 护盾药剂 | +20 护甲 30s | 60 | COMMON |
| `speed_boost` | 加速器 | +50% 移速 10s | 40 | COMMON |
| `rage_pot` | 狂暴药剂 | +50% 伤害 15s | 80 | RARE |

### 3.4 单元测试计划

```
Game.Runtime.Tests.Item
  ✓ ItemDataValue_ApplyToTank_BonusesApplied()
  ✓ ItemDataValue_RemoveFromTank_BonusesRemoved()
  ✓ ItemDataValue_Stacking_LimitsRespected()
  ✓ ItemDataValue_Consumable_ConsumeReducesCount()
  ✓ ItemDataSO_ToDataValue_AllFieldsMatch()    [待ItemDataSO完成]
```

---

## 四、玩家存档数据 (Player Save)

### 4.1 双存档结构

```
PlayerBattleSaveSO (ScriptableObject)        PlayerSaveDataValue (VO)
┌───────────────────────────┐                ┌────────────────────┐
│ 战局内临时存档 (SO引用)     │                │ 玩家全局存档 (JSON)   │
│ - selectedCharacterRef    │                │ - playerId         │
│ - purchasedWeaponsRefs[]  │                │ - playerIndex      │
│ - difficultyRef           │                ├────────────────────┤
├───────────────────────────┤                │ 游戏进度            │
│ 值对象运行时数据            │                │ - highestLevel     │
│ - currentWave             │                │ - totalPlayTime    │
│ - elapsedTime             │                │ - totalKills       │
│ - currentResources        │                ├────────────────────┤
│ - killCount               │                │ 货币/解锁           │
│ - currentHp / maxHp       │                │ - gold             │
│ - stageWeapons[]          │                │ - unlockedTanks[]  │
│ - stageItems[]            │                │ - unlockedWeapons[]│
└───────────────────────────┘                │ - unlockedItems[]  │
                                             │ - unlockedSkins[]  │
       两种用途:                               ├────────────────────┤
       1. 战局内临时存档 (SO)                  │ 统计               │
       2. 玩家持久存档 (JSON → Application.  │ - totalWins        │
          persistentDataPath)                 │ - totalLosses      │
                                             │ - lastSaveTime     │
                                             ├────────────────────┤
                                             │ 方法               │
                                             │ - ToJson()         │
                                             │ - FromJson()       │
                                             │ - GetWinRate()     │
                                             └────────────────────┘
```

### 4.2 当前已有文件

| 文件 | 说明 | 状态 |
|------|------|------|
| `PlayerBattleSaveSO.cs` | 战局存档 SO | ✅ 完整 |
| `PlayerSaveDataValue.cs` | 持久存档 VO (JSON) | ✅ 完整 |
| `Resources/PlayerBattleSaveSO.asset` | 存档实例 | ❌ 未找到(需确认) |
| `SaveManager.cs` | 存档管理 Controller | ✅ 保留 |

### 4.3 单元测试计划

```
Game.Runtime.Tests.Save
  ✓ PlayerSaveDataValue_DefaultConstructor_GeneratesPlayerId()
  ✓ PlayerSaveDataValue_ToJson_RoundTrip_DataPreserved()
  ✓ PlayerSaveDataValue_UnlockTank_DuplicateNoOp()
  ✓ PlayerSaveDataValue_RecordWinLoss_StatsTracked()
  ✓ PlayerSaveDataValue_GetWinRate_ZeroDivisionSafe()
  ✓ PlayerBattleSaveSO_ClearBattleData_AllReset()
  ✓ PlayerBattleSaveSO_AddPurchasedWeapon_NoDuplicates()
```

---

## 五、游戏设置数据 (Game Settings)

### 5.1 数据结构 (新增)

```
GameSettingsSO (ScriptableObject)            GameSettingsValue (VO)
┌────────────────────┐                [新增] ┌────────────────────┐
│ 音频               │                       │ 音频               │
│ - masterVolume     │                       │ - masterVolume     │
│ - musicVolume      │                       │ - musicVolume      │
│ - sfxVolume        │                       │ - sfxVolume        │
├────────────────────┤                       │ - muteAll          │
│ 画面               │                       ├────────────────────┤
│ - qualityLevel     │                       │ 画面               │
│ - fullscreen       │                       │ - qualityLevel     │
│ - resolutionIndex  │                       │ - fullscreen       │
│ - vSync            │                       │ - resolution       │
├────────────────────┤                       │ - vSync            │
│ 游戏               │                       ├────────────────────┤
│ - language         │                       │ 游戏               │
│ - cameraShake      │                       │ - language         │
│ - showDamageNumbers│                       │ - cameraShake      │
│ - autoCollectRange │                       │ - showDamageNumbers│
├────────────────────┤                       │ - autoCollectRange │
│ 控制               │                       ├────────────────────┤
│ - invertY          │                       │ 控制               │
│ - sensitivityX     │                       │ - invertY          │
│ - sensitivityY     │                       │ - sensitivityX     │
└────────────────────┘                       │ - sensitivityY     │
                                             ├────────────────────┤
                                             │ 方法               │
                                             │ - SaveToPlayerPrefs│
                                             │ - LoadFromPlayerPrefs
                                             └────────────────────┘
```

### 5.2 实现要点

- `GameSettingsSO` 作为 Inspector 配置模板（方便批量修改默认值）
- `GameSettingsValue` 运行时内存实例
- 持久化使用 `PlayerPrefs`（轻量设置无需 JSON 文件）
- 提供 `static GameSettingsValue Default` 默认值属性

### 5.3 单元测试计划

```
Game.Runtime.Tests.Settings
  ✓ GameSettingsValue_DefaultValues_Reasonable()
  ✓ GameSettingsValue_SaveLoad_PlayerPrefsRoundTrip()
  ✓ GameSettingsValue_VolumeClamping_0to100()
```

---

## 六、资产资源规划 (ComfyUI)

### 6.1 战车 Sprite (Side-view Top-down)

| 资产 | 数量 | 分辨率 | 备注 |
|------|------|--------|------|
| 战车头像 (圆形icon) | 10 | 128×128 | 选择界面头像 |
| 战车本体 Sprite | 10 | 256×256 | 战斗场景中的战车贴图 |
| 战车残骸动画 | 10 | 128×128×4帧 | 被摧毁时的爆炸残骸 |

### 6.2 武器 Sprite

| 资产 | 数量 | 分辨率 | 备注 |
|------|------|--------|------|
| 武器图标 | 18 | 64×64 | 选择/商店界面图标 |
| 弹道/弹头 Sprite | 20 | 32×32 | 子弹/导弹/火焰等 |

### 6.3 道具 Sprite

| 资产 | 数量 | 分辨率 | 备注 |
|------|------|--------|------|
| 道具图标 | 12 | 64×64 | 商店和背包中显示 |

### 6.4 生成策略

使用 ComfyUI Flux 模型 + LoRA 军事风格。推荐工作流：
1. 生成概念图（512×512）→ 2. 图生图细化（统一风格）→ 3. 分割导出透明PNG

---

## 七、TDD 实施流程（完整示例）

### 示例: 从零实现 `GameSettingsSO`

**Step 1**: 写 SO 脚本

```csharp
// Scripts/Runtime/ValueObject/ScriptableObjects/GameSettingsSO.cs
[CreateAssetMenu(fileName = "GameSettings", menuName = "铁皮突突/游戏设置")]
public class GameSettingsSO : ScriptableObject
{
    [SerializeField] [Range(0, 1)] private float _masterVolume = 0.8f;
    [SerializeField] private int _qualityLevel = 2;
    [SerializeField] private bool _fullscreen = true;
    // ...
    
    public GameSettingsValue ToDataValue() { /* ... */ }
}
```

**Step 2**: 写 VO 脚本

```csharp
// Scripts/Runtime/ValueObject/GameSettingsValue.cs
[System.Serializable]
public class GameSettingsValue
{
    public float MasterVolume = 0.8f;
    public int QualityLevel = 2;
    public bool Fullscreen = true;
    
    public void SaveToPlayerPrefs() { /* PlayerPrefs.SetFloat("MasterVolume", MasterVolume) */ }
    public static GameSettingsValue LoadFromPlayerPrefs() { /* 读取 PlayerPrefs */ }
}
```

**Step 3**: 写单元测试 → 运行 → ❌ 失败

```csharp
// Tests/Runtime/ValueObject/GameSettingsValueTests.cs
[Test]
public void DefaultValues_AreReasonable()
{
    var settings = new GameSettingsValue();
    Assert.AreEqual(0.8f, settings.MasterVolume);    // ❌ 尚未实现
    Assert.IsTrue(settings.Fullscreen);               // ❌ 尚未实现
}
```

**Step 4**: 实现 VO 逻辑 → 运行 → ✅ 通过

**Step 5**: 整合 SO → 创建 `.asset` 文件

**Step 6**: 写 Presenter 集成 → 对照 UXML 绑定

---

## 八、字体资源

### 8.1 已存在的字体文件

| 路径 | 格式 | 状态 |
|------|------|------|
| `Assets/Arts/Fonts/JyunsaiKaai-Regular.ttf` | TrueType | ✅ 已存在 |
| `Assets/Arts/Fonts/JyunsaiKaai-Regular SDF.asset` | TMP SDF | ✅ 已存在 |

**结论**: 使用 `JyunsaiKaai-Regular SDF` 作为 UI Toolkit 字体。
在 USS 中通过 `-unity-font-definition: resource("Assets/Arts/Fonts/JyunsaiKaai-Regular SDF.asset");` 引用。

### 8.2 推荐额外字体

| 字体 | 用途 | 获取方式 |
|------|------|----------|
| Rajdhani | HUD数字、标题（军事风格） | Google Fonts 下载 |
| Noto Sans SC | 正文中文 | Unity Package Manager |

---

## 九、数据流完整图

```
                    ┌──────────────────────┐
                    │   Resources.Load()   │
                    │   (Unity 资源系统)    │
                    └──────┬──────┬───────┘
                           │      │
              ┌────────────┘      └────────────┐
              ▼                                 ▼
    ┌─────────────────┐              ┌──────────────────┐
    │  CharacterDataSO │              │  WeaponDataSO    │
    │  TankDataSO      │              │  ItemDataSO      │
    │  DifficultyDataSO│              │  EnemyDataSO     │
    │  PlayerBattleSave│              │  GameSettingsSO  │
    └────────┬─────────┘              └────────┬─────────┘
             │                                 │
             ▼  ToDataValue()                  ▼
    ┌─────────────────┐              ┌──────────────────┐
    │  TankDataValue   │              │  WeaponDataValue │
    │  EnemyDataValue  │              │  ItemDataValue   │
    └────────┬─────────┘              └────────┬─────────┘
             │                                 │
             └────────┬───────────┬─────────────┘
                      │           │
                      ▼           ▼
              ┌────────────┐ ┌────────────┐
              │ GameManager │ │ SaveManager│
              │ (单例持有)  │ │ (JSON存档) │
              └──────┬─────┘ └──────┬─────┘
                     │              │
                     ▼              ▼
              ┌────────────┐ ┌──────────────┐
              │ UIPresenter│ │ PlayerSave   │
              │ (显示数据) │ │ DataValue    │
              └────────────┘ └──────────────┘
```

---

## 十、实施优先级

| 优先级 | 任务 | 产出 | 依赖 | 预估价 |
|--------|------|------|------|--------|
| P0 | 扩展 WeaponDataSO (加 category/damageType/rarity) | 脚本修改 | 无 | 1h |
| P0 | 新建 ItemDataSO | 脚本新建 | 无 | 1h |
| P0 | 新建 GameSettingsSO + GameSettingsValue | 脚本新建 | 无 | 1h |
| P0 | 更新 WeaponType/WeaponDataValue 枚举 | 脚本修改 | 无 | 0.5h |
| P0 | 重写 CharacterDataSO (加 vehicleType/dodge/specialType) | 脚本修改 | 无 | 1h |
| P0 | **单元测试**: WeaponDataValue + TankDataValue + ItemDataValue | 测试脚本 | 各 VO 就绪 | 2h |
| P1 | ComfyUI: 生成10种战车头像+本体 Sprite | 图片资源 | 描述文本 | 2h |
| P1 | ComfyUI: 生成18个武器图标+弹道 Sprite | 图片资源 | 描述文本 | 2h |
| P1 | 创建新10个 CharacterSO asset | .asset文件 | CharacterDataSO扩展完成 | 1h |
| P1 | 创建新8个 WeaponSO asset (新增8种武器) | .asset文件 | WeaponDataSO扩展完成 | 1h |
| P1 | 创建 ItemSO asset (8个被动+4个消耗品) | .asset文件 | ItemDataSO完成 | 0.5h |
| P2 | SaveManager + PlayerSaveDataValue 集成测试 | 测试脚本 | 存档类就绪 | 1h |
| P2 | 数据 Editor 工具 (批量生成 SO) | Editor脚本 | 各 SO 就绪 | 2h |

---

## 附录 A: 当前已有文件变更清单

### A.1 需要修改的文件 (5个)

| 文件 | 修改内容 |
|------|---------|
| `CharacterDataSO.cs` | + VehicleType, + dodge bonus, + specialAbilityType |
| `WeaponDataSO.cs` | + WeaponCategory, + DamageType, + WeaponRarity, + projectileSpeed, + knockback |
| `WeaponDataValue.cs` | + WeaponCategory, + DamageType, + WeaponRarity 枚举 |
| `ItemDataValue.cs` | 移除硬编码工厂方法，改用 SO 加载 |
| `TankDataSO.cs` | 建议废弃/合并到 CharacterDataSO |

### A.2 需要新建的文件 (6个)

| 文件 | 说明 |
|------|------|
| `ItemDataSO.cs` | 道具 SO 定义 |
| `GameSettingsSO.cs` | 设置 SO 定义 |
| `GameSettingsValue.cs` | 设置 VO 定义 |
| 18个 WeaponSO .asset | 其中8个新增 |
| 10个 CharacterSO .asset | 重新创建（保留旧5个并增加5个新） |
| 12个 ItemSO .asset | 全新创建 |

### A.3 需要删除的文件

```
Scripts/Runtime/View/            全部删除 (重写为 UI Toolkit Presenter)
Scripts/Editor/*Creator.cs       全部删除 (UI Creator 不再需要)
Resources/Prefabs/UI/            全部删除 (uGUI Prefab)
```

### A.4 游戏内已知问题

| 问题 | 说明 |
|------|------|
| 狙击型引用"狙击枪"武器 | 武器资源不存在，需要修复为"穿甲炮" |
| PlayerBattleSaveSO.asset | 存档实例需确认是否存在于 Resources |
| 战车 Sprite 缺失 | 全部使用默认方块，需 ComfyUI 生成 |
| 武器图标缺失 | 全部使用默认方块，需 ComfyUI 生成 |

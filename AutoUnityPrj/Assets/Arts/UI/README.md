# 坦克突突游戏 - UI 美术资源

## 目录结构

```
Assets/Arts/UI/
├── README.md                  # 本文件
├── CharacterPortraits/        # 战车角色头像 (10)
│   ├── meta.json              # ComfyUI 生成参数记录
│   └── prompt.txt             # 生成提示词参考
├── WeaponIcons/               # 武器图标 (18)
│   ├── meta.json              # ComfyUI 生成参数记录
│   └── prompt.txt             # 生成提示词参考
├── ItemIcons/                 # 道具图标 (待生成)
├── kenney_space/              # Kenney Space 资源包 (UI 框架素材)
└── kenney_space.meta
```

## 角色头像 (CharacterPortraits)

10 种战车，PNG 1024×1024，白色背景。

| 文件名 | 中文名 | 类型 | 定位 |
|--------|--------|------|------|
| HeavyTank.png | 重型坦克 | 坦克 | 高血量高护甲，移动慢 |
| LightTank.png | 轻型坦克 | 坦克 | 高速侦察，灵活机动 |
| Artillery.png | 自行火炮 | 火炮 | 远程范围打击，弹道高 |
| APC.png | 装甲运兵车 | 装甲车 | 运载+支援，可部署步兵 |
| AssaultGun.png | 突击炮 | 火炮 | 正面攻坚，装甲厚 |
| IFV.png | 步兵战车 | 装甲车 | 多功能战斗，火力均衡 |
| SPAAG.png | 自行防空炮 | 防空 | 对空+轻甲目标克星 |
| MissileCarrier.png | 导弹车 | 支援 | 远程精确制导打击 |
| ScoutJeep.png | 侦察吉普 | 轻型 | 高速游击，视野广 |
| Engineer.png | 工程车 | 支援 | 维修+部署防御工事 |

### 引用方式 (UI Toolkit)

UXML 中引用前景图时使用 `background-image`：

```xml
<Image class="character-portrait" background-image="Assets/Arts/UI/CharacterPortraits/HeavyTank.png" />
```

或通过 USS 设置：

```css
.portrait-heavy-tank {
    background-image: url("Assets/Arts/UI/CharacterPortraits/HeavyTank.png");
}
```

## 武器图标 (WeaponIcons)

5 大类 18 种武器，PNG 1024×1024，白色背景。

### 主炮类 (Main Cannon) - 5

| 文件名 | 中文名 | 特点 |
|--------|--------|------|
| LightCannon.png | 轻型火炮 | 射速快，伤害低 |
| StandardCannon.png | 标准火炮 | 均衡，适用性广 |
| HeavyCannon.png | 重型火炮 | 伤害高，射速慢 |
| QuickFireCannon.png | 速射炮 | 极高射速，散布大 |
| BunkerBuster.png | 攻城炮 | 对固定目标特化 |

### 机枪类 (Machine Gun) - 4

| 文件名 | 中文名 | 特点 |
|--------|--------|------|
| LightMG.png | 轻机枪 | 持续压制火力 |
| HeavyMG.png | 重机枪 | 高穿深，对轻甲 |
| Gatling.png | 加特林 | 极高射速，预热机制 |
| EMG.png | 电磁机枪 | 能量武器，无视护甲 |

### 导弹类 (Missile) - 3

| 文件名 | 中文名 | 特点 |
|--------|--------|------|
| ATGM.png | 反坦克导弹 | 线导，高穿深 |
| Rocket.png | 火箭弹 | 面杀伤，弹道弯曲 |
| GuidedMissile.png | 制导导弹 | 自动追踪目标 |

### 喷射类 (Sprayer) - 4

| 文件名 | 中文名 | 特点 |
|--------|--------|------|
| FlameSprayer.png | 火焰喷射器 | 持续灼烧，AOE |
| CryoSprayer.png | 冷冻喷射器 | 减速冻结敌人 |
| WaterCannon.png | 高压水炮 | 击退+冲散阵型 |
| AcidSprayer.png | 酸液喷射器 | 腐蚀装甲，持续伤害 |

### 近战类 (Melee) - 2

| 文件名 | 中文名 | 特点 |
|--------|--------|------|
| Drill.png | 钻头 | 破甲特化，贴脸伤害 |
| Chainsaw.png | 链锯 | 持续切割，高DPS |

### 引用方式

同角色头像，通过 UXML `background-image` 或 USS `url()` 引用。

## 资源生成

所有美术资源使用 ComfyUI z_image_turbo 生成（2026-05-22 更新）：
- **模型**: z_image_turbo_bf16.safetensors（diffusion_models）
- **CLIP**: qwen_3_4b.safetensors（type: qwen_image）
- **VAE**: ae.safetensors
- **尺寸**: 1024×1024
- **步数**: 4（turbo 模型，极少步数即可高质量输出）
- **CFG**: 2.5
- **采样器**: euler
- **调度器**: simple
- **背景**: 白色

每个目录下的 `meta.json` 记录了每张图片的完整生成参数和种子。

## 命名规范

- **文件名**: PascalCase（与 C# 代码保持一致）
- **分类**: 子目录名 = 资源类别（CharacterPortraits / WeaponIcons / ItemIcons）
- **引用**: 通过相对路径 `Assets/Arts/UI/{Category}/{Name}.png`

## 待生成资源

- `ItemIcons/` 道具图标（弹药箱、能量包、血包、护盾、加速、强化等）
- `Backgrounds/` 菜单背景图
- `Effects/` UI特效（选中高亮、切换过渡等）

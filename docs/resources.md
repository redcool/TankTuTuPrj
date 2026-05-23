# Resources 目录结构

## 目录概览

```
Assets/Resources/
├── PrefabFinal/          # 主角战车
│   └── PlayerTank.prefab
├── Prefabs/              # 其他游戏对象
│   ├── UI/
│   ├── Cars/
│   ├── Monsters/
│   ├── Weapons/
│   ├── Bullet/
│   └── Items/
└── ScriptableObjects/    # 数据资源
    ├── Characters/
    └── EnergyDrop/
```

## 详细清单

### PrefabFinal (主角战车)
| 文件 | 用途 |
|------|------|
| `PlayerTank.prefab` | 玩家控制的主角坦克 |

### Prefabs/UI (界面)
| 文件 | 用途 |
|------|------|
| `StartMenuCanvas.prefab` | 开始菜单界面 |
| `CharacterSelectCanvas.prefab` | 角色选择界面 |
| `CharacterCardPrefab.prefab` | 角色卡片预制体 |
| `HUDCanvas.prefab` | 战斗HUD界面 |
| `PlayerSelectedDetailCanvas.prefab` | 玩家选择详情界面 |
| `SelectionCanvas.prefab` | 选择流程界面 (角色/武器/难度) |

### Prefabs/Cars (载具)
| 文件 | 用途 |
|------|------|
| `suv.prefab` | SUV 战车 |

### Prefabs/Monsters (怪物)
| 文件 | 用途 |
|------|------|
| `Common/animal-beaver.prefab` | 普通怪 - 海狸 |
| `Common/animal-cow.prefab` | 普通怪 - 奶牛 |
| `Boss/animal-elephant.prefab` | Boss - 大象 |

### Prefabs/Weapons (武器)
| 文件 | 用途 |
|------|------|
| `blaster-a.prefab` | 武器A |
| `blaster-b.prefab` | 武器B |
| `blaster-d.prefab` | 武器D |
| `scope-large-a.prefab` | 大型瞄准器 |

### Prefabs/Bullet (子弹)
| 文件 | 用途 |
|------|------|
| `CommonBullet.prefab` | 通用子弹 |

### Prefabs/Items (道具)
| 文件 | 用途 |
|------|------|
| `Block/goods1.prefab` | 阻塞物 |
| `Box/TreasureBox1.prefab` | 宝箱 |

### ScriptableObjects/Characters (角色数据)
| 文件 | 用途 |
|------|------|
| `Lucky.asset` | 幸运角色 |
| `Engineer.asset` | 工程师角色 |
| `Ranger.asset` | 游侠角色 |
| `Brawler.asset` | 斗士角色 |
| `WellRounded.asset` | 均衡角色 |

### ScriptableObjects/EnergyDrop (能量掉落)
| 文件 | 用途 |
|------|------|
| `DefaultEnergyDrop.asset` | 默认能量块数据 |

### ScriptableObjects/PlayerBattleSave (玩家战局存档)
| 文件 | 用途 |
|------|------|
| `PlayerBattleSaveSO.asset` | 玩家战局存档数据 |

---

## 已完成的代码文件

### View 脚本
| 文件 | 用途 |
|------|------|
| `SelectionController.cs` | 通用选择控制器 |
| `SelectionItem.cs` | 选择项高亮组件 |
| `CharacterSelectionController.cs` | 角色选择控制器 |
| `WeaponSelectionController.cs` | 武器选择控制器 |
| `DifficultySelectionController.cs` | 难度选择控制器 |
| `CharacterCard.cs` | 角色卡片 (悬停+点击) |
| `PlayerSelectedDetailView.cs` | 玩家选择详情面板 |

### ScriptableObject 数据
| 文件 | 用途 |
|------|------|
| `PlayerBattleSaveSO.cs` | 玩家战局存档 |
| `CharacterDataSO.cs` | 角色数据 (已添加Properties) |
| `WeaponDataSO.cs` | 武器数据 (已添加Properties) |

### Editor 脚本 (MenuItem)
| 文件 | 菜单路径 | 功能 |
|------|----------|------|
| `HUDCanvasCreator.cs` | 铁皮突突/创建UI/创建 HUD Canvas Prefab | 创建HUD界面 |
| `PlayerSelectedDetailCanvasCreator.cs` | 铁皮突突/创建UI/创建玩家选择详情 Canvas Prefab | 创建详情界面 |
| `SimpleSelectionCanvasCreator.cs` | 铁皮突突/创建UI/创建选择流程 Canvas | 创建选择界面 |

---

## Input 配置

| 文件 | 用途 |
|------|------|
| `Assets/Input/TankTuTu.inputactions` | Unity Input System 配置文件 |

---

## 待完成任务

1. ✅ **45度跟随相机** - FollowCamera.cs 已创建并添加到 Main Camera
2. ✅ **UI完整连接** - View 脚本绑定到 Prefab
3. ✅ **选择系统** - SelectionController + Character/Weapon/DifficultySelectionController
4. ✅ **角色卡片交互** - CharacterCard 悬停+点击事件
5. ⏳ **角色选择流程连接** - 需要在 GameStart 场景中订阅 CharacterSelectView.OnCharacterConfirmed 事件来显示武器选择界面
6. ⏳ **武器选择界面** - 需要创建 WeaponSelectionPanel 的内容

---

## CharacterSelectCanvas 角色选择界面

### Prefab 结构
```
CharacterSelectCanvas
├── Background (Image)
├── TitleText (Text)
├── CharacterGrid (ScrollRect)
│   ├── Viewport (Mask)
│   └── Content (GridLayoutGroup) ← 角色卡片容器
├── Scrollbar Horizontal (隐藏)
├── Scrollbar Vertical
├── ButtonPanel (HorizontalLayoutGroup)
└── StatsPanel (Image)
    └── StatsText (Text)
```

### 组件绑定
- **CharacterSelectView** - 界面控制脚本
- **CharacterDetailPanel** - 角色详情面板

### 角色数据加载
- 从 `Resources/ScriptableObjects/Characters/` 加载
- 5个角色: Lucky, Engineer, Ranger, Brawler, WellRounded
- 使用 `Resources.LoadAll<CharacterDataSO>()`

### CharacterCardPrefab 结构
```
CharacterCardPrefab
├── CardIcon (Image)
├── CardName (Text)
├── CardStats (Text)
├── LockOverlay (Image)
└── UnlockText (Text)
```

### 数据流
1. CharacterSelectView.LoadCharacterData() → 加载 CharacterDataSO
2. CharacterSelectView.BuildCharacterCards() → 生成卡片到 Content
3. 点击卡片 → CharacterCard.OnCharacterSelected → CharacterDetailPanel.ShowCharacter()
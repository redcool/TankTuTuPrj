# Editor 代码规范说明

## 1. 基本规则

### 文件放置
- Editor 脚本统一放在 `Assets/Scripts/Editor/` 目录下
- 命名规则：`功能名.cs`，PascalCase，与类名一致
- 每个文件一个类

### 菜单路径
Editor 工具菜单统一挂在 `Tools/铁皮突突/` 根路径下：

```
Tools/
└── 铁皮突突/
    ├── 批量生成车辆预制体 (从 kenney_car)
    ├── 批量生成武器预制体 (从 kenney_blaster)
    ├── 批量生成车辆+武器预制体
    └── ...其他工具
```

规则：
- 根路径固定为 `Tools/铁皮突突/`，不允许其他根路径
- 子菜单按功能分组，动词在前（如 `批量生成...`、`检查...`、`修复...`）
- 所有菜单项使用中文，保持与游戏术语一致

### 代码风格
- `[MenuItem]` 路径用字符串常量定义，避免手写拼写错误
- 源路径和目标路径用 `private const string` 定义在类顶部
- 每个 `[MenuItem]` 方法简短（< 10 行），复杂逻辑抽取到私有方法
- 操作完成必须调用 `AssetDatabase.Refresh()`
- 任何操作都要有明确的成功/失败日志
- 必须处理 `AssetDatabase.IsValidFolder` 检查，不假设路径存在

## 2. 资源生成类规范

### 命名
- 类名：`Create{产物描述}`
- 方法名：`Create{具体产物}Prefabs`
- 路径常量：`{类型}_SOURCE`（源）、`{类型}_TARGET`（目标）

### 过滤器
- 生成预制体时，必须加文件名过滤器
- 车辆类：使用白名单（只包含已知的车辆模型名称）
- 武器类：按命名前缀过滤
- 过滤条件用 `private static` 方法封装
- 跳过文件计入 `skipped` 计数，用于日志

### 生成逻辑
```
查找源 FBX → 文件名过滤 → 加载模型 → 写入目标目录 → AssetDatabase.Refresh
```

- 已存在预制体：更新（覆盖）
- 不存在预制体：新建
- 生成/更新逻辑应该统一，不要重复代码

### 示例模板

```csharp
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 功能描述
/// </summary>
public static class CreateXxxPrefabs
{
    private const string SOURCE = "Assets/Arts/xxx";
    private const string TARGET = "Assets/Resources/Prefabs/Xxx";

    [MenuItem("Tools/铁皮突突/生成Xxx预制体")]
    private static void CreatePrefabs()
    {
        // 1. 检查源目录
        if (!AssetDatabase.IsValidFolder(SOURCE)) return;
        
        // 2. 确保目标目录
        EnsureFolderExists(TARGET);
        
        // 3. 查找FBX
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { SOURCE });
        
        // 4. 遍历生成
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = Path.GetFileNameWithoutExtension(path);
            
            if (!IsValidModel(name)) continue;
            
            SavePrefab(path, TARGET);
        }
        
        AssetDatabase.Refresh();
    }
    
    private static bool IsValidModel(string name) { /* 过滤器 */ }
    
    private static void SavePrefab(string sourcePath, string targetDir)
    {
        string name = Path.GetFileNameWithoutExtension(sourcePath);
        string prefabPath = $"{targetDir}/{name}.prefab";
        
        var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(fbx);
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
    }
    
    private static void EnsureFolderExists(string path) { /* 逐层创建 */ }
}
```

## 3. 注意事项

### 必须做的
- 每次菜单操作都要输出 `Debug.Log`，便于追踪
- 操作前后调用 `AssetDatabase.Refresh()`
- 文件路径用正斜杠 `/`（Unity 标准）
- 使用 `AssetDatabase` API 操作资源，不用 `File` 类

### 禁止做的
- 禁止修改 `Assets/Arts/` 下的源模型文件
- 禁止在 Editor 脚本中使用 `Resources.Load`（那是 Runtime 用的）
- 禁止在 Editor 脚本中引用 Runtime 的业务逻辑类（减少耦合）
- 禁止使用 `AssetDatabase.DeleteAsset` 等破坏性操作
- 禁止菜单路径使用英文（除非标准化术语）

### 目标目录规则
- `Resources/Prefabs/Cars/`：只存放以 `car_` 开头的车辆模型预制体
- `Resources/Prefabs/Weapons/`：只存放以 `blaster-` 开头的武器模型预制体
- 生成代码必须检查此规则，不合规的文件跳过

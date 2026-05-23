using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 编辑器工具 — 从 kenney_car / kenney_blaster 的 FBX 批量生成预制体
/// 输出到 Resources/Prefabs/Cars/（car_ 前缀）和 Resources/Prefabs/Weapons/
/// 供 TankController 运行时动态加载
///
/// 菜单: Tools → 铁皮突突 → 批量生成...
/// </summary>
public static class CreateModelPrefabs
{
    private const string CARS_SOURCE = "Assets/Arts/kenney_car";
    private const string CARS_TARGET = "Assets/Resources/Prefabs/Cars";
    private const string WEAPONS_SOURCE = "Assets/Arts/kenney_blaster";
    private const string WEAPONS_TARGET = "Assets/Resources/Prefabs/Weapons";

    [MenuItem("Tools/铁皮突突/批量生成车辆预制体 (从 kenney_car)")]
    private static void CreateCarPrefabs()
    {
        CreateCarPrefabsFromFBX();
    }

    [MenuItem("Tools/铁皮突突/批量生成武器预制体 (从 kenney_blaster)")]
    private static void CreateWeaponPrefabs()
    {
        CreateWeaponPrefabsFromFBX();
    }

    [MenuItem("Tools/铁皮突突/批量生成车辆+武器预制体")]
    private static void CreateAllPrefabs()
    {
        CreateCarPrefabsFromFBX();
        CreateWeaponPrefabsFromFBX();
        Debug.Log("[CreateModelPrefabs] 全部预制体生成完成");
    }

    // ── 战车 ──────────────────────────────────────────────

    private static void CreateCarPrefabsFromFBX()
    {
        if (!AssetDatabase.IsValidFolder(CARS_SOURCE))
        {
            Debug.LogError($"[CreateModelPrefabs] 源目录不存在: {CARS_SOURCE}");
            return;
        }

        EnsureFolderExists(CARS_TARGET);

        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { CARS_SOURCE });
        int created = 0, skipped = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            // 源文件名以 car_ 开头的才导出，否则跳过
            if (!fileName.StartsWith("car_"))
            {
                skipped++;
                continue;
            }

            SavePrefabIfChanged(assetPath, $"{CARS_TARGET}/{fileName}.prefab");
            created++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[CreateModelPrefabs] 车辆预制体完成：创建/更新 {created} 个，跳过 {skipped} 个");
    }

    // ── 武器 ──────────────────────────────────────────────

    private static void CreateWeaponPrefabsFromFBX()
    {
        if (!AssetDatabase.IsValidFolder(WEAPONS_SOURCE))
        {
            Debug.LogError($"[CreateModelPrefabs] 源目录不存在: {WEAPONS_SOURCE}");
            return;
        }

        EnsureFolderExists(WEAPONS_TARGET);

        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { WEAPONS_SOURCE });
        int created = 0, skipped = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            // 武器只处理 blaster- 前缀的
            if (!fileName.StartsWith("blaster-"))
            {
                skipped++;
                continue;
            }

            SavePrefabIfChanged(assetPath, $"{WEAPONS_TARGET}/{fileName}.prefab");
            created++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[CreateModelPrefabs] 武器预制体完成：创建/更新 {created} 个，跳过 {skipped} 个");
    }

    // ── 通用 ──────────────────────────────────────────────

    /// <summary>
    /// 将 FBX 模型保存/更新为预制体
    /// </summary>
    private static void SavePrefabIfChanged(string sourceAssetPath, string prefabPath)
    {
        GameObject fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(sourceAssetPath);
        if (fbxAsset == null) return;

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset);
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
    }

    /// <summary>
    /// 逐层创建文件夹（如果不存在）
    /// </summary>
    private static void EnsureFolderExists(string path)
    {
        string[] parts = path.Replace("Assets/", "").Split('/');
        string current = "Assets";
        foreach (string part in parts)
        {
            string next = current + "/" + part;
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, part);
            }
            current = next;
        }
    }
}

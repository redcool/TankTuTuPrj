using UnityEngine;
using UnityEditor;
using System.IO;
using Game.Runtime.ValueObject.ScriptableObjects;

/// <summary>
/// Editor 工具 — 创建能量块掉落数据 ScriptableObject
/// 属性 = 基础值 + 随机偏移（每次生成不同）
/// 新建 SO 会打开 Inspector 供进一步编辑
///
/// 菜单: Tools → 铁皮突突 → 创建能量块掉落数据
/// </summary>
public static class CreateEnergyDropDataSO
{
    private const string TARGET_DIR = "Assets/Resources/ScriptableObjects/EnergyDrop";

    // 基础值
    private const int   BASE_AMOUNT       = 1;
    private const float BASE_COLLECT_RNG  = 1.5f;
    private const float BASE_LIFETIME     = 30f;
    private const float BASE_MAGNET_RNG   = 3f;
    private const float BASE_MAGNET_SPD   = 5f;

    // 随机偏移范围
    private const float RNG_COLLECT_RNG = 0.5f;  // ±0.5
    private const float RNG_LIFETIME    = 10f;   // ±10
    private const float RNG_MAGNET_RNG  = 1f;    // ±1
    private const float RNG_MAGNET_SPD  = 1.5f;  // ±1.5

    [MenuItem("Tools/铁皮突突/创建能量块掉落数据")]
    private static void Create()
    {
        if (!Directory.Exists(TARGET_DIR))
            Directory.CreateDirectory(TARGET_DIR);

        var so = ScriptableObject.CreateInstance<EnergyDropDataSO>();

        // ── 固定基础值 ──
        so.DefaultAmount = BASE_AMOUNT;

        // ── 基础 + 随机 ──
        float collectRng = BASE_COLLECT_RNG + Random.Range(-RNG_COLLECT_RNG, RNG_COLLECT_RNG);
        float lifetime   = BASE_LIFETIME    + Random.Range(-RNG_LIFETIME,    RNG_LIFETIME);
        float magnetRng  = BASE_MAGNET_RNG  + Random.Range(-RNG_MAGNET_RNG,  RNG_MAGNET_RNG);
        float magnetSpd  = BASE_MAGNET_SPD  + Random.Range(-RNG_MAGNET_SPD,  RNG_MAGNET_SPD);

        so.CollectRange = Mathf.Max(0.1f, collectRng);
        so.Lifetime     = Mathf.Max(1,    lifetime);
        so.MagnetRange  = Mathf.Max(0,    magnetRng);
        so.MagnetSpeed  = Mathf.Max(0,    magnetSpd);
        so.UseMagnet    = true;

        string path = AssetDatabase.GenerateUniqueAssetPath($"{TARGET_DIR}/EnergyDrop_New.asset");
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CreateEnergyDropDataSO] 创建: {path}\n" +
                  $"  拾取范围={so.CollectRange:F2}({BASE_COLLECT_RNG}±{RNG_COLLECT_RNG}) " +
                  $"存活={so.Lifetime:F1}s({BASE_LIFETIME}±{RNG_LIFETIME}) " +
                  $"磁铁范围={so.MagnetRange:F1}({BASE_MAGNET_RNG}±{RNG_MAGNET_RNG}) " +
                  $"磁力={so.MagnetSpeed:F1}({BASE_MAGNET_SPD}±{RNG_MAGNET_SPD})");

        Selection.activeObject = so;
    }
}

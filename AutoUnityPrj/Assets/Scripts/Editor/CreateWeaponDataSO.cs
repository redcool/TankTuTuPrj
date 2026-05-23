using UnityEngine;
using UnityEditor;
using System.IO;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

/// <summary>
/// Editor 工具 — 创建武器数据 ScriptableObject
/// 属性 = 基础值 + 随机偏移（每次生成不同）
/// 新建 SO 会打开 Inspector 供进一步编辑
///
/// 菜单: Tools → 铁皮突突 → 创建武器数据
/// </summary>
public static class CreateWeaponDataSO
{
    private const string TARGET_DIR = "Assets/Resources/ScriptableObjects/Weapons";

    // 基础值
    private const float BASE_DMG     = 20f;
    private const float BASE_ATK_SPD = 1f;
    private const float BASE_RANGE   = 10f;
    private const int   BASE_PRICE   = 100;

    // 随机偏移范围
    private const float RNG_DMG     = 10f;   // ±10
    private const float RNG_ATK_SPD = 0.3f;  // ±0.3
    private const float RNG_RANGE   = 3f;    // ±3
    private const int   RNG_PRICE   = 50;    // ±50

    [MenuItem("Tools/铁皮突突/创建武器数据")]
    private static void Create()
    {
        if (!Directory.Exists(TARGET_DIR))
            Directory.CreateDirectory(TARGET_DIR);

        var so = ScriptableObject.CreateInstance<WeaponDataSO>();

        // ── 基础值 ──
        so.WeaponNameSetter = "新武器";
        so.WeaponCategorySetter = WeaponCategory.MainCannon;
        so.WeaponTypeSetter     = WeaponType.MainCannon;
        so.IsDefaultSetter      = false;

        // ── 基础 + 随机 ──
        float dmg    = BASE_DMG     + Random.Range(-RNG_DMG,     RNG_DMG);
        float atkSpd = BASE_ATK_SPD + Random.Range(-RNG_ATK_SPD, RNG_ATK_SPD);
        float range  = BASE_RANGE   + Random.Range(-RNG_RANGE,   RNG_RANGE);
        int   price  = BASE_PRICE   + Random.Range(-RNG_PRICE,   RNG_PRICE);

        so.DamageSetter      = Mathf.Max(1, dmg);
        so.AttackSpeedSetter = Mathf.Max(0.1f, atkSpd);
        so.RangeSetter       = Mathf.Max(1, range);
        so.PriceSetter       = Mathf.Max(0, price);
        so.PierceSetter      = 1f;
        so.ProjectileCountSetter = 1;

        string id = $"weapon_{Random.Range(1000, 9999)}";
        so.WeaponIdSetter = id;

        string path = AssetDatabase.GenerateUniqueAssetPath($"{TARGET_DIR}/Weapon_{id}.asset");
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CreateWeaponDataSO] 创建: {path}\n" +
                  $"  伤害={so.Damage:F1}({BASE_DMG}±{RNG_DMG}) 攻速={so.AttackSpeed:F2}({BASE_ATK_SPD}±{RNG_ATK_SPD}) " +
                  $"范围={so.Range:F1}({BASE_RANGE}±{RNG_RANGE}) 价格={so.Price}({BASE_PRICE}±{RNG_PRICE})");

        Selection.activeObject = so;
    }
}

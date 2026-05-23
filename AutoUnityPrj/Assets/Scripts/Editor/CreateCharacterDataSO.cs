using UnityEngine;
using UnityEditor;
using System.IO;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

/// <summary>
/// Editor 工具 — 创建角色数据 ScriptableObject
/// 属性 = 基础值 + 随机偏移（每次生成不同）
/// 新建 SO 会打开 Inspector 供进一步编辑
///
/// 菜单: Tools → 铁皮突突 → 创建角色数据
/// </summary>
public static class CreateCharacterDataSO
{
    private const string TARGET_DIR = "Assets/Resources/ScriptableObjects/Characters";

    // 基础值
    private const int    BASE_HP      = 100;
    private const float  BASE_SPEED   = 3f;
    private const float  BASE_ATK_SPD = 1f;
    private const float  BASE_RANGE   = 10f;
    private const int    BASE_ARMOR   = 0;

    // 随机偏移范围
    private const int    RNG_HP      = 20;    // ±20
    private const float  RNG_SPEED   = 0.5f;  // ±0.5
    private const float  RNG_ATK_SPD = 0.2f;  // ±0.2
    private const float  RNG_RANGE   = 2f;    // ±2
    private const int    RNG_ARMOR   = 2;     // ±2
    private const float  RNG_CRIT    = 5f;    // ±5%

    [MenuItem("Tools/铁皮突突/创建角色数据")]
    private static void Create()
    {
        if (!Directory.Exists(TARGET_DIR))
            Directory.CreateDirectory(TARGET_DIR);

        var so = ScriptableObject.CreateInstance<CharacterDataSO>();

        // ── 基础值 ──
        so.CharacterName = "新角色";
        so.VehicleType   = VehicleType.TANK;
        so.AimAccuracy   = 0.85f;
        so.IsUnlocked    = true;
        so.StartingWeaponPaths = new[] { "ScriptableObjects/Weapons/Weapon_MainCannon_StandardCannon" };

        // ── 基础 + 随机 ──
        int    hp      = BASE_HP      + Random.Range(-RNG_HP,      RNG_HP);
        float  speed   = BASE_SPEED   + Random.Range(-RNG_SPEED,   RNG_SPEED);
        float  atkSpd  = BASE_ATK_SPD + Random.Range(-RNG_ATK_SPD, RNG_ATK_SPD);
        float  range   = BASE_RANGE   + Random.Range(-RNG_RANGE,   RNG_RANGE);
        int    armor   = BASE_ARMOR   + Random.Range(-RNG_ARMOR,   RNG_ARMOR);
        float  crit    = Random.Range(0, RNG_CRIT);

        so.MaxHealth   = Mathf.Max(1, hp);
        so.MoveSpeed   = Mathf.Max(0.1f, speed);
        so.AttackSpeed = Mathf.Max(0.1f, atkSpd);
        so.Range       = Mathf.Max(1, range);
        so.Armor       = Mathf.Max(0, armor);
        so.CritRate    = Mathf.Clamp(crit, 0, 100);

        string id = $"char_{Random.Range(1000, 9999)}";
        so.CharacterId = id;

        string path = AssetDatabase.GenerateUniqueAssetPath($"{TARGET_DIR}/Character_{id}.asset");
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CreateCharacterDataSO] 创建: {path}\n" +
                  $"  HP={so.MaxHealth}({BASE_HP}±{RNG_HP}) 移速={so.MoveSpeed:F2}({BASE_SPEED}±{RNG_SPEED}) " +
                  $"攻速={so.AttackSpeed:F2}({BASE_ATK_SPD}±{RNG_ATK_SPD}) " +
                  $"范围={so.Range:F1}({BASE_RANGE}±{RNG_RANGE}) 护甲={so.Armor}({BASE_ARMOR}±{RNG_ARMOR}) 暴击={so.CritRate:F0}%");

        Selection.activeObject = so;
    }
}

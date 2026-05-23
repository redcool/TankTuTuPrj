using UnityEngine;
using UnityEditor;
using System.IO;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

/// <summary>
/// Editor 工具 — 创建敌人数据 ScriptableObject
/// 属性 = 基础值 + 随机偏移（每次生成不同）
/// 新建 SO 会打开 Inspector 供进一步编辑
///
/// 菜单: Tools → 铁皮突突 → 创建敌人数据
/// </summary>
public static class CreateEnemyDataSO
{
    private const string TARGET_DIR = "Assets/Resources/ScriptableObjects/Enemies";

    // 基础值 (Normal / Elite / Boss)
    private static readonly int[]   BASE_HP    = { 50,  150, 500 };
    private static readonly float[] BASE_SPEED = { 2f,  1.8f, 1.2f };
    private static readonly float[] BASE_DMG   = { 10f, 20f, 35f };

    // 随机偏移范围
    private const int   RNG_HP    = 20;    // ±20
    private const float RNG_SPEED = 0.5f;  // ±0.5
    private const float RNG_DMG   = 5f;    // ±5
    private const float RNG_RANGE = 0.5f;  // ±0.5
    private const int   RNG_ARMOR = 2;     // ±2

    private static readonly EnemyType[] _enemyTypes = {
        EnemyType.Normal,
        EnemyType.Elite,
        EnemyType.Boss,
    };

    [MenuItem("Tools/铁皮突突/创建敌人数据")]
    private static void Create()
    {
        if (!Directory.Exists(TARGET_DIR))
            Directory.CreateDirectory(TARGET_DIR);

        // 随机选一个类型
        EnemyType type     = _enemyTypes[Random.Range(0, _enemyTypes.Length)];
        int       typeIdx  = (int)type;
        string    typeName = type switch
        {
            EnemyType.Normal => "普通",
            EnemyType.Elite  => "精英",
            EnemyType.Boss   => "Boss",
            _                => "普通"
        };

        var so = ScriptableObject.CreateInstance<EnemyDataSO>();

        // ── 基础 + 随机 ──
        int   hp    = BASE_HP[typeIdx]    + Random.Range(-RNG_HP,    RNG_HP);
        float speed = BASE_SPEED[typeIdx] + Random.Range(-RNG_SPEED, RNG_SPEED);
        float dmg   = BASE_DMG[typeIdx]   + Random.Range(-RNG_DMG,   RNG_DMG);
        float range = 1.5f                + Random.Range(-RNG_RANGE, RNG_RANGE);
        int   armor = Random.Range(0, RNG_ARMOR + 1);

        so.EnemyId       = $"enemy_{Random.Range(1000, 9999)}";
        so.EnemyName     = $"{typeName}敌人";
        so.EnemyType     = type;
        so.MaxHealth     = Mathf.Max(1, hp);
        so.MoveSpeed     = Mathf.Max(0.1f, speed);
        so.AttackDamage  = Mathf.Max(1, dmg);
        so.AttackRange   = Mathf.Max(0.1f, range);
        so.AttackInterval = Mathf.Max(0.1f, 1f + Random.Range(-0.3f, 0.3f));
        so.CritRate      = Random.Range(0, 10f);
        so.Armor         = Mathf.Max(0, armor);
        so.EnergyDrop    = type == EnemyType.Boss ? Random.Range(5, 16) : Random.Range(1, 4);
        so.DropChance    = 1f;
        so.DropTreasureBox   = type == EnemyType.Boss;
        so.TreasureBoxDropChance = type == EnemyType.Boss ? Random.Range(50, 101) : 0;

        string path = AssetDatabase.GenerateUniqueAssetPath($"{TARGET_DIR}/Enemy_{so.EnemyId}.asset");
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[CreateEnemyDataSO] 创建: {path} [{typeName}]\n" +
                  $"  HP={so.MaxHealth}({BASE_HP[typeIdx]}±{RNG_HP}) 移速={so.MoveSpeed:F2}({BASE_SPEED[typeIdx]}±{RNG_SPEED}) " +
                  $"攻击={so.AttackDamage:F1}({BASE_DMG[typeIdx]}±{RNG_DMG}) 范围={so.AttackRange:F1} 护甲={so.Armor}");

        Selection.activeObject = so;
    }
}

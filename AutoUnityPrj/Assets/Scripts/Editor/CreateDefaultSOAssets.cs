#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.Editor
{
    /// <summary>
    /// 编辑器工具 - 一键创建所有默认ScriptableObject资产
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class CreateDefaultSOAssets
    {
        private const string TANK_PATH = "Assets/ScriptableObjects/Tank/";
        private const string WEAPON_PATH = "Assets/ScriptableObjects/Weapon/";
        private const string ENEMY_PATH = "Assets/ScriptableObjects/Enemy/";
        private const string ITEM_PATH = "Assets/ScriptableObjects/Item/";

        [MenuItem("铁皮突突/创建默认ScriptableObject资产", false, 1)]
        public static void CreateAllDefaults()
        {
            CreateDefaultTank();
            CreateDefaultWeapons();
            CreateDefaultEnemies();
            CreateDefaultItems();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[CreateDefaultSOAssets] 所有默认ScriptableObject资产创建完成！");
        }

        private static void CreateDefaultTank()
        {
            var tankData = ScriptableObject.CreateInstance<TankDataSO>();
            tankData.name = "DefaultTank";
            AssetDatabase.CreateAsset(tankData, TANK_PATH + "DefaultTank.asset");
            Debug.Log("  ✓ 创建: DefaultTank.asset");
        }

        private static void CreateDefaultWeapons()
        {
            // 默认机枪
            var blaster = ScriptableObject.CreateInstance<WeaponDataSO>();
            blaster.name = "DefaultBlaster";
            AssetDatabase.CreateAsset(blaster, WEAPON_PATH + "DefaultBlaster.asset");

            Debug.Log("  ✓ 创建: DefaultBlaster.asset");
        }

        private static void CreateDefaultEnemies()
        {
            // 海狸
            var beaver = ScriptableObject.CreateInstance<EnemyDataSO>();
            beaver.name = "Beaver";
            AssetDatabase.CreateAsset(beaver, ENEMY_PATH + "Beaver.asset");

            // 奶牛
            var cow = ScriptableObject.CreateInstance<EnemyDataSO>();
            cow.name = "Cow";
            AssetDatabase.CreateAsset(cow, ENEMY_PATH + "Cow.asset");

            // 大象Boss
            var elephant = ScriptableObject.CreateInstance<EnemyDataSO>();
            elephant.name = "ElephantBoss";
            AssetDatabase.CreateAsset(elephant, ENEMY_PATH + "ElephantBoss.asset");

            Debug.Log("  ✓ 创建: Beaver.asset, Cow.asset, ElephantBoss.asset");
        }

        private static void CreateDefaultItems()
        {
            // 预留，后续添加道具SO
            Debug.Log("  ✓ Item目录已就绪");
        }
    }
}
#endif

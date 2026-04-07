using UnityEngine;
using UnityEditor;
using System.IO;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Editor
{
    /// <summary>
    /// 编辑器工具 - 创建默认角色数据
    /// 菜单: IronTutu → Create Default Characters
    /// </summary>
    public class CreateDefaultCharacters : EditorWindow
    {
        [MenuItem("IronTutu/Create Default Characters")]
        public static void CreateDefaults()
        {
            string folderPath = "Assets/Resources/ScriptableObjects/Characters";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 5个默认解锁角色（参考土豆兄弟的初始角色）
            var characters = new CharacterDataSO[]
            {
                CreateWellRounded(),    // 均衡型 - 默认解锁
                CreateBrawler(),        // 近战型 - 默认解锁
                CreateRanger(),         // 远程型 - 默认解锁
                CreateEngineer(),       // 工程型 - 默认解锁
                CreateLucky(),          // 幸运型 - 默认解锁
            };

            foreach (var character in characters)
            {
                string assetPath = $"{folderPath}/{character.characterName}.asset";
                AssetDatabase.CreateAsset(character, assetPath);
                Debug.Log($"[创建角色] {character.characterName} → {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[铁皮突突] 5个默认角色数据创建完成！");
        }

        private static CharacterDataSO CreateWellRounded()
        {
            var so = ScriptableObject.CreateInstance<CharacterDataSO>();
            so.characterName = "WellRounded";
            so.description = "Balanced tank, suitable for beginners";
            so.maxHpBonus = 0;
            so.speedBonusPercent = 0;
            so.attackSpeedBonusPercent = 0;
            so.critChanceBonus = 0;
            so.armorBonus = 0;
            so.rangeBonusPercent = 0;
            so.luckBonus = 0;
            so.harvestingBonus = 0;
            so.startingWeaponPaths = new[] { "Weapons/DefaultBlaster" };
            so.isUnlockedByDefault = true;
            so.unlockCondition = "";
            so.specialAbility = "No special ability, all stats balanced";
            return so;
        }

        private static CharacterDataSO CreateBrawler()
        {
            var so = ScriptableObject.CreateInstance<CharacterDataSO>();
            so.characterName = "Brawler";
            so.description = "Melee focused, high attack speed and dodge";
            so.maxHpBonus = 20;
            so.speedBonusPercent = 0.1f;
            so.attackSpeedBonusPercent = 0.3f;
            so.critChanceBonus = 0.05f;
            so.armorBonus = 2;
            so.rangeBonusPercent = -0.3f;
            so.luckBonus = 0;
            so.harvestingBonus = 0;
            so.startingWeaponPaths = new[] { "Weapons/IronBall" };
            so.isUnlockedByDefault = true;
            so.unlockCondition = "";
            so.specialAbility = "Melee damage +30%, range -30%";
            return so;
        }

        private static CharacterDataSO CreateRanger()
        {
            var so = ScriptableObject.CreateInstance<CharacterDataSO>();
            so.characterName = "Ranger";
            so.description = "Ranged focused, large range and high precision";
            so.maxHpBonus = -10;
            so.speedBonusPercent = 0;
            so.attackSpeedBonusPercent = 0;
            so.critChanceBonus = 0.1f;
            so.armorBonus = 0;
            so.rangeBonusPercent = 0.5f;
            so.luckBonus = 0;
            so.harvestingBonus = 0;
            so.startingWeaponPaths = new[] { "Weapons/Laser" };
            so.isUnlockedByDefault = true;
            so.unlockCondition = "";
            so.specialAbility = "Range +50%, crit chance +10%, cannot equip melee weapons";
            return so;
        }

        private static CharacterDataSO CreateEngineer()
        {
            var so = ScriptableObject.CreateInstance<CharacterDataSO>();
            so.characterName = "Engineer";
            so.description = "Turret focused, auto defense";
            so.maxHpBonus = 10;
            so.speedBonusPercent = -0.05f;
            so.attackSpeedBonusPercent = 0;
            so.critChanceBonus = 0;
            so.armorBonus = 5;
            so.rangeBonusPercent = 0;
            so.luckBonus = 0;
            so.harvestingBonus = 5;
            so.startingWeaponPaths = new[] { "Weapons/Flame" };
            so.isUnlockedByDefault = true;
            so.unlockCondition = "";
            so.specialAbility = "Engineering damage +25%, turret spawn range reduced";
            return so;
        }

        private static CharacterDataSO CreateLucky()
        {
            var so = ScriptableObject.CreateInstance<CharacterDataSO>();
            so.characterName = "Lucky";
            so.description = "High luck, high drop rate";
            so.maxHpBonus = 0;
            so.speedBonusPercent = 0;
            so.attackSpeedBonusPercent = -0.2f;
            so.critChanceBonus = 0.15f;
            so.armorBonus = 0;
            so.rangeBonusPercent = 0;
            so.luckBonus = 50;
            so.harvestingBonus = 10;
            so.startingWeaponPaths = new[] { "Weapons/DefaultBlaster" };
            so.isUnlockedByDefault = true;
            so.unlockCondition = "";
            so.specialAbility = "Luck +50, drop rate +20%, attack speed -20%";
            return so;
        }
    }
}

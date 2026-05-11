#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.Editor
{
    /// <summary>
    /// 编辑器工具 - 创建默认角色数据
    /// </summary>
    public class CreateDefaultCharacters
    {
        [MenuItem("铁皮突突/创建数据/创建默认角色数据")]
        public static void CreateDefaults()
        {
            string folderPath = "Assets/Resources/ScriptableObjects/Characters";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 5个默认解锁角色
            CreateCharacterAsset(folderPath, "均衡型", "均衡型战车,适合新手", 0, 0, 0, 0, 0, 0, 0, 0, new[] { "ScriptableObjects/Weapons/主炮" }, true, "无特殊能力,所有属性均衡");
            CreateCharacterAsset(folderPath, "突击型", "近战Focused,高攻速和闪避", 20, 0.1f, 0.3f, 0.05f, 2, -0.3f, 0, 0, new[] { "ScriptableObjects/Weapons/加农炮" }, true, "近战伤害+30%,范围-30%");
            CreateCharacterAsset(folderPath, "狙击型", "远程Focused,大范围和高精度", -10, 0, 0, 0.1f, 0, 0.5f, 0, 0, new[] { "ScriptableObjects/Weapons/主炮" }, true, "范围+50%,暴击+10%");
            CreateCharacterAsset(folderPath, "工程型", "炮塔Focused,自动防御", 10, -0.05f, 0, 0, 5, 0, 0, 5, new[] { "ScriptableObjects/Weapons/榴弹炮" }, true, "工程伤害+25%,炮塔生成范围减少");
            CreateCharacterAsset(folderPath, "幸运型", "高幸运,高掉落率", 0, 0, -0.2f, 0.15f, 0, 0, 50, 10, new[] { "ScriptableObjects/Weapons/主炮" }, true, "幸运+50,掉落率+20%,攻速-20%");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[铁皮突突] 5个默认角色数据创建完成!");
        }

        private static void CreateCharacterAsset(string folder, string name, string desc, 
            int maxHpBonus, float speedBonus, float atkSpeedBonus, float critBonus, int armorBonus, 
            float rangeBonus, int luckBonus, int harvestBonus, string[] weapons, bool unlocked, string ability)
        {
            string assetPath = $"{folder}/{name}.asset";
            
            if (File.Exists(assetPath))
            {
                Debug.Log($"[CreateDefaultCharacters] {name} 已存在,跳过");
                return;
            }

            var so = ScriptableObject.CreateInstance<CharacterDataSO>();
            
            // 使用 setter 直接设置属性
            so.CharacterName = name;
            so.Description = desc;
            so.MaxHpBonus = maxHpBonus;
            so.SpeedBonusPercent = speedBonus;
            so.AttackSpeedBonusPercent = atkSpeedBonus;
            so.CritChanceBonus = critBonus;
            so.ArmorBonus = armorBonus;
            so.RangeBonusPercent = rangeBonus;
            so.LuckBonus = luckBonus;
            so.HarvestingBonus = harvestBonus;
            so.StartingWeaponPaths = weapons;
            so.IsUnlockedByDefault = unlocked;
            so.SpecialAbility = ability;
            
            AssetDatabase.CreateAsset(so, assetPath);
            Debug.Log($"[CreateDefaultCharacters] 创建: {name}");
        }
    }
}
#endif
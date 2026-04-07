using UnityEngine;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 角色数据 ScriptableObject - 定义可选角色的属性和初始配置
    /// 参考土豆兄弟的角色选择系统
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "IronTutu/CharacterData")]
    public class CharacterDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string characterName;
        public Sprite icon;
        [TextArea(2, 3)]
        public string description;

        [Header("Stat Bonuses")]
        [Tooltip("Max HP bonus")]
        public int maxHpBonus;
        [Tooltip("Speed bonus (percentage)")]
        public float speedBonusPercent;
        [Tooltip("Attack speed bonus (percentage)")]
        public float attackSpeedBonusPercent;
        [Tooltip("Crit chance bonus")]
        public float critChanceBonus;
        [Tooltip("Armor bonus")]
        public int armorBonus;
        [Tooltip("Range bonus (percentage)")]
        public float rangeBonusPercent;
        [Tooltip("Luck bonus")]
        public int luckBonus;
        [Tooltip("Harvesting bonus")]
        public int harvestingBonus;

        [Header("Starting Weapons")]
        [Tooltip("Resource paths for starting weapons")]
        public string[] startingWeaponPaths;

        [Header("Unlock")]
        [Tooltip("Unlocked by default")]
        public bool isUnlockedByDefault = true;
        [Tooltip("Unlock condition text (shown when locked)")]
        public string unlockCondition;
        [Tooltip("Required progress value to unlock")]
        public int unlockRequirement;

        [Header("Special Ability")]
        [Tooltip("Special ability description")]
        [TextArea(2, 4)]
        public string specialAbility;

        /// <summary>
        /// 检查角色是否已解锁
        /// </summary>
        public bool IsUnlocked()
        {
            if (isUnlockedByDefault) return true;
            // TODO: 从存档系统读取进度
            return false;
        }

        /// <summary>
        /// 获取完整的属性描述文本（中文显示）
        /// </summary>
        public string GetStatsDescription()
        {
            var sb = new System.Text.StringBuilder();

            if (maxHpBonus != 0)
                sb.AppendLine(maxHpBonus > 0 ? $"+{maxHpBonus} 最大生命" : $"{maxHpBonus} 最大生命");
            if (speedBonusPercent != 0)
                sb.AppendLine(speedBonusPercent > 0 ? $"+{speedBonusPercent * 100:F0}% 移速" : $"{speedBonusPercent * 100:F0}% 移速");
            if (attackSpeedBonusPercent != 0)
                sb.AppendLine(attackSpeedBonusPercent > 0 ? $"+{attackSpeedBonusPercent * 100:F0}% 攻速" : $"{attackSpeedBonusPercent * 100:F0}% 攻速");
            if (critChanceBonus != 0)
                sb.AppendLine(critChanceBonus > 0 ? $"+{critChanceBonus * 100:F0}% 暴击" : $"{critChanceBonus * 100:F0}% 暴击");
            if (armorBonus != 0)
                sb.AppendLine(armorBonus > 0 ? $"+{armorBonus} 护甲" : $"{armorBonus} 护甲");
            if (rangeBonusPercent != 0)
                sb.AppendLine(rangeBonusPercent > 0 ? $"+{rangeBonusPercent * 100:F0}% 范围" : $"{rangeBonusPercent * 100:F0}% 范围");
            if (luckBonus != 0)
                sb.AppendLine(luckBonus > 0 ? $"+{luckBonus} 幸运" : $"{luckBonus} 幸运");
            if (harvestingBonus != 0)
                sb.AppendLine(harvestingBonus > 0 ? $"+{harvestingBonus} 收获" : $"{harvestingBonus} 收获");

            if (!string.IsNullOrEmpty(specialAbility))
            {
                sb.AppendLine();
                sb.AppendLine(specialAbility);
            }

            return sb.ToString();
        }
    }
}

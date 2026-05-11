using UnityEngine;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 角色数据 ScriptableObject - 定义可选角色的属性和初始配置
    /// 参考土豆兄弟的角色选择系统
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "铁皮突突/角色数据")]
    public class CharacterDataSO : ScriptableObject
    {
        [Header("基础信息")]
        [SerializeField] private string _characterName = "";
        [SerializeField] private Sprite _icon;
        [TextArea(2, 3)]
        [SerializeField] private string _description = "";

        [Header("属性加成")]
        [Tooltip("最大生命加成")]
        [SerializeField] private int _maxHpBonus = 0;
        [Tooltip("移速加成(百分比)")]
        [SerializeField] private float _speedBonusPercent = 0f;
        [Tooltip("攻速加成(百分比)")]
        [SerializeField] private float _attackSpeedBonusPercent = 0f;
        [Tooltip("暴击几率加成")]
        [SerializeField] private float _critChanceBonus = 0f;
        [Tooltip("护甲加成")]
        [SerializeField] private int _armorBonus = 0;
        [Tooltip("范围加成(百分比)")]
        [SerializeField] private float _rangeBonusPercent = 0f;
        [Tooltip("幸运加成")]
        [SerializeField] private int _luckBonus = 0;
        [Tooltip("收获加成")]
        [SerializeField] private int _harvestingBonus = 0;

        [Header("初始武器")]
        [Tooltip("初始武器资源路径")]
        [SerializeField] private string[] _startingWeaponPaths;

        [Header("解锁")]
        [Tooltip("默认解锁")]
        [SerializeField] private bool _isUnlockedByDefault = true;
        [Tooltip("解锁条件文本(锁定时显示)")]
        [SerializeField] private string _unlockCondition = "";
        [Tooltip("解锁所需进度值")]
        [SerializeField] private int _unlockRequirement = 0;

        [Header("特殊能力")]
        [Tooltip("特殊能力描述")]
        [TextArea(2, 4)]
        [SerializeField] private string _specialAbility = "";

        #region Properties

        public string CharacterName
        {
            get => _characterName;
            set => _characterName = value;
        }

        public Sprite Icon
        {
            get => _icon;
            set => _icon = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public int MaxHpBonus
        {
            get => _maxHpBonus;
            set => _maxHpBonus = value;
        }

        public float SpeedBonusPercent
        {
            get => _speedBonusPercent;
            set => _speedBonusPercent = value;
        }

        public float AttackSpeedBonusPercent
        {
            get => _attackSpeedBonusPercent;
            set => _attackSpeedBonusPercent = value;
        }

        public float CritChanceBonus
        {
            get => _critChanceBonus;
            set => _critChanceBonus = value;
        }

        public int ArmorBonus
        {
            get => _armorBonus;
            set => _armorBonus = value;
        }

        public float RangeBonusPercent
        {
            get => _rangeBonusPercent;
            set => _rangeBonusPercent = value;
        }

        public int LuckBonus
        {
            get => _luckBonus;
            set => _luckBonus = value;
        }

        public int HarvestingBonus
        {
            get => _harvestingBonus;
            set => _harvestingBonus = value;
        }

        public string[] StartingWeaponPaths
        {
            get => _startingWeaponPaths;
            set => _startingWeaponPaths = value;
        }

        public bool IsUnlockedByDefault
        {
            get => _isUnlockedByDefault;
            set => _isUnlockedByDefault = value;
        }

        public string UnlockCondition
        {
            get => _unlockCondition;
            set => _unlockCondition = value;
        }

        public int UnlockRequirement
        {
            get => _unlockRequirement;
            set => _unlockRequirement = value;
        }

        public string SpecialAbility
        {
            get => _specialAbility;
            set => _specialAbility = value;
        }

        #endregion

        /// <summary>
        /// 检查角色是否已解锁
        /// </summary>
        public bool IsUnlocked()
        {
            if (_isUnlockedByDefault) return true;
            // TODO: 从存档系统读取进度
            return false;
        }

        /// <summary>
        /// 获取完整的属性描述文本（中文显示）
        /// </summary>
        public string GetStatsDescription()
        {
            var sb = new System.Text.StringBuilder();

            if (_maxHpBonus != 0)
                sb.AppendLine(_maxHpBonus > 0 ? $"+{_maxHpBonus} 最大生命" : $"{_maxHpBonus} 最大生命");
            if (_speedBonusPercent != 0)
                sb.AppendLine(_speedBonusPercent > 0 ? $"+{_speedBonusPercent * 100:F0}% 移速" : $"{_speedBonusPercent * 100:F0}% 移速");
            if (_attackSpeedBonusPercent != 0)
                sb.AppendLine(_attackSpeedBonusPercent > 0 ? $"+{_attackSpeedBonusPercent * 100:F0}% 攻速" : $"{_attackSpeedBonusPercent * 100:F0}% 攻速");
            if (_critChanceBonus != 0)
                sb.AppendLine(_critChanceBonus > 0 ? $"+{_critChanceBonus * 100:F0}% 暴击" : $"{_critChanceBonus * 100:F0}% 暴击");
            if (_armorBonus != 0)
                sb.AppendLine(_armorBonus > 0 ? $"+{_armorBonus} 护甲" : $"{_armorBonus} 护甲");
            if (_rangeBonusPercent != 0)
                sb.AppendLine(_rangeBonusPercent > 0 ? $"+{_rangeBonusPercent * 100:F0}% 范围" : $"{_rangeBonusPercent * 100:F0}% 范围");
            if (_luckBonus != 0)
                sb.AppendLine(_luckBonus > 0 ? $"+{_luckBonus} 幸运" : $"{_luckBonus} 幸运");
            if (_harvestingBonus != 0)
                sb.AppendLine(_harvestingBonus > 0 ? $"+{_harvestingBonus} 收获" : $"{_harvestingBonus} 收获");

            if (!string.IsNullOrEmpty(_specialAbility))
            {
                sb.AppendLine();
                sb.AppendLine(_specialAbility);
            }

            return sb.ToString();
        }
    }
}
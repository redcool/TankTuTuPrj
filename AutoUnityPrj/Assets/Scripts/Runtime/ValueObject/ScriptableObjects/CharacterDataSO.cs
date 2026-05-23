using UnityEngine;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 车辆类型枚举
    /// </summary>
    public enum VehicleType
    {
        TANK,       // 主战/重型坦克
        LIGHT,      // 轻型/侦察车
        APC,        // 运兵/工程支援车
        SPG,        // 自行火炮/高炮
        SPECIAL     // 特种（喷火等）
    }

    /// <summary>
    /// 角色数据 ScriptableObject — 单一数据源
    /// 持有基值 + 正负修正（参考土豆兄弟的角色系统）
    /// ToTankDataValue() 合并产出最终运行时属性
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "铁皮突突/角色数据")]
    public class CharacterDataSO : ScriptableObject
    {
        [Header("标识")]
        [SerializeField] private string _characterId = "";
        [SerializeField] private string _characterName = "";
        [SerializeField] private VehicleType _vehicleType = VehicleType.TANK;
        [SerializeField] private Sprite _icon;
        [TextArea(2, 3)]
        [SerializeField] private string _description = "";

        // ══════════════════════════════════════════════
        //  基值字段
        // ══════════════════════════════════════════════

        [Header("基值 — 生命")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _healthRegen = 0.5f;
        [SerializeField] private float _lifesteal = 0f;

        [Header("基值 — 伤害")]
        [SerializeField] private float _percentDamage = 0f;
        [SerializeField] private float _rangedDamage = 0f;
        [SerializeField] private float _meleeDamage = 0f;
        [SerializeField] private float _elementDamage = 0f;
        [SerializeField] private float _engineering = 0f;

        [Header("基值 — 战斗")]
        [SerializeField] private float _attackSpeed = 5f;
        [SerializeField] private float _critRate = 5f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private float _aimAccuracy = 0.85f;

        [Header("基值 — 防御")]
        [SerializeField] private int _armor = 0;
        [SerializeField] private float _dodge = 0f;

        [Header("基值 — 移动")]
        [SerializeField] private float _moveSpeed = 3f;

        [Header("基值 — 成长")]
        [SerializeField] private float _luck = 0f;
        [SerializeField] private float _harvest = 1f;

        // ══════════════════════════════════════════════
        //  正负修正字段（模仿土豆兄弟角色系统）
        // ══════════════════════════════════════════════

        [Header("属性修正（正/负，参考 Brotato 角色设计）")]
        [Tooltip("最大生命修正")]
        [SerializeField] private int _maxHpMod = 0;
        [Tooltip("移速修正(百分比)")]
        [SerializeField] private float _speedModPercent = 0f;
        [Tooltip("攻速修正(百分比)")]
        [SerializeField] private float _attackSpeedModPercent = 0f;
        [Tooltip("暴击几率修正")]
        [SerializeField] private float _critChanceMod = 0f;
        [Tooltip("护甲修正")]
        [SerializeField] private int _armorMod = 0;
        [Tooltip("范围修正(百分比)")]
        [SerializeField] private float _rangeModPercent = 0f;
        [Tooltip("闪避修正(百分比)")]
        [SerializeField] private float _dodgeMod = 0f;
        [Tooltip("幸运修正")]
        [SerializeField] private int _luckMod = 0;
        [Tooltip("收获修正")]
        [SerializeField] private float _harvestMod = 0f;

        [Header("初始武器")]
        [Tooltip("初始武器资源路径")]
        [SerializeField] private string[] _startingWeaponPaths;

        [Header("解锁")]
        [Tooltip("当前是否已解锁 (true=可选, false=锁定)")]
        [SerializeField] private bool _isUnlocked = true;
        [SerializeField] private string _unlockCondition = "";
        [SerializeField] private int _unlockRequirement = 0;

        [Header("特殊能力")]
        [TextArea(2, 4)]
        [SerializeField] private string _specialAbility = "";
        [SerializeField] private string _specialAbilityType = "";

        #region Properties — 标识

        public string CharacterId     { get => _characterId; set => _characterId = value; }
        public string CharacterName   { get => _characterName; set => _characterName = value; }
        public VehicleType VehicleType { get => _vehicleType; set => _vehicleType = value; }
        public Sprite Icon            { get => _icon; set => _icon = value; }
        public string Description     { get => _description; set => _description = value; }

        #endregion

        #region Properties — 基值

        public int     MaxHealth      { get => _maxHealth; set => _maxHealth = Mathf.Max(1, value); }
        public float   HealthRegen    { get => _healthRegen; set => _healthRegen = Mathf.Max(0, value); }
        public float   Lifesteal      { get => _lifesteal; set => _lifesteal = Mathf.Clamp01(value); }
        public float   PercentDamage  { get => _percentDamage; set => _percentDamage = value; }
        public float   RangedDamage   { get => _rangedDamage; set => _rangedDamage = value; }
        public float   MeleeDamage    { get => _meleeDamage; set => _meleeDamage = value; }
        public float   ElementDamage  { get => _elementDamage; set => _elementDamage = value; }
        public float   Engineering    { get => _engineering; set => _engineering = value; }
        public float   AttackSpeed    { get => _attackSpeed; set => _attackSpeed = Mathf.Max(0.1f, value); }
        public float   CritRate       { get => _critRate; set => _critRate = Mathf.Clamp(value, 0, 100); }
        public float   Range          { get => _range; set => _range = Mathf.Max(0, value); }
        public float   AimAccuracy    { get => _aimAccuracy; set => _aimAccuracy = Mathf.Clamp01(value); }
        public int     Armor          { get => _armor; set => _armor = Mathf.Max(0, value); }
        public float   Dodge          { get => _dodge; set => _dodge = Mathf.Clamp(value, 0, 100); }
        public float   MoveSpeed      { get => _moveSpeed; set => _moveSpeed = Mathf.Max(0.1f, value); }
        public float   Luck           { get => _luck; set => _luck = value; }
        public float   Harvest        { get => _harvest; set => _harvest = Mathf.Max(0.1f, value); }

        #endregion

        #region Properties — 修正

        public int     MaxHpMod           { get => _maxHpMod; set => _maxHpMod = value; }
        public float   SpeedModPercent    { get => _speedModPercent; set => _speedModPercent = value; }
        public float   AttackSpeedModPercent { get => _attackSpeedModPercent; set => _attackSpeedModPercent = value; }
        public float   CritChanceMod      { get => _critChanceMod; set => _critChanceMod = value; }
        public int     ArmorMod           { get => _armorMod; set => _armorMod = value; }
        public float   RangeModPercent    { get => _rangeModPercent; set => _rangeModPercent = value; }
        public float   DodgeMod           { get => _dodgeMod; set => _dodgeMod = Mathf.Clamp(value, -100, 100); }
        public int     LuckMod            { get => _luckMod; set => _luckMod = value; }
        public float   HarvestMod         { get => _harvestMod; set => _harvestMod = value; }

        #endregion

        #region Properties — 其他

        public string[] StartingWeaponPaths  { get => _startingWeaponPaths; set => _startingWeaponPaths = value; }
        public bool    IsUnlocked            { get => _isUnlocked; set => _isUnlocked = value; }
        public string  UnlockCondition       { get => _unlockCondition; set => _unlockCondition = value; }
        public int     UnlockRequirement     { get => _unlockRequirement; set => _unlockRequirement = value; }
        public string  SpecialAbility        { get => _specialAbility; set => _specialAbility = value; }
        public string  SpecialAbilityType    { get => _specialAbilityType; set => _specialAbilityType = value; }

        #endregion

        /// <summary>
        /// 检查角色是否已解锁（运行时，后续对接存档系统）
        /// </summary>
        public bool CheckUnlocked()
        {
            if (_isUnlocked) return true;
            // TODO: 从存档系统读取进度
            return false;
        }

        /// <summary>
        /// 生成运行时 TankDataValue（基值 + 修正）
        /// </summary>
        public TankDataValue ToTankDataValue()
        {
            var data = new TankDataValue();
            data.MaxHealth      = _maxHealth + _maxHpMod;
            data.HealthRegen    = _healthRegen;
            data.Lifesteal      = _lifesteal;
            data.PercentDamage  = _percentDamage;
            data.RangedDamage   = _rangedDamage;
            data.MeleeDamage    = _meleeDamage;
            data.ElementDamage  = _elementDamage;
            data.Engineering    = _engineering;
            data.AttackSpeed    = Mathf.Max(0.1f, _attackSpeed * (1f + _attackSpeedModPercent));
            data.CritRate       = Mathf.Clamp(_critRate + _critChanceMod * 100f, 0, 100);
            data.Range          = _range * (1f + _rangeModPercent);
            data.AimAccuracy    = _aimAccuracy;
            data.Armor          = Mathf.Max(0, _armor + _armorMod);
            data.Dodge          = Mathf.Clamp(_dodge + _dodgeMod, 0, 100);
            data.MoveSpeed      = Mathf.Max(0.1f, _moveSpeed * (1f + _speedModPercent));
            data.Luck           = _luck + _luckMod;
            data.Harvest        = Mathf.Max(0.1f, _harvest + _harvestMod);
            data.CurrentHealth  = data.MaxHealth;
            return data;
        }

        /// <summary>
        /// 获取属性描述文本（用于 UI 详情面板）
        /// </summary>
        public string GetStatsDescription()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"生命: {_maxHealth}{FormatMod(_maxHpMod)}");
            sb.AppendLine($"生命再生: {_healthRegen:F1}/s");
            sb.AppendLine($"移速: {_moveSpeed:F1}{FormatModPercent(_speedModPercent)}");
            sb.AppendLine($"攻速: {_attackSpeed:F1}{FormatModPercent(_attackSpeedModPercent)}");
            sb.AppendLine($"伤害: {_percentDamage * 100:F0}%");
            if (_rangedDamage != 0) sb.AppendLine($"远程伤害: +{_rangedDamage}");
            if (_meleeDamage != 0) sb.AppendLine($"近战伤害: +{_meleeDamage}");
            if (_elementDamage != 0) sb.AppendLine($"元素伤害: +{_elementDamage}");
            sb.AppendLine($"暴击: {_critRate:F0}%{FormatModPercent(_critChanceMod)}");
            sb.AppendLine($"护甲: {_armor}{FormatMod(_armorMod)}");
            sb.AppendLine($"闪避: {_dodge:F0}%{FormatModPercent(_dodgeMod)}");
            sb.AppendLine($"范围: {_range:F1}{FormatModPercent(_rangeModPercent)}");
            sb.AppendLine($"幸运: {_luck:F0}{FormatMod(_luckMod)}");
            sb.AppendLine($"收获: {_harvest:F1}{FormatModFloat(_harvestMod)}");

            if (!string.IsNullOrEmpty(_specialAbility))
            {
                sb.AppendLine();
                sb.AppendLine(_specialAbility);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取简短的一行摘要（用于卡片）
        /// </summary>
        public string GetCardSubtitle()
        {
            return $"HP {_maxHealth} | 移速 {_moveSpeed:F1}";
        }

        // ── 辅助格式化 ──
        private static string FormatMod(int val) => val == 0 ? "" : (val > 0 ? $" (+{val})" : $" ({val})");
        private static string FormatModPercent(float val) => val == 0f ? "" : (val > 0f ? $" (+{val * 100:F0}%)" : $" ({val * 100:F0}%)");
        private static string FormatModFloat(float val) => val == 0f ? "" : (val > 0f ? $" (+{val:F1})" : $" ({val:F1})");
    }
}

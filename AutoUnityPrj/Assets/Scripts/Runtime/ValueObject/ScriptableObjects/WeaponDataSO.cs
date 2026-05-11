using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 武器数据 ScriptableObject - 可在Inspector中配置
    /// 作者：AI
    /// 最后修改时间：2026-04-09
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "铁皮突突/武器数据")]
    public class WeaponDataSO : ScriptableObject
    {
        [Header("基础信息")]
        [SerializeField] private string _weaponId = "";
        [SerializeField] private string _weaponName = "";
        [SerializeField] private Sprite _icon;
        [TextArea(2, 4)]
        [SerializeField] private string _description = "";
        [SerializeField] private WeaponType _weaponType = WeaponType.MainCannon;

        [Header("基础属性")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _maxLevel = 5;

        [Header("特殊属性")]
        [SerializeField] private float _pierce = 1f;
        [SerializeField] private float _area = 0f;
        [SerializeField] private float _duration = 0f;

        [Header("商业属性")]
        [SerializeField] private int _price = 100;
        [SerializeField] private int _upgradeCost = 100;
        [SerializeField] private bool _isDefault = false;

        #region Properties

        public string WeaponId => _weaponId;
        public string WeaponName => _weaponName;
        public Sprite Icon => _icon;
        public Sprite IconSetter
        {
            get => _icon;
            set => _icon = value;
        }
        public string Description => _description;
        public WeaponType WeaponType => _weaponType;
        public float Damage => _damage;
        public float AttackSpeed => _attackSpeed;
        public float Range => _range;
        public int Level => _level;
        public int MaxLevel => _maxLevel;
        public float Pierce => _pierce;
        public float Area => _area;
        public float Duration => _duration;
        public int Price => _price;
        public int UpgradeCost => _upgradeCost;
        public bool IsDefault => _isDefault;

        public int PriceSetter
        {
            get => _price;
            set => _price = Mathf.Max(0, value);
        }

        public bool IsDefaultSetter
        {
            get => _isDefault;
            set => _isDefault = value;
        }

        public string DescriptionSetter
        {
            get => _description;
            set => _description = value;
        }

        public string WeaponIdSetter
        {
            get => _weaponId;
            set => _weaponId = value;
        }

        public string WeaponNameSetter
        {
            get => _weaponName;
            set => _weaponName = value;
        }

        public WeaponType WeaponTypeSetter
        {
            get => _weaponType;
            set => _weaponType = value;
        }

        public float DamageSetter
        {
            get => _damage;
            set => _damage = Mathf.Max(0, value);
        }

        public float AttackSpeedSetter
        {
            get => _attackSpeed;
            set => _attackSpeed = Mathf.Max(0.1f, value);
        }

        public float RangeSetter
        {
            get => _range;
            set => _range = Mathf.Max(0, value);
        }

        public int LevelSetter
        {
            get => _level;
            set => _level = Mathf.Clamp(value, 1, _maxLevel);
        }

        public int MaxLevelSetter
        {
            get => _maxLevel;
            set => _maxLevel = Mathf.Max(1, value);
        }

        public float PierceSetter
        {
            get => _pierce;
            set => _pierce = Mathf.Max(1, value);
        }

        public float AreaSetter
        {
            get => _area;
            set => _area = Mathf.Max(0, value);
        }

        public float DurationSetter
        {
            get => _duration;
            set => _duration = Mathf.Max(0, value);
        }

        public int UpgradeCostSetter
        {
            get => _upgradeCost;
            set => _upgradeCost = Mathf.Max(0, value);
        }

        #endregion

        /// <summary>
        /// 转换为 WeaponDataValue
        /// </summary>
        public WeaponDataValue ToDataValue()
        {
            var data = new WeaponDataValue(_weaponId, _weaponName, _weaponType, _damage, _attackSpeed, _range);
            data.Pierce = _pierce;
            data.Area = _area;
            data.Duration = _duration;
            return data;
        }
    }
}

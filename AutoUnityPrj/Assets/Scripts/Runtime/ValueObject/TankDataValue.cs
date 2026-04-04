using UnityEngine;

/// <summary>
/// 战车数据值对象 - 存储战车的15种属性数值
/// 作者：AI
/// 最后修改时间：2026-04-03
/// </summary>
namespace Game.Runtime.ValueObject
{
    [System.Serializable]
    public class TankDataValue
    {
        // 生命属性
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _healthRegen = 0.5f;
        [SerializeField] private float _lifesteal = 0f;

        // 伤害属性
        [SerializeField] private float _percentDamage = 0f;
        [SerializeField] private float _rangedDamage = 0f;
        [SerializeField] private float _meleeDamage = 0f;
        [SerializeField] private float _elementDamage = 0f;
        [SerializeField] private float _engineering = 0f;

        // 战斗属性
        [SerializeField] private float _attackSpeed = 5f;
        [SerializeField] private float _critRate = 5f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private float _aimAccuracy = 0.85f;  // 瞄准精度阈值（点乘值）

        // 防御属性
        [SerializeField] private int _armor = 0;
        [SerializeField] private float _dodge = 0f;

        // 移动属性
        [SerializeField] private float _moveSpeed = 3f;

        // 成长属性
        [SerializeField] private float _luck = 0f;
        [SerializeField] private float _harvest = 1f;

        // 当前生命值（非持久化）
        private int _currentHealth;

        #region 属性访问器

        public int MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = Mathf.Max(0, value);
        }

        public float HealthRegen
        {
            get => _healthRegen;
            set => _healthRegen = Mathf.Max(0, value);
        }

        public float Lifesteal
        {
            get => _lifesteal;
            set => _lifesteal = Mathf.Clamp(value, 0, 100);
        }

        public float PercentDamage
        {
            get => _percentDamage;
            set => _percentDamage = value;
        }

        public float RangedDamage
        {
            get => _rangedDamage;
            set => _rangedDamage = value;
        }

        public float MeleeDamage
        {
            get => _meleeDamage;
            set => _meleeDamage = value;
        }

        public float ElementDamage
        {
            get => _elementDamage;
            set => _elementDamage = value;
        }

        public float Engineering
        {
            get => _engineering;
            set => _engineering = value;
        }

        public float AttackSpeed
        {
            get => _attackSpeed;
            set => _attackSpeed = Mathf.Max(-90, value);
        }

        public float CritRate
        {
            get => _critRate;
            set => _critRate = Mathf.Clamp(value, 0, 100);
        }

        public float Range
        {
            get => _range;
            set => _range = Mathf.Max(0, value);
        }

        public float AimAccuracy
        {
            get => _aimAccuracy;
            set => _aimAccuracy = Mathf.Clamp(value, 0f, 1f);
        }

        public int Armor
        {
            get => _armor;
            set => _armor = Mathf.Max(0, value);
        }

        public float Dodge
        {
            get => _dodge;
            set => _dodge = Mathf.Clamp(value, 0, 100);
        }

        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = Mathf.Max(0.1f, value);
        }

        public float Luck
        {
            get => _luck;
            set => _luck = Mathf.Max(0, value);
        }

        public float Harvest
        {
            get => _harvest;
            set => _harvest = Mathf.Max(0.1f, value);
        }

        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        #endregion

        /// <summary>
        /// 构造函数 - 初始化默认数据
        /// </summary>
        public TankDataValue()
        {
            ResetToDefault();
        }

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void ResetToDefault()
        {
            _maxHealth = 100;
            _healthRegen = 0.5f;
            _lifesteal = 0f;
            _percentDamage = 0f;
            _rangedDamage = 0f;
            _meleeDamage = 0f;
            _elementDamage = 0f;
            _engineering = 0f;
            _attackSpeed = 5f;
            _critRate = 5f;
            _range = 5f;
            _armor = 0;
            _dodge = 0f;
            _moveSpeed = 3f;
            _luck = 0f;
            _harvest = 1f;
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// 从存档数据恢复
        /// </summary>
        public void LoadFromSave(TankSaveData data)
        {
            _maxHealth = data.maxHealth;
            _healthRegen = data.healthRegen;
            _lifesteal = data.lifesteal;
            _percentDamage = data.percentDamage;
            _rangedDamage = data.rangedDamage;
            _meleeDamage = data.meleeDamage;
            _elementDamage = data.elementDamage;
            _engineering = data.engineering;
            _attackSpeed = data.attackSpeed;
            _critRate = data.critRate;
            _range = data.range;
            _armor = data.armor;
            _dodge = data.dodge;
            _moveSpeed = data.moveSpeed;
            _luck = data.luck;
            _harvest = data.harvest;
            _currentHealth = data.currentHealth;
        }

        /// <summary>
        /// 导出为存档数据
        /// </summary>
        public TankSaveData ExportToSave()
        {
            return new TankSaveData
            {
                maxHealth = _maxHealth,
                healthRegen = _healthRegen,
                lifesteal = _lifesteal,
                percentDamage = _percentDamage,
                rangedDamage = _rangedDamage,
                meleeDamage = _meleeDamage,
                elementDamage = _elementDamage,
                engineering = _engineering,
                attackSpeed = _attackSpeed,
                critRate = _critRate,
                range = _range,
                armor = _armor,
                dodge = _dodge,
                moveSpeed = _moveSpeed,
                luck = _luck,
                harvest = _harvest,
                currentHealth = _currentHealth
            };
        }

        /// <summary>
        /// 存档数据结构
        /// </summary>
        [System.Serializable]
        public struct TankSaveData
        {
            public int maxHealth;
            public float healthRegen;
            public float lifesteal;
            public float percentDamage;
            public float rangedDamage;
            public float meleeDamage;
            public float elementDamage;
            public float engineering;
            public float attackSpeed;
            public float critRate;
            public float range;
            public int armor;
            public float dodge;
            public float moveSpeed;
            public float luck;
            public float harvest;
            public int currentHealth;
        }
    }
}
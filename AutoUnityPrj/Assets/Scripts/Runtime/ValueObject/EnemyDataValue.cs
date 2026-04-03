using UnityEngine;

namespace Game.Runtime.ValueObject
{
    /// <summary>
    /// 敌人类型枚举
    /// </summary>
    public enum EnemyType
    {
        Normal,   // 普通小怪
        Elite,    // 精英怪
        Boss      // Boss
    }

    /// <summary>
    /// 敌人数据值对象 - 存储敌人的属性
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    [System.Serializable]
    public class EnemyDataValue
    {
        // 敌人标识
        [SerializeField] private string _enemyId = "";
        [SerializeField] private string _enemyName = "";
        [SerializeField] private EnemyType _enemyType = EnemyType.Normal;

        // 基础属性
        [SerializeField] private int _maxHealth = 50;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _attackDamage = 10f;
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _attackInterval = 1f;

        // 战斗属性
        [SerializeField] private float _critRate = 5f;
        [SerializeField] private int _armor = 0;

        // 掉落属性
        [SerializeField] private int _energyDrop = 1;
        [SerializeField] private float _dropChance = 1f;
        [SerializeField] private bool _dropTreasureBox = false;
        [SerializeField] private int _treasureBoxDropChance = 0;

        // 内部状态
        private int _currentHealth;

        #region 属性访问器

        public string EnemyId
        {
            get => _enemyId;
            set => _enemyId = value;
        }

        public string EnemyName
        {
            get => _enemyName;
            set => _enemyName = value;
        }

        public EnemyType EnemyType
        {
            get => _enemyType;
            set => _enemyType = value;
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = Mathf.Max(1, value);
        }

        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = Mathf.Max(0.1f, value);
        }

        public float AttackDamage
        {
            get => _attackDamage;
            set => _attackDamage = Mathf.Max(0, value);
        }

        public float AttackRange
        {
            get => _attackRange;
            set => _attackRange = Mathf.Max(0, value);
        }

        public float AttackInterval
        {
            get => _attackInterval;
            set => _attackInterval = Mathf.Max(0.1f, value);
        }

        public float CritRate
        {
            get => _critRate;
            set => _critRate = Mathf.Clamp(value, 0, 100);
        }

        public int Armor
        {
            get => _armor;
            set => _armor = Mathf.Max(0, value);
        }

        public int EnergyDrop
        {
            get => _energyDrop;
            set => _energyDrop = Mathf.Max(0, value);
        }

        public float DropChance
        {
            get => _dropChance;
            set => _dropChance = Mathf.Clamp(value, 0, 1);
        }

        public bool DropTreasureBox
        {
            get => _dropTreasureBox;
            set => _dropTreasureBox = value;
        }

        public int TreasureBoxDropChance
        {
            get => _treasureBoxDropChance;
            set => _treasureBoxDropChance = Mathf.Clamp(value, 0, 100);
        }

        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public bool IsAlive => _currentHealth > 0;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public EnemyDataValue()
        {
            _enemyId = "";
            _enemyName = "";
            _enemyType = EnemyType.Normal;
            ResetToDefault();
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public EnemyDataValue(string id, string name, EnemyType type, int health, float speed)
        {
            _enemyId = id;
            _enemyName = name;
            _enemyType = type;
            _maxHealth = health;
            _moveSpeed = speed;
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// 重置为默认值
        /// </summary>
        public void ResetToDefault()
        {
            _maxHealth = 50;
            _moveSpeed = 2f;
            _attackDamage = 10f;
            _attackRange = 1.5f;
            _attackInterval = 1f;
            _critRate = 5f;
            _armor = 0;
            _energyDrop = 1;
            _dropChance = 1f;
            _dropTreasureBox = false;
            _treasureBoxDropChance = 0;
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// 造成伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            int actualDamage = damage;
            if (_armor > 0)
            {
                actualDamage = Mathf.Max(1, damage - _armor);
            }
            _currentHealth -= actualDamage;
        }

        /// <summary>
        /// 检查是否掉落资源
        /// </summary>
        public bool ShouldDropEnergy()
        {
            return _dropChance >= 1f || Random.value < _dropChance;
        }

        /// <summary>
        /// 检查是否掉落宝箱
        /// </summary>
        public bool ShouldDropTreasureBox()
        {
            return _dropTreasureBox && (_treasureBoxDropChance >= 100 || Random.value < _treasureBoxDropChance / 100f);
        }

        #region MVP预设敌人

        /// <summary>
        /// 创建海狸小怪
        /// </summary>
        public static EnemyDataValue CreateBeaver()
        {
            return new EnemyDataValue("beaver", "海狸", EnemyType.Normal, 30, 3f)
            {
                _attackDamage = 8f,
                _attackRange = 1.2f,
                _attackInterval = 1.2f,
                _energyDrop = 1,
                _dropChance = 1f
            };
        }

        /// <summary>
        /// 创建奶牛小怪
        /// </summary>
        public static EnemyDataValue CreateCow()
        {
            return new EnemyDataValue("cow", "奶牛", EnemyType.Normal, 50, 1.5f)
            {
                _attackDamage = 15f,
                _attackRange = 1.5f,
                _attackInterval = 1.5f,
                _armor = 2,
                _energyDrop = 2,
                _dropChance = 1f
            };
        }

        /// <summary>
        /// 创建大象Boss
        /// </summary>
        public static EnemyDataValue CreateElephantBoss()
        {
            return new EnemyDataValue("elephant", "大象", EnemyType.Boss, 500, 1f)
            {
                _attackDamage = 30f,
                _attackRange = 2.5f,
                _attackInterval = 2f,
                _armor = 10,
                _critRate = 10f,
                _energyDrop = 20,
                _dropChance = 1f,
                _dropTreasureBox = true,
                _treasureBoxDropChance = 100
            };
        }

        #endregion
    }
}

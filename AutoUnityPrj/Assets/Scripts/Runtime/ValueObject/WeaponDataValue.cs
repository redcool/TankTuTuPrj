using UnityEngine;

/// <summary>
/// 武器数据值对象 - 存储武器的伤害、攻速、范围等属性
/// 作者：AI
/// 最后修改时间：2026-04-03
/// </summary>
namespace Game.Runtime.ValueObject
{
    /// <summary>
    /// 武器大类枚举
    /// </summary>
    public enum WeaponCategory
    {
        MainCannon,     // 主炮类 - 单发高伤
        MachineGun,     // 机枪类 - 高速连射
        Missile,        // 导弹类 - 高爆发
        Sprayer,        // 喷射类 - 持续元素伤害
        Melee           // 近战类 - 高风险高回报
    }

    /// <summary>
    /// 伤害类型枚举
    /// </summary>
    public enum DamageType
    {
        PHYSICAL,       // 物理
        FIRE,           // 火焰
        ICE,            // 冰冻
        ACID,           // 酸液
        ENERGY          // 能量
    }

    /// <summary>
    /// 武器稀有度枚举
    /// </summary>
    public enum WeaponRarity
    {
        COMMON,         // 白色 - 商店常见
        RARE,           // 蓝色 - 商店较少
        EPIC,           // 紫色 - 稀有掉落
        LEGENDARY       // 橙色 - BOSS掉落
    }

    /// <summary>
    /// 武器类型枚举 - 战车专用武器类型（子类）
    /// </summary>
    public enum WeaponType
    {
        MainCannon,     // 主炮 - 高伤害 单发
        Howitzer,       // 榴弹炮 - 范围伤害
        Cannon,         // 加农炮 - 均衡输出
        Gatling,        // 机关炮 - 快速连射
        Missile,        // 导弹 - 高精度追踪
        Rocket,         // 火箭弹 - 弹幕覆盖
        Tesla,          // 电磁炮 - 链式伤害
        Laser,          // 激光炮 - 持续伤害
        LightMG,        // 轻机枪 - 稳定弹道
        HeavyMG,        // 重机枪 - 高单发
        Shotgun,        // 霰弹枪 - 扇形散射
        Mortar,         // 迫击炮 - 抛物线高抛
        Cruise,         // 巡航导弹 - 超大范围
        Flame,          // 火焰喷射器 - 灼烧DoT
        Cryo,           // 冷冻喷射器 - 减速控制
        WaterCannon,    // 高压水炮 - 击退推开
        Acid,           // 酸液喷射器 - 减甲腐蚀
        Drill,          // 旋转电锯 - 持续近身伤害
        Blade,          // 巨型斩刀 - 大范围挥砍
        Hammer,         // 震荡锤 - 范围眩晕
        Lance           // 冲击钻 - 突进伤害
    }

    [System.Serializable]
    public class WeaponDataValue
    {
        // 武器标识
        [SerializeField] private string _weaponId;
        [SerializeField] private string _weaponName;
        [SerializeField] private WeaponCategory _weaponCategory = WeaponCategory.MainCannon;
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private DamageType _damageType = DamageType.PHYSICAL;
        [SerializeField] private WeaponRarity _rarity = WeaponRarity.COMMON;

        // 基础属性
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _maxLevel = 5;

        // 特殊属性
        [SerializeField] private float _pierce = 1f;        // 穿透次数
        [SerializeField] private float _area = 0f;          // 爆炸/范围半径
        [SerializeField] private float _duration = 0f;       // 持续时间(DoT/持续)
        [SerializeField] private float _projectileSpeed = 10f; // 弹道速度
        [SerializeField] private int _projectileCount = 1;    // 每发子弹数
        [SerializeField] private float _knockback = 0f;       // 击退力

        // 升级属性
        [SerializeField] private int _upgradeCost = 100;
        [SerializeField] private float _upgradeDamagePerLevel = 0.15f; // 每级伤害提升%

        // 内部状态
        private float _lastAttackTime;
        private int _currentExp;

        #region 属性访问器

        public string WeaponId
        {
            get => _weaponId;
            set => _weaponId = value;
        }

        public string WeaponName
        {
            get => _weaponName;
            set => _weaponName = value;
        }

        public WeaponCategory WeaponCategory
        {
            get => _weaponCategory;
            set => _weaponCategory = value;
        }

        public WeaponType WeaponType
        {
            get => _weaponType;
            set => _weaponType = value;
        }

        public DamageType DamageType
        {
            get => _damageType;
            set => _damageType = value;
        }

        public WeaponRarity Rarity
        {
            get => _rarity;
            set => _rarity = value;
        }

        public float Damage
        {
            get => _damage;
            set => _damage = Mathf.Max(0, value);
        }

        public float AttackSpeed
        {
            get => _attackSpeed;
            set => _attackSpeed = Mathf.Max(0.1f, value);
        }

        public float Range
        {
            get => _range;
            set => _range = Mathf.Max(0, value);
        }

        public int Level
        {
            get => _level;
            set => _level = Mathf.Clamp(value, 1, _maxLevel);
        }

        public int MaxLevel
        {
            get => _maxLevel;
            set => _maxLevel = Mathf.Max(1, value);
        }

        public float Pierce
        {
            get => _pierce;
            set => _pierce = Mathf.Max(1, value);
        }

        public float Area
        {
            get => _area;
            set => _area = Mathf.Max(0, value);
        }

        public float Duration
        {
            get => _duration;
            set => _duration = Mathf.Max(0, value);
        }

        public float ProjectileSpeed
        {
            get => _projectileSpeed;
            set => _projectileSpeed = Mathf.Max(0.1f, value);
        }

        public int ProjectileCount
        {
            get => _projectileCount;
            set => _projectileCount = Mathf.Max(1, value);
        }

        public float Knockback
        {
            get => _knockback;
            set => _knockback = Mathf.Max(0, value);
        }

        public float UpgradeDamagePerLevel
        {
            get => _upgradeDamagePerLevel;
            set => _upgradeDamagePerLevel = Mathf.Max(0, value);
        }

        public int UpgradeCost
        {
            get => _upgradeCost;
            set => _upgradeCost = Mathf.Max(0, value);
        }

        public float LastAttackTime
        {
            get => _lastAttackTime;
            set => _lastAttackTime = value;
        }

        public int CurrentExp
        {
            get => _currentExp;
            set => _currentExp = Mathf.Max(0, value);
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public WeaponDataValue()
        {
            _weaponId = "";
            _weaponName = "";
            _weaponCategory = WeaponCategory.MainCannon;
            _weaponType = WeaponType.MainCannon;
            _damageType = DamageType.PHYSICAL;
            _rarity = WeaponRarity.COMMON;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public WeaponDataValue(string id, string name, WeaponCategory category, WeaponType type,
            float damage, float attackSpeed, float range,
            DamageType damageType = DamageType.PHYSICAL, WeaponRarity rarity = WeaponRarity.COMMON)
        {
            _weaponId = id;
            _weaponName = name;
            _weaponCategory = category;
            _weaponType = type;
            _damageType = damageType;
            _rarity = rarity;
            _damage = damage;
            _attackSpeed = attackSpeed;
            _range = range;
            _level = 1;
            _currentExp = 0;
        }

        /// <summary>
        /// 检查是否可以攻击
        /// </summary>
        public bool CanAttack()
        {
            return Time.time >= _lastAttackTime + (1f / _attackSpeed);
        }

        /// <summary>
        /// 执行攻击（更新攻击时间）
        /// </summary>
        public void ExecuteAttack()
        {
            _lastAttackTime = Time.time;
        }

        /// <summary>
        /// 升级
        /// </summary>
        public bool Upgrade()
        {
            if (_level >= _maxLevel) return false;

            _level++;
            _damage *= (1f + _upgradeDamagePerLevel);  // 每次升级+upgradeDamagePerLevel%伤害
            _attackSpeed *= 1.05f;  // 攻速+5%
            _upgradeCost *= 2;
            return true;
        }

        /// <summary>
        /// 获取最终的伤害值（计算战车属性加成）
        /// </summary>
        public float GetFinalDamage(TankDataValue tankData)
        {
            float damage = _damage;

            if (tankData == null) return damage;

            // 百分比伤害加成
            damage *= (1 + tankData.PercentDamage / 100f);

            // 武器类型伤害加成 (使用大类)
            switch (_weaponCategory)
            {
                case WeaponCategory.MainCannon:
                    damage *= (1 + tankData.RangedDamage / 100f);
                    break;
                case WeaponCategory.Missile:
                    damage *= (1 + tankData.ElementDamage / 100f);
                    break;
                case WeaponCategory.Sprayer:
                    damage *= (1 + tankData.ElementDamage / 100f);
                    break;
                case WeaponCategory.Melee:
                    damage *= (1 + tankData.MeleeDamage / 100f);
                    break;
                case WeaponCategory.MachineGun:
                default:
                    // 机枪类受 ranged+melee 折中影响
                    damage *= (1 + (tankData.RangedDamage + tankData.MeleeDamage) / 200f);
                    break;
            }

            // 工程加成影响建造/喷射类
            if (_weaponCategory == WeaponCategory.Sprayer || _weaponType == WeaponType.Drill)
            {
                damage *= (1 + tankData.Engineering / 100f);
            }

            return damage;
        }

        /// <summary>
        /// 获取实际攻击范围（计算战车属性加成）
        /// </summary>
        public float GetFinalRange(TankDataValue tankData)
        {
            if (tankData == null) return _range;
            return _range + tankData.Range;
        }
    }
}
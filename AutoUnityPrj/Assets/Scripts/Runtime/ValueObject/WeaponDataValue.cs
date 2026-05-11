using UnityEngine;

/// <summary>
/// 武器数据值对象 - 存储武器的伤害、攻速、范围等属性
/// 作者：AI
/// 最后修改时间：2026-04-03
/// </summary>
namespace Game.Runtime.ValueObject
{
    /// <summary>
    /// 武器类型枚举 - 战车专用武器类型
    /// </summary>
    public enum WeaponType
    {
        MainCannon,     // 主炮 - 高伤害 单发
        Howitzer,       // 榴弹炮 - 范围伤害
        Cannon,         // 加农炮 - 均衡输出
        Gatling,        // 机关炮 - 快速连射
        Missile,        // 导弹 - 高精度追踪
        Rocket,        // 火箭弹 - 弹幕覆盖
        Tesla,          // 电磁炮 - 链式伤害
        Laser           // 激光炮 - 持续伤害
    }

    [System.Serializable]
    public class WeaponDataValue
    {
        // 武器标识
        [SerializeField] private string _weaponId;
        [SerializeField] private string _weaponName;
        [SerializeField] private WeaponType _weaponType;

        // 基础属性
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private float _range = 5f;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _maxLevel = 5;

        // 特殊属性
        [SerializeField] private float _pierce = 1f;      // 穿透
        [SerializeField] private float _area = 0f;        // 范围伤害
        [SerializeField] private float _duration = 0f;     // 持续时间

        // 升级所需经验
        [SerializeField] private int _upgradeCost = 100;

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

        public WeaponType WeaponType
        {
            get => _weaponType;
            set => _weaponType = value;
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

        public int MaxLevel => _maxLevel;

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
            _weaponType = WeaponType.MainCannon;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public WeaponDataValue(string id, string name, WeaponType type, float damage, float attackSpeed, float range)
        {
            _weaponId = id;
            _weaponName = name;
            _weaponType = type;
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
            _damage *= 1.2f;  // 每次升级+20%伤害
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

            // 类型伤害加成
            switch (_weaponType)
            {
                case WeaponType.MainCannon:
                case WeaponType.Cannon:
                    damage *= (1 + tankData.RangedDamage / 100f);
                    break;
                case WeaponType.Howitzer:
                case WeaponType.Rocket:
                case WeaponType.Missile:
                    damage *= (1 + tankData.ElementDamage / 100f);
                    break;
                case WeaponType.Gatling:
                case WeaponType.Tesla:
                case WeaponType.Laser:
                    damage *= (1 + tankData.MeleeDamage / 100f);
                    break;
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
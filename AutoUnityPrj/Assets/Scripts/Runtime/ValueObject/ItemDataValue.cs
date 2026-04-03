using UnityEngine;

namespace Game.Runtime.ValueObject
{
    /// <summary>
    /// 道具类型枚举
    /// </summary>
    public enum ItemType
    {
        Consumable,  // 消耗品
        Passive,     // 被动物品
        Upgrade      // 升级物品
    }

    /// <summary>
    /// 道具数据值对象 - 存储道具的属性
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    [System.Serializable]
    public class ItemDataValue
    {
        // 道具标识
        [SerializeField] private string _itemId = "";
        [SerializeField] private string _itemName = "";
        [SerializeField] private ItemType _itemType = ItemType.Consumable;
        [SerializeField] private string _description = "";

        // 数值属性
        [SerializeField] private int _price = 100;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _maxLevel = 5;

        // 属性加成（使用字符串key-value存储动态属性）
        [SerializeField] private float _maxHealthBonus = 0;
        [SerializeField] private float _healthRegenBonus = 0;
        [SerializeField] private float _damageBonus = 0;
        [SerializeField] private float _attackSpeedBonus = 0;
        [SerializeField] private float _moveSpeedBonus = 0;
        [SerializeField] private float _critRateBonus = 0;
        [SerializeField] private float _armorBonus = 0;
        [SerializeField] private float _luckBonus = 0;
        [SerializeField] private float _harvestBonus = 0;

        // 堆叠属性
        [SerializeField] private int _stackCount = 1;
        [SerializeField] private int _maxStack = 99;
        [SerializeField] private bool _canStack = true;

        // 稀有度
        [SerializeField] private float _rarity = 50f;  // 0-100, 影响掉落概率

        #region 属性访问器

        public string ItemId
        {
            get => _itemId;
            set => _itemId = value;
        }

        public string ItemName
        {
            get => _itemName;
            set => _itemName = value;
        }

        public ItemType ItemType
        {
            get => _itemType;
            set => _itemType = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public int Price
        {
            get => _price;
            set => _price = Mathf.Max(0, value);
        }

        public int Level
        {
            get => _level;
            set => _level = Mathf.Clamp(value, 1, _maxLevel);
        }

        public int MaxLevel => _maxLevel;

        public float MaxHealthBonus
        {
            get => _maxHealthBonus;
            set => _maxHealthBonus = value;
        }

        public float HealthRegenBonus
        {
            get => _healthRegenBonus;
            set => _healthRegenBonus = value;
        }

        public float DamageBonus
        {
            get => _damageBonus;
            set => _damageBonus = value;
        }

        public float AttackSpeedBonus
        {
            get => _attackSpeedBonus;
            set => _attackSpeedBonus = value;
        }

        public float MoveSpeedBonus
        {
            get => _moveSpeedBonus;
            set => _moveSpeedBonus = value;
        }

        public float CritRateBonus
        {
            get => _critRateBonus;
            set => _critRateBonus = value;
        }

        public float ArmorBonus
        {
            get => _armorBonus;
            set => _armorBonus = value;
        }

        public float LuckBonus
        {
            get => _luckBonus;
            set => _luckBonus = value;
        }

        public float HarvestBonus
        {
            get => _harvestBonus;
            set => _harvestBonus = value;
        }

        public int StackCount
        {
            get => _stackCount;
            set
            {
                if (_canStack)
                    _stackCount = Mathf.Clamp(value, 1, _maxStack);
                else
                    _stackCount = 1;
            }
        }

        public int MaxStack => _maxStack;

        public bool CanStack => _canStack;

        public float Rarity
        {
            get => _rarity;
            set => _rarity = Mathf.Clamp(value, 0, 100);
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ItemDataValue()
        {
            _itemId = "";
            _itemName = "";
            _itemType = ItemType.Consumable;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public ItemDataValue(string id, string name, ItemType type, int price)
        {
            _itemId = id;
            _itemName = name;
            _itemType = type;
            _price = price;
        }

        /// <summary>
        /// 应用道具属性到战车
        /// </summary>
        public void ApplyToTank(TankDataValue tankData)
        {
            tankData.MaxHealth += (int)_maxHealthBonus;
            tankData.HealthRegen += _healthRegenBonus;
            tankData.PercentDamage += _damageBonus;
            tankData.AttackSpeed += _attackSpeedBonus;
            tankData.MoveSpeed += _moveSpeedBonus;
            tankData.CritRate += _critRateBonus;
            tankData.Armor += (int)_armorBonus;
            tankData.Luck += _luckBonus;
            tankData.Harvest += _harvestBonus;
        }

        /// <summary>
        /// 移除道具属性（用于卸载）
        /// </summary>
        public void RemoveFromTank(TankDataValue tankData)
        {
            tankData.MaxHealth -= (int)_maxHealthBonus;
            tankData.HealthRegen -= _healthRegenBonus;
            tankData.PercentDamage -= _damageBonus;
            tankData.AttackSpeed -= _attackSpeedBonus;
            tankData.MoveSpeed -= _moveSpeedBonus;
            tankData.CritRate -= _critRateBonus;
            tankData.Armor -= (int)_armorBonus;
            tankData.Luck -= _luckBonus;
            tankData.Harvest -= _harvestBonus;
        }

        #region MVP预设道具

        /// <summary>
        /// 创建生命之心（+20最大生命）
        /// </summary>
        public static ItemDataValue CreateHeart()
        {
            return new ItemDataValue("heart", "生命之心", ItemType.Passive, 50)
            {
                _description = "永久增加20点最大生命值",
                _maxHealthBonus = 20,
                _canStack = true,
                _maxStack = 10,
                _rarity = 30f
            };
        }

        /// <summary>
        /// 创建敏捷之靴（+5%攻速）
        /// </summary>
        public static ItemDataValue CreateBoots()
        {
            return new ItemDataValue("boots", "敏捷之靴", ItemType.Passive, 80)
            {
                _description = "永久增加5%攻击速度",
                _attackSpeedBonus = 5f,
                _canStack = true,
                _maxStack = 5,
                _rarity = 40f
            };
        }

        /// <summary>
        /// 创建力量护腕（+10%伤害）
        /// </summary>
        public static ItemDataValue CreateBracer()
        {
            return new ItemDataValue("bracer", "力量护腕", ItemType.Passive, 100)
            {
                _description = "永久增加10%伤害",
                _damageBonus = 10f,
                _canStack = true,
                _maxStack = 5,
                _rarity = 50f
            };
        }

        /// <summary>
        /// 创建幸运硬币（+5%幸运）
        /// </summary>
        public static ItemDataValue CreateCoin()
        {
            return new ItemDataValue("coin", "幸运硬币", ItemType.Passive, 150)
            {
                _description = "永久增加5%幸运值",
                _luckBonus = 5f,
                _canStack = true,
                _maxStack = 10,
                _rarity = 60f
            };
        }

        /// <summary>
        /// 创建丰收戒指（+10%收获）
        /// </summary>
        public static ItemDataValue CreateRing()
        {
            return new ItemDataValue("ring", "丰收戒指", ItemType.Passive, 200)
            {
                _description = "永久增加10%资源获取量",
                _harvestBonus = 0.1f,
                _canStack = true,
                _maxStack = 5,
                _rarity = 70f
            };
        }

        #endregion
    }
}

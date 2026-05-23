using UnityEngine;
using Game.Runtime.ValueObject;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 道具数据 ScriptableObject - 可在 Inspector 中配置
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "铁皮突突/道具数据")]
    public class ItemDataSO : ScriptableObject
    {
        [Header("基础信息")]
        [SerializeField] private string _itemId = "";
        [SerializeField] private string _itemName = "";
        [SerializeField] private Sprite _icon;
        [TextArea(2, 4)]
        [SerializeField] private string _description = "";
        [SerializeField] private ItemType _itemType = ItemType.Consumable;

        [Header("数值属性")]
        [SerializeField] private int _price = 100;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _maxLevel = 5;

        [Header("属性加成")]
        [SerializeField] private float _maxHealthBonus = 0;
        [SerializeField] private float _healthRegenBonus = 0;
        [SerializeField] private float _damageBonus = 0;
        [SerializeField] private float _attackSpeedBonus = 0;
        [SerializeField] private float _moveSpeedBonus = 0;
        [SerializeField] private float _critRateBonus = 0;
        [SerializeField] private float _armorBonus = 0;
        [SerializeField] private float _luckBonus = 0;
        [SerializeField] private float _harvestBonus = 0;

        [Header("堆叠属性")]
        [SerializeField] private bool _canStack = true;
        [SerializeField] private int _maxStack = 99;

        [Header("稀有度")]
        [SerializeField] private float _rarity = 50f; // 0-100, 影响掉落概率

        #region Properties

        public string ItemId => _itemId;
        public string ItemName => _itemName;
        public Sprite Icon => _icon;
        public string Description => _description;
        public ItemType ItemType => _itemType;
        public int Price => _price;
        public int Level => _level;
        public int MaxLevel => _maxLevel;
        public float MaxHealthBonus => _maxHealthBonus;
        public float HealthRegenBonus => _healthRegenBonus;
        public float DamageBonus => _damageBonus;
        public float AttackSpeedBonus => _attackSpeedBonus;
        public float MoveSpeedBonus => _moveSpeedBonus;
        public float CritRateBonus => _critRateBonus;
        public float ArmorBonus => _armorBonus;
        public float LuckBonus => _luckBonus;
        public float HarvestBonus => _harvestBonus;
        public bool CanStack => _canStack;
        public int MaxStack => _maxStack;
        public float Rarity => _rarity;

        public Sprite IconSetter
        {
            get => _icon;
            set => _icon = value;
        }

        public string DescriptionSetter
        {
            get => _description;
            set => _description = value;
        }

        public int PriceSetter
        {
            get => _price;
            set => _price = Mathf.Max(0, value);
        }

        public float RaritySetter
        {
            get => _rarity;
            set => _rarity = Mathf.Clamp(value, 0, 100);
        }

        #endregion

        /// <summary>
        /// 转换为 ItemDataValue
        /// </summary>
        public ItemDataValue ToDataValue()
        {
            var data = new ItemDataValue(_itemId, _itemName, _itemType, _price)
            {
                Description = _description,
                MaxHealthBonus = _maxHealthBonus,
                HealthRegenBonus = _healthRegenBonus,
                DamageBonus = _damageBonus,
                AttackSpeedBonus = _attackSpeedBonus,
                MoveSpeedBonus = _moveSpeedBonus,
                CritRateBonus = _critRateBonus,
                ArmorBonus = _armorBonus,
                LuckBonus = _luckBonus,
                HarvestBonus = _harvestBonus,
                StackCount = 1,
                CanStack = _canStack,
                MaxStack = _maxStack,
                Rarity = _rarity
            };
            return data;
        }
    }
}

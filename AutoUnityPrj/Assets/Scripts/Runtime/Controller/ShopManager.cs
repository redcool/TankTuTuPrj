using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 商城管理器 - 管理商品列表、刷新、购买
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        // 常量
        private const int MAX_SHOP_SLOTS = 6;
        private const int REFRESH_COST = 50;

        // 序列化字段
        [Header("商品池")]
        [SerializeField] private List<ShopItem> _itemPool = new List<ShopItem>();

        // 私有字段
        private List<ShopItem> _currentItems = new List<ShopItem>();
        private int _refreshCount = 0;

        // 公有属性
        public List<ShopItem> CurrentItems => _currentItems;
        public int RefreshCost => REFRESH_COST;

        // 事件
        public delegate void ShopEvent();
        public event ShopEvent OnShopRefresh;
        public event ShopEvent OnItemPurchased;

        /// <summary>
        /// 初始化商城
        /// </summary>
        public void Initialize()
        {
            RefreshShop();
        }

        /// <summary>
        /// 刷新商品列表
        /// </summary>
        public void RefreshShop()
        {
            _currentItems.Clear();
            _refreshCount++;

            int itemCount = Mathf.Min(MAX_SHOP_SLOTS, _itemPool.Count);

            // 随机选择商品
            var shuffled = new List<ShopItem>(_itemPool);
            for (int i = 0; i < itemCount; i++)
            {
                int index = Random.Range(0, shuffled.Count);
                _currentItems.Add(shuffled[index]);
                shuffled.RemoveAt(index);
            }

            OnShopRefresh?.Invoke();
            Debug.Log($"[ShopManager] 商城刷新完成，共 {_currentItems.Count} 件商品");
        }

        /// <summary>
        /// 购买商品
        /// </summary>
        public bool PurchaseItem(int slotIndex, int playerIndex)
        {
            if (slotIndex < 0 || slotIndex >= _currentItems.Count)
            {
                Debug.LogWarning("[ShopManager] 无效的槽位索引");
                return false;
            }

            var item = _currentItems[slotIndex];
            int cost = item.Price;

            // 检查玩家资源
            if (!GameManager.Instance.SpendResource(playerIndex, cost))
            {
                Debug.LogWarning("[ShopManager] 资源不足");
                return false;
            }

            // 应用商品效果
            ApplyItemEffect(item, playerIndex);

            // 从商城移除
            _currentItems.RemoveAt(slotIndex);

            OnItemPurchased?.Invoke();
            Debug.Log($"[ShopManager] 购买成功: {item.Name}");
            return true;
        }

        /// <summary>
        /// 应用商品效果
        /// </summary>
        private void ApplyItemEffect(ShopItem item, int playerIndex)
        {
            var levelManager = FindObjectOfType<LevelManager>();
            if (levelManager == null || levelManager.PlayerTanks == null) return;

            var tank = levelManager.PlayerTanks[playerIndex];
            if (tank == null || tank.TankData == null) return;

            // 根据商品类型应用加成
            switch (item.ItemType)
            {
                case ShopItemType.HealthBoost:
                    tank.TankData.MaxHealth += (int)item.Value;
                    tank.TankData.CurrentHealth += (int)item.Value;
                    break;
                case ShopItemType.DamageBoost:
                    tank.TankData.PercentDamage += item.Value;
                    break;
                case ShopItemType.SpeedBoost:
                    tank.TankData.AttackSpeed += item.Value;
                    break;
                case ShopItemType.ArmorBoost:
                    tank.TankData.Armor += (int)item.Value;
                    break;
                case ShopItemType.LuckBoost:
                    tank.TankData.Luck += item.Value;
                    break;
            }
        }

        /// <summary>
        /// 设置商品池
        /// </summary>
        public void SetItemPool(List<ShopItem> pool)
        {
            _itemPool = new List<ShopItem>(pool);
        }
    }

    /// <summary>
    /// 商城商品数据
    /// </summary>
    [System.Serializable]
    public class ShopItem
    {
        public string Id;
        public string Name;
        public ShopItemType ItemType;
        public int Price;
        public float Value;
        public string Description;

        public ShopItem(string id, string name, ShopItemType type, int price, float value, string desc)
        {
            Id = id;
            Name = name;
            ItemType = type;
            Price = price;
            Value = value;
            Description = desc;
        }
    }

    /// <summary>
    /// 商品类型
    /// </summary>
    public enum ShopItemType
    {
        HealthBoost,    // 生命提升
        DamageBoost,    // 伤害提升
        SpeedBoost,     // 攻速提升
        ArmorBoost,     // 护甲提升
        LuckBoost       // 幸运提升
    }
}

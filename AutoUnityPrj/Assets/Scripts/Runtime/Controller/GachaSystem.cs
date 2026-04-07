using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 抽卡系统 - 管理武器/道具抽卡、概率计算、幸运值修正
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class GachaSystem : MonoBehaviour
    {
        // 常量
        private const int GACHA_COST = 100;
        private const float BASE_LUCK_MODIFIER = 0.02f; // 每点幸运值增加2%稀有概率

        // 序列化字段
        [Header("抽卡池")]
        [SerializeField] private List<GachaItem> _gachaPool = new List<GachaItem>();

        [Header("抽卡设置")]
        [SerializeField] private int _gachaCost = GACHA_COST;
        [SerializeField] private int _pullCount = 1;

        // 私有字段
        private int _totalPulls = 0;
        private int _pityCounter = 0;
        private const int PITY_THRESHOLD = 10; // 保底次数

        // 公有属性
        public int GachaCost => _gachaCost;
        public int TotalPulls => _totalPulls;

        // 事件
        public delegate void GachaEvent(List<GachaItem> results);
        public event GachaEvent OnGachaResult;

        /// <summary>
        /// 单次抽卡
        /// </summary>
        public List<GachaItem> Pull(int playerIndex)
        {
            // 检查资源
            if (!GameManager.Instance.SpendResource(playerIndex, _gachaCost))
            {
                Debug.LogWarning("[GachaSystem] 资源不足");
                return new List<GachaItem>();
            }

            var results = new List<GachaItem>();

            for (int i = 0; i < _pullCount; i++)
            {
                var item = RollItem(playerIndex);
                if (item != null)
                {
                    results.Add(item);
                }
            }

            _totalPulls += _pullCount;
            OnGachaResult?.Invoke(results);

            Debug.Log($"[GachaSystem] 抽卡完成，获得 {results.Count} 件物品");
            return results;
        }

        /// <summary>
        /// 滚动单个物品
        /// </summary>
        private GachaItem RollItem(int playerIndex)
        {
            // 获取幸运值
            float luck = GetPlayerLuck(playerIndex);

            // 保底机制
            _pityCounter++;
            if (_pityCounter >= PITY_THRESHOLD)
            {
                _pityCounter = 0;
                return GetGuaranteedRareItem();
            }

            // 计算权重（幸运值影响稀有物品概率）
            float totalWeight = 0;
            var weightedPool = new List<(GachaItem item, float weight)>();

            foreach (var item in _gachaPool)
            {
                float weight = item.BaseWeight;

                // 幸运值修正：稀有物品获得更多加成
                if (item.Rarity >= 0.7f)
                {
                    weight *= (1 + luck * BASE_LUCK_MODIFIER);
                }

                weightedPool.Add((item, weight));
                totalWeight += weight;
            }

            // 加权随机
            float roll = Random.Range(0, totalWeight);
            float cumulative = 0;

            foreach (var (item, weight) in weightedPool)
            {
                cumulative += weight;
                if (roll <= cumulative)
                {
                    return item;
                }
            }

            return _gachaPool[0];
        }

        /// <summary>
        /// 获取保底稀有物品
        /// </summary>
        private GachaItem GetGuaranteedRareItem()
        {
            var rareItems = _gachaPool.FindAll(i => i.Rarity >= 0.7f);
            if (rareItems.Count > 0)
            {
                return rareItems[Random.Range(0, rareItems.Count)];
            }
            return _gachaPool[Random.Range(0, _gachaPool.Count)];
        }

        /// <summary>
        /// 获取玩家幸运值
        /// </summary>
        private float GetPlayerLuck(int playerIndex)
        {
            var levelManager = FindObjectOfType<LevelManager>();
            if (levelManager == null || levelManager.PlayerTanks == null) return 0;

            var tank = levelManager.PlayerTanks[playerIndex];
            if (tank == null || tank.TankData == null) return 0;

            return tank.TankData.Luck;
        }

        /// <summary>
        /// 设置抽卡池
        /// </summary>
        public void SetGachaPool(List<GachaItem> pool)
        {
            _gachaPool = new List<GachaItem>(pool);
        }

        /// <summary>
        /// 重置保底计数
        /// </summary>
        public void ResetPity()
        {
            _pityCounter = 0;
        }
    }

    /// <summary>
    /// 抽卡物品数据
    /// </summary>
    [System.Serializable]
    public class GachaItem
    {
        public string Id;
        public string Name;
        public float Rarity;      // 稀有度 0-1
        public float BaseWeight;  // 基础权重
        public string Description;

        public GachaItem(string id, string name, float rarity, float weight, string desc)
        {
            Id = id;
            Name = name;
            Rarity = rarity;
            BaseWeight = weight;
            Description = desc;
        }
    }
}

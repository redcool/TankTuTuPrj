using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.ValueObject
{
    /// <summary>
    /// 玩家存档数据值对象 - 存储玩家进度、解锁内容等
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    [System.Serializable]
    public class PlayerSaveDataValue
    {
        // 玩家标识
        [SerializeField] private string _playerId = "";
        [SerializeField] private int _playerIndex = 0;

        // 游戏进度
        [SerializeField] private int _highestLevel = 1;
        [SerializeField] private int _totalPlayTime = 0;  // 秒
        [SerializeField] private int _totalKills = 0;

        // 货币
        [SerializeField] private int _gold = 0;

        // 解锁内容
        [SerializeField] private List<string> _unlockedTanks = new List<string> { "suv" };
        [SerializeField] private List<string> _unlockedWeapons = new List<string> { "blaster_a" };
        [SerializeField] private List<string> _unlockedItems = new List<string>();
        [SerializeField] private List<string> _unlockedSkins = new List<string>();

        // 统计
        [SerializeField] private int _totalWins = 0;
        [SerializeField] private int _totalLosses = 0;

        // 时间戳
        [SerializeField] private long _lastSaveTime = 0;

        #region 属性访问器

        public string PlayerId
        {
            get => _playerId;
            set => _playerId = value;
        }

        public int PlayerIndex
        {
            get => _playerIndex;
            set => _playerIndex = value;
        }

        public int HighestLevel
        {
            get => _highestLevel;
            set => _highestLevel = Mathf.Max(1, value);
        }

        public int TotalPlayTime
        {
            get => _totalPlayTime;
            set => _totalPlayTime = Mathf.Max(0, value);
        }

        public int TotalKills
        {
            get => _totalKills;
            set => _totalKills = Mathf.Max(0, value);
        }

        public int Gold
        {
            get => _gold;
            set => _gold = Mathf.Max(0, value);
        }

        public List<string> UnlockedTanks => _unlockedTanks;
        public List<string> UnlockedWeapons => _unlockedWeapons;
        public List<string> UnlockedItems => _unlockedItems;
        public List<string> UnlockedSkins => _unlockedSkins;

        public int TotalWins
        {
            get => _totalWins;
            set => _totalWins = Mathf.Max(0, value);
        }

        public int TotalLosses
        {
            get => _totalLosses;
            set => _totalLosses = Mathf.Max(0, value);
        }

        public long LastSaveTime
        {
            get => _lastSaveTime;
            set => _lastSaveTime = value;
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PlayerSaveDataValue()
        {
            _playerId = System.Guid.NewGuid().ToString();
            _lastSaveTime = System.DateTime.Now.Ticks;
        }

        /// <summary>
        /// 构造函数（指定玩家索引）
        /// </summary>
        public PlayerSaveDataValue(int playerIndex)
        {
            _playerIndex = playerIndex;
            _playerId = $"player_{playerIndex}";
            _lastSaveTime = System.DateTime.Now.Ticks;
        }

        /// <summary>
        /// 检查战车是否已解锁
        /// </summary>
        public bool IsTankUnlocked(string tankId)
        {
            return _unlockedTanks.Contains(tankId);
        }

        /// <summary>
        /// 解锁战车
        /// </summary>
        public bool UnlockTank(string tankId)
        {
            if (!_unlockedTanks.Contains(tankId))
            {
                _unlockedTanks.Add(tankId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查武器是否已解锁
        /// </summary>
        public bool IsWeaponUnlocked(string weaponId)
        {
            return _unlockedWeapons.Contains(weaponId);
        }

        /// <summary>
        /// 解锁武器
        /// </summary>
        public bool UnlockWeapon(string weaponId)
        {
            if (!_unlockedWeapons.Contains(weaponId))
            {
                _unlockedWeapons.Add(weaponId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查道具是否已解锁
        /// </summary>
        public bool IsItemUnlocked(string itemId)
        {
            return _unlockedItems.Contains(itemId);
        }

        /// <summary>
        /// 解锁道具
        /// </summary>
        public bool UnlockItem(string itemId)
        {
            if (!_unlockedItems.Contains(itemId))
            {
                _unlockedItems.Add(itemId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加游戏时间
        /// </summary>
        public void AddPlayTime(int seconds)
        {
            _totalPlayTime += seconds;
        }

        /// <summary>
        /// 添加击杀数
        /// </summary>
        public void AddKills(int count)
        {
            _totalKills += count;
        }

        /// <summary>
        /// 记录胜利
        /// </summary>
        public void RecordWin()
        {
            _totalWins++;
        }

        /// <summary>
        /// 记录失败
        /// </summary>
        public void RecordLoss()
        {
            _totalLosses++;
        }

        /// <summary>
        /// 获取胜率
        /// </summary>
        public float GetWinRate()
        {
            int total = _totalWins + _totalLosses;
            return total > 0 ? (float)_totalWins / total * 100f : 0f;
        }

        /// <summary>
        /// 更新保存时间
        /// </summary>
        public void UpdateSaveTime()
        {
            _lastSaveTime = System.DateTime.Now.Ticks;
        }

        /// <summary>
        /// 保存为JSON
        /// </summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        /// <summary>
        /// 从JSON加载
        /// </summary>
        public static PlayerSaveDataValue FromJson(string json)
        {
            return JsonUtility.FromJson<PlayerSaveDataValue>(json);
        }
    }
}

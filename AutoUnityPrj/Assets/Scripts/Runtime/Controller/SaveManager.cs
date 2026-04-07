using UnityEngine;
using System.IO;
using Game.Runtime.ValueObject;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 存档管理器 - 管理本地JSON存储、解锁进度、存档读取
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        // 常量
        private const string SAVE_FILE_NAME = "player_save.json";
        private const string SAVE_FOLDER = "Saves";

        // 私有字段
        private PlayerSaveDataValue _currentSave;
        private string _savePath;

        // 公有属性
        public PlayerSaveDataValue CurrentSave => _currentSave;

        private void Awake()
        {
            _savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER, SAVE_FILE_NAME);
            LoadGame();
        }

        /// <summary>
        /// 保存游戏
        /// </summary>
        public void SaveGame()
        {
            if (_currentSave == null)
            {
                _currentSave = new PlayerSaveDataValue();
            }

            _currentSave.UpdateSaveTime();

            // 确保目录存在
            string dir = Path.GetDirectoryName(_savePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = _currentSave.ToJson();
            File.WriteAllText(_savePath, json);

            Debug.Log($"[SaveManager] 游戏已保存到: {_savePath}");
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        public void LoadGame()
        {
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                _currentSave = PlayerSaveDataValue.FromJson(json);
                Debug.Log($"[SaveManager] 存档加载成功: {_currentSave.PlayerId}");
            }
            else
            {
                // 创建新存档
                _currentSave = new PlayerSaveDataValue();
                Debug.Log("[SaveManager] 创建新存档");
            }
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeleteSave()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
                _currentSave = null;
                Debug.Log("[SaveManager] 存档已删除");
            }
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool HasSave()
        {
            return File.Exists(_savePath);
        }

        /// <summary>
        /// 解锁战车
        /// </summary>
        public bool UnlockTank(string tankId)
        {
            if (_currentSave == null) return false;

            bool unlocked = _currentSave.UnlockTank(tankId);
            if (unlocked)
            {
                SaveGame();
                Debug.Log($"[SaveManager] 解锁战车: {tankId}");
            }
            return unlocked;
        }

        /// <summary>
        /// 解锁武器
        /// </summary>
        public bool UnlockWeapon(string weaponId)
        {
            if (_currentSave == null) return false;

            bool unlocked = _currentSave.UnlockWeapon(weaponId);
            if (unlocked)
            {
                SaveGame();
                Debug.Log($"[SaveManager] 解锁武器: {weaponId}");
            }
            return unlocked;
        }

        /// <summary>
        /// 解锁道具
        /// </summary>
        public bool UnlockItem(string itemId)
        {
            if (_currentSave == null) return false;

            bool unlocked = _currentSave.UnlockItem(itemId);
            if (unlocked)
            {
                SaveGame();
                Debug.Log($"[SaveManager] 解锁道具: {itemId}");
            }
            return unlocked;
        }

        /// <summary>
        /// 记录关卡完成
        /// </summary>
        public void RecordLevelComplete(int levelNumber, int kills, int resources)
        {
            if (_currentSave == null) return;

            if (levelNumber > _currentSave.HighestLevel)
            {
                _currentSave.HighestLevel = levelNumber;
            }

            _currentSave.RecordWin();
            _currentSave.AddKills(kills);
            SaveGame();
        }

        /// <summary>
        /// 记录关卡失败
        /// </summary>
        public void RecordLevelFailed()
        {
            if (_currentSave == null) return;

            _currentSave.RecordLoss();
            SaveGame();
        }

        /// <summary>
        /// 添加金币
        /// </summary>
        public void AddGold(int amount)
        {
            if (_currentSave == null) return;

            _currentSave.Gold += amount;
            SaveGame();
        }

        /// <summary>
        /// 花费金币
        /// </summary>
        public bool SpendGold(int amount)
        {
            if (_currentSave == null) return false;

            if (_currentSave.Gold >= amount)
            {
                _currentSave.Gold -= amount;
                SaveGame();
                return true;
            }
            return false;
        }
    }
}

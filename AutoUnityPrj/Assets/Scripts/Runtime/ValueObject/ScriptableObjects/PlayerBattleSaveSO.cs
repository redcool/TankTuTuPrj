using UnityEngine;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.ValueObject.ScriptableObjects
{
    /// <summary>
    /// 玩家战局存档 SO - 存储当前战局的数据
    /// 不使用PlayerPrefs,使用ScriptableObject持久化
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerBattleSave", menuName = "铁皮突突/玩家战局存档")]
    public class PlayerBattleSaveSO : ScriptableObject
    {
        [Header("角色数据")]
        [SerializeField] private CharacterDataSO _selectedCharacter;
        [SerializeField] private string _characterId;

        [Header("武器数据")]
        [SerializeField] private WeaponDataSO _startingWeapon;
        [SerializeField] private WeaponDataSO[] _purchasedWeapons;
        
        [Header("道具数据")]
        [SerializeField] private ItemDataValue[] _startingItems;
        [SerializeField] private ItemDataValue[] _purchasedItems;

        [Header("难度")]
        [SerializeField] private int _difficulty = 1;
        [SerializeField] private string _difficultyName = "普通";

        [Header("战局状态")]
        [SerializeField] private int _currentWave;
        [SerializeField] private float _elapsedTime;
        [SerializeField] private int _currentResources;
        [SerializeField] private int _killCount;

        [Header("玩家状态")]
        [SerializeField] private int _currentHp;
        [SerializeField] private int _maxHp;

        #region Properties

        public CharacterDataSO SelectedCharacter => _selectedCharacter;
        public string CharacterId => _characterId;
        public WeaponDataSO StartingWeapon => _startingWeapon;
        public WeaponDataSO[] PurchasedWeapons => _purchasedWeapons;
        public ItemDataValue[] StartingItems => _startingItems;
        public ItemDataValue[] PurchasedItems => _purchasedItems;
        public int Difficulty => _difficulty;
        public string DifficultyName => _difficultyName;
        public int CurrentWave => _currentWave;
        public float ElapsedTime => _elapsedTime;
        public int CurrentResources => _currentResources;
        public int KillCount => _killCount;
        public int CurrentHp => _currentHp;
        public int MaxHp => _maxHp;

        #endregion

        #region Setters

        public void SetSelectedCharacter(CharacterDataSO character)
        {
            _selectedCharacter = character;
            _characterId = character != null ? character.name : "";
        }

        public void SetStartingWeapon(WeaponDataSO weapon)
        {
            _startingWeapon = weapon;
        }

        public void SetDifficulty(int difficulty, string difficultyName)
        {
            _difficulty = difficulty;
            _difficultyName = difficultyName;
        }

        public void AddPurchasedWeapon(WeaponDataSO weapon)
        {
            if (weapon == null) return;
            
            if (_purchasedWeapons == null)
            {
                _purchasedWeapons = new WeaponDataSO[0];
            }
            
            // 检查是否已存在
            foreach (var w in _purchasedWeapons)
            {
                if (w == weapon) return;
            }
            
            var list = new System.Collections.Generic.List<WeaponDataSO>(_purchasedWeapons);
            list.Add(weapon);
            _purchasedWeapons = list.ToArray();
        }

        public void AddPurchasedItem(ItemDataValue item)
        {
            if (item == null) return;
            
            if (_purchasedItems == null)
            {
                _purchasedItems = new ItemDataValue[0];
            }
            
            var list = new System.Collections.Generic.List<ItemDataValue>(_purchasedItems);
            list.Add(item);
            _purchasedItems = list.ToArray();
        }

        public void UpdateBattleState(int wave, float time, int resources, int kills)
        {
            _currentWave = wave;
            _elapsedTime = time;
            _currentResources = resources;
            _killCount = kills;
        }

        public void UpdatePlayerState(int currentHp, int maxHp)
        {
            _currentHp = currentHp;
            _maxHp = maxHp;
        }

        #endregion

        /// <summary>
        /// 清除战局数据
        /// </summary>
        public void ClearBattleData()
        {
            _selectedCharacter = null;
            _characterId = "";
            _startingWeapon = null;
            _purchasedWeapons = new WeaponDataSO[0];
            _startingItems = new ItemDataValue[0];
            _purchasedItems = new ItemDataValue[0];
            _difficulty = 1;
            _difficultyName = "普通";
            _currentWave = 0;
            _elapsedTime = 0;
            _currentResources = 0;
            _killCount = 0;
            _currentHp = 0;
            _maxHp = 0;
        }

        /// <summary>
        /// 获取存档描述
        /// </summary>
        public string GetSaveDescription()
        {
            return $"角色: {_characterId} | 难度: {_difficultyName} | 波次: {_currentWave} | 击杀: {_killCount}";
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Controller
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private int _playerCount = 1;
        [SerializeField] private TankController[] _tanks;

        private Dictionary<int, int> _playerResources = new Dictionary<int, int>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayerResources();
        }

        private void InitializePlayerResources()
        {
            for (int i = 0; i < 4; i++)
            {
                _playerResources[i] = 0;
            }
        }

        public void AddResource(int playerIndex, int amount)
        {
            if (_playerResources.ContainsKey(playerIndex))
            {
                _playerResources[playerIndex] += amount;
            }
        }

        public int GetResource(int playerIndex)
        {
            return _playerResources.ContainsKey(playerIndex) ? _playerResources[playerIndex] : 0;
        }

        public bool SpendResource(int playerIndex, int amount)
        {
            if (_playerResources.ContainsKey(playerIndex) && _playerResources[playerIndex] >= amount)
            {
                _playerResources[playerIndex] -= amount;
                return true;
            }
            return false;
        }

        public void OnTankDeath(int playerIndex)
        {
            Debug.Log("Player " + playerIndex + " tank destroyed!");
        }

        public TankController GetTank(int playerIndex)
        {
            if (_tanks != null && playerIndex >= 0 && playerIndex < _tanks.Length)
            {
                return _tanks[playerIndex];
            }
            return null;
        }

        public int GetPlayerCount()
        {
            return _playerCount;
        }

        /// <summary>
        /// 生成能量块掉落
        /// </summary>
        public void SpawnEnergyDrop(Vector3 position, int amount)
        {
            // TODO: 实现能量块预制体生成
            Debug.Log($"[GameManager] 掉落能量块: {amount} 在位置 {position}");
        }

        /// <summary>
        /// 生成宝箱掉落
        /// </summary>
        public void SpawnTreasureBox(Vector3 position)
        {
            // TODO: 实现宝箱预制体生成
            Debug.Log($"[GameManager] 掉落宝箱在位置 {position}");
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

namespace Game.Runtime.Controller
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private int _playerCount = 1;

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

        public int GetPlayerCount()
        {
            return _playerCount;
        }

        [Header("资源预制体")]
        [SerializeField] private GameObject _energyDropPrefab;
        [SerializeField] private GameObject _treasureBoxPrefab;

        [Header("掉落数据 (ScriptableObject)")]
        [SerializeField] private Game.Runtime.ValueObject.ScriptableObjects.EnergyDropDataSO _energyDropData;

        /// <summary>
        /// 能量块预制体（setter供SceneInitializer调用）
        /// </summary>
        public GameObject EnergyDropPrefab
        {
            get => _energyDropPrefab;
            set => _energyDropPrefab = value;
        }

        /// <summary>
        /// 宝箱预制体（setter供SceneInitializer调用）
        /// </summary>
        public GameObject TreasureBoxPrefab
        {
            get => _treasureBoxPrefab;
            set => _treasureBoxPrefab = value;
        }

        /// <summary>
        /// 能量块掉落数据（setter供SceneInitializer调用）
        /// </summary>
        public Game.Runtime.ValueObject.ScriptableObjects.EnergyDropDataSO EnergyDropData
        {
            get => _energyDropData;
            set => _energyDropData = value;
        }

        /// <summary>
        /// 生成能量块掉落
        /// </summary>
        public void SpawnEnergyDrop(Vector3 position, int amount)
        {
            if (_energyDropPrefab == null)
            {
                Debug.LogWarning("[GameManager] 未配置能量块预制体");
                return;
            }

            Vector3 dropPos = position + new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f));
            GameObject drop = Instantiate(_energyDropPrefab, dropPos, Quaternion.identity);

            // 检查并添加 EnergyDrop 组件（如果预制体没有）
            var energyDrop = drop.GetComponent<EnergyDrop>();
            if (energyDrop == null)
            {
                energyDrop = drop.AddComponent<EnergyDrop>();
                ApplyDefaultEnergyDropData(energyDrop);
            }

            energyDrop.SetAmount(amount);
        }

        /// <summary>
        /// 应用默认的能量块数据
        /// </summary>
        private void ApplyDefaultEnergyDropData(EnergyDrop energyDrop)
        {
            // 优先使用 SO 数据
            if (_energyDropData != null)
            {
                energyDrop.SetCollectRange(_energyDropData.CollectRange);
                energyDrop.SetLifetime(_energyDropData.Lifetime);
                energyDrop.SetMagnetSettings(_energyDropData.MagnetRange, _energyDropData.MagnetSpeed, _energyDropData.UseMagnet);
            }
        }

        /// <summary>
        /// 生成宝箱掉落
        /// </summary>
        public void SpawnTreasureBox(Vector3 position)
        {
            if (_treasureBoxPrefab == null)
            {
                Debug.LogWarning("[GameManager] 未配置宝箱预制体");
                return;
            }

            Vector3 dropPos = position + new Vector3(0, 0.5f, 0);
            Instantiate(_treasureBoxPrefab, dropPos, Quaternion.identity);
        }

        /// <summary>
        /// 波次完成回调
        /// </summary>
        public void OnWavesComplete()
        {
            Debug.Log("[GameManager] 所有波次完成！");
        }

        /// <summary>
        /// 更新HUD资源显示
        /// </summary>
        public void UpdateHUDResource(int playerIndex)
        {
            var hud = FindObjectOfType<Game.Runtime.View.HUDView>();
            if (hud != null)
            {
                hud.UpdateResource(GetResource(playerIndex));
            }
        }
    }
}
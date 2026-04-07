using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 多人管理器 - 管理多手柄识别、战车生成、玩家数据
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class MultiplayerManager : MonoBehaviour
    {
        // 常量
        private const int MAX_PLAYERS = 4;

        // 序列化字段
        [Header("战车预制体")]
        [SerializeField] private GameObject _tankPrefab;
        [SerializeField] private TankDataSO _defaultTankData;

        [Header("生成设置")]
        [SerializeField] private Vector3 _spawnOffset = new Vector3(2, 0, 0);

        // 私有字段
        private List<PlayerData> _players = new List<PlayerData>();
        private Dictionary<int, TankController> _playerTanks = new Dictionary<int, TankController>();

        // 公有属性
        public List<PlayerData> Players => _players;
        public int PlayerCount => _players.Count;
        public int MaxPlayers => MAX_PLAYERS;

        // 事件
        public delegate void PlayerEvent(int playerIndex, TankController tank);
        public event PlayerEvent OnPlayerJoined;
        public event PlayerEvent OnPlayerLeft;

        private void Awake()
        {
            // 监听设备连接
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void Start()
        {
            // 检测已连接的手柄
            DetectConnectedDevices();
        }

        private void OnDestroy()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        /// <summary>
        /// 检测设备连接变化
        /// </summary>
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (device is Gamepad)
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        OnGamepadConnected(device as Gamepad);
                        break;
                    case InputDeviceChange.Removed:
                        OnGamepadDisconnected(device as Gamepad);
                        break;
                }
            }
        }

        /// <summary>
        /// 检测已连接设备
        /// </summary>
        private void DetectConnectedDevices()
        {
            var gamepads = Gamepad.all;
            foreach (var gamepad in gamepads)
            {
                if (_players.Count < MAX_PLAYERS)
                {
                    AddPlayer(gamepad);
                }
            }

            // 如果没有手柄，至少添加一个键盘玩家
            if (_players.Count == 0)
            {
                AddPlayer(null); // null 表示键盘
            }
        }

        /// <summary>
        /// 手柄连接
        /// </summary>
        private void OnGamepadConnected(Gamepad gamepad)
        {
            if (_players.Count >= MAX_PLAYERS)
            {
                Debug.Log("[MultiplayerManager] 已达最大玩家数");
                return;
            }

            // 检查是否已存在
            foreach (var player in _players)
            {
                if (player.Device == gamepad) return;
            }

            AddPlayer(gamepad);
            Debug.Log($"[MultiplayerManager] 手柄连接: {gamepad.displayName}");
        }

        /// <summary>
        /// 手柄断开
        /// </summary>
        private void OnGamepadDisconnected(Gamepad gamepad)
        {
            var player = _players.Find(p => p.Device == gamepad);
            if (player != null)
            {
                RemovePlayer(player.PlayerIndex);
                Debug.Log($"[MultiplayerManager] 手柄断开: {gamepad.displayName}");
            }
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        private void AddPlayer(Gamepad gamepad)
        {
            int playerIndex = _players.Count;
            var player = new PlayerData(playerIndex, gamepad);
            _players.Add(player);

            // 生成战车
            SpawnTankForPlayer(player);

            Debug.Log($"[MultiplayerManager] 玩家 {playerIndex} 加入");
        }

        /// <summary>
        /// 移除玩家
        /// </summary>
        private void RemovePlayer(int playerIndex)
        {
            var player = _players.Find(p => p.PlayerIndex == playerIndex);
            if (player == null) return;

            // 销毁战车
            if (_playerTanks.ContainsKey(playerIndex))
            {
                var tank = _playerTanks[playerIndex];
                if (tank != null)
                {
                    OnPlayerLeft?.Invoke(playerIndex, tank);
                    Destroy(tank.gameObject);
                }
                _playerTanks.Remove(playerIndex);
            }

            _players.Remove(player);
            Debug.Log($"[MultiplayerManager] 玩家 {playerIndex} 离开");
        }

        /// <summary>
        /// 为玩家生成战车
        /// </summary>
        private void SpawnTankForPlayer(PlayerData player)
        {
            if (_tankPrefab == null)
            {
                Debug.LogWarning("[MultiplayerManager] 未配置战车预制体");
                return;
            }

            // 计算生成位置
            Vector3 spawnPos = player.PlayerIndex * _spawnOffset;

            // 实例化战车
            GameObject tankObj = Instantiate(_tankPrefab, spawnPos, Quaternion.identity);
            var tankController = tankObj.GetComponent<TankController>();

            if (tankController != null)
            {
                // 设置玩家索引
                tankController.gameObject.name = $"PlayerTank_{player.PlayerIndex}";

                // 设置战车数据
                if (_defaultTankData != null)
                {
                    // 通过反射设置 SO
                    var field = typeof(TankController).GetField("_tankDataSO",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    field?.SetValue(tankController, _defaultTankData);
                    tankController.GetType().GetMethod("InitializeData",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.Invoke(tankController, null);
                }
            }

            _playerTanks[player.PlayerIndex] = tankController;
            OnPlayerJoined?.Invoke(player.PlayerIndex, tankController);
        }

        /// <summary>
        /// 获取玩家的战车
        /// </summary>
        public TankController GetPlayerTank(int playerIndex)
        {
            return _playerTanks.ContainsKey(playerIndex) ? _playerTanks[playerIndex] : null;
        }

        /// <summary>
        /// 获取玩家数据
        /// </summary>
        public PlayerData GetPlayer(int playerIndex)
        {
            return _players.Find(p => p.PlayerIndex == playerIndex);
        }

        /// <summary>
        /// 获取所有战车
        /// </summary>
        public TankController[] GetAllTanks()
        {
            return _playerTanks.Values.ToArray();
        }
    }

    /// <summary>
    /// 玩家数据
    /// </summary>
    public class PlayerData
    {
        public int PlayerIndex { get; private set; }
        public Gamepad Device { get; private set; }
        public bool IsKeyboard => Device == null;

        public PlayerData(int index, Gamepad device)
        {
            PlayerIndex = index;
            Device = device;
        }
    }
}

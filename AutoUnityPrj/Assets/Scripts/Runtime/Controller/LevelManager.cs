using UnityEngine;
using System.Collections;
using Game.Runtime.View;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 关卡管理器 - 管理60秒关卡、倒计时、结算
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // 常量
        public const float LEVEL_DURATION = 60f;

        // 序列化字段
        [Header("关卡设置")]
        [SerializeField] private int _levelNumber = 1;
        [SerializeField] private float _levelDuration = LEVEL_DURATION;

        [Header("组件引用")]
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private HUDView _hudView;
        [SerializeField] private TankController[] _playerTanks;

        // 私有字段
        private float _remainingTime;
        private bool _isLevelActive;
        private bool _isLevelComplete;

        // 公有属性
        public float RemainingTime => _remainingTime;
        public int LevelNumber => _levelNumber;
        public bool IsLevelActive => _isLevelActive;
        public bool IsLevelComplete => _isLevelComplete;

        /// <summary>
        /// 玩家战车数组（setter供SceneInitializer调用）
        /// </summary>
        public TankController[] PlayerTanks
        {
            get => _playerTanks;
            set => _playerTanks = value;
        }

        /// <summary>
        /// 敌人生成器（setter供SceneInitializer调用）
        /// </summary>
        public EnemySpawner EnemySpawnerRef
        {
            get => _enemySpawner;
            set => _enemySpawner = value;
        }

        /// <summary>
        /// HUD视图（setter供SceneInitializer调用）
        /// </summary>
        public HUDView HUDViewRef
        {
            get => _hudView;
            set => _hudView = value;
        }

        // 事件
        public delegate void LevelEvent();
        public static event LevelEvent OnLevelStart;
        public static event LevelEvent OnLevelEnd;
        public static event LevelEvent OnLevelComplete;

        private void Awake()
        {
            _remainingTime = _levelDuration;
        }

        private void Start()
        {
            // 自动开始关卡
            StartLevel();
        }

        /// <summary>
        /// 开始关卡
        /// </summary>
        public void StartLevel()
        {
            if (_isLevelActive) return;

            _isLevelActive = true;
            _isLevelComplete = false;
            _remainingTime = _levelDuration;

            // 通知事件
            OnLevelStart?.Invoke();

            // 更新HUD
            if (_hudView != null)
            {
                _hudView.Show();
                _hudView.UpdateTimer(_remainingTime);
                _hudView.UpdateWave(1, _enemySpawner != null ? _enemySpawner.TotalEnemies : 0);
            }

            // 启动敌人生成
            if (_enemySpawner != null)
            {
                _enemySpawner.StartSpawning();
            }

            Debug.Log($"[LevelManager] 开始关卡 {_levelNumber}");
        }

        /// <summary>
        /// 结束关卡
        /// </summary>
        public void EndLevel()
        {
            if (!_isLevelActive) return;

            _isLevelActive = false;
            OnLevelEnd?.Invoke();

            // 停止敌人生成
            if (_enemySpawner != null)
            {
                _enemySpawner.StopSpawning();
            }

            Debug.Log($"[LevelManager] 关卡 {_levelNumber} 结束");
        }

        /// <summary>
        /// 完成关卡
        /// </summary>
        public void CompleteLevel()
        {
            if (_isLevelComplete) return;

            _isLevelComplete = true;
            _isLevelActive = false;
            OnLevelComplete?.Invoke();

            Debug.Log($"[LevelManager] 关卡 {_levelNumber} 完成！");
        }

        private void Update()
        {
            if (!_isLevelActive) return;

            _remainingTime -= Time.deltaTime;

            // 更新HUD
            if (_hudView != null)
            {
                _hudView.UpdateTimer(Mathf.Max(0, _remainingTime));
            }

            // 时间到
            if (_remainingTime <= 0)
            {
                _remainingTime = 0;
                CompleteLevel();
            }
        }

        /// <summary>
        /// 获取关卡进度（0-1）
        /// </summary>
        public float GetProgress()
        {
            return 1f - (_remainingTime / _levelDuration);
        }

        /// <summary>
        /// 重置关卡
        /// </summary>
        public void ResetLevel()
        {
            _isLevelActive = false;
            _isLevelComplete = false;
            _remainingTime = _levelDuration;
        }
    }
}

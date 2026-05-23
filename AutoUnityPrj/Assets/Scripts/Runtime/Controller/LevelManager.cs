using UnityEngine;
using System;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// 关卡管理器 - 管理60秒关卡、倒计时、结算
    /// 旧版uGUI HUDView/ResultView 已删除，改用事件驱动对接新UI Toolkit
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // 常量
        public const float LEVEL_DURATION = 60f;

        // 事件（供 UI Toolkit HUD/Result Presenter 订阅）
        public event Action<float> OnHUDUpdateTimer;
        public event Action<int, int> OnHUDUpdateWave;
        public event Action<int> OnHUDUpdateResource;
        public event Action OnHUDShow;
        public event Action OnHUDHide;
        public event Action<bool, int, int, float> OnResultShow;

        // 序列化字段
        [Header("关卡设置")]
        [SerializeField] private int _levelNumber = 1;
        [SerializeField] private float _levelDuration = LEVEL_DURATION;

        [Header("组件引用")]
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private TankController[] _playerTanks;

        // 私有字段
        private float _remainingTime;
        private bool _isLevelActive;
        private bool _isLevelComplete;
        private int _totalKills;
        private float _timeUsed;

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
        /// 通知HUD资源更新（供GameManager调用）
        /// </summary>
        public void NotifyHUDResourceUpdate(int resource)
        {
            OnHUDUpdateResource?.Invoke(resource);
        }

        /// <summary>
        /// 敌人生成器（setter供SceneInitializer调用）
        /// </summary>
        public EnemySpawner EnemySpawnerRef
        {
            get => _enemySpawner;
            set => _enemySpawner = value;
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
            // 旧版 uGUI ResultView 已移除，结算事件由 UI Toolkit ResultPresenter 通过事件订阅
            // TODO: 对接 UIFlowManager 的 HUD/Result 状态
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

            // HUD 事件（UI Toolkit Presenter 订阅）
            OnHUDShow?.Invoke();
            OnHUDUpdateTimer?.Invoke(_remainingTime);
            OnHUDUpdateWave?.Invoke(1, _enemySpawner != null ? _enemySpawner.TotalEnemies : 0);

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
            _timeUsed = _levelDuration - _remainingTime;

            // 统计击杀
            _totalKills = _enemySpawner != null ? _enemySpawner.EnemiesKilled : 0;

            OnLevelComplete?.Invoke();

            // 显示结算界面
            ShowResult();

            Debug.Log($"[LevelManager] 关卡 {_levelNumber} 完成！击杀: {_totalKills}, 用时: {_timeUsed:F1}s");
        }

        /// <summary>
        /// 显示结算界面
        /// </summary>
        private void ShowResult()
        {
            OnHUDHide?.Invoke();

            int playerResource = GameManager.Instance?.GetResource(0) ?? 0;
            OnResultShow?.Invoke(_isLevelComplete, _totalKills, playerResource, _timeUsed);
        }

        /// <summary>
        /// 继续下一关
        /// </summary>
        private void OnContinueClicked()
        {
            _levelNumber++;
            ResetLevel();
            StartLevel();
        }

        /// <summary>
        /// 返回主菜单
        /// </summary>
        private void OnReturnClicked()
        {
            Debug.Log("[LevelManager] 返回主菜单");
            // TODO: 加载主菜单场景
        }

        private void Update()
        {
            if (!_isLevelActive) return;

            _remainingTime -= Time.deltaTime;

            // HUD 事件（UI Toolkit Presenter 订阅）
            OnHUDUpdateTimer?.Invoke(Mathf.Max(0, _remainingTime));

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

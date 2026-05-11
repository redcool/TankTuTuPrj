using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.View
{
    /// <summary>
    /// 选项面板 - 游戏模式选择、无尽模式开关、合作模式开关(4手柄接入)
    /// 参考土豆兄弟右侧选项面板
    /// </summary>
    public class OptionsPanel : MonoBehaviour
    {
        [Header("游戏模式")]
        [SerializeField] private Toggle _normalModeToggle;
        [SerializeField] private Toggle _abyssModeToggle;

        [Header("无尽模式")]
        [SerializeField] private Toggle _endlessToggle;
        [SerializeField] private Text _endlessLabel;

        [Header("合作模式")]
        [SerializeField] private Toggle _coopToggle;
        [SerializeField] private Text _coopLabel;
        [SerializeField] private Text _playerCountText;

        // 游戏设置
        public enum GameMode { Normal, Abyss }
        public GameMode CurrentMode { get; private set; } = GameMode.Normal;
        public bool IsEndless { get; private set; } = false;
        public bool IsCoop { get; private set; } = false;
        public int PlayerCount { get; private set; } = 1;

        private void Awake()
        {
            SetupToggles();
            UpdatePlayerCountDisplay();
        }

        private void SetupToggles()
        {
            // 模式切换 - 互斥
            if (_normalModeToggle != null)
            {
                _normalModeToggle.isOn = true;
                _normalModeToggle.onValueChanged.AddListener(OnNormalModeChanged);
            }
            if (_abyssModeToggle != null)
            {
                _abyssModeToggle.isOn = false;
                _abyssModeToggle.onValueChanged.AddListener(OnAbyssModeChanged);
            }

            // 无尽模式
            if (_endlessToggle != null)
            {
                _endlessToggle.isOn = false;
                _endlessToggle.onValueChanged.AddListener(OnEndlessChanged);
            }

            // 合作模式
            if (_coopToggle != null)
            {
                _coopToggle.isOn = false;
                _coopToggle.onValueChanged.AddListener(OnCoopChanged);
            }
        }

        private void OnNormalModeChanged(bool value)
        {
            if (value)
            {
                CurrentMode = GameMode.Normal;
                if (_abyssModeToggle != null) _abyssModeToggle.isOn = false;
            }
        }

        private void OnAbyssModeChanged(bool value)
        {
            if (value)
            {
                CurrentMode = GameMode.Abyss;
                if (_normalModeToggle != null) _normalModeToggle.isOn = false;
            }
        }

        private void OnEndlessChanged(bool value)
        {
            IsEndless = value;
        }

        private void OnCoopChanged(bool value)
        {
            IsCoop = value;
            PlayerCount = value ? 4 : 1;
            UpdatePlayerCountDisplay();

            // TODO: 合作模式开启时，初始化多手柄
            if (value)
            {
                Debug.Log("[OptionsPanel] 合作模式开启，准备接入4手柄");
                // InitializeMultiplayerControllers();
            }
        }

        private void UpdatePlayerCountDisplay()
        {
            if (_playerCountText != null)
            {
                _playerCountText.text = IsCoop ? "4人合作" : "单人";
            }
        }

        /// <summary>
        /// 获取当前游戏配置摘要
        /// </summary>
        public string GetConfigSummary()
        {
            string mode = CurrentMode == GameMode.Normal ? "普通" : "深渊";
            string endless = IsEndless ? "无尽" : "";
            string coop = IsCoop ? "4人合作" : "单人";
            return $"{mode} {endless} {coop}".Trim();
        }
    }
}

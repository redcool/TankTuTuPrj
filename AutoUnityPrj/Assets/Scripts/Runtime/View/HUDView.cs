using UnityEngine;
using TMPro;

namespace Game.Runtime.View
{
    /// <summary>
    /// HUD界面 - 显示时间、生命值、货币等战斗信息
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _resourceText;
        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private GameObject _resourcePanel;

        // 私有字段
        private float _currentTime;
        private int _currentHealth;
        private int _currentResource;
        private int _currentWave;

        /// <summary>
        /// 更新计时器
        /// </summary>
        public void UpdateTimer(float time)
        {
            _currentTime = time;
            if (_timerText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                _timerText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        /// <summary>
        /// 更新生命值
        /// </summary>
        public void UpdateHealth(int current, int max)
        {
            _currentHealth = current;
            if (_healthText != null)
            {
                _healthText.text = $"{current}/{max}";
            }
        }

        /// <summary>
        /// 更新资源数量
        /// </summary>
        public void UpdateResource(int amount)
        {
            _currentResource = amount;
            if (_resourceText != null)
            {
                _resourceText.text = amount.ToString();
            }
        }

        /// <summary>
        /// 更新波次
        /// </summary>
        public void UpdateWave(int wave, int total)
        {
            _currentWave = wave;
            if (_waveText != null)
            {
                _waveText.text = $"波次 {wave}/{total}";
            }
        }

        /// <summary>
        /// 设置资源面板可见性
        /// </summary>
        public void SetResourcePanelVisible(bool visible)
        {
            if (_resourcePanel != null)
            {
                _resourcePanel.SetActive(visible);
            }
        }

        /// <summary>
        /// 隐藏HUD
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示HUD
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}

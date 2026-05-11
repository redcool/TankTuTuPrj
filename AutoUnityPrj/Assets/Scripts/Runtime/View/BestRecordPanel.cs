using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.View
{
    /// <summary>
    /// 最高记录面板 - 显示通关难度记录
    /// 参考土豆兄弟右侧记录面板
    /// </summary>
    public class BestRecordPanel : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Image _trophyIcon;
        [SerializeField] private Text _recordText;
        [SerializeField] private Text _bestDifficultyText;
        [SerializeField] private Text _bestWaveText;
        [SerializeField] private Text _bestTimeText;

        private void Awake()
        {
            LoadBestRecord();
        }

        /// <summary>
        /// 从存档系统加载最高记录
        /// </summary>
        public void LoadBestRecord()
        {
            // TODO: 从存档系统读取
            // 临时显示默认值
            if (_bestDifficultyText != null)
            {
                _bestDifficultyText.text = "难度 0";
            }
            if (_bestWaveText != null)
            {
                _bestWaveText.text = "最高波次: 15";
            }
            if (_bestTimeText != null)
            {
                _bestTimeText.text = "通关时间: 12:34";
            }
            if (_recordText != null)
            {
                _recordText.text = "暂无记录";
            }
        }

        /// <summary>
        /// 更新记录显示
        /// </summary>
        public void UpdateRecord(int difficulty, int maxWave, float clearTime)
        {
            if (_bestDifficultyText != null)
            {
                _bestDifficultyText.text = $"难度 {difficulty}";
            }
            if (_bestWaveText != null)
            {
                _bestWaveText.text = $"最高波次: {maxWave}";
            }
            if (_bestTimeText != null)
            {
                int minutes = Mathf.FloorToInt(clearTime / 60);
                int seconds = Mathf.FloorToInt(clearTime % 60);
                _bestTimeText.text = $"通关时间: {minutes:D2}:{seconds:D2}";
            }
            if (_recordText != null)
            {
                _recordText.text = $"最佳记录";
            }
        }
    }
}

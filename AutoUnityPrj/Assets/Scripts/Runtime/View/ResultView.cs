using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.View
{
    /// <summary>
    /// 结算界面 - 显示关卡结果、资源统计、继续/返回按钮
    /// 作者：AI
    /// 最后修改时间：2026-04-03
    /// </summary>
    public class ResultView : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _killsText;
        [SerializeField] private Text _resourceText;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _returnButton;

        // 回调
        public delegate void ResultAction();
        public event ResultAction OnContinue;
        public event ResultAction OnReturn;

        private void Awake()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(OnContinueClicked);
            }
            if (_returnButton != null)
            {
                _returnButton.onClick.AddListener(OnReturnClicked);
            }
        }

        /// <summary>
        /// 显示结算界面
        /// </summary>
        public void ShowResult(bool isComplete, int kills, int resources, float timeUsed)
        {
            if (_resultPanel != null)
            {
                _resultPanel.SetActive(true);
            }

            if (_titleText != null)
            {
                _titleText.text = isComplete ? "关卡完成！" : "时间到！";
            }

            if (_timeText != null)
            {
                int minutes = Mathf.FloorToInt(timeUsed / 60f);
                int seconds = Mathf.FloorToInt(timeUsed % 60f);
                _timeText.text = $"用时: {minutes:00}:{seconds:00}";
            }

            if (_killsText != null)
            {
                _killsText.text = $"击杀: {kills}";
            }

            if (_resourceText != null)
            {
                _resourceText.text = $"资源: {resources}";
            }
        }

        /// <summary>
        /// 隐藏结算界面
        /// </summary>
        public void Hide()
        {
            if (_resultPanel != null)
            {
                _resultPanel.SetActive(false);
            }
        }

        private void OnContinueClicked()
        {
            Hide();
            OnContinue?.Invoke();
        }

        private void OnReturnClicked()
        {
            Hide();
            OnReturn?.Invoke();
        }
    }
}

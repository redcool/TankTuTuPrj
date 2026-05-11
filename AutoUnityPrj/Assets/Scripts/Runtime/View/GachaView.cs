using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Game.Runtime.Controller;

using GachaItem = Game.Runtime.Controller.GachaItem;

namespace Game.Runtime.View
{
    /// <summary>
    /// 抽卡界面 - 武器/道具抽卡
    /// 显示抽卡动画和结果，支持幸运值概率修正
    /// 作者：AI
    /// 最后修改时间：2026-04-09
    /// </summary>
    public class GachaView : MonoBehaviour
    {
        [Header("UI路径")]
        [SerializeField] private string _pullButtonPath = "ButtonPanel/PullButton";
        [SerializeField] private string _pullCostPath = "ButtonPanel/PullButton/CostText";
        [SerializeField] private string _resultPanelPath = "ResultPanel";
        [SerializeField] private string _resultContainerPath = "ResultPanel/ResultGrid";
        [SerializeField] private string _playerResourcePath = "PlayerInfo/ResourceText";
        [SerializeField] private string _closeButtonPath = "ButtonPanel/CloseButton";

        [Header("UI引用")]
        [SerializeField] private Button _pullButton;
        [SerializeField] private TextMeshProUGUI _pullCostText;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private Transform _resultContainer;
        [SerializeField] private TextMeshProUGUI _playerResourceText;
        [SerializeField] private Button _closeButton;

        [Header("结果项预制体")]
        [SerializeField] private GameObject _resultItemPrefab;

        [Header("抽卡系统")]
        [SerializeField] private GachaSystem _gachaSystem;

        // 私有字段
        private int _currentPlayerIndex = 0;
        private bool _isAnimating = false;

        // 事件
        public delegate void GachaAction();
        public event GachaAction OnGachaCompleted;
        public event GachaAction OnGachaClosed;

        private void Awake()
        {
            FindUIElements();
            SetupButtons();
        }

        private void Start()
        {
            if (_gachaSystem == null)
            {
                _gachaSystem = FindObjectOfType<GachaSystem>();
            }

            if (_gachaSystem != null)
            {
                _gachaSystem.OnGachaResult += OnGachaResult;
            }

            if (_resultPanel != null)
            {
                _resultPanel.SetActive(false);
            }

            UpdateResourceDisplay();
        }

        private void OnDestroy()
        {
            if (_gachaSystem != null)
            {
                _gachaSystem.OnGachaResult -= OnGachaResult;
            }
        }

        private void FindUIElements()
        {
            if (_pullButton == null && !string.IsNullOrEmpty(_pullButtonPath))
            {
                var btnT = transform.Find(_pullButtonPath);
                if (btnT != null) _pullButton = btnT.GetComponent<Button>();
            }

            if (_pullCostText == null && !string.IsNullOrEmpty(_pullCostPath))
            {
                var txtT = transform.Find(_pullCostPath);
                if (txtT != null) _pullCostText = txtT.GetComponent<TextMeshProUGUI>();
            }

            if (_resultPanel == null && !string.IsNullOrEmpty(_resultPanelPath))
            {
                var panelT = transform.Find(_resultPanelPath);
                if (panelT != null) _resultPanel = panelT.gameObject;
            }

            if (_resultContainer == null && !string.IsNullOrEmpty(_resultContainerPath))
            {
                _resultContainer = transform.Find(_resultContainerPath);
            }

            if (_playerResourceText == null && !string.IsNullOrEmpty(_playerResourcePath))
            {
                var txtT = transform.Find(_playerResourcePath);
                if (txtT != null) _playerResourceText = txtT.GetComponent<TextMeshProUGUI>();
            }

            if (_closeButton == null && !string.IsNullOrEmpty(_closeButtonPath))
            {
                var btnT = transform.Find(_closeButtonPath);
                if (btnT != null) _closeButton = btnT.GetComponent<Button>();
            }
        }

        private void SetupButtons()
        {
            if (_pullButton != null)
            {
                _pullButton.onClick.RemoveAllListeners();
                _pullButton.onClick.AddListener(OnPullClicked);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveAllListeners();
                _closeButton.onClick.AddListener(OnCloseClicked);
            }
        }

        /// <summary>
        /// 显示抽卡界面
        /// </summary>
        public void Show(int playerIndex = 0)
        {
            _currentPlayerIndex = playerIndex;
            gameObject.SetActive(true);
            UpdateResourceDisplay();
        }

        /// <summary>
        /// 隐藏抽卡界面
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnPullClicked()
        {
            if (_isAnimating || _gachaSystem == null) return;

            // 检查资源
            int resource = GameManager.Instance.GetResource(_currentPlayerIndex);
            if (resource < _gachaSystem.GachaCost)
            {
                Debug.LogWarning("[GachaView] 抽卡费用不足");
                return;
            }

            // 开始抽卡
            _isAnimating = true;
            _pullButton.interactable = false;

            // 执行抽卡
            var results = _gachaSystem.Pull(_currentPlayerIndex);
            ShowResults(results);

            UpdateResourceDisplay();
            _isAnimating = false;
            _pullButton.interactable = true;
        }

        private void OnGachaResult(List<GachaItem> results)
        {
            // 显示结果（可选：播放动画）
        }

        private void ShowResults(List<GachaItem> results)
        {
            if (_resultPanel == null || _resultContainer == null) return;

            // 清除现有结果
            foreach (Transform child in _resultContainer)
            {
                Destroy(child.gameObject);
            }

            // 显示结果面板
            _resultPanel.SetActive(true);

            // 生成结果项
            foreach (var item in results)
            {
                CreateResultItem(item);
            }

            Debug.Log($"[GachaView] 显示抽卡结果: {results.Count} 件物品");
        }

        private void CreateResultItem(GachaItem item)
        {
            GameObject itemObj;

            if (_resultItemPrefab != null)
            {
                itemObj = Instantiate(_resultItemPrefab, _resultContainer);
            }
            else
            {
                // 创建简易结果项
                itemObj = new GameObject($"GachaResult_{item.Name}");
                itemObj.transform.SetParent(_resultContainer, false);

                var img = itemObj.AddComponent<Image>();
                img.color = GetRarityColor(item.Rarity);

                var txt = itemObj.AddComponent<TextMeshProUGUI>();
                txt.text = item.Name;
                txt.alignment = TextAlignmentOptions.Center;
            }
        }

        private Color GetRarityColor(float rarity)
        {
            if (rarity >= 0.9f) return Color.yellow;        // SSR
            if (rarity >= 0.7f) return new Color(1f, 0.5f, 0f); // SR - 橙色
            if (rarity >= 0.4f) return Color.magenta;        // R - 紫色
            return Color.gray;                               // N - 灰色
        }

        private void OnCloseClicked()
        {
            Hide();
            OnGachaClosed?.Invoke();
        }

        /// <summary>
        /// 更新玩家资源显示
        /// </summary>
        private void UpdateResourceDisplay()
        {
            if (_playerResourceText != null && GameManager.Instance != null)
            {
                int resource = GameManager.Instance.GetResource(_currentPlayerIndex);
                _playerResourceText.text = $"资源: {resource}";
            }

            if (_pullCostText != null && _gachaSystem != null)
                _pullCostText.text = $"抽卡: {_gachaSystem.GachaCost}";
        }

        /// <summary>
        /// 设置抽卡系统（外部调用）
        /// </summary>
        public void SetGachaSystem(GachaSystem system)
        {
            if (_gachaSystem != null)
            {
                _gachaSystem.OnGachaResult -= OnGachaResult;
            }

            _gachaSystem = system;

            if (_gachaSystem != null)
            {
                _gachaSystem.OnGachaResult += OnGachaResult;
            }
        }
    }
}
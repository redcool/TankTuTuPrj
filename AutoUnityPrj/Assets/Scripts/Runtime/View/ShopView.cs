using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Game.Runtime.Controller;

using ShopItem = Game.Runtime.Controller.ShopItem;

namespace Game.Runtime.View
{
    /// <summary>
    /// 商城界面 - 购买武器和道具
    /// 显示商品列表，支持刷新，购买后自动装备到战车
    /// 作者：AI
    /// 最后修改时间：2026-04-09
    /// </summary>
    public class ShopView : MonoBehaviour
    {
        [Header("UI路径")]
        [SerializeField] private string _itemGridPath = "ItemGrid/Viewport/Content";
        [SerializeField] private string _refreshButtonPath = "ButtonPanel/RefreshButton";
        [SerializeField] private string _confirmButtonPath = "ButtonPanel/ConfirmButton";
        [SerializeField] private string _playerResourcePath = "PlayerInfo/ResourceText";
        [SerializeField] private string _refreshCostPath = "ButtonPanel/RefreshButton/CostText";

        [Header("UI引用")]
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private TextMeshProUGUI _playerResourceText;
        [SerializeField] private TextMeshProUGUI _refreshCostText;

        [Header("商品项预制体")]
        [SerializeField] private GameObject _shopItemPrefab;

        [Header("商城管理器")]
        [SerializeField] private ShopManager _shopManager;

        // 私有字段
        private List<GameObject> _itemSlots = new List<GameObject>();
        private int _currentPlayerIndex = 0;

        // 事件
        public delegate void ShopAction();
        public event ShopAction OnShopClosed;
        public event ShopAction OnNextLevel;

        private void Awake()
        {
            FindUIElements();
            SetupButtons();
        }

        private void Start()
        {
            if (_shopManager == null)
            {
                _shopManager = FindObjectOfType<ShopManager>();
            }

            if (_shopManager != null)
            {
                _shopManager.Initialize();
                _shopManager.OnShopRefresh += UpdateShopDisplay;
                _shopManager.OnItemPurchased += OnItemBought;
            }

            UpdateResourceDisplay();
        }

        private void OnDestroy()
        {
            if (_shopManager != null)
            {
                _shopManager.OnShopRefresh -= UpdateShopDisplay;
                _shopManager.OnItemPurchased -= OnItemBought;
            }
        }

        private void FindUIElements()
        {
            if (_itemContainer == null && !string.IsNullOrEmpty(_itemGridPath))
            {
                _itemContainer = transform.Find(_itemGridPath);
            }

            if (_refreshButton == null && !string.IsNullOrEmpty(_refreshButtonPath))
            {
                var btnT = transform.Find(_refreshButtonPath);
                if (btnT != null) _refreshButton = btnT.GetComponent<Button>();
            }

            if (_confirmButton == null && !string.IsNullOrEmpty(_confirmButtonPath))
            {
                var btnT = transform.Find(_confirmButtonPath);
                if (btnT != null) _confirmButton = btnT.GetComponent<Button>();
            }

            if (_playerResourceText == null && !string.IsNullOrEmpty(_playerResourcePath))
            {
                var txtT = transform.Find(_playerResourcePath);
                if (txtT != null) _playerResourceText = txtT.GetComponent<TextMeshProUGUI>();
            }

            if (_refreshCostText == null && !string.IsNullOrEmpty(_refreshCostPath))
            {
                var txtT = transform.Find(_refreshCostPath);
                if (txtT != null) _refreshCostText = txtT.GetComponent<TextMeshProUGUI>();
            }
        }

        private void SetupButtons()
        {
            if (_refreshButton != null)
            {
                _refreshButton.onClick.RemoveAllListeners();
                _refreshButton.onClick.AddListener(OnRefreshClicked);
            }

            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(OnConfirmClicked);
            }
        }

        /// <summary>
        /// 显示商城界面
        /// </summary>
        public void Show(int playerIndex = 0)
        {
            _currentPlayerIndex = playerIndex;
            gameObject.SetActive(true);
            UpdateShopDisplay();
            UpdateResourceDisplay();
        }

        /// <summary>
        /// 隐藏商城界面
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 更新商城显示
        /// </summary>
        private void UpdateShopDisplay()
        {
            if (_shopManager == null || _itemContainer == null) return;

            // 清除现有商品
            foreach (Transform child in _itemContainer)
            {
                Destroy(child.gameObject);
            }
            _itemSlots.Clear();

            // 生成商品项
            var items = _shopManager.CurrentItems;
            for (int i = 0; i < items.Count; i++)
            {
                CreateShopItem(items[i], i);
            }
        }

        private void CreateShopItem(ShopItem item, int index)
        {
            GameObject itemObj;

            if (_shopItemPrefab != null)
            {
                itemObj = Instantiate(_shopItemPrefab, _itemContainer);
            }
            else
            {
                // 创建简易商品项
                itemObj = new GameObject($"ShopItem_{index}");
                itemObj.transform.SetParent(_itemContainer, false);

                var img = itemObj.AddComponent<Image>();
                img.color = Color.white;

                var btn = itemObj.AddComponent<Button>();
                btn.onClick.AddListener(() => OnItemClicked(index));
            }

            _itemSlots.Add(itemObj);
        }

        private void OnItemClicked(int index)
        {
            if (_shopManager == null) return;
            
            var items = _shopManager.CurrentItems;
            if (index < 0 || index >= items.Count) return;

            var item = items[index];

            // 购买商品
            if (_shopManager.PurchaseItem(index, _currentPlayerIndex))
            {
                Debug.Log($"[ShopView] 购买商品: {_shopManager.CurrentItems[index].Name}");
            }
        }

        private void OnRefreshClicked()
        {
            if (_shopManager == null) return;

            // 检查资源
            int resource = GameManager.Instance.GetResource(_currentPlayerIndex);
            if (resource < _shopManager.RefreshCost)
            {
                Debug.LogWarning("[ShopView] 刷新费用不足");
                return;
            }

            // 消费并刷新
            GameManager.Instance.SpendResource(_currentPlayerIndex, _shopManager.RefreshCost);
            _shopManager.RefreshShop();
            UpdateResourceDisplay();
        }

        private void OnItemBought()
        {
            UpdateResourceDisplay();
        }

        private void OnConfirmClicked()
        {
            Hide();
            OnNextLevel?.Invoke();
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

            if (_refreshCostText != null && _shopManager != null)
            {
                _refreshCostText.text = $"刷新: {_shopManager.RefreshCost}";
            }
        }
    }
}
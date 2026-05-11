using UnityEngine;
using UnityEngine.UI;
using Game.Runtime.ValueObject.ScriptableObjects;
using Game.Runtime.View;

namespace Game.Runtime.View
{
    /// <summary>
    /// 武器选择界面 - 通过事件系统与 PlayerSelectionControl 通信
    /// 显隐由 PlayerSelectionControl 控制
    /// </summary>
    public class WeaponSelectionView : MonoBehaviour
    {
        [Header("UI路径")]
        [SerializeField] public string _weaponGridPath = "WeaponGrid";
        [SerializeField] public string _backButtonPath = "BackButton";
        [SerializeField] public string _titleTextPath = "Title";

        [Header("UI引用")]
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _titleText;
        [SerializeField] private PlayerSelectedDetailView _detailPanel;

        [Header("武器卡片预制体")]
        [SerializeField] private GameObject _weaponCardPrefab;
        private const string CARD_PREFAB_PATH = "Prefabs/UI/WeaponCardPrefab";

        [Header("武器数据列表")]
        [SerializeField] private WeaponDataSO[] _weaponDataList;

        // 生成的卡片数组
        private WeaponCard[] _weaponCards;

        // 回调事件
        public System.Action<WeaponDataSO> OnWeaponConfirmed;

        private void Awake()
        {
            FindUIElements();
            SetupButtons();
        }

        private void Start()
        {
            LoadWeaponData();
            LoadCardPrefab();
            BuildWeaponCards();
        }

        /// <summary>
        /// 加载武器数据
        /// </summary>
        private void LoadWeaponData()
        {
            var loadedWeapons = Resources.LoadAll<WeaponDataSO>("ScriptableObjects/Weapons");
            
            if (loadedWeapons != null && loadedWeapons.Length > 0)
            {
                _weaponDataList = loadedWeapons;
                Debug.Log($"[WeaponSelectionView] 从Resources加载了 {_weaponDataList.Length} 个武器数据");
            }
            else
            {
                Debug.LogWarning("[WeaponSelectionView] 未找到武器数据");
                _weaponDataList = new WeaponDataSO[0];
            }
        }

        /// <summary>
        /// 加载武器卡片预制体
        /// </summary>
        private void LoadCardPrefab()
        {
            if (_weaponCardPrefab == null)
            {
                _weaponCardPrefab = Resources.Load<GameObject>(CARD_PREFAB_PATH);
            }
        }

        /// <summary>
        /// 按路径查找UI元素
        /// </summary>
        private void FindUIElements()
        {
            if (_cardContainer == null && !string.IsNullOrEmpty(_weaponGridPath))
            {
                _cardContainer = transform.Find(_weaponGridPath);
            }

            if (_backButton == null && !string.IsNullOrEmpty(_backButtonPath))
            {
                var btnT = transform.Find(_backButtonPath);
                if (btnT != null) _backButton = btnT.GetComponent<Button>();
            }

            if (_titleText == null && !string.IsNullOrEmpty(_titleTextPath))
            {
                var txtT = transform.Find(_titleTextPath);
                if (txtT != null) _titleText = txtT.GetComponent<Text>();
            }

            if (_detailPanel == null)
            {
                _detailPanel = FindObjectOfType<PlayerSelectedDetailView>();
            }
        }

        private void SetupButtons()
        {
            if (_backButton != null)
            {
                _backButton.onClick.RemoveAllListeners();
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        /// <summary>
        /// 构建武器卡片网格
        /// </summary>
        private void BuildWeaponCards()
        {
            if (_cardContainer == null || _weaponCardPrefab == null || _weaponDataList == null)
            {
                Debug.LogWarning("[WeaponSelectionView] 构建卡片失败: 缺少必要组件");
                return;
            }

            // 清除现有卡片
            foreach (Transform child in _cardContainer)
            {
                Destroy(child.gameObject);
            }

            _weaponCards = new WeaponCard[_weaponDataList.Length];

            for (int i = 0; i < _weaponDataList.Length; i++)
            {
                var weaponData = _weaponDataList[i];
                var cardObj = Instantiate(_weaponCardPrefab, _cardContainer);
                var card = cardObj.GetComponent<WeaponCard>();

                if (card != null)
                {
                    card.Initialize(weaponData);
                    card.OnWeaponSelected += OnCardSelected;
                    card.OnWeaponHovered += OnCardHovered;
                    _weaponCards[i] = card;
                }
            }

            Debug.Log($"[WeaponSelectionView] 生成了 {_weaponCards.Length} 个武器卡片");
        }

        /// <summary>
        /// 武器卡片被点击 - 发布事件
        /// </summary>
        private void OnCardSelected(WeaponDataSO weapon)
        {
            // 清除其他卡片的选中状态
            if (_weaponCards != null)
            {
                foreach (var card in _weaponCards)
                {
                    if (card != null) card.ClearSelection();
                }
            }

            // 高亮当前选中
            var currentCard = System.Array.Find(_weaponCards, c => c.GetWeaponData() == weapon);
            if (currentCard != null) currentCard.SetSelected(true);

            Debug.Log($"[WeaponSelectionView] 确认选择武器: {weapon.WeaponName}");

            // 保存选择的武器
            PlayerPrefs.SetString("SelectedWeaponId", weapon.WeaponId);

            // 发布事件 - 由 PlayerSelectionControl 处理后续流程
            SelectionEventManager.Instance.Publish(SelectionEventType.WeaponSelected, weapon);

            // 触发回调事件
            OnWeaponConfirmed?.Invoke(weapon);
        }

        /// <summary>
        /// 武器卡片悬停 - 发布事件显示详情
        /// </summary>
        private void OnCardHovered(WeaponDataSO weapon)
        {
            if (_detailPanel != null)
            {
                _detailPanel.SetWeapon(weapon);
                _detailPanel.Show();
            }

            // 发布悬停事件
            SelectionEventManager.Instance.Publish(SelectionEventType.WeaponHovered, weapon);
        }

        /// <summary>
        /// 返回按钮点击 - 发布返回事件
        /// </summary>
        private void OnBackClicked()
        {
            SelectionEventManager.Instance.Publish(SelectionEventType.BackToPrevious);
        }
    }
}
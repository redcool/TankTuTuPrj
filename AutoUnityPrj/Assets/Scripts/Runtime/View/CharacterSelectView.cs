using UnityEngine;
using UnityEngine.UI;
using Game.Runtime.ValueObject.ScriptableObjects;
using Game.Runtime.View;

namespace Game.Runtime.View
{
    /// <summary>
    /// 角色选择界面 - 通过事件系统与 PlayerSelectionControl 通信
    /// 玩家选择角色后进入武器选择
    /// 显隐由 PlayerSelectionControl 控制
    /// </summary>
    public class CharacterSelectView : MonoBehaviour
    {
        [Header("UI路径")]
        [SerializeField] private string _characterGridPath = "CharacterGrid";
        [SerializeField] private string _backButtonPath = "ButtonPanel/BackButton";
        [SerializeField] private string _titleTextPath = "TitleText";
        [SerializeField] private string _detailPanelPath = "StatsPanel/CharacterDetailPanel";

        [Header("UI引用")]
        [SerializeField] private ScrollRect _characterGridScroll;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _titleText;
        [SerializeField] private CharacterDetailPanel _characterDetailPanel;

        [Header("角色卡片预制体")]
        [SerializeField] private GameObject _characterCardPrefab;
        private const string CARD_PREFAB_PATH = "Prefabs/UI/CharacterCardPrefab";

        [Header("角色数据列表")]
        [SerializeField] private CharacterDataSO[] _characterDataSOList;

        // 生成的卡片数组
        private CharacterCard[] _characterCards;

        // 回调事件（供其他模块订阅）
        public delegate void CharacterSelectAction(CharacterDataSO character);
        public event CharacterSelectAction OnCharacterConfirmed;

        private void Awake()
        {
            FindUIElements();
            SetupButtons();
        }

        private void Start()
        {
            LoadCharacterData();
            LoadCardPrefab();
            BuildCharacterCards();
        }

        /// <summary>
        /// 加载角色数据SO - 优先从Resources加载
        /// </summary>
        private void LoadCharacterData()
        {
            var loadedCharacters = Resources.LoadAll<CharacterDataSO>("ScriptableObjects/Characters");
            
            if (loadedCharacters != null && loadedCharacters.Length > 0)
            {
                _characterDataSOList = loadedCharacters;
                Debug.Log($"[CharacterSelectView] 从Resources加载了 {_characterDataSOList.Length} 个角色数据");
            }
            else if (_characterDataSOList != null && _characterDataSOList.Length > 0)
            {
                Debug.Log($"[CharacterSelectView] 使用Inspector配置的 {_characterDataSOList.Length} 个角色数据");
            }
            else
            {
                Debug.LogWarning("[CharacterSelectView] 未找到任何角色数据，请执行 IronTutu → Create Default Characters");
                _characterDataSOList = new CharacterDataSO[0];
            }
        }

        /// <summary>
        /// 加载角色卡片预制体
        /// </summary>
        private void LoadCardPrefab()
        {
            if (_characterCardPrefab == null)
            {
                _characterCardPrefab = Resources.Load<GameObject>(CARD_PREFAB_PATH);
                if (_characterCardPrefab != null)
                {
                    Debug.Log($"[CharacterSelectView] 从Resources加载卡片预制体: {CARD_PREFAB_PATH}");
                }
                else
                {
                    Debug.LogWarning("[CharacterSelectView] 未找到卡片预制体，请配置或放到Resources目录");
                }
            }
        }

        /// <summary>
        /// 按路径查找UI元素
        /// </summary>
        private void FindUIElements()
        {
            if (_characterGridScroll == null && !string.IsNullOrEmpty(_characterGridPath))
            {
                var gridT = transform.Find(_characterGridPath);
                if (gridT != null)
                {
                    _characterGridScroll = gridT.GetComponent<ScrollRect>();
                    if (_characterGridScroll != null && _characterGridScroll.content != null)
                    {
                        _cardContainer = _characterGridScroll.content;
                    }
                }
            }

            if (_cardContainer == null && !string.IsNullOrEmpty(_characterGridPath))
            {
                _cardContainer = transform.Find(_characterGridPath + "/Viewport/Content")
                              ?? transform.Find(_characterGridPath + "/Content");
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

            if (_characterDetailPanel == null && !string.IsNullOrEmpty(_detailPanelPath))
            {
                var panelT = transform.Find(_detailPanelPath);
                if (panelT != null) _characterDetailPanel = panelT.GetComponent<CharacterDetailPanel>();
            }

            if (_characterDetailPanel == null)
            {
                _characterDetailPanel = FindObjectOfType<CharacterDetailPanel>();
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
        /// 构建角色卡片数组
        /// </summary>
        private void BuildCharacterCards()
        {
            if (_cardContainer == null || _characterCardPrefab == null || _characterDataSOList == null)
            {
                Debug.LogWarning("[CharacterSelectView] 构建卡片失败: 缺少必要组件");
                return;
            }

            // 清除现有卡片
            foreach (Transform child in _cardContainer)
            {
                Destroy(child.gameObject);
            }

            _characterCards = new CharacterCard[_characterDataSOList.Length];

            for (int i = 0; i < _characterDataSOList.Length; i++)
            {
                var characterData = _characterDataSOList[i];
                var cardObj = Instantiate(_characterCardPrefab, _cardContainer);
                var card = cardObj.GetComponent<CharacterCard>();

                if (card != null)
                {
                    card.Initialize(characterData);
                    card.OnCharacterSelected += OnCardSelected;
                    card.OnCharacterHovered += OnCardHovered;
                    _characterCards[i] = card;
                }
                else
                {
                    Debug.LogWarning($"[CharacterSelectView] 卡片 {i} ({characterData.CharacterName}) 缺少CharacterCard组件");
                }
            }

            Debug.Log($"[CharacterSelectView] 生成了 {_characterCards.Length} 个角色卡片");
        }

        /// <summary>
        /// 角色卡片被点击 - 发布事件
        /// </summary>
        private void OnCardSelected(CharacterDataSO character)
        {
            // 清除其他卡片的选中状态
            if (_characterCards != null)
            {
                foreach (var card in _characterCards)
                {
                    if (card != null) card.ClearSelection();
                }
            }
            
            // 高亮当前选中
            var currentCard = System.Array.Find(_characterCards, c => c.GetCharacterData() == character);
            if (currentCard != null) currentCard.SetSelected(true);
            
            Debug.Log($"[CharacterSelectView] 确认选择角色: {character.CharacterName}");

            // 保存选中的角色
            PlayerPrefs.SetString("SelectedCharacterId", character.name);

            // 发布事件 - 由 PlayerSelectionControl 处理后续流程
            SelectionEventManager.Instance.Publish(SelectionEventType.CharacterSelected, character);

            // 触发回调事件
            OnCharacterConfirmed?.Invoke(character);
        }

        /// <summary>
        /// 角色卡片悬停 - 发布事件显示详情
        /// </summary>
        private void OnCardHovered(CharacterDataSO character)
        {
            // 更新本地详情面板
            if (_characterDetailPanel != null)
            {
                _characterDetailPanel.ShowCharacter(character);
            }

            // 发布悬停事件
            SelectionEventManager.Instance.Publish(SelectionEventType.CharacterHovered, character);
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
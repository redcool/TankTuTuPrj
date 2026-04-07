using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.View
{
    /// <summary>
    /// 角色选择界面 - 类似土豆兄弟的角色选择
    /// 点击StartButton后从StartMenu隐藏，显示此界面
    /// 玩家选择角色后进入Level_0关卡
    /// </summary>
    public class CharacterSelectView : MonoBehaviour
    {
        [Header("UI路径")]
        [SerializeField] private string _characterGridPath = "CharacterGrid";
        [SerializeField] private string _confirmButtonPath = "ButtonPanel/ConfirmButton";
        [SerializeField] private string _backButtonPath = "ButtonPanel/BackButton";
        [SerializeField] private string _titleTextPath = "TitleText";
        [SerializeField] private string _statsTextPath = "StatsPanel/StatsText";

        [Header("UI引用")]
        [SerializeField] private ScrollRect _characterGridScroll;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _statsText;

        [Header("角色卡片预制体")]
        [SerializeField] private GameObject _characterCardPrefab;
        private const string CARD_PREFAB_PATH = "Prefabs/UI/CharacterCardPrefab";

        [Header("角色数据列表")]
        [Tooltip("角色数据SO列表，Start时自动生成CharacterCard。优先使用Inspector配置，否则从Resources加载")]
        [SerializeField] private CharacterDataSO[] _characterDataSOList;

        // 当前选中的角色
        private CharacterDataSO _selectedCharacter;
        // 生成的卡片数组
        private CharacterCard[] _characterCards;

        // 回调
        public delegate void CharacterSelectAction(CharacterDataSO character);
        public event CharacterSelectAction OnCharacterConfirmed;
        public event System.Action OnBackToMenu;

        private void Awake()
        {
            FindUIElements();
            SetupButtons();
        }

        private void Start()
        {
            // 加载角色数据
            LoadCharacterData();

            // 加载卡片预制体
            LoadCardPrefab();

            // 自动生成角色卡片
            BuildCharacterCards();

            // 默认隐藏
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 加载角色数据SO - 优先从Resources加载
        /// </summary>
        private void LoadCharacterData()
        {
            // 优先从Resources加载
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
            // 查找CharacterGrid的ScrollRect，通过其content属性确定卡片容器
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

            // 兜底：如果ScrollRect没找到，尝试直接查找Content
            if (_cardContainer == null && !string.IsNullOrEmpty(_characterGridPath))
            {
                _cardContainer = transform.Find(_characterGridPath + "/Viewport/Content")
                              ?? transform.Find(_characterGridPath + "/Content");
            }

            if (_confirmButton == null && !string.IsNullOrEmpty(_confirmButtonPath))
            {
                var btnT = transform.Find(_confirmButtonPath);
                if (btnT != null) _confirmButton = btnT.GetComponent<Button>();
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

            if (_statsText == null && !string.IsNullOrEmpty(_statsTextPath))
            {
                var txtT = transform.Find(_statsTextPath);
                if (txtT != null) _statsText = txtT.GetComponent<Text>();
            }
        }

        private void SetupButtons()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveAllListeners();
                _confirmButton.onClick.AddListener(OnConfirmClicked);
                _confirmButton.interactable = false;
            }

            if (_backButton != null)
            {
                _backButton.onClick.RemoveAllListeners();
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        /// <summary>
        /// 构建角色卡片数组 - 在Start时自动调用
        /// </summary>
        private void BuildCharacterCards()
        {
            if (_cardContainer == null)
            {
                Debug.LogWarning("[CharacterSelectView] _cardContainer 为空，无法生成卡片。请确保CharacterGrid有ScrollRect组件且content已配置");
                return;
            }

            if (_characterCardPrefab == null)
            {
                Debug.LogWarning("[CharacterSelectView] _characterCardPrefab 为空，无法生成卡片");
                return;
            }

            if (_characterDataSOList == null || _characterDataSOList.Length == 0)
            {
                Debug.LogWarning("[CharacterSelectView] _characterDataSOList 为空，无法生成卡片");
                return;
            }

            // 清除现有卡片
            foreach (Transform child in _cardContainer)
            {
                Destroy(child.gameObject);
            }

            // 初始化卡片数组
            _characterCards = new CharacterCard[_characterDataSOList.Length];

            // 为每个角色数据生成卡片，放入cardContainer（ScrollRect.content）
            for (int i = 0; i < _characterDataSOList.Length; i++)
            {
                var characterData = _characterDataSOList[i];
                var cardObj = Instantiate(_characterCardPrefab, _cardContainer);
                var card = cardObj.GetComponent<CharacterCard>();

                if (card != null)
                {
                    card.Initialize(characterData);
                    card.OnCharacterSelected += OnCardSelected;
                    _characterCards[i] = card;
                }
                else
                {
                    Debug.LogWarning($"[CharacterSelectView] 卡片 {i} ({characterData.characterName}) 缺少CharacterCard组件");
                }
            }

            Debug.Log($"[CharacterSelectView] 生成了 {_characterCards.Length} 个角色卡片到 {_cardContainer.name}");
        }

        /// <summary>
        /// 显示角色选择界面
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏角色选择界面
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 角色卡片被点击
        /// </summary>
        private void OnCardSelected(CharacterDataSO character)
        {
            _selectedCharacter = character;

            // 更新统计信息面板
            if (_statsText != null)
            {
                _statsText.text = $"<b>{character.characterName}</b>\n\n{character.GetStatsDescription()}";
            }

            // 启用确认按钮
            if (_confirmButton != null)
                _confirmButton.interactable = true;

            // 高亮选中的卡片
            if (_characterCards != null)
            {
                foreach (var card in _characterCards)
                {
                    if (card != null)
                    {
                        card.SetSelected(false);
                    }
                }
            }
        }

        private void OnConfirmClicked()
        {
            if (_selectedCharacter == null) return;

            Hide();
            OnCharacterConfirmed?.Invoke(_selectedCharacter);

            // 加载关卡
            SceneManager.LoadScene("Level_0");
        }

        private void OnBackClicked()
        {
            Hide();
            OnBackToMenu?.Invoke();
        }
    }
}

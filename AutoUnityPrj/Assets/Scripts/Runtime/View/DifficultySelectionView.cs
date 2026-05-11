using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Game.Runtime.ValueObject.ScriptableObjects;
using Game.Runtime.View;

namespace Game.Runtime.View
{
    public class DifficultySelectionView : MonoBehaviour
    {
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _titleText;
        [SerializeField] private PlayerSelectedDetailView _detailPanel;
        [SerializeField] private DifficultyDataSO[] _difficultyDataList;

        private DifficultyCardView[] _difficultyCards;

        private void Awake()
        {
            FindUIElements();
            SetupButtons();
        }

        private void Start()
        {
            LoadDifficultyData();
            BuildDifficultyCards();
        }

        private void LoadDifficultyData()
        {
            var loadedDifficulties = Resources.LoadAll<DifficultyDataSO>("ScriptableObjects/Difficulties");
            
            if (loadedDifficulties != null && loadedDifficulties.Length > 0)
            {
                _difficultyDataList = loadedDifficulties;
                System.Array.Sort(_difficultyDataList, (a, b) => a.DifficultyLevel.CompareTo(b.DifficultyLevel));
                Debug.Log("[DifficultySelectionView] from Resources loaded " + _difficultyDataList.Length + " difficulty data");
            }
            else
            {
                Debug.LogWarning("[DifficultySelectionView] not found difficulty data, use default config");
                _difficultyDataList = CreateDefaultDifficulties();
            }
        }

        private DifficultyDataSO[] CreateDefaultDifficulties()
        {
            var difficulties = new DifficultyDataSO[7];
            string[] names = new string[] { "beginner", "easy", "normal", "hard", "expert", "master", "nightmare" };
            
            for (int i = 0; i < 7; i++)
            {
                var data = ScriptableObject.CreateInstance<DifficultyDataSO>();
                data.SetPrivateField("_difficultyName", names[i]);
                data.SetPrivateField("_difficultyLevel", i);
                data.SetPrivateField("_description", GetDefaultDescription(i));
                
                data.SetPrivateField("_enemyCountMultiplier", 0.5f + i * 0.5f);
                data.SetPrivateField("_enemyHpMultiplier", 1f + i * 0.3f);
                data.SetPrivateField("_enemySpeedMultiplier", 1f + i * 0.1f);
                data.SetPrivateField("_enemyDamageMultiplier", 1f + i * 0.25f);
                data.SetPrivateField("_spawnIntervalMultiplier", Mathf.Max(0.1f, 1f - i * 0.1f));
                data.SetPrivateField("_dropRateMultiplier", 1f + i * 0.15f);
                data.SetPrivateField("_expMultiplier", 1f + i * 0.2f);
                
                difficulties[i] = data;
            }
            
            return difficulties;
        }

        private string GetDefaultDescription(int level)
        {
            switch (level)
            {
                case 0: return "beginner_desc";
                case 1: return "easy_desc";
                case 2: return "normal_desc";
                case 3: return "hard_desc";
                case 4: return "expert_desc";
                case 5: return "master_desc";
                case 6: return "nightmare_desc";
                default: return "";
            }
        }

        private void FindUIElements()
        {
            _cardContainer = transform.Find("DifficultyGrid");
            var backBtn = transform.Find("BackButton");
            if (backBtn != null) _backButton = backBtn.GetComponent<Button>();
            var titleT = transform.Find("Title");
            if (titleT != null) _titleText = titleT.GetComponent<Text>();

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

        private void BuildDifficultyCards()
        {
            if (_cardContainer == null || _difficultyDataList == null || _difficultyDataList.Length == 0)
            {
                Debug.LogWarning("[DifficultySelectionView] build card failed: missing components");
                return;
            }

            foreach (Transform child in _cardContainer)
            {
                Destroy(child.gameObject);
            }

            var cardPrefab = Resources.Load<GameObject>("Prefabs/UI/DifficultyCardPrefab");

            _difficultyCards = new DifficultyCardView[_difficultyDataList.Length];

            for (int i = 0; i < _difficultyDataList.Length; i++)
            {
                var difficultyData = _difficultyDataList[i];
                GameObject cardObj;
                
                if (cardPrefab != null)
                {
                    cardObj = Instantiate(cardPrefab, _cardContainer);
                }
                else
                {
                    cardObj = CreateSimpleCard(difficultyData);
                    cardObj.transform.SetParent(_cardContainer, false);
                }
                
                var card = cardObj.GetComponent<DifficultyCardView>();
                
                if (card != null)
                {
                    card.Initialize(difficultyData);
                    card.OnDifficultySelected += OnCardSelected;
                    card.OnDifficultyHovered += OnCardHovered;
                    _difficultyCards[i] = card;
                }
            }

            Debug.Log("[DifficultySelectionView] generated " + _difficultyCards.Length + " difficulty cards");
        }

        private GameObject CreateSimpleCard(DifficultyDataSO data)
        {
            var cardObj = new GameObject("Card_" + data.DifficultyLevel);
            
            var rect = cardObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 120);
            
            var image = cardObj.AddComponent<Image>();
            image.color = GetDifficultyColor(data.DifficultyLevel);
            
            var btn = cardObj.AddComponent<Button>();
            btn.targetGraphic = image;
            
            cardObj.AddComponent<DifficultyCardView>();
            
            return cardObj;
        }

        private Color GetDifficultyColor(int level)
        {
            switch (level)
            {
                case 0: return Color.gray;
                case 1: return Color.green;
                case 2: return Color.blue;
                case 3: return new Color(1f, 0.5f, 0f);
                case 4: return Color.red;
                case 5: return new Color(0.5f, 0f, 0.5f);
                case 6: return new Color(1f, 0f, 1f);
                default: return Color.white;
            }
        }

        private void OnCardSelected(DifficultyDataSO difficulty)
        {
            if (_difficultyCards != null)
            {
                foreach (var card in _difficultyCards)
                {
                    if (card != null) card.ClearSelection();
                }
            }

            var currentCard = System.Array.Find(_difficultyCards, c => c.GetDifficultyData() == difficulty);
            if (currentCard != null) currentCard.SetSelected(true);

            Debug.Log("[DifficultySelectionView] confirm select difficulty: " + difficulty.DifficultyName);

            PlayerPrefs.SetInt("SelectedDifficulty", difficulty.DifficultyLevel);
            PlayerPrefs.SetString("SelectedDifficultyName", difficulty.DifficultyName);

            SelectionEventManager.Instance.Publish(SelectionEventType.DifficultySelected, difficulty.DifficultyLevel);
        }

        private void OnCardHovered(DifficultyDataSO difficulty)
        {
            if (_detailPanel != null)
            {
                _detailPanel.SetDifficulty(difficulty.DifficultyLevel);
                _detailPanel.Show();
            }

            SelectionEventManager.Instance.Publish(SelectionEventType.DifficultyHovered, difficulty);
        }

        private void OnBackClicked()
        {
            SelectionEventManager.Instance.Publish(SelectionEventType.BackToPrevious);
        }
    }
}

public static class DifficultyDataSOExtensions
{
    public static void SetPrivateField(this ScriptableObject obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
}
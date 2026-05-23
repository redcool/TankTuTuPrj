using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.UI
{
    /// <summary>
    /// 难度选择 Presenter - 单 UIDocument 面板
    /// </summary>
    public class DifficultySelectPresenter : MonoBehaviour
    {
        private UIFlowManager _flow;
        private VisualElement _panel;
        private VisualElement _cardRow;
        private VisualElement _detailArea;
        private Label _detailName;
        private Label _detailDesc;
        private VisualElement _detailMods;
        private Button _btnStart;
        private Button _btnBack;

        private readonly List<DifficultyDataSO> _allDifficulties = new List<DifficultyDataSO>();
        private DifficultyDataSO _selectedDifficulty;
        private VisualElement _currentlySelectedCard;

        private void Start()
        {
            _flow = UIFlowManager.Instance;
            _flow.Initialize();
            if (_flow.CurrentState == UIState.None) return;

            _panel = _flow.Root.Q<VisualElement>("panel-difficulty-select");
            if (_panel == null)
            {
                Debug.LogError("[DifficultySelect] 未找到 panel-difficulty-select 面板容器");
                return;
            }

            _cardRow = _panel.Q<VisualElement>("card-row");
            _detailArea = _panel.Q<VisualElement>("detail-area");
            _detailName = _panel.Q<Label>("detail-name");
            _detailDesc = _panel.Q<Label>("detail-desc");
            _detailMods = _panel.Q<VisualElement>("detail-mods");
            _btnStart = _panel.Q<Button>("btn-start");
            _btnBack = _panel.Q<Button>("btn-back");

            if (_btnStart != null)
                _btnStart.clicked += OnStartClicked;
            if (_btnBack != null)
                _btnBack.clicked += OnBackClicked;

            LoadDifficulties();
        }

        private void OnDestroy()
        {
            if (_btnStart != null)
                _btnStart.clicked -= OnStartClicked;
            if (_btnBack != null)
                _btnBack.clicked -= OnBackClicked;
        }

        private void LoadDifficulties()
        {
            _allDifficulties.Clear();
            if (_cardRow != null) _cardRow.Clear();

            var difficulties = Resources.LoadAll<DifficultyDataSO>("ScriptableObjects/Difficulties");

            // 按难度等级排序 (0~6)，只显示前 6 级
            System.Array.Sort(difficulties, (a, b) =>
                a.DifficultyLevel.CompareTo(b.DifficultyLevel));

            // 只取前 6 个难度 (1级~6级)
            var displayCount = Mathf.Min(difficulties.Length, 6);
            for (int i = 0; i < displayCount; i++)
            {
                _allDifficulties.Add(difficulties[i]);
                CreateCard(difficulties[i]);
            }

            // 默认选中第一个
            if (difficulties.Length > 0)
            {
                SelectDifficulty(difficulties[0]);
                if (_cardRow?.childCount > 0)
                {
                    var firstCard = _cardRow[0];
                    UpdateCardSelection(firstCard);
                }
            }
        }

        private void CreateCard(DifficultyDataSO difficulty)
        {
            var card = new VisualElement();
            card.AddToClassList("ds-card");

            // 难度图标
            var iconImage = new Image();
            iconImage.AddToClassList("ds-card-icon");
            var iconPath = System.IO.Path.Combine(
                UnityEngine.Application.dataPath.Replace("/Assets", ""),
                "Assets", "Arts", "UI", "DifficultyIcons",
                $"{difficulty.name}.png"
            );
            if (System.IO.File.Exists(iconPath))
            {
                var tex = new Texture2D(2, 2);
                tex.LoadImage(System.IO.File.ReadAllBytes(iconPath));
                iconImage.image = tex;
            }
            card.Add(iconImage);

            var nameLabel = new Label($"{(difficulty.DifficultyLevel + 1)}级");
            nameLabel.AddToClassList("ds-card-name");

            var starsLabel = new Label(GetStarString(difficulty.StarRating));
            starsLabel.AddToClassList("ds-card-stars");

            card.Add(nameLabel);
            card.Add(starsLabel);

            // 难度倍率数据
            var statsLabel = new Label(
                $"HP x{difficulty.EnemyHealthMultiplier:F1}\n" +
                $"DMG x{difficulty.EnemyDamageMultiplier:F1}\n" +
                $"SPD x{difficulty.EnemySpeedMultiplier:F1}"
            );
            statsLabel.AddToClassList("ds-card-stats");
            card.Add(statsLabel);

            card.RegisterCallback<ClickEvent>(evt =>
            {
                SelectDifficulty(difficulty);
                UpdateCardSelection(card);
            });

            card.userData = difficulty;
            _cardRow?.Add(card);
        }

        private void SelectDifficulty(DifficultyDataSO difficulty)
        {
            if (difficulty == null) return;
            _selectedDifficulty = difficulty;

            if (_detailName != null) _detailName.text = $"{(difficulty.DifficultyLevel + 1)}级 - {difficulty.DifficultyName}";
            if (_detailDesc != null) _detailDesc.text = difficulty.Description;

            // 修正词条
            if (_detailMods != null)
            {
                _detailMods.Clear();
                AddModRow(_detailMods, "敌人生命", $"{difficulty.EnemyHealthMultiplier * 100:F0}%");
                AddModRow(_detailMods, "敌人伤害", $"{difficulty.EnemyDamageMultiplier * 100:F0}%");
                AddModRow(_detailMods, "敌人速度", $"{difficulty.EnemySpeedMultiplier * 100:F0}%");
                AddModRow(_detailMods, "敌人生成速度", $"{difficulty.EnemySpawnRateMultiplier * 100:F0}%");
                AddModRow(_detailMods, "资源倍率", $"{difficulty.ResourceMultiplier * 100:F0}%");
            }
        }

        private static void AddModRow(VisualElement parent, string label, string value)
        {
            var row = new VisualElement();
            row.AddToClassList("ds-detail-mod-row");

            var labelEl = new Label(label);
            labelEl.AddToClassList("text-secondary");

            var valueEl = new Label(value);
            valueEl.AddToClassList("text-body");

            row.Add(labelEl);
            row.Add(valueEl);
            parent.Add(row);
        }

        private void UpdateCardSelection(VisualElement newSelected)
        {
            if (_currentlySelectedCard != null)
                _currentlySelectedCard.RemoveFromClassList("selected");
            _currentlySelectedCard = newSelected;
            _currentlySelectedCard?.AddToClassList("selected");
        }

        private void OnStartClicked()
        {
            if (_selectedDifficulty == null)
            {
                Debug.LogWarning("[DifficultySelect] 未选择难度!");
                return;
            }

            Debug.Log($"[DifficultySelect] 开始战斗! 难度: {_selectedDifficulty.DifficultyName}");

            // 保存难度数据到 GameManager
            var gm = Object.FindObjectOfType<Game.Runtime.Controller.GameManager>();
            if (gm != null)
            {
                gm.SelectedDifficultyLevel = _selectedDifficulty.StarRating;
                gm.SelectedDifficultyData = _selectedDifficulty;
            }
            else
            {
                Debug.LogWarning("[DifficultySelect] 未找到 GameManager");
            }

            _flow.StartBattle();
        }

        private void OnBackClicked()
        {
            _flow.GoBack();
        }

        private static string GetStarString(int stars)
        {
            return new string('★', Mathf.Clamp(stars, 1, 6))
                   + new string('☆', Mathf.Clamp(6 - stars, 0, 5));
        }
    }
}

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.UI
{
    /// <summary>
    /// 角色选择 Presenter — 单 UIDocument 面板
    /// </summary>
    public class CharacterSelectPresenter : MonoBehaviour
    {
        [Header("资源引用")]
        [SerializeField] private VisualTreeAsset _cardTemplate;

        private UIFlowManager _flow;
        private VisualElement _panel;
        private VisualElement _cardGrid;
        private VisualElement _detailPanel;
        private Button _btnConfirm;
        private Button _btnBack;

        private readonly List<CharacterDataSO> _allCharacters = new List<CharacterDataSO>();
        private CharacterDataSO _selectedCharacter;

        // 缓存详情标签
        private Label _detailTitle;
        private Label _detailVehicleTag;
        private VisualElement _detailStats;
        private Label _detailPlaceholder;
        private Label _detailAbilityTitle;
        private Label _detailAbilityDesc;
        private Label _detailWeaponLabel;

        private VisualElement _currentlySelectedCard;

        private void Start()
        {
            _flow = UIFlowManager.Instance;
            _flow.Initialize();
            if (_flow.CurrentState == UIState.None) return;

            _panel = _flow.Root.Q<VisualElement>("panel-character-select");
            if (_panel == null)
            {
                Debug.LogError("[CharacterSelect] 未找到 panel-character-select 面板容器");
                return;
            }

            _cardGrid = _panel.Q<VisualElement>("card-grid");
            _detailPanel = _panel.Q<VisualElement>("detail-panel");
            _btnConfirm = _panel.Q<Button>("btn-confirm");
            _btnBack = _panel.Q<Button>("btn-back");

            _detailTitle = _panel.Q<Label>("detail-title");
            _detailVehicleTag = _panel.Q<Label>("detail-vehicle-tag");
            _detailStats = _panel.Q<VisualElement>("detail-stats");
            _detailPlaceholder = _panel.Q<Label>("detail-placeholder");
            _detailAbilityTitle = _panel.Q<Label>("detail-ability-title");
            _detailAbilityDesc = _panel.Q<Label>("detail-ability-desc");
            _detailWeaponLabel = _panel.Q<Label>("detail-weapon-label");

            if (_btnConfirm != null)
                _btnConfirm.clicked += OnConfirmClicked;
            if (_btnBack != null)
                _btnBack.clicked += OnBackClicked;

            LoadCharacters();

            // 监听 FlowManager 状态切换，面板显示时刷新战车数据
            UIFlowManager.OnStateChanged += OnFlowStateChanged;
        }

        private void OnDestroy()
        {
            if (_btnConfirm != null)
                _btnConfirm.clicked -= OnConfirmClicked;
            if (_btnBack != null)
                _btnBack.clicked -= OnBackClicked;

            UIFlowManager.OnStateChanged -= OnFlowStateChanged;
        }

        private void OnFlowStateChanged(UIState oldState, UIState newState)
        {
            if (newState == UIState.CharacterSelect)
            {
                // 每次进入角色选择时刷新
                LoadCharacters();
            }
        }

        /// <summary>
        /// 从 Resources 加载所有角色
        /// </summary>
        private void LoadCharacters()
        {
            _allCharacters.Clear();
            if (_cardGrid != null)
                _cardGrid.Clear();

            var characters = Resources.LoadAll<CharacterDataSO>("ScriptableObjects/Characters");
            _allCharacters.AddRange(characters);

            if (characters.Length == 0)
            {
                Debug.LogWarning("[CharacterSelect] 未找到角色数据!");
                return;
            }

            foreach (var character in characters)
            {
                CreateCard(character);
            }

            if (characters.Length > 0)
            {
                SelectCharacter(characters[0]);
            }
        }

        /// <summary>
        /// 创建单张角色卡片
        /// </summary>
        private void CreateCard(CharacterDataSO character)
        {
            if (_cardTemplate == null)
            {
                Debug.LogError("[CharacterSelect] 角色卡片模板未赋值!");
                return;
            }

            var cardInstance = _cardTemplate.CloneTree();
            var root = cardInstance.Q<VisualElement>("card-root");

            var nameLabel = cardInstance.Q<Label>("cc-name");
            var tagLabel = cardInstance.Q<Label>("cc-vehicle-tag");
            var hpLabel = cardInstance.Q<Label>("cc-hp");
            var speedLabel = cardInstance.Q<Label>("cc-speed");
            var lockedOverlay = cardInstance.Q<VisualElement>("cc-locked");
            var lockIcon = cardInstance.Q<Image>("cc-lock-icon");
            var lockText = cardInstance.Q<Label>("cc-lock-text");
            var icon = cardInstance.Q<VisualElement>("cc-icon");

            if (nameLabel != null) nameLabel.text = character.CharacterName;
            if (tagLabel != null) tagLabel.text = GetVehicleTypeName(character.VehicleType);
            if (tagLabel != null) tagLabel.AddToClassList(GetVehicleTagClass(character.VehicleType));

            // 加载角色头像
            var iconImage = cardInstance.Q<Image>("cc-icon-image");
            if (iconImage != null)
            {
                var iconName = GetIconNameFromAsset(character.name);
                var iconPath = Path.Combine(
                    Application.dataPath.Replace("/Assets", ""),
                    "Assets", "Arts", "UI", "CharacterPortraits",
                    $"{iconName}.png"
                );
                if (File.Exists(iconPath))
                {
                    var tex = new Texture2D(2, 2);
                    tex.LoadImage(File.ReadAllBytes(iconPath));
                    iconImage.image = tex;
                }
            }

            // 容器颜色（兜底背景色）
            if (icon != null)
            {
                icon.AddToClassList(GetIconColorClass(character.VehicleType));
            }

            // 卡片摘要
            if (hpLabel != null) hpLabel.text = $"HP {character.MaxHealth}";
            if (speedLabel != null) speedLabel.text = $"移速 {character.MoveSpeed:F1}";

            // 初始化：默认隐藏锁定图标
            if (lockIcon != null) lockIcon.style.display = DisplayStyle.None;
            // lock-text 始终在布局中（opacity控制显隐，保证卡片高度一致）

            if (!character.IsUnlocked)
            {
                if (lockIcon != null) lockIcon.style.display = DisplayStyle.Flex;
                if (lockText != null) { lockText.text = character.UnlockCondition; lockText.AddToClassList("visible"); }
                if (root != null) root.AddToClassList("locked");
            }

            if (root != null)
            {
                root.RegisterCallback<ClickEvent>(evt =>
                {
                    if (!character.IsUnlocked) return;
                    SelectCharacter(character);
                    UpdateCardSelection(root);
                });
            }

            if (root != null) root.userData = character;

            _cardGrid?.Add(cardInstance);
        }

        private void SelectCharacter(CharacterDataSO character)
        {
            if (character == null) return;
            _selectedCharacter = character;

            if (_detailTitle != null) _detailTitle.text = character.CharacterName;
            if (_detailVehicleTag != null)
            {
                _detailVehicleTag.text = GetVehicleTypeName(character.VehicleType);
                _detailVehicleTag.ClearClassList();
                _detailVehicleTag.AddToClassList("tag");
                _detailVehicleTag.AddToClassList(GetVehicleTagClass(character.VehicleType));
            }

            if (_detailStats != null)
            {
                _detailStats.Clear();
                var statsText = character.GetStatsDescription();
                if (!string.IsNullOrEmpty(statsText))
                {
                    var statsLabel = new Label(statsText);
                    statsLabel.AddToClassList("text-body");
                    _detailStats.Add(statsLabel);
                }
            }

            if (_detailPlaceholder != null) _detailPlaceholder.style.display = DisplayStyle.None;

            if (_detailAbilityTitle != null) _detailAbilityTitle.text = "特殊能力";
            if (_detailAbilityDesc != null) _detailAbilityDesc.text = character.SpecialAbility;

            if (_detailWeaponLabel != null)
            {
                _detailWeaponLabel.text = (character.StartingWeaponPaths != null && character.StartingWeaponPaths.Length > 0)
                    ? $"初始武器: {character.StartingWeaponPaths[0]}"
                    : "";
            }
        }

        private void UpdateCardSelection(VisualElement newSelectedCard)
        {
            if (_currentlySelectedCard != null)
                _currentlySelectedCard.RemoveFromClassList("selected");
            _currentlySelectedCard = newSelectedCard;
            _currentlySelectedCard?.AddToClassList("selected");
        }

        private void OnConfirmClicked()
        {
            if (_selectedCharacter == null)
            {
                Debug.LogWarning("[CharacterSelect] 未选择角色!");
                return;
            }

            if (!_selectedCharacter.IsUnlocked)
            {
                Debug.LogWarning($"[CharacterSelect] 角色 {_selectedCharacter.CharacterName} 未解锁!");
                return;
            }

            // 保存选中的角色 ID 到 GameManager（跨场景传递）
            var gm = Object.FindObjectOfType<Game.Runtime.Controller.GameManager>();
            if (gm != null)
            {
                gm.SelectedCharacterId = _selectedCharacter.CharacterId;
                gm.SelectedCharacterData = _selectedCharacter;
                Debug.Log($"[CharacterSelect] 选择角色: {_selectedCharacter.CharacterName} (ID: {_selectedCharacter.CharacterId})");
            }
            else
            {
                Debug.LogWarning("[CharacterSelect] 未找到 GameManager，角色选择不会被传递到战斗场景");
            }

            _flow.GoToWeaponSelect();
        }

        private void OnBackClicked()
        {
            _flow.GoBack();
        }

        private static string GetVehicleTypeName(VehicleType type)
        {
            return type switch
            {
                VehicleType.TANK => "坦克",
                VehicleType.LIGHT => "轻型",
                VehicleType.APC => "支援",
                VehicleType.SPG => "远程",
                VehicleType.SPECIAL => "特种",
                _ => "未知"
            };
        }

        private static string GetVehicleTagClass(VehicleType type)
        {
            return type switch
            {
                VehicleType.TANK => "tag-tank",
                VehicleType.LIGHT => "tag-light",
                VehicleType.APC => "tag-apc",
                VehicleType.SPG => "tag-spg",
                VehicleType.SPECIAL => "tag-special",
                _ => ""
            };
        }

        private static string GetIconColorClass(VehicleType type)
        {
            return type switch
            {
                VehicleType.TANK    => "cc-icon-tank",
                VehicleType.LIGHT   => "cc-icon-light",
                VehicleType.APC     => "cc-icon-apc",
                VehicleType.SPG     => "cc-icon-spg",
                VehicleType.SPECIAL => "cc-icon-special",
                _ => "cc-icon-tank"
            };
        }

        /// <summary>
        /// 从 SO 资源名提取图标文件名（不含扩展名）
        /// "Character_HeavyTank" -> "HeavyTank"
        /// </summary>
        private static string GetIconNameFromAsset(string assetName)
        {
            var idx = assetName.LastIndexOf('_');
            return idx >= 0 ? assetName.Substring(idx + 1) : assetName;
        }
    }
}

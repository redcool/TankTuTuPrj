using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Game.Runtime.ValueObject;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.UI
{
    /// <summary>
    /// 武器选择 Presenter — 单 UIDocument 面板
    /// 初始选择 1 把武器，后续可在商店购买更多插槽（最多 _maxWeaponSlots）
    /// </summary>
    public class WeaponSelectPresenter : MonoBehaviour
    {
        [Header("资源引用")]
        [SerializeField] private VisualTreeAsset _cardTemplate;

        [Header("配置")]
        [SerializeField] private int _maxWeaponSlots = 6;

        private UIFlowManager _flow;
        private VisualElement _panel;
        private VisualElement _cardGrid;
        private VisualElement _detailPanel;
        private Button _btnConfirm;
        private Button _btnBack;

        private readonly List<WeaponDataSO> _allWeapons = new List<WeaponDataSO>();
        private WeaponDataSO _selectedWeapon;
        private VisualElement _currentlySelectedCard;

        private Label _detailTitle;
        private Label _detailRarity;
        private Label _detailDesc;
        private VisualElement _detailStats;
        private VisualElement _detailSpecial;

        private void Start()
        {
            _flow = UIFlowManager.Instance;
            _flow.Initialize();
            if (_flow.CurrentState == UIState.None) return;

            _panel = _flow.Root.Q<VisualElement>("panel-weapon-select");
            if (_panel == null)
            {
                Debug.LogError("[WeaponSelect] 未找到 panel-weapon-select 面板容器");
                return;
            }

            _cardGrid = _panel.Q<VisualElement>("card-grid");
            _detailPanel = _panel.Q<VisualElement>("detail-panel");
            _btnConfirm = _panel.Q<Button>("btn-confirm");
            _btnBack = _panel.Q<Button>("btn-back");

            _detailTitle = _panel.Q<Label>("detail-title");
            _detailRarity = _panel.Q<Label>("detail-rarity");
            _detailDesc = _panel.Q<Label>("detail-desc");
            _detailStats = _panel.Q<VisualElement>("detail-stats");
            _detailSpecial = _panel.Q<VisualElement>("detail-special");

            if (_btnConfirm != null)
                _btnConfirm.clicked += OnConfirmClicked;
            if (_btnBack != null)
                _btnBack.clicked += OnBackClicked;

            LoadWeapons();
            ShowAllWeapons();
        }

        private void OnDestroy()
        {
            if (_btnConfirm != null)
                _btnConfirm.clicked -= OnConfirmClicked;
            if (_btnBack != null)
                _btnBack.clicked -= OnBackClicked;
        }

        private void LoadWeapons()
        {
            _allWeapons.Clear();
            var weapons = Resources.LoadAll<WeaponDataSO>("ScriptableObjects/Weapons");
            _allWeapons.AddRange(weapons);
            Debug.Log($"[WeaponSelect] 已加载 {weapons.Length} 件武器");
        }

        /// <summary>
        /// 在网格中展示所有武器（无分类过滤）
        /// </summary>
        private void ShowAllWeapons()
        {
            _cardGrid?.Clear();

            foreach (var weapon in _allWeapons)
                CreateCard(weapon);

            // 默认选中第一个
            if (_cardGrid?.childCount > 0)
            {
                var firstCard = _cardGrid[0];
                var weaponSo = firstCard?.userData as WeaponDataSO;
                if (weaponSo != null)
                {
                    SelectWeapon(weaponSo);
                    UpdateCardSelection(firstCard);
                }
            }
        }

        private void CreateCard(WeaponDataSO weapon)
        {
            if (_cardTemplate == null) return;

            var cardInstance = _cardTemplate.CloneTree();
            var root = cardInstance.Q<VisualElement>("card-root");

            var nameLabel = cardInstance.Q<Label>("wc-name");
            var dmgLabel = cardInstance.Q<Label>("wc-dmg");
            var typeTag = cardInstance.Q<Label>("wc-type-tag");

            if (nameLabel != null) nameLabel.text = weapon.WeaponName;
            if (dmgLabel != null) dmgLabel.text = $"伤害: {weapon.Damage}  攻速: {weapon.AttackSpeed:F1}";
            if (typeTag != null)
            {
                typeTag.text = GetCategoryName(weapon.WeaponCategory);
            }

            if (root != null)
                root.AddToClassList(GetRarityClass(weapon.Rarity));

            // 加载武器图标
            var iconImage = cardInstance.Q<Image>("wc-icon-image");
            if (iconImage != null)
            {
                var iconName = GetIconNameFromAsset(weapon.name);
                var iconPath = Path.Combine(
                    Application.dataPath.Replace("/Assets", ""),
                    "Assets", "Arts", "UI", "WeaponIcons",
                    $"{iconName}.png"
                );
                if (File.Exists(iconPath))
                {
                    var tex = new Texture2D(2, 2);
                    tex.LoadImage(File.ReadAllBytes(iconPath));
                    iconImage.image = tex;
                }
            }

            if (root != null)
            {
                root.RegisterCallback<ClickEvent>(evt =>
                {
                    SelectWeapon(weapon);
                    UpdateCardSelection(root);
                });
                root.userData = weapon;
            }

            _cardGrid?.Add(cardInstance);
        }

        private void SelectWeapon(WeaponDataSO weapon)
        {
            if (weapon == null) return;
            _selectedWeapon = weapon;

            if (_detailTitle != null) _detailTitle.text = weapon.WeaponName;
            if (_detailRarity != null)
            {
                _detailRarity.text = $"稀有度: {GetRarityName(weapon.Rarity)}  |  类型: {GetCategoryName(weapon.WeaponCategory)}";
                _detailRarity.AddToClassList(GetRarityClass(weapon.Rarity));
            }
            if (_detailDesc != null) _detailDesc.text = weapon.Description;

            if (_detailStats != null)
            {
                _detailStats.Clear();
                AddStat("伤害", weapon.Damage.ToString("F0"));
                AddStat("攻速", weapon.AttackSpeed.ToString("F1"));
                AddStat("范围", weapon.Range.ToString("F0"));
                AddStat("穿透", weapon.Pierce.ToString("F0"));
                if (weapon.Area > 0)
                    AddStat("爆炸范围", weapon.Area.ToString("F1"));
                if (weapon.Knockback > 0)
                    AddStat("击退", weapon.Knockback.ToString("F0"));
                if (weapon.ProjectileCount > 1)
                    AddStat("弹片", weapon.ProjectileCount.ToString());
            }

            if (_detailSpecial != null)
            {
                _detailSpecial.Clear();
                var specialTexts = new List<string>();
                if (weapon.Duration > 0) specialTexts.Add($"持续 {weapon.Duration}s");
                if (weapon.Area > 1.5f) specialTexts.Add("范围伤害");
                if (weapon.Knockback > 2f) specialTexts.Add("强击退");
                if (weapon.ProjectileCount > 3) specialTexts.Add("散射");
                if (weapon.IsDefault) specialTexts.Add("默认武器");

                var specialLabel = new Label(specialTexts.Count > 0
                    ? string.Join(" | ", specialTexts)
                    : "标准属性");
                specialLabel.AddToClassList("text-body");
                _detailSpecial.Add(specialLabel);
            }
        }

        private void AddStat(string name, string value)
        {
            if (_detailStats == null) return;
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.marginBottom = 4;

            var nameLabel = new Label(name);
            nameLabel.AddToClassList("text-secondary");

            var valueLabel = new Label(value);
            valueLabel.AddToClassList("text-body");

            row.Add(nameLabel);
            row.Add(valueLabel);
            _detailStats.Add(row);
        }

        private void UpdateCardSelection(VisualElement newSelected)
        {
            if (_currentlySelectedCard != null)
                _currentlySelectedCard.RemoveFromClassList("selected");
            _currentlySelectedCard = newSelected;
            _currentlySelectedCard?.AddToClassList("selected");
        }

        private void OnConfirmClicked()
        {
            if (_selectedWeapon == null)
            {
                Debug.LogWarning("[WeaponSelect] 未选择武器!");
                return;
            }
            Debug.Log($"[WeaponSelect] 确认选择: {_selectedWeapon.WeaponName}");

            // 保存选中武器数据到 GameManager（兼容单武器与多武器列表）
            var gm = Object.FindObjectOfType<Game.Runtime.Controller.GameManager>();
            if (gm != null)
            {
                // 单武器旧字段
                gm.SelectedWeaponId = _selectedWeapon.name;
                gm.SelectedWeaponData = _selectedWeapon;

                // 多武器列表（当前选择 1 把，后续商店可扩展）
                gm.SelectedWeaponDatas.Clear();
                gm.SelectedWeaponDatas.Add(_selectedWeapon);
                gm.SelectedWeaponIdList.Clear();
                gm.SelectedWeaponIdList.Add(_selectedWeapon.name);
            }
            else
            {
                Debug.LogWarning("[WeaponSelect] 未找到 GameManager");
            }

            _flow.GoToDifficultySelect();
        }

        private void OnBackClicked() => _flow.GoBack();

        private static string GetCategoryName(WeaponCategory cat) => cat switch
        {
            ValueObject.WeaponCategory.MainCannon => "主炮",
            ValueObject.WeaponCategory.MachineGun => "机枪",
            ValueObject.WeaponCategory.Missile => "导弹",
            ValueObject.WeaponCategory.Sprayer => "喷射",
            ValueObject.WeaponCategory.Melee => "近战",
            _ => "未知"
        };

        private static string GetRarityName(WeaponRarity rarity) => rarity switch
        {
            WeaponRarity.COMMON => "普通",
            WeaponRarity.RARE => "稀有",
            WeaponRarity.EPIC => "史诗",
            WeaponRarity.LEGENDARY => "传说",
            _ => "普通"
        };

        private static string GetRarityClass(WeaponRarity rarity) => rarity switch
        {
            WeaponRarity.COMMON => "rarity-common",
            WeaponRarity.RARE => "rarity-rare",
            WeaponRarity.EPIC => "rarity-epic",
            WeaponRarity.LEGENDARY => "rarity-legendary",
            _ => "rarity-common"
        };

        /// <summary>
        /// 从 SO 资源名提取图标文件名（不含扩展名）
        /// "Weapon_MainCannon_LightCannon" -> "LightCannon"
        /// </summary>
        private static string GetIconNameFromAsset(string assetName)
        {
            var idx = assetName.LastIndexOf('_');
            return idx >= 0 ? assetName.Substring(idx + 1) : assetName;
        }
    }
}

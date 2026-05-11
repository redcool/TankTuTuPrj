using UnityEngine;
using UnityEngine.UI;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.View
{
    /// <summary>
    /// 玩家选择详情面板 - 显示已选择的角色、武器、难度详情
    /// 显示在游戏开始前或准备阶段的详细信息
    /// </summary>
    public class PlayerSelectedDetailView : MonoBehaviour
    {
        #region Character Section
        [Header("角色详情")]
        [SerializeField] private Image _characterIcon;
        [SerializeField] private Text _characterName;
        [SerializeField] private Text _characterType;
        [SerializeField] private Text _characterDescription;
        #endregion

        #region Weapon Section
        [Header("武器详情")]
        [SerializeField] private Image _weaponIcon;
        [SerializeField] private Text _weaponName;
        [SerializeField] private Text _weaponType;
        [SerializeField] private Text _weaponDescription;
        #endregion

        #region Difficulty Section
        [Header("难度详情")]
        [SerializeField] private Image _difficultyIcon;
        [SerializeField] private Text _difficultyName;
        [SerializeField] private Text _difficultyType;
        [SerializeField] private Text _difficultyDescription;
        #endregion

        // 当前选择的数据
        private CharacterDataSO _selectedCharacter;
        private WeaponDataSO _selectedWeapon;
        private int _selectedDifficulty = 1;

        // 难度配置
        private readonly string[] _difficultyNames = { "", "简单", "普通", "困难", "梦魇" };
        private readonly string[] _difficultyDescriptions = { 
            "", 
            "适合新手的入门难度，敌人较弱", 
            "标准难度，平衡的挑战", 
            "高难度，适合有经验的玩家", 
            "极限挑战，只有高手能通过"
        };

        private void Awake()
        {
            FindUIElements();
        }

        private void Start()
        {
            // 默认隐藏
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 按路径查找UI元素
        /// </summary>
        private void FindUIElements()
        {
            // 角色详情
            if (_characterIcon == null) _characterIcon = transform.Find("CharacterSection/Icon")?.GetComponent<Image>();
            if (_characterName == null) _characterName = transform.Find("CharacterSection/NameText")?.GetComponent<Text>();
            if (_characterType == null) _characterType = transform.Find("CharacterSection/TypeText")?.GetComponent<Text>();
            if (_characterDescription == null) _characterDescription = transform.Find("CharacterSection/DescriptionText")?.GetComponent<Text>();

            // 武器详情
            if (_weaponIcon == null) _weaponIcon = transform.Find("WeaponSection/Icon")?.GetComponent<Image>();
            if (_weaponName == null) _weaponName = transform.Find("WeaponSection/NameText")?.GetComponent<Text>();
            if (_weaponType == null) _weaponType = transform.Find("WeaponSection/TypeText")?.GetComponent<Text>();
            if (_weaponDescription == null) _weaponDescription = transform.Find("WeaponSection/DescriptionText")?.GetComponent<Text>();

            // 难度详情
            if (_difficultyIcon == null) _difficultyIcon = transform.Find("DifficultySection/Icon")?.GetComponent<Image>();
            if (_difficultyName == null) _difficultyName = transform.Find("DifficultySection/NameText")?.GetComponent<Text>();
            if (_difficultyType == null) _difficultyType = transform.Find("DifficultySection/TypeText")?.GetComponent<Text>();
            if (_difficultyDescription == null) _difficultyDescription = transform.Find("DifficultySection/DescriptionText")?.GetComponent<Text>();
        }

        /// <summary>
        /// 设置选择的角色
        /// </summary>
        public void SetCharacter(CharacterDataSO character)
        {
            _selectedCharacter = character;
            UpdateCharacterDisplay();
        }

        /// <summary>
        /// 设置选择的武器
        /// </summary>
        public void SetWeapon(WeaponDataSO weapon)
        {
            _selectedWeapon = weapon;
            UpdateWeaponDisplay();
        }

        /// <summary>
        /// 设置选择的难度
        /// </summary>
        public void SetDifficulty(int difficulty)
        {
            _selectedDifficulty = Mathf.Clamp(difficulty, 1, 4);
            UpdateDifficultyDisplay();
        }

        /// <summary>
        /// 更新角色显示
        /// </summary>
        private void UpdateCharacterDisplay()
        {
            if (_selectedCharacter == null) return;

            if (_characterIcon != null && _selectedCharacter.Icon != null)
                _characterIcon.sprite = _selectedCharacter.Icon;

            if (_characterName != null)
                _characterName.text = _selectedCharacter.CharacterName;

            if (_characterType != null)
                _characterType.text = "角色";

            if (_characterDescription != null)
            {
                _characterDescription.text = _selectedCharacter.GetStatsDescription();
                if (!string.IsNullOrEmpty(_selectedCharacter.SpecialAbility))
                {
                    _characterDescription.text += "\n" + _selectedCharacter.SpecialAbility;
                }
            }
        }

        /// <summary>
        /// 更新武器显示
        /// </summary>
        private void UpdateWeaponDisplay()
        {
            if (_selectedWeapon == null) return;

            // 转换为ValueObject获取属性
            var weaponData = _selectedWeapon.ToDataValue();

            if (_weaponName != null)
                _weaponName.text = weaponData.WeaponName;

            if (_weaponType != null)
                _weaponType.text = weaponData.WeaponType.ToString();

            if (_weaponDescription != null)
            {
                _weaponDescription.text = $"伤害: {weaponData.Damage:F1}\n" +
                    $"攻速: {weaponData.AttackSpeed:F1}/s\n" +
                    $"范围: {weaponData.Range:F1}\n" +
                    $"等级: {weaponData.Level}/{weaponData.MaxLevel}";
            }
        }

        /// <summary>
        /// 更新难度显示
        /// </summary>
        private void UpdateDifficultyDisplay()
        {
            if (_difficultyName != null)
                _difficultyName.text = _difficultyNames[_selectedDifficulty];

            if (_difficultyType != null)
                _difficultyType.text = "难度等级";

            if (_difficultyDescription != null)
                _difficultyDescription.text = _difficultyDescriptions[_selectedDifficulty];
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            // 刷新显示
            UpdateCharacterDisplay();
            UpdateWeaponDisplay();
            UpdateDifficultyDisplay();
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
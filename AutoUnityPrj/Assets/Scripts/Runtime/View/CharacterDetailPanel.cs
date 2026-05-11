using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.View
{
    /// <summary>
    /// 角色详情面板 - 显示选中角色的图标、名称、属性、特殊能力
    /// 参考土豆兄弟右侧详情面板布局
    /// </summary>
    public class CharacterDetailPanel : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _statsText;
        [SerializeField] private Text _abilityText;
        [SerializeField] private Text _weaponsText;
        [SerializeField] private GameObject _emptyHint;

        private void Awake()
        {
            if (_emptyHint != null)
                _emptyHint.SetActive(true);
        }

        /// <summary>
        /// 显示角色详情
        /// </summary>
        public void ShowCharacter(ValueObject.ScriptableObjects.CharacterDataSO character)
        {
            if (character == null)
            {
                ShowEmpty();
                return;
            }

            if (_emptyHint != null)
                _emptyHint.SetActive(false);

            if (_iconImage != null && character.Icon != null)
            {
                _iconImage.sprite = character.Icon;
                _iconImage.enabled = true;
            }

            if (_nameText != null)
            {
                _nameText.text = character.CharacterName;
            }

            if (_statsText != null)
            {
                _statsText.text = BuildStatsText(character);
            }

            if (_abilityText != null && !string.IsNullOrEmpty(character.SpecialAbility))
            {
                _abilityText.text = $"<b>特殊能力</b>\n{character.SpecialAbility}";
            }

            if (_weaponsText != null && character.StartingWeaponPaths != null && character.StartingWeaponPaths.Length > 0)
            {
                _weaponsText.text = $"<b>初始武器</b>\n{string.Join("\n", character.StartingWeaponPaths)}";
            }
        }

        /// <summary>
        /// 显示空状态提示
        /// </summary>
        public void ShowEmpty()
        {
            if (_emptyHint != null)
                _emptyHint.SetActive(true);

            if (_iconImage != null)
                _iconImage.enabled = false;

            if (_nameText != null)
                _nameText.text = "";
            if (_statsText != null)
                _statsText.text = "";
            if (_abilityText != null)
                _abilityText.text = "";
            if (_weaponsText != null)
                _weaponsText.text = "";
        }

        private string BuildStatsText(ValueObject.ScriptableObjects.CharacterDataSO character)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<b>属性</b>");

            AddStatLine(sb, "最大生命", character.MaxHpBonus, "");
            AddStatLine(sb, "移速", character.SpeedBonusPercent, "%");
            AddStatLine(sb, "攻速", character.AttackSpeedBonusPercent, "%");
            AddStatLine(sb, "暴击", character.CritChanceBonus, "%");
            AddStatLine(sb, "护甲", character.ArmorBonus, "");
            AddStatLine(sb, "范围", character.RangeBonusPercent, "%");
            AddStatLine(sb, "幸运", character.LuckBonus, "");
            AddStatLine(sb, "收获", character.HarvestingBonus, "");

            return sb.ToString();
        }

        private void AddStatLine(System.Text.StringBuilder sb, string label, float value, string unit)
        {
            if (Mathf.Approximately(value, 0)) return;

            string sign = value > 0 ? "+" : "";
            string displayValue = unit == "%" ? $"{value * 100:F0}" : $"{value:F0}";
            string color = value > 0 ? "#4CAF50" : "#F44336";
            sb.AppendLine($"<color={color}>{sign}{displayValue}{unit}</color> {label}");
        }

        private void AddStatLine(System.Text.StringBuilder sb, string label, int value, string unit)
        {
            if (value == 0) return;

            string sign = value > 0 ? "+" : "";
            string color = value > 0 ? "#4CAF50" : "#F44336";
            sb.AppendLine($"<color={color}>{sign}{value}{unit}</color> {label}");
        }
    }
}

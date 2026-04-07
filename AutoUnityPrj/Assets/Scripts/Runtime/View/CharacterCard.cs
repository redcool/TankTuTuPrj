using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.View
{
    /// <summary>
    /// 角色卡片 - 显示在角色选择界面中的单个角色项
    /// 参考土豆兄弟的角色网格布局
    /// </summary>
    public class CharacterCard : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _statsText;
        [SerializeField] private Image _lockOverlay;
        [SerializeField] private Text _unlockText;
        [SerializeField] private Button _selectButton;

        private ValueObject.ScriptableObjects.CharacterDataSO _characterData;
        private bool _isSelected;

        public System.Action<ValueObject.ScriptableObjects.CharacterDataSO> OnCharacterSelected;

        /// <summary>
        /// 初始化角色卡片
        /// </summary>
        public void Initialize(ValueObject.ScriptableObjects.CharacterDataSO characterData)
        {
            _characterData = characterData;

            if (_nameText != null)
                _nameText.text = characterData.characterName;

            if (_iconImage != null)
            {
                _iconImage.sprite = characterData.icon;
                _iconImage.enabled = characterData.icon != null;
            }

            bool isUnlocked = characterData.IsUnlocked();

            if (_statsText != null && isUnlocked)
                _statsText.text = characterData.GetStatsDescription();

            if (_lockOverlay != null)
                _lockOverlay.gameObject.SetActive(!isUnlocked);

            if (_unlockText != null)
            {
                _unlockText.gameObject.SetActive(!isUnlocked);
                if (!isUnlocked)
                    _unlockText.text = characterData.unlockCondition;
            }

            if (_selectButton != null)
            {
                _selectButton.interactable = isUnlocked;
                _selectButton.onClick.RemoveAllListeners();
                _selectButton.onClick.AddListener(OnCardClicked);
            }
        }

        private void OnCardClicked()
        {
            _isSelected = true;
            OnCharacterSelected?.Invoke(_characterData);
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            // 可以在这里添加选中视觉效果
        }
    }
}

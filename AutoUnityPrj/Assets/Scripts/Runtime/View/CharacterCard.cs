using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.View
{
    /// <summary>
    /// 角色卡片 - 显示角色头像的卡片
    /// 鼠标悬停显示详情，点击确认选择
    /// </summary>
    public class CharacterCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI引用")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _lockOverlay;
        [SerializeField] private Image _selectedHighlight;

        private CharacterDataSO _characterData;
        private bool _isSelected;

        // 事件
        public System.Action<CharacterDataSO> OnCharacterSelected;
        public System.Action<CharacterDataSO> OnCharacterHovered;

        /// <summary>
        /// 初始化角色卡片
        /// </summary>
        public void Initialize(CharacterDataSO characterData)
        {
            _characterData = characterData;

            if (_iconImage != null)
            {
                _iconImage.sprite = characterData.Icon;
                _iconImage.enabled = characterData.Icon != null;
            }

            bool isUnlocked = characterData.IsUnlocked();

            if (_lockOverlay != null)
                _lockOverlay.gameObject.SetActive(!isUnlocked);

            if (_selectedHighlight != null)
                _selectedHighlight.gameObject.SetActive(false);

            // 点击事件
            var btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = isUnlocked;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnCardClicked);
            }
        }

        /// <summary>
        /// 鼠标进入 - 悬停显示详情
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"[CharacterCard] 悬停: {_characterData.CharacterName}");
            OnCharacterHovered?.Invoke(_characterData);
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 可以选择是否清除详情显示
        }

        /// <summary>
        /// 点击 - 确认选择
        /// </summary>
        private void OnCardClicked()
        {
            _isSelected = true;

            if (_selectedHighlight != null)
                _selectedHighlight.gameObject.SetActive(true);

            Debug.Log($"[CharacterCard] 确认选择: {_characterData.CharacterName}");
            OnCharacterSelected?.Invoke(_characterData);
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            if (_selectedHighlight != null)
                _selectedHighlight.gameObject.SetActive(selected);
        }

        /// <summary>
        /// 清除选中状态
        /// </summary>
        public void ClearSelection()
        {
            _isSelected = false;
            if (_selectedHighlight != null)
                _selectedHighlight.gameObject.SetActive(false);
        }

        /// <summary>
        /// 获取角色数据
        /// </summary>
        public CharacterDataSO GetCharacterData() => _characterData;
    }
}

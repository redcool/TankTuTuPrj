using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.View
{
    /// <summary>
    /// 难度卡片 - 显示难度图标和名称
    /// 鼠标悬停显示详情,点击确认选择
    /// </summary>
    public class DifficultyCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI引用")]
        [SerializeField] public Image _iconImage;
        [SerializeField] public Text _nameText;
        [SerializeField] public Image _selectedHighlight;
        [SerializeField] public Image _lockedOverlay;

        private DifficultyDataSO _difficultyData;
        private bool _isSelected;

        // 事件
        public System.Action<DifficultyDataSO> OnDifficultySelected;
        public System.Action<DifficultyDataSO> OnDifficultyHovered;

        /// <summary>
        /// 初始化难度卡片
        /// </summary>
        public void Initialize(DifficultyDataSO difficultyData)
        {
            _difficultyData = difficultyData;

            if (_iconImage != null && difficultyData.Icon != null)
            {
                _iconImage.sprite = difficultyData.Icon;
                _iconImage.enabled = true;
            }
            else if (_iconImage != null)
            {
                // 使用颜色占位
                _iconImage.sprite = null;
                _iconImage.enabled = true;
                _iconImage.color = GetDifficultyColor(difficultyData.DifficultyLevel);
            }

            if (_nameText != null)
            {
                _nameText.text = difficultyData.DifficultyName;
            }

            if (_selectedHighlight != null)
                _selectedHighlight.gameObject.SetActive(false);

            if (_lockedOverlay != null)
                _lockedOverlay.gameObject.SetActive(false);

            // 点击事件
            var btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnCardClicked);
            }
        }

        /// <summary>
        /// 根据难度等级获取颜色
        /// </summary>
        private Color GetDifficultyColor(int level)
        {
            switch (level)
            {
                case 0: return Color.gray;        // 新手
                case 1: return Color.green;       // 简单
                case 2: return Color.blue;        // 普通
                case 3: return new Color(1f, 0.5f, 0f); // 困难
                case 4: return Color.red;       // 专家
                case 5: return new Color(0.5f, 0f, 0.5f); // 大师
                case 6: return new Color(1f, 0f, 1f); // 梦魇
                default: return Color.white;
            }
        }

        /// <summary>
        /// 鼠标进入 - 悬停显示详情
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"[DifficultyCardView] 悬停: {_difficultyData.DifficultyName}");
            OnDifficultyHovered?.Invoke(_difficultyData);
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 可选择是否清除详情
        }

        /// <summary>
        /// 点击 - 确认选择
        /// </summary>
        private void OnCardClicked()
        {
            _isSelected = true;

            if (_selectedHighlight != null)
                _selectedHighlight.gameObject.SetActive(true);

            Debug.Log($"[DifficultyCardView] 确认选择难度: {_difficultyData.DifficultyName}");
            OnDifficultySelected?.Invoke(_difficultyData);
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
        /// 获取难度数据
        /// </summary>
        public DifficultyDataSO GetDifficultyData() => _difficultyData;
    }
}
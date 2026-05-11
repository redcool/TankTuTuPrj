using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.View
{
    /// <summary>
    /// 武器卡片 - 显示武器图标
    /// 鼠标悬停显示详情,点击确认选择
    /// </summary>
    public class WeaponCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI引用")]
        [SerializeField] public Image _iconImage;
        [SerializeField] public Image _selectedHighlight;
        [SerializeField] public Image _lockedOverlay;

        private WeaponDataSO _weaponData;
        private bool _isSelected;

        // 事件
        public System.Action<WeaponDataSO> OnWeaponSelected;
        public System.Action<WeaponDataSO> OnWeaponHovered;

        /// <summary>
        /// 初始化武器卡片
        /// </summary>
        public void Initialize(WeaponDataSO weaponData)
        {
            _weaponData = weaponData;

            if (_iconImage != null && weaponData.Icon != null)
            {
                _iconImage.sprite = weaponData.Icon;
                _iconImage.enabled = true;
            }
            else if (_iconImage != null)
            {
                // 使用颜色占位
                _iconImage.sprite = null;
                _iconImage.enabled = true;
                _iconImage.color = GetWeaponTypeColor(weaponData.WeaponType);
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
        /// 根据武器类型获取颜色
        /// </summary>
        private Color GetWeaponTypeColor(Game.Runtime.ValueObject.WeaponType type)
        {
            switch (type)
            {
                case Game.Runtime.ValueObject.WeaponType.MainCannon:
                case Game.Runtime.ValueObject.WeaponType.Cannon:
                    return new Color(1f, 0.4f, 0.4f); // 红色 - 火炮
                case Game.Runtime.ValueObject.WeaponType.Howitzer:
                case Game.Runtime.ValueObject.WeaponType.Rocket:
                    return new Color(1f, 0.6f, 0.2f); // 橙色 - 榴弹/火箭
                case Game.Runtime.ValueObject.WeaponType.Gatling:
                    return new Color(1f, 1f, 0.4f); // 黄色 - 机关炮
                case Game.Runtime.ValueObject.WeaponType.Missile:
                    return new Color(0.4f, 1f, 0.4f); // 绿色 - 导弹
                case Game.Runtime.ValueObject.WeaponType.Tesla:
                    return new Color(0.4f, 0.8f, 1f); // 蓝色 - 电磁
                case Game.Runtime.ValueObject.WeaponType.Laser:
                    return new Color(0.8f, 0.4f, 1f); // 紫色 - 激光
                default:
                    return Color.gray;
            }
        }

        /// <summary>
        /// 鼠标进入 - 悬停显示详情
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"[WeaponCard] 悬停: {_weaponData.WeaponName}");
            OnWeaponHovered?.Invoke(_weaponData);
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

            Debug.Log($"[WeaponCard] 确认选择武器: {_weaponData.WeaponName}");
            OnWeaponSelected?.Invoke(_weaponData);
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
        /// 获取武器数据
        /// </summary>
        public WeaponDataSO GetWeaponData() => _weaponData;
    }
}
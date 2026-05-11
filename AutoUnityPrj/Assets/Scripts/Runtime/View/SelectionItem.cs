using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.View
{
    /// <summary>
    /// 选择项组件 - 挂载在每个可选项上,用于显示高亮框
    /// </summary>
    public class SelectionItem : MonoBehaviour
    {
        [Header("高亮边框")]
        [SerializeField] private Image _selectionFrame;
        
        [Header("选中效果")]
        [SerializeField] private Color _selectedColor = Color.yellow;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private float _scaleWhenSelected = 1.1f;

        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
            SetSelected(false);
        }

        /// <summary>
        /// 设置选中状态
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (_selectionFrame != null)
            {
                _selectionFrame.color = selected ? _selectedColor : _normalColor;
                _selectionFrame.gameObject.SetActive(selected);
            }

            transform.localScale = selected ? _originalScale * _scaleWhenSelected : _originalScale;
        }
    }
}
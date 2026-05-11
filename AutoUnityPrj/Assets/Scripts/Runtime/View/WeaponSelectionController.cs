using UnityEngine;
using UnityEngine.UI;
using Game.Runtime.ValueObject.ScriptableObjects;
using UnityEngine.SceneManagement;

namespace Game.Runtime.View
{
    /// <summary>
    /// 武器选择控制器 - 处理武器选择流程
    /// 玩家移动选择框显示武器详情,点击确认后进入难度选择
    /// </summary>
    public class WeaponSelectionController : SelectionController
    {
        [Header("武器数据")]
        [SerializeField] private WeaponDataSO[] _weaponDataList;

        [Header("输出")]
        [SerializeField] protected SelectionController _nextController;
        [SerializeField] protected GameObject _thisPanel;

        /// <summary>
        /// 设置下一个控制器 (供Editor脚本调用)
        /// </summary>
        public void SetNextController(SelectionController controller)
        {
            _nextController = controller;
        }

        /// <summary>
        /// 设置当前面板 (供Editor脚本调用)
        /// </summary>
        public void SetThisPanel(GameObject panel)
        {
            _thisPanel = panel;
        }

        private WeaponDataSO _selectedWeapon;

        protected override void Start()
        {
            base.Start();
            LoadWeaponData();
            OnCancel += HandleCancel;
        }

        private void LoadWeaponData()
        {
            var loadedWeapons = Resources.LoadAll<WeaponDataSO>("ScriptableObjects/Weapons");
            if (loadedWeapons != null && loadedWeapons.Length > 0)
            {
                _weaponDataList = loadedWeapons;
            }
            else
            {
                Debug.LogWarning("[WeaponSelectionController] 未找到武器数据");
            }
        }

        private void HandleCancel()
        {
            Debug.Log("[WeaponSelectionController] 取消,返回角色选择");
            if (_thisPanel != null)
            {
                _thisPanel.SetActive(false);
            }
            var prevPanel = transform.parent.Find("CharacterSelectionPanel");
            if (prevPanel != null)
            {
                prevPanel.gameObject.SetActive(true);
            }
        }

        protected override void InitializeItems()
        {
            _items.Clear();
            if (_itemsContainer != null)
            {
                foreach (Transform child in _itemsContainer)
                {
                    var selectionItem = child.GetComponent<SelectionItem>();
                    if (selectionItem == null)
                    {
                        selectionItem = child.gameObject.AddComponent<SelectionItem>();
                    }
                    _items.Add(selectionItem);
                }
            }
            Debug.Log($"[WeaponSelectionController] 找到 {_items.Count} 个武器选项");
        }

        protected override void OnSelectionChanged(int index)
        {
            base.OnSelectionChanged(index);

            if (_weaponDataList != null && index >= 0 && index < _weaponDataList.Length)
            {
                _selectedWeapon = _weaponDataList[index];
                if (_detailPanel != null)
                {
                    _detailPanel.SetWeapon(_selectedWeapon);
                    _detailPanel.Show();
                }
                Debug.Log($"[WeaponSelectionController] 选中武器: {_selectedWeapon.WeaponName}");
            }
        }

        protected override void ConfirmSelection()
        {
            if (_selectedWeapon == null)
            {
                Debug.LogWarning("[WeaponSelectionController] 未选择武器");
                return;
            }

            PlayerPrefs.SetString("SelectedWeaponId", _selectedWeapon.name);
            Debug.Log($"[WeaponSelectionController] 确认武器: {_selectedWeapon.WeaponName}, 进入难度选择");

            if (_thisPanel != null)
            {
                _thisPanel.SetActive(false);
            }

            if (_nextController != null)
            {
                var nextPanel = _nextController.GetComponentInParent<Canvas>();
                if (nextPanel != null)
                {
                    nextPanel.gameObject.SetActive(true);
                    _nextController.SetCurrentIndex(0);
                }
            }
            else
            {
                SceneManager.LoadScene("Level_0");
            }
        }
    }
}
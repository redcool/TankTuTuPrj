using UnityEngine;
using UnityEngine.UI;
using Game.Runtime.ValueObject.ScriptableObjects;
using Game.Runtime.View;
using DifficultyDataSO = Game.Runtime.ValueObject.ScriptableObjects.DifficultyDataSO;

namespace Game.Runtime.View
{
    /// <summary>
    /// 玩家选择流程控制器 - 统一管理所有选择界面的显示/隐藏
    /// 通过事件系统与各 View 通信，实现解耦
    /// 负责协调: 开始菜单 → 角色选择 → 武器选择 → 难度选择 → 开始游戏
    /// </summary>
    public class PlayerSelectionControl : MonoBehaviour
    {
        [Header("Canvas 引用")]
        [SerializeField] private Canvas _startMenuCanvas;
        [SerializeField] private Canvas _characterSelectCanvas;
        [SerializeField] private Canvas _weaponSelectionCanvas;
        [SerializeField] private Canvas _difficultySelectionCanvas;
        [SerializeField] private Canvas _selectionCanvas; // 详情面板（屏幕上部）

        [Header("View 脚本")]
        [SerializeField] private CharacterSelectView _characterSelectView;
        [SerializeField] private WeaponSelectionView _weaponSelectionView;
        [SerializeField] private DifficultySelectionView _difficultySelectionView;
        [SerializeField] private PlayerSelectedDetailView _selectionDetailView;

        [Header("输入系统")]
        [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;

        // 当前选择状态
        private CharacterDataSO _selectedCharacter;
        private WeaponDataSO _selectedWeapon;
        private int _selectedDifficulty = 1;

        // 当前流程阶段
        private enum SelectionPhase
        {
            StartMenu,
            CharacterSelect,
            WeaponSelect,
            DifficultySelect,
            GameStart
        }
        private SelectionPhase _currentPhase = SelectionPhase.StartMenu;

        private void Awake()
        {
            // 自动获取 Input 组件
            if (_playerInput == null)
            {
                var inputObj = GameObject.Find("Input");
                if (inputObj != null)
                {
                    _playerInput = inputObj.GetComponent<UnityEngine.InputSystem.PlayerInput>();
                }
            }

            // 自动查找各 Canvas
            AutoFindCanvases();

            // 订阅事件
            SubscribeEvents();

            // 初始状态：只显示开始菜单
            ShowStartMenu();
        }

        private void OnDestroy()
        {
            // 取消事件订阅
            UnsubscribeEvents();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        private void SubscribeEvents()
        {
            var eventManager = SelectionEventManager.Instance;
            eventManager.Subscribe(SelectionEventType.StartGame, OnStartGameEvent);
            eventManager.Subscribe(SelectionEventType.CharacterSelected, OnCharacterSelectedEvent);
            eventManager.Subscribe(SelectionEventType.WeaponSelected, OnWeaponSelectedEvent);
            eventManager.Subscribe(SelectionEventType.DifficultySelected, OnDifficultySelectedEvent);
            eventManager.Subscribe(SelectionEventType.CharacterHovered, OnCharacterHoveredEvent);
            eventManager.Subscribe(SelectionEventType.WeaponHovered, OnWeaponHoveredEvent);
            eventManager.Subscribe(SelectionEventType.DifficultyHovered, OnDifficultyHoveredEvent);
            eventManager.Subscribe(SelectionEventType.BackToPrevious, OnBackToPreviousEvent);
        }

        /// <summary>
        /// 取消事件订阅
        /// </summary>
        private void UnsubscribeEvents()
        {
            var eventManager = SelectionEventManager.Instance;
            eventManager.Unsubscribe(SelectionEventType.StartGame, OnStartGameEvent);
            eventManager.Unsubscribe(SelectionEventType.CharacterSelected, OnCharacterSelectedEvent);
            eventManager.Unsubscribe(SelectionEventType.WeaponSelected, OnWeaponSelectedEvent);
            eventManager.Unsubscribe(SelectionEventType.DifficultySelected, OnDifficultySelectedEvent);
            eventManager.Unsubscribe(SelectionEventType.CharacterHovered, OnCharacterHoveredEvent);
            eventManager.Unsubscribe(SelectionEventType.WeaponHovered, OnWeaponHoveredEvent);
            eventManager.Unsubscribe(SelectionEventType.DifficultyHovered, OnDifficultyHoveredEvent);
            eventManager.Unsubscribe(SelectionEventType.BackToPrevious, OnBackToPreviousEvent);
        }

        #region 事件处理

        private void OnStartGameEvent(SelectionEventData data)
        {
            ShowCharacterSelect();
        }

        private void OnCharacterSelectedEvent(SelectionEventData data)
        {
            if (data.Data is CharacterDataSO character)
            {
                _selectedCharacter = character;
                PlayerPrefs.SetString("SelectedCharacterId", character.CharacterName);

                if (_selectionDetailView != null)
                {
                    _selectionDetailView.SetCharacter(character);
                }

                ShowWeaponSelect();
            }
        }

        private void OnWeaponSelectedEvent(SelectionEventData data)
        {
            if (data.Data is WeaponDataSO weapon)
            {
                _selectedWeapon = weapon;
                PlayerPrefs.SetString("SelectedWeaponId", weapon.WeaponId);

                if (_selectionDetailView != null)
                {
                    _selectionDetailView.SetWeapon(weapon);
                }

                ShowDifficultySelect();
            }
        }

        private void OnDifficultySelectedEvent(SelectionEventData data)
        {
            if (data.Data is int difficulty)
            {
                _selectedDifficulty = difficulty;
                PlayerPrefs.SetInt("SelectedDifficulty", difficulty);

                Debug.Log($"[PlayerSelectionControl] 开始游戏 - 角色: {_selectedCharacter?.CharacterName}, 武器: {_selectedWeapon?.WeaponName}, 难度: {difficulty}");

                UnityEngine.SceneManagement.SceneManager.LoadScene("Level_0");
            }
        }

        private void OnCharacterHoveredEvent(SelectionEventData data)
        {
            if (data.Data is CharacterDataSO character && _selectionDetailView != null)
            {
                _selectionDetailView.SetCharacter(character);
                _selectionDetailView.Show();
            }
        }

        private void OnWeaponHoveredEvent(SelectionEventData data)
        {
            if (data.Data is WeaponDataSO weapon && _selectionDetailView != null)
            {
                _selectionDetailView.SetWeapon(weapon);
                _selectionDetailView.Show();
            }
        }

        private void OnDifficultyHoveredEvent(SelectionEventData data)
        {
            if (data.Data is DifficultyDataSO difficulty && _selectionDetailView != null)
            {
                _selectionDetailView.SetDifficulty(difficulty.DifficultyLevel);
                _selectionDetailView.Show();
            }
        }

        private void OnBackToPreviousEvent(SelectionEventData data)
        {
            switch (_currentPhase)
            {
                case SelectionPhase.CharacterSelect:
                    ShowStartMenu();
                    break;
                case SelectionPhase.WeaponSelect:
                    ShowCharacterSelect();
                    break;
                case SelectionPhase.DifficultySelect:
                    ShowWeaponSelect();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 自动查找所有 Canvas
        /// </summary>
        private void AutoFindCanvases()
        {
            if (_startMenuCanvas == null) _startMenuCanvas = transform.Find("StartMenuCanvas")?.GetComponent<Canvas>();
            if (_characterSelectCanvas == null) _characterSelectCanvas = transform.Find("CharacterSelectCanvas")?.GetComponent<Canvas>();
            if (_weaponSelectionCanvas == null) _weaponSelectionCanvas = transform.Find("WeaponSelectionCanvas")?.GetComponent<Canvas>();
            if (_difficultySelectionCanvas == null) _difficultySelectionCanvas = transform.Find("DifficultySelectionCanvas")?.GetComponent<Canvas>();
            if (_selectionCanvas == null) _selectionCanvas = transform.Find("SelectionCanvas")?.GetComponent<Canvas>();

            // 获取 View 脚本
            if (_characterSelectView == null && _characterSelectCanvas != null) _characterSelectView = _characterSelectCanvas.GetComponent<CharacterSelectView>();
            if (_weaponSelectionView == null && _weaponSelectionCanvas != null) _weaponSelectionView = _weaponSelectionCanvas.GetComponent<WeaponSelectionView>();
            if (_difficultySelectionView == null && _difficultySelectionCanvas != null) _difficultySelectionView = _difficultySelectionCanvas.GetComponent<DifficultySelectionView>();
            if (_selectionDetailView == null && _selectionCanvas != null) _selectionDetailView = _selectionCanvas.GetComponent<PlayerSelectedDetailView>();
        }

        #region 界面显示控制

        /// <summary>
        /// 显示开始菜单
        /// </summary>
        public void ShowStartMenu()
        {
            HideAllCanvases();
            if (_startMenuCanvas != null) _startMenuCanvas.gameObject.SetActive(true);
            _currentPhase = SelectionPhase.StartMenu;
            Debug.Log("[PlayerSelectionControl] 显示开始菜单");
        }

        /// <summary>
        /// 显示角色选择界面
        /// </summary>
        public void ShowCharacterSelect()
        {
            HideAllCanvases();
            if (_characterSelectCanvas != null) _characterSelectCanvas.gameObject.SetActive(true);
            if (_selectionCanvas != null) _selectionCanvas.gameObject.SetActive(true);
            _currentPhase = SelectionPhase.CharacterSelect;
            Debug.Log("[PlayerSelectionControl] 显示角色选择");
        }

        /// <summary>
        /// 显示武器选择界面
        /// </summary>
        public void ShowWeaponSelect()
        {
            HideAllCanvases();
            // 只显示武器选择界面和详情面板，关闭角色选择界面
            if (_weaponSelectionCanvas != null) _weaponSelectionCanvas.gameObject.SetActive(true);
            if (_selectionCanvas != null) _selectionCanvas.gameObject.SetActive(true);
            _currentPhase = SelectionPhase.WeaponSelect;
            Debug.Log("[PlayerSelectionControl] 显示武器选择");
        }

        /// <summary>
        /// 显示难度选择界面
        /// </summary>
        public void ShowDifficultySelect()
        {
            HideAllCanvases();
            // 只显示难度选择界面和详情面板，武器选择需要关闭
            if (_difficultySelectionCanvas != null) _difficultySelectionCanvas.gameObject.SetActive(true);
            if (_selectionCanvas != null) _selectionCanvas.gameObject.SetActive(true);
            _currentPhase = SelectionPhase.DifficultySelect;
            Debug.Log("[PlayerSelectionControl] 显示难度选择");
        }

        /// <summary>
        /// 隐藏所有 Canvas
        /// </summary>
        private void HideAllCanvases()
        {
            if (_startMenuCanvas != null) _startMenuCanvas.gameObject.SetActive(false);
            if (_characterSelectCanvas != null) _characterSelectCanvas.gameObject.SetActive(false);
            if (_weaponSelectionCanvas != null) _weaponSelectionCanvas.gameObject.SetActive(false);
            if (_difficultySelectionCanvas != null) _difficultySelectionCanvas.gameObject.SetActive(false);
            if (_selectionCanvas != null) _selectionCanvas.gameObject.SetActive(false);
        }

        #endregion

        #region Getter

        public CharacterDataSO SelectedCharacter => _selectedCharacter;
        public WeaponDataSO SelectedWeapon => _selectedWeapon;
        public int SelectedDifficulty => _selectedDifficulty;

        #endregion
    }
}
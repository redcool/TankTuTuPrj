using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Runtime.UI
{
    /// <summary>
    /// UI 状态枚举
    /// </summary>
    public enum UIState
    {
        None,
        StartMenu,
        CharacterSelect,
        WeaponSelect,
        DifficultySelect,
        BattleHUD,
        Result,
        Shop,
        Gacha,
        Settings
    }

    /// <summary>
    /// UI 流程管理器 — 单 UIDocument 驱动
    /// 纯单例，各 Presenters 首次访问时自动 init
    /// 支持导航栈（前进/后退）
    /// </summary>
    public class UIFlowManager
    {
        private static UIFlowManager _instance;
        public static UIFlowManager Instance =>
            _instance ?? (_instance = new UIFlowManager());

        private VisualElement _root;
        private readonly Dictionary<UIState, VisualElement> _panels =
            new Dictionary<UIState, VisualElement>();
        private UIState _currentState = UIState.None;
        private bool _initialized = false;

        /// <summary>
        /// 导航栈 — 记录每步从哪里来
        /// </summary>
        private readonly Stack<UIState> _navStack = new Stack<UIState>();

        public UIState CurrentState => _currentState;
        /// <summary>获取根 VisualElement</summary>
        public VisualElement Root => _root;

        /// <summary>
        /// 状态切换事件 (oldState, newState)
        /// </summary>
        public static event Action<UIState, UIState> OnStateChanged;

        /// <summary>
        /// 初始化（自动查找场景中第一个 UIDocument）
        /// </summary>
        public void Initialize(UIDocument doc = null)
        {
            if (_initialized) return;

            if (doc == null)
                doc = UnityEngine.Object.FindObjectOfType<UIDocument>();

            if (doc == null || doc.rootVisualElement == null)
            {
                Debug.LogWarning("[UIFlowManager] 未找到 UIDocument，延迟初始化");
                return;
            }

            _root = doc.rootVisualElement;
            BuildPanelMap();
            _initialized = true;

            Show(UIState.StartMenu);
        }

        /// <summary>
        /// 重建 Panel 映射（外部可调用以刷新）
        /// </summary>
        public void BuildPanelMap()
        {
            _panels.Clear();
            if (_root == null) return;

            RegisterPanel(UIState.StartMenu, "panel-start-menu");
            RegisterPanel(UIState.CharacterSelect, "panel-character-select");
            RegisterPanel(UIState.WeaponSelect, "panel-weapon-select");
            RegisterPanel(UIState.DifficultySelect, "panel-difficulty-select");
            RegisterPanel(UIState.BattleHUD, "panel-battle-hud");
            RegisterPanel(UIState.Result, "panel-result");
            RegisterPanel(UIState.Shop, "panel-shop");
            RegisterPanel(UIState.Gacha, "panel-gacha");
            RegisterPanel(UIState.Settings, "panel-settings");
        }

        private void RegisterPanel(UIState state, string panelName)
        {
            var panel = _root.Q<VisualElement>(panelName);
            if (panel == null)
            {
                Debug.LogWarning($"[UIFlowManager] 面板 '{panelName}' 未在 UXML 中找到");
                return;
            }

            _panels[state] = panel;
            // 默认只显示 StartMenu，其他隐藏
            if (state != UIState.StartMenu)
                panel.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 显示指定 UI，隐藏其他
        /// </summary>
        public void Show(UIState newState)
        {
            if (!_initialized)
            {
                Debug.LogWarning("[UIFlowManager] 未初始化，跳过 Show");
                return;
            }
            if (newState == _currentState) return;

            var oldState = _currentState;
            _currentState = newState;

            // 隐藏所有面板
            foreach (var kvp in _panels)
            {
                if (kvp.Value != null)
                    kvp.Value.style.display = DisplayStyle.None;
            }

            // 显示目标面板
            if (_panels.TryGetValue(newState, out var target) && target != null)
                target.style.display = DisplayStyle.Flex;
            else
                Debug.LogWarning($"[UIFlowManager] 面板 '{newState}' 未注册");

            OnStateChanged?.Invoke(oldState, newState);
            Debug.Log($"[UIFlowManager] {oldState} → {newState}");
        }

        /// <summary>
        /// 前进到下一步（压入导航栈）
        /// </summary>
        private void NavigateTo(UIState nextState)
        {
            if (_currentState != UIState.None)
                _navStack.Push(_currentState);
            Show(nextState);
        }

        /// <summary>
        /// 返回上一步（弹出导航栈）
        /// </summary>
        private void NavigateBack()
        {
            if (_navStack.Count > 0)
            {
                var previousState = _navStack.Pop();
                Show(previousState);
            }
            else
            {
                Debug.Log("[UIFlowManager] 导航栈为空，返回主菜单");
                Show(UIState.StartMenu);
            }
        }

        // --- 快捷导航 ---
        public void GoToStartMenu()
        {
            _navStack.Clear();
            Show(UIState.StartMenu);
        }

        public void GoToCharacterSelect()
        {
            NavigateTo(UIState.CharacterSelect);
        }

        public void GoToWeaponSelect()
        {
            NavigateTo(UIState.WeaponSelect);
        }

        public void GoToDifficultySelect()
        {
            NavigateTo(UIState.DifficultySelect);
        }

        public void GoToSettings()
        {
            NavigateTo(UIState.Settings);
        }

        public void GoToResult(bool victory)
        {
            NavigateTo(UIState.Result);
        }

        public void GoToShop()
        {
            NavigateTo(UIState.Shop);
        }

        public void GoToGacha()
        {
            NavigateTo(UIState.Gacha);
        }

        /// <summary>
        /// 返回上一步
        /// </summary>
        public void GoBack()
        {
            NavigateBack();
        }

        public void StartBattle()
        {
            Debug.Log("[UIFlowManager] 开始战斗!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level_0");
        }

        public void ClearAll()
        {
            _panels.Clear();
            _navStack.Clear();
            _currentState = UIState.None;
            _initialized = false;
        }
    }
}

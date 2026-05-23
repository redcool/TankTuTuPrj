using UnityEngine;
using UnityEngine.UIElements;
using Game.Runtime.Controller;

namespace Game.Runtime.UI
{
    /// <summary>
    /// 开始菜单 Presenter — 单 UIDocument 面板
    /// </summary>
    public class StartMenuPresenter : MonoBehaviour
    {
        private UIFlowManager _flow;
        private VisualElement _root;

        private Button _btnContinue;
        private VisualElement _continueSeparator;
        private Button _btnNewGame;
        private Button _btnSettings;
        private Button _btnQuit;

        private SaveManager _saveManager;

        private void Awake()
        {
            _saveManager = FindObjectOfType<SaveManager>();
        }

        private void Start()
        {
            _flow = UIFlowManager.Instance;
            _flow.Initialize();
            if (_flow.CurrentState == UIState.None) return;

            _root = _flow.Root;

            _btnContinue = _root.Q<Button>("btn-continue");
            _continueSeparator = _root.Q<VisualElement>("continue-separator");
            _btnNewGame = _root.Q<Button>("btn-new-game");
            _btnSettings = _root.Q<Button>("btn-settings");
            _btnQuit = _root.Q<Button>("btn-quit");

            if (_btnContinue != null)
                _btnContinue.clicked += OnContinueClicked;
            if (_btnNewGame != null)
                _btnNewGame.clicked += OnNewGameClicked;
            if (_btnSettings != null)
                _btnSettings.clicked += OnSettingsClicked;
            if (_btnQuit != null)
                _btnQuit.clicked += OnQuitClicked;

            RefreshContinueButton();
        }

        private void OnDestroy()
        {
            if (_btnContinue != null)
                _btnContinue.clicked -= OnContinueClicked;
            if (_btnNewGame != null)
                _btnNewGame.clicked -= OnNewGameClicked;
            if (_btnSettings != null)
                _btnSettings.clicked -= OnSettingsClicked;
            if (_btnQuit != null)
                _btnQuit.clicked -= OnQuitClicked;
        }

        private void RefreshContinueButton()
        {
            bool canContinue = false;

            if (_saveManager != null && _saveManager.HasSave())
            {
                var save = _saveManager.CurrentSave;
                if (save != null)
                    canContinue = save.HasInProgressBattle;
            }

            bool show = canContinue;
            if (_btnContinue != null)
                _btnContinue.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            if (_continueSeparator != null)
                _continueSeparator.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnContinueClicked()
        {
            Debug.Log("[StartMenu] 继续战斗");
            _flow.GoToDifficultySelect();
        }

        private void OnNewGameClicked()
        {
            Debug.Log("[StartMenu] 新游戏");
            _flow.GoToCharacterSelect();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("[StartMenu] 设置");
            _flow.GoToSettings();
        }

        private void OnQuitClicked()
        {
            Debug.Log("[StartMenu] 退出");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}


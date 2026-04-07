using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.View
{
    /// <summary>
    /// 开始界面 - 开始游戏、继续游戏按钮
    /// 点击StartButton后隐藏自己，显示CharacterSelectView
    /// 作者：AI
    /// 最后修改时间：2026-04-07
    /// </summary>
    public class StartView : MonoBehaviour
    {
        [Header("UI路径")]
        [SerializeField] private string _startButtonPath = "ButtonPanel/StartButton";
        [SerializeField] private string _continueButtonPath = "ButtonPanel/ContinueButton";

        [Header("UI引用")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _continueButton;

        [Header("角色选择界面")]
        [SerializeField] private CharacterSelectView _characterSelectView;

        // 回调
        public delegate void StartAction();
        public event StartAction OnStartGame;
        public event StartAction OnContinueGame;

        private void Awake()
        {
            FindUIElements();

            if (_startButton != null)
            {
                _startButton.onClick.AddListener(OnStartClicked);
            }
            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        /// <summary>
        /// 按路径查找UI元素
        /// </summary>
        private void FindUIElements()
        {
            if (_startButton == null && !string.IsNullOrEmpty(_startButtonPath))
            {
                var btnTransform = transform.Find(_startButtonPath);
                if (btnTransform != null)
                    _startButton = btnTransform.GetComponent<Button>();
            }
            if (_continueButton == null && !string.IsNullOrEmpty(_continueButtonPath))
            {
                var btnTransform = transform.Find(_continueButtonPath);
                if (btnTransform != null)
                    _continueButton = btnTransform.GetComponent<Button>();
            }

            // 自动查找角色选择界面
            if (_characterSelectView == null)
            {
                _characterSelectView = FindObjectOfType<CharacterSelectView>();
            }
        }

        private void Start()
        {
            // 检查是否有存档
            UpdateContinueButton();
        }

        /// <summary>
        /// 更新继续按钮状态
        /// </summary>
        private void UpdateContinueButton()
        {
            if (_continueButton != null)
            {
                var saveManager = FindObjectOfType<Game.Runtime.Controller.SaveManager>();
                _continueButton.interactable = saveManager != null && saveManager.HasSave();
            }
        }

        private void OnStartClicked()
        {
            // 隐藏开始菜单
            HideStartMenu();
            // 显示角色选择界面
            if (_characterSelectView != null)
            {
                _characterSelectView.Show();
            }
            OnStartGame?.Invoke();
        }

        private void OnContinueClicked()
        {
            OnContinueGame?.Invoke();
            LoadLevel("Level_0");
        }

        /// <summary>
        /// 隐藏开始菜单（隐藏自身Canvas或父级）
        /// </summary>
        private void HideStartMenu()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                canvas.gameObject.SetActive(false);
            else
                gameObject.SetActive(false);
        }

        /// <summary>
        /// 加载关卡场景
        /// </summary>
        private void LoadLevel(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}

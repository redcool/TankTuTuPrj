using UnityEngine;
using UnityEngine.UI;
using Game.Runtime.View;

namespace Game.Runtime.View
{
    /// <summary>
    /// 开始界面 - 开始游戏、继续游戏按钮
    /// 通过事件系统与 PlayerSelectionControl 通信
    /// 作者：AI
    /// 最后修改时间：2026-04-10
    /// </summary>
    public class StartView : MonoBehaviour
    {
        [Header("UI路径")]
        [SerializeField] private string _startButtonPath = "ButtonPanel/StartButton";
        [SerializeField] private string _continueButtonPath = "ButtonPanel/ContinueButton";

        [Header("UI引用")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _continueButton;

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

        private void OnDestroy()
        {
            // 取消事件订阅（如果需要监听返回事件）
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

        /// <summary>
        /// 开始游戏按钮点击 - 发布事件
        /// </summary>
        private void OnStartClicked()
        {
            // 发布事件，让 PlayerSelectionControl 订阅并处理
            SelectionEventManager.Instance.Publish(SelectionEventType.StartGame);
            OnStartGame?.Invoke();
        }

        private void OnContinueClicked()
        {
            OnContinueGame?.Invoke();
            LoadLevel("Level_0");
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
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Game.Runtime.ValueObject.ScriptableObjects;
using UnityEngine.InputSystem;

namespace Game.Runtime.View
{
    /// <summary>
    /// 选择控制器 - 通用选择系统,支持键盘/手柄/鼠标
    /// 使用 Unity Input System 处理输入
    /// </summary>
    public class SelectionController : MonoBehaviour
    {
        [Header("选项容器")]
        [SerializeField] protected Transform _itemsContainer;
        
        // 公共访问器 (供Editor脚本和子类使用)
        public Transform ItemsContainer => _itemsContainer;
        
        [Header("选择框")]
        [SerializeField] private RectTransform _selectionFrame;
        
        [Header("输入设置")]
        [SerializeField] private float _inputDeadzone = 0.5f;
        [SerializeField] private float _inputHoldDelay = 0.2f;
        
        [Header("详情面板")]
        [SerializeField] protected PlayerSelectedDetailView _detailPanel;
        
        // 公共访问器 (供Editor脚本和子类使用)
        public PlayerSelectedDetailView DetailPanel => _detailPanel;

        // 当前选项列表
        protected List<SelectionItem> _items = new List<SelectionItem>();
        protected int _currentIndex = 0;
        
        // 输入状态
        protected Vector2 _lastInput;
        protected float _inputHoldTimer;
        protected bool _isHolding;

        // 输入系统引用
        private PlayerInput _playerInput;

        // 事件
        public Action<int> OnItemSelected;
        public Action OnCancel;

        protected virtual void Awake()
        {
            // 获取 PlayerInput 组件
            var inputObj = GameObject.Find("Input");
            if (inputObj != null)
            {
                _playerInput = inputObj.GetComponent<PlayerInput>();
            }
        }

        protected virtual void Start()
        {
            InitializeItems();
            if (_items.Count > 0)
            {
                UpdateSelection(0);
            }
        }

        /// <summary>
        /// 处理输入 - 使用 Input System
        /// </summary>
        protected virtual void HandleInput()
        {
            Vector2 input = Vector2.zero;

            // 使用 Input System 读取输入
            // 如果有 PlayerInput 组件，使用它；否则回退到键盘
            if (_playerInput != null)
            {
                // 通过当前设备获取输入
                input = GetInputFromPlayerInput();
            }
            else
            {
                // 回退到键盘输入
                input = GetKeyboardInput();
            }

            if (input.magnitude > _inputDeadzone)
            {
                if (input != _lastInput || !_isHolding)
                {
                    // 新输入 - 立即移动
                    MoveSelection(input);
                    _isHolding = true;
                    _inputHoldTimer = 0;
                    _lastInput = input;
                }
                else
                {
                    // 长按 - 延迟后连续移动
                    _inputHoldTimer += Time.deltaTime;
                    if (_inputHoldTimer > _inputHoldDelay)
                    {
                        _inputHoldTimer = 0;
                        MoveSelection(input);
                    }
                }
            }
            else
            {
                _isHolding = false;
                _lastInput = Vector2.zero;
            }

            // 确认输入 (Space 或鼠标左键)
            if (Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0) || (Keyboard.current != null && Keyboard.current[Key.Space].wasPressedThisFrame))
            {
                ConfirmSelection();
            }

            // 取消输入 (Escape 或 B 按钮)
            if (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape) || (Keyboard.current != null && Keyboard.current[Key.Escape].wasPressedThisFrame))
            {
                OnCancel?.Invoke();
            }
        }

        /// <summary>
        /// 从 PlayerInput 获取输入
        /// </summary>
        private Vector2 GetInputFromPlayerInput()
        {
            Vector2 input = Vector2.zero;
            var keyboard = Keyboard.current;

            // 键盘输入
            if (keyboard != null)
            {
                if (keyboard[Key.UpArrow].wasPressedThisFrame || keyboard[Key.W].wasPressedThisFrame)
                    input.y = 1;
                if (keyboard[Key.DownArrow].wasPressedThisFrame || keyboard[Key.S].wasPressedThisFrame)
                    input.y = -1;
                if (keyboard[Key.LeftArrow].wasPressedThisFrame || keyboard[Key.A].wasPressedThisFrame)
                    input.x = -1;
                if (keyboard[Key.RightArrow].wasPressedThisFrame || keyboard[Key.D].wasPressedThisFrame)
                    input.x = 1;
            }

            // 手柄输入
            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                Vector2 leftStick = gamepad.leftStick.ReadValue();
                if (leftStick.magnitude > input.magnitude)
                {
                    input = leftStick;
                }
            }

            return input;
        }

        /// <summary>
        /// 回退到键盘输入 (旧版 Input Manager)
        /// </summary>
        private Vector2 GetKeyboardInput()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            return new Vector2(h, v);
        }

        /// <summary>
        /// 初始化选项列表
        /// </summary>
        protected virtual void InitializeItems()
        {
            _items.Clear();
            if (_itemsContainer != null)
            {
                foreach (Transform child in _itemsContainer)
                {
                    var item = child.GetComponent<SelectionItem>();
                    if (item != null)
                    {
                        _items.Add(item);
                    }
                }
            }
            Debug.Log($"[SelectionController] 初始化了 {_items.Count} 个选项");
        }

        /// <summary>
        /// 移动选择
        /// </summary>
        protected virtual void MoveSelection(Vector2 input)
        {
            if (_items.Count == 0) return;

            int newIndex = _currentIndex;

            // 水平移动 - 左右
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > 0)
                    newIndex = Mathf.Min(_currentIndex + 1, _items.Count - 1);
                else
                    newIndex = Mathf.Max(_currentIndex - 1, 0);
            }
            // 垂直移动 - 上下
            else
            {
                int columnCount = GetColumnCount();
                if (columnCount > 0)
                {
                    if (input.y > 0)
                        newIndex = Mathf.Max(_currentIndex - columnCount, 0);
                    else
                        newIndex = Mathf.Min(_currentIndex + columnCount, _items.Count - 1);
                }
            }

            if (newIndex != _currentIndex)
            {
                UpdateSelection(newIndex);
            }
        }

        /// <summary>
        /// 获取列数 (用于网格布局)
        /// </summary>
        protected virtual int GetColumnCount()
        {
            return 1;
        }

        /// <summary>
        /// 更新选择状态
        /// </summary>
        protected virtual void UpdateSelection(int index)
        {
            _currentIndex = index;

            // 更新所有选项的高亮状态
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].SetSelected(i == _currentIndex);
            }

            // 更新选择框位置
            if (_selectionFrame != null && _items.Count > _currentIndex && _items[_currentIndex] != null)
            {
                RectTransform itemRect = _items[_currentIndex].GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    _selectionFrame.position = itemRect.position;
                    _selectionFrame.sizeDelta = itemRect.sizeDelta + new Vector2(20, 20);
                }
            }

            // 更新详情面板
            OnSelectionChanged(_currentIndex);
        }

        /// <summary>
        /// 当选择改变时调用 - 子类重写
        /// </summary>
        protected virtual void OnSelectionChanged(int index)
        {
        }

        /// <summary>
        /// 确认选择
        /// </summary>
        protected virtual void ConfirmSelection()
        {
            if (_items.Count > 0 && _currentIndex >= 0 && _currentIndex < _items.Count)
            {
                OnItemSelected?.Invoke(_currentIndex);
                Debug.Log($"[SelectionController] 确认选择: {_currentIndex}");
            }
        }

        /// <summary>
        /// 设置当前选项 (供外部调用)
        /// </summary>
        public void SetCurrentIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                UpdateSelection(index);
            }
        }

        /// <summary>
        /// 获取当前索引
        /// </summary>
        public int GetCurrentIndex() => _currentIndex;
    }
}
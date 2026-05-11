using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game.Runtime.Controller
{
    /// <summary>
    /// UI管理器 - 统一管理所有界面的加载、显示和切换
    /// 使用 Resources.Load 动态加载 Prefab
    /// 作者：AI
    /// 最后修改时间：2026-04-09
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        // 单例
        public static UIManager Instance { get; private set; }

        [Header("UI Prefab路径")]
        [SerializeField] private string _uiPrefabPath = "Prefabs/UI/";

        [Header("预加载的界面")]
        [SerializeField] private string[] _preloadCanvases = new string[]
        {
            "StartMenuCanvas",
            "CharacterSelectCanvas",
            "SelectionCanvas"
        };

        // 界面缓存
        private Dictionary<string, GameObject> _loadedCanvases = new Dictionary<string, GameObject>();
        private GameObject _currentCanvas;

        // 私有字段
        private Canvas _mainCanvas;

        private void Awake()
        {
            // 单例模式
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 创建主Canvas
            CreateMainCanvas();

            // 预加载界面
            PreloadCanvases();
        }

        /// <summary>
        /// 创建主Canvas（如果不存在）
        /// </summary>
        private void CreateMainCanvas()
        {
            var existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas != null)
            {
                _mainCanvas = existingCanvas;
                return;
            }

            GameObject canvasObj = new GameObject("MainCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            _mainCanvas = canvas;
            Debug.Log("[UIManager] 创建主Canvas");
        }

        /// <summary>
        /// 预加载常用界面
        /// </summary>
        private void PreloadCanvases()
        {
            foreach (var canvasName in _preloadCanvases)
            {
                LoadCanvas(canvasName, false);
            }
            Debug.Log($"[UIManager] 预加载 {_preloadCanvases.Length} 个界面");
        }

        /// <summary>
        /// 加载界面（如果已缓存则返回缓存）
        /// </summary>
        public GameObject LoadCanvas(string canvasName, bool showImmediately = true)
        {
            if (_loadedCanvases.ContainsKey(canvasName))
            {
                if (showImmediately)
                {
                    ShowCanvas(canvasName);
                }
                return _loadedCanvases[canvasName];
            }

            // 从Resources加载
            string fullPath = _uiPrefabPath + canvasName;
            GameObject prefab = Resources.Load<GameObject>(fullPath);

            if (prefab == null)
            {
                Debug.LogError($"[UIManager] 无法加载界面: {fullPath}");
                return null;
            }

            // 实例化
            GameObject canvasObj = Instantiate(prefab, _mainCanvas.transform);
            canvasObj.name = canvasName;

            // 缓存
            _loadedCanvases[canvasName] = canvasObj;

            if (showImmediately)
            {
                ShowCanvas(canvasName);
            }
            else
            {
                canvasObj.SetActive(false);
            }

            Debug.Log($"[UIManager] 加载界面: {canvasName}");
            return canvasObj;
        }

        /// <summary>
        /// 显示指定界面，隐藏当前界面
        /// </summary>
        public void ShowCanvas(string canvasName)
        {
            // 隐藏当前界面
            if (_currentCanvas != null)
            {
                _currentCanvas.SetActive(false);
            }

            // 加载并显示新界面
            if (!_loadedCanvases.ContainsKey(canvasName))
            {
                LoadCanvas(canvasName, true);
                return;
            }

            GameObject canvas = _loadedCanvases[canvasName];
            canvas.SetActive(true);
            _currentCanvas = canvas;

            Debug.Log($"[UIManager] 显示界面: {canvasName}");
        }

        /// <summary>
        /// 隐藏指定界面
        /// </summary>
        public void HideCanvas(string canvasName)
        {
            if (_loadedCanvases.ContainsKey(canvasName))
            {
                _loadedCanvases[canvasName].SetActive(false);
                if (_currentCanvas == _loadedCanvases[canvasName])
                {
                    _currentCanvas = null;
                }
            }
        }

        /// <summary>
        /// 卸载指定界面（从缓存中移除）
        /// </summary>
        public void UnloadCanvas(string canvasName)
        {
            if (_loadedCanvases.ContainsKey(canvasName))
            {
                Destroy(_loadedCanvases[canvasName]);
                _loadedCanvases.Remove(canvasName);
                Debug.Log($"[UIManager] 卸载界面: {canvasName}");
            }
        }

        /// <summary>
        /// 获取已加载的界面
        /// </summary>
        public GameObject GetCanvas(string canvasName)
        {
            return _loadedCanvases.ContainsKey(canvasName) ? _loadedCanvases[canvasName] : null;
        }

        /// <summary>
        /// 检查界面是否已加载
        /// </summary>
        public bool IsCanvasLoaded(string canvasName)
        {
            return _loadedCanvases.ContainsKey(canvasName);
        }

        /// <summary>
        /// 清理所有界面（切换场景时调用）
        /// </summary>
        public void ClearAllCanvases()
        {
            foreach (var kvp in _loadedCanvases)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
            }
            _loadedCanvases.Clear();
            _currentCanvas = null;
            Debug.Log("[UIManager] 清理所有界面");
        }

        /// <summary>
        /// 获取当前显示的界面
        /// </summary>
        public GameObject GetCurrentCanvas()
        {
            return _currentCanvas;
        }
    }
}
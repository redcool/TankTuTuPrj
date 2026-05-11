using System;
using System.Collections.Generic;
using UnityEngine;
using ReactUnity;
using ReactUnity.UGUI;

namespace ReactUnityTemplate
{
    /// <summary>
    /// Unity 与 ReactUI 之间的数据桥接
    /// 
    /// 使用方式：
    /// 1. 在 Unity 场景中创建一个空对象，挂载此脚本
    /// 2. 创建 Canvas，添加 ReactRendererUGUI 组件
    /// 3. 将 ReactRendererUGUI 拖入 UIBridge 的 Renderer 字段
    /// 4. 在需要的地方调用 UpdateGameState() 推送数据
    /// </summary>
    public class UIBridge : MonoBehaviour
    {
        [Header("React 渲染器")]
        [SerializeField] private ReactRendererUGUI renderer;

        [Header("开发模式（编辑器中使用）")]
        [SerializeField] private bool devMode = true;
        [SerializeField] private string devUrl = "http://localhost:3000/index.js";

        public static UIBridge Instance { get; private set; }

        // 游戏数据（由 Unity 游戏逻辑填充）
        [Serializable]
        public class GameData
        {
            public int health = 100;
            public int maxHealth = 100;
            public int mana = 50;
            public int maxMana = 50;
            public int stamina = 100;
            public int maxStamina = 100;
            public int exp = 0;
            public int maxExp = 100;
            public int level = 1;
            public int score = 0;
            public int gold = 0;
            public List<ItemData> items = new();
            public List<SkillData> skills = new();
            public PositionData position = new();
            public bool menuOpen = false;
            public bool inventoryOpen = false;
        }

        [Serializable]
        public class ItemData
        {
            public int id;
            public string name;
            public string icon;
            public int count;
            public int slot;
        }

        [Serializable]
        public class SkillData
        {
            public int id;
            public string name;
            public string icon;
            public float cooldown;
            public float maxCooldown;
            public int mpCost;
        }

        [Serializable]
        public class PositionData
        {
            public float x;
            public float y;
        }

        // 当前游戏数据
        private GameData _gameData = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // 初始化 React 环境
            if (renderer == null)
            {
                Debug.LogError("[UIBridge] Renderer 未设置！请拖入 ReactRendererUGUI 组件。");
                return;
            }

            // 注册 Unity → React 的回调方法
            RegisterUnityMethods();

            // 加载 React 脚本
            LoadReactScript();
        }

        /// <summary>
        /// 注册 Unity 方法，供 React 端调用
        /// </summary>
        private void RegisterUnityMethods()
        {
            var context = renderer.Context;
            var globals = context.Globals;

            // 背包操作
            globals["OpenInventory"] = (Action)OpenInventory;
            globals["UseItem"]        = (Action<int>)UseItem;
            globals["EquipItem"]      = (Action<int, int>)EquipItem;
            globals["DropItem"]       = (Action<int>)DropItem;

            // HUD 操作
            globals["ShowDamageNumber"] = (Action<float, float, float>)ShowDamageNumber;

            // 菜单操作
            globals["OpenMenu"]  = (Action<string>)OpenMenu;
            globals["CloseMenu"] = (Action)CloseMenu;
            globals["SetQuality"]= (Action<int>)SetQuality;
            globals["SetVolume"] = (Action<float>)SetVolume;

            // 技能
            globals["CastSkill"] = (Action<int>)CastSkill;
            globals["UsePotion"] = (Action)UsePotion;

            Debug.Log("[UIBridge] Unity 方法注册完成");
        }

        /// <summary>
        /// 加载 React 脚本（开发模式用本地服务器，生产模式用 StreamingAssets）
        /// </summary>
        private void LoadReactScript()
        {
#if UNITY_EDITOR
            if (devMode)
            {
                    var src = new ScriptSource();
                src.SourcePath = devUrl;
                
                renderer.Source = src;
                Debug.Log($"[UIBridge] 开发模式：从 {devUrl} 加载");
            }
            else
#endif
            {
                var path = System.IO.Path.Combine(
                    Application.streamingAssetsPath,
                    "react-ui/index.js"
                );
                renderer.Source = ScriptSource.Resource($"require('{path}');");
                Debug.Log($"[UIBridge] 生产模式：从 {path} 加载");
            }
        }

        // ============================================================
        // Unity → React：推送游戏数据
        // ============================================================

        /// <summary>
        /// 更新游戏状态（每帧或定时调用）
        /// </summary>
        public void UpdateGameState(GameData data)
        {
            _gameData = data;
            if (renderer != null)
            {
                renderer.Globals["gameState"] = data;
            }
        }

        /// <summary>
        /// 快速更新单个属性（减少 GC）
        /// </summary>
        public void UpdateHealth(int health, int maxHealth)
        {
            _gameData.health = health;
            _gameData.maxHealth = maxHealth;
            PushPartialUpdate();
        }

        public void UpdateMana(int mana, int maxMana)
        {
            _gameData.mana = mana;
            _gameData.maxMana = maxMana;
            PushPartialUpdate();
        }

        private void PushPartialUpdate()
        {
            if (renderer != null)
            {
                renderer.Globals["gameState"] = _gameData;
            }
        }

        // ============================================================
        // React → Unity：回调实现
        // ============================================================

        private void OpenInventory()
        {
            Debug.Log("[UIBridge] OpenInventory");
            // TODO: 通知游戏系统打开背包 UI
        }

        private void UseItem(int itemId)
        {
            Debug.Log($"[UIBridge] UseItem: {itemId}");
            // TODO: 调用背包系统使用道具
        }

        private void EquipItem(int itemId, int slot)
        {
            Debug.Log($"[UIBridge] EquipItem: {itemId} -> slot {slot}");
        }

        private void DropItem(int itemId)
        {
            Debug.Log($"[UIBridge] DropItem: {itemId}");
        }

        private void ShowDamageNumber(float value, float x, float y)
        {
            Debug.Log($"[UIBridge] ShowDamageNumber: {value} at ({x}, {y})");
        }

        private void OpenMenu(string menuName)
        {
            Debug.Log($"[UIBridge] OpenMenu: {menuName}");
            _gameData.menuOpen = true;
            PushPartialUpdate();
        }

        private void CloseMenu()
        {
            Debug.Log("[UIBridge] CloseMenu");
            _gameData.menuOpen = false;
            _gameData.inventoryOpen = false;
            PushPartialUpdate();
        }

        private void SetQuality(int level)
        {
            Debug.Log($"[UIBridge] SetQuality: {level}");
            QualitySettings.SetQualityLevel(level);
        }

        private void SetVolume(float volume)
        {
            Debug.Log($"[UIBridge] SetVolume: {volume}");
            AudioListener.volume = volume;
        }

        private void CastSkill(int skillId)
        {
            Debug.Log($"[UIBridge] CastSkill: {skillId}");
            // TODO: 触发技能系统
        }

        private void UsePotion()
        {
            Debug.Log("[UIBridge] UsePotion");
            // TODO: 使用药水
        }
    }
}

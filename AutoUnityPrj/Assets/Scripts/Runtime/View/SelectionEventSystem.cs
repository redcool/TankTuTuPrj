using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Runtime.ValueObject.ScriptableObjects;

namespace Game.Runtime.View
{
    /// <summary>
    /// 选择流程事件类型
    /// </summary>
    public enum SelectionEventType
    {
        StartGame,              // 开始游戏按钮点击
        CharacterSelected,      // 角色选择确认
        WeaponSelected,         // 武器选择确认
        DifficultySelected,     // 难度选择确认
        CharacterHovered,       // 角色悬停（显示详情）
        WeaponHovered,          // 武器悬停（显示详情）
        DifficultyHovered,      // 难度悬停（显示详情）
        BackToPrevious,         // 返回上一个界面
    }

    /// <summary>
    /// 选择流程事件数据
    /// </summary>
    public class SelectionEventData
    {
        public SelectionEventType EventType { get; private set; }
        public object Data { get; private set; }

        public SelectionEventData(SelectionEventType eventType, object data = null)
        {
            EventType = eventType;
            Data = data;
        }
    }

    /// <summary>
    /// 选择流程事件管理器 - 观察者模式实现
    /// 使用 C# Action 实现的简单事件系统
    /// </summary>
    public class SelectionEventManager : MonoBehaviour
    {
        private static SelectionEventManager _instance;
        public static SelectionEventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("SelectionEventManager");
                    _instance = go.AddComponent<SelectionEventManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        // 事件字典
        private readonly Dictionary<SelectionEventType, Action<SelectionEventData>> _events = new();

        private void OnDestroy()
        {
            _instance = null;
            ClearAll();
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(SelectionEventType eventType, Action<SelectionEventData> callback)
        {
            if (!_events.ContainsKey(eventType))
            {
                _events[eventType] = null;
            }
            _events[eventType] += callback;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Unsubscribe(SelectionEventType eventType, Action<SelectionEventData> callback)
        {
            if (_events.ContainsKey(eventType))
            {
                _events[eventType] -= callback;
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish(SelectionEventType eventType, object data = null)
        {
            if (_events.TryGetValue(eventType, out var callback))
            {
                callback?.Invoke(new SelectionEventData(eventType, data));
            }
        }

        /// <summary>
        /// 清除所有事件订阅
        /// </summary>
        public void ClearAll()
        {
            _events.Clear();
        }
    }
}
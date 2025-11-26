using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.UI.Ab_Panel
{
    public class pack
    {
        // 事件队列优化：防止高频事件积压
        private Queue<Action> eventQueue = new Queue<Action>();
        private float eventProcessInterval = 0.016f; // 约60fps
        private float lastProcessTime;
        private int maxEventsPerFrame = 10;

        // UI状态自动同步
        private Dictionary<string, object> stateCache = new Dictionary<string, object>();
        private bool autoRefreshEnabled = true;

        // 页面栈回退记录
        private Stack<string> pageHistory = new Stack<string>();
        private int maxHistoryDepth = 20;

        public void EnqueueEvent(Action eventAction)
        {
            if (eventQueue.Count < 100) // 防止无限积压
            {
                eventQueue.Enqueue(eventAction);
            }
        }

        public void ProcessEvents()
        {
            if (Time.time - lastProcessTime < eventProcessInterval) return;

            int processed = 0;
            while (eventQueue.Count > 0 && processed < maxEventsPerFrame)
            {
                try
                {
                    eventQueue.Dequeue()?.Invoke();
                    processed++;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Event processing error: {ex.Message}");
                }
            }

            lastProcessTime = Time.time;
        }

        public void UpdateState(string key, object value)
        {
            stateCache[key] = value;
            if (autoRefreshEnabled)
            {
                EnqueueEvent(() => RefreshUI(key));
            }
        }

        public void PushPage(string pageName)
        {
            if (pageHistory.Count >= maxHistoryDepth)
            {
                var temp = new Stack<string>(pageHistory);
                pageHistory.Clear();
                for (int i = 0; i < maxHistoryDepth - 1; i++)
                {
                    if (temp.Count > 0) pageHistory.Push(temp.Pop());
                }
            }
            pageHistory.Push(pageName);
        }

        public bool TryPopPage(out string pageName)
        {
            pageName = null;
            if (pageHistory.Count > 0)
            {
                pageName = pageHistory.Pop();
                return true;
            }
            return false;
        }

        private void RefreshUI(string key)
        {
            // Mock UI刷新逻辑
        }
    }
}

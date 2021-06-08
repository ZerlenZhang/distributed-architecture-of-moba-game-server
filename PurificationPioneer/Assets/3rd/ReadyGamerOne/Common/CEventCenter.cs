using System;
using System.Collections.Generic;

namespace ReadyGamerOne.Common
{
    /// <summary>
    /// 事件中心
    /// 监听分发事件
    /// </summary>CallBack
    public static class CEventCenter
    {
        #region Private

        /// <summary>
        /// 消息字典
        /// </summary>
        private static Dictionary<string, List<Delegate>> listeners =
            new Dictionary<string, List<Delegate>>();

        private static List<Delegate> onceActions = new List<Delegate>();
        

        #endregion


        /// <summary>
        /// 添加监听事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public static void AddListener(string type, Action listener)
        {
            AddListener(type, listener, false);
        }
        public static void AddListener(string type, Action listener, bool callOnce)
        {
            if (!listeners.ContainsKey(type))
                listeners.Add(type, new List<Delegate>());
            listeners[type].Add(listener);
            if (callOnce)
                onceActions.Add(listener);
        }
        public static void AddListener<T>(string type, Action<T> listener)
        {
            AddListener<T>(type, listener, false);
        }
        public static void AddListener<T>(string type, Action<T> listener, bool callOnce)
        {
            if (!listeners.ContainsKey(type))
                listeners.Add(type, new List<Delegate>());
            listeners[type].Add(listener);
            if (callOnce)
                onceActions.Add(listener);
        }
        public static void AddListener<T, U>(string type, Action<T, U> listener)
        {
            AddListener<T, U>(type, listener, false);
        }
        public static void AddListener<T, U>(string type, Action<T, U> listener, bool callOnce)
        {
            if (!listeners.ContainsKey(type))
                listeners.Add(type, new List<Delegate>());
            listeners[type].Add(listener);
            if (callOnce)
                onceActions.Add(listener);
        }
        /// <summary>
        /// 移出监听事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public static void RemoveListener(string type, Action listener)
        {
            if (listeners.ContainsKey(type))
                listeners[type].Remove(listener);
        }
        public static void RemoveListener<T>(string type, Action<T> listener)
        {
            if (listeners.ContainsKey(type))
                listeners[type].Remove(listener);
        }
        public static void RemoveListener<T, U>(string type, Action<T, U> listener)
        {
            if (listeners.ContainsKey(type))
                listeners[type].Remove(listener);
        }

        public static void BroadMessage(string type)
        {
            if (listeners.ContainsKey(type))
            {
                var list = listeners[type];
                for (var i = 0; i < list.Count; i++)
                {
                    var f = list[i] as Action;
                    if (null != f)
                    {
                        f();
                        if (onceActions.Contains(f))
                        {
                            list.Remove(f);
                            onceActions.Remove(f);
                        }
                    }
                }
            }
        }
        public static void BroadMessage<T>(string type, T arg1)
        {
            if (listeners.ContainsKey(type))
            {
                var list = listeners[type];
                for (var i = 0; i < list.Count; i++)
                {
                    var f = list[i] as Action<T>;
                    if (null != f)
                    {
                        f(arg1);
                        if (onceActions.Contains(f))
                        {
                            list.Remove(f);
                            onceActions.Remove(f);
                        }
                    }
                }
            }
        }
        public static void BroadMessage<T, U>(string type, T arg1, U arg2)
        {
            if (listeners.ContainsKey(type))
            {
                var list = listeners[type];
                for (var i = 0; i < list.Count; i++)
                {
                    var f = list[i] as Action<T, U>;
                    if (null != f)
                    {
                        f(arg1, arg2);
                        if (onceActions.Contains(f))
                        {
                            list.Remove(f);
                            onceActions.Remove(f);
                        }
                    }
                }
            }
        }
        public static void Clear()
        {
            listeners.Clear();
            onceActions.Clear();
        }
    }
}

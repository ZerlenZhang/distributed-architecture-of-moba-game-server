using System;
using ReadyGamerOne.Global;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace ReadyGamerOne.View
{
    /// <summary>
    /// 窗口基类
    /// </summary>
    public abstract class AbstractUI
    {
        public Transform m_TransFrom = null; //位置
        private string m_sResName = ""; //资源名
        private bool m_bIsVisible = false; //是否可见

        /// <summary>
        /// 类对象初始化
        /// 添加对象使用必须的事件监听
        /// Create时调用
        /// </summary>
        protected virtual void InitWindow()
        {
            MainLoop.Instance.AddUpdateFunc(Update);
        }

        /// <summary>
        /// 类对象释放
        /// Destroy时调用
        /// </summary>
        protected virtual void RealseWindow()
        {
            MainLoop.Instance.RemoveUpdateFunc(Update);
        }

        /// <summary>
        /// 游戏事件注册
        /// </summary>
        protected virtual void OnAddListener()
        {
        }

        /// <summary>
        /// 游戏事件注销
        /// </summary>
        protected virtual void OnRemoveListener()
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {

        }

        /// <summary>
        /// 是否可见
        /// </summary>
        /// <returns></returns>
        public bool IsVisable()
        {
            return m_bIsVisible;
        }

        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <returns></returns>
        protected virtual void Create(string path)
        {
            m_sResName = path;

            if (m_TransFrom)
            {
                Debug.LogError("Window Create Error Exist");
                return;
            }

            if (string.IsNullOrEmpty(m_sResName))
            {
                Debug.LogError("Windows Create Error ResName is empty");
                return;
            }

            if (GlobalVar.GCanvasObj == null)
                throw new Exception("你忘了初始化GlobalVar.GCanvasObj");

            var canvas = Global.GlobalVar.GCanvasObj.transform;

            if (null == canvas)
                throw new Exception("画布获取失败");

            var obj = ResourceMgr.InstantiateGameObject(m_sResName, canvas);

            if (obj == null)
            {
                Debug.LogError("Window Create Error LoadRes WindowName = " + m_sResName);
                return;
            }

            m_TransFrom = obj.transform;

            m_TransFrom.gameObject.SetActive(false);

            InitWindow();
        }

        /// <summary>
        /// 销毁窗体
        /// </summary>
        public virtual void DestroyThis(PointerEventData eventData = null)
        {
            if (m_TransFrom)
            {
                OnRemoveListener();
                RealseWindow();
                Object.Destroy(m_TransFrom.gameObject);
                m_TransFrom = null;
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (m_TransFrom && m_TransFrom.gameObject.activeSelf == false)
            {
                m_TransFrom.gameObject.SetActive(true);
                m_bIsVisible = true;
                OnAddListener();
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if (m_TransFrom && m_TransFrom.gameObject.activeSelf == true)
            {
                OnRemoveListener();
                m_TransFrom.gameObject.SetActive(false);
            }

            m_bIsVisible = false;

        }
        
    }
}

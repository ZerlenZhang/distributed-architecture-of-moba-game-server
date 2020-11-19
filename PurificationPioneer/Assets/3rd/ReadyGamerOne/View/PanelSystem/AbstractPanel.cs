using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReadyGamerOne.View
{
    /// <inheritdoc />
    /// <summary>
    /// 可用栈盛放的栈窗口
    /// </summary>
    public abstract class AbstractPanel : AbstractUI, IStackPanel
    {
        #region Static

        internal static void RegisterPanels(Type outType)
        {
            foreach (var type in Assembly.GetAssembly(outType).GetTypes())
            {
                if (type.IsSubclassOf(typeof(AbstractPanel)))
                {
                    RegisterPanel(type);
                }
            }
        }
        private static void RegisterPanel(Type panelType)
        {
            Assert.IsTrue(panelType.IsSubclassOf(typeof(AbstractPanel)));
            if(panelDic.ContainsKey(panelType.Name))
            {   
                Debug.LogWarning("注意Project中是不是有同名类继承了AbstractPanel");
                return;
            }
            
            var p = Activator.CreateInstance(panelType) as AbstractPanel;
            panelDic.Add(panelType.Name, p);
        }        

        private static Dictionary<string, AbstractPanel> panelDic =
            new Dictionary<string, AbstractPanel>();
        internal static AbstractPanel GetPanel(string name)
        {
            if(!panelDic.ContainsKey(name))
                Debug.LogError("你好像没有在GameMgr_RegisterUiPanel中注册这个Panel类："+name);
            return panelDic[name];
        }


        
        #endregion

        public sealed override void DestroyThis(PointerEventData eventData = null)
        {
            base.DestroyThis(eventData);
        }

        #region IStackPanel

        public virtual void Enable()
        {
            if (m_TransFrom == null)
            {
//                Debug.Log("Load");
                Load();
            }
            this.Show();
        }

        public virtual void Disable()
        {
            this.Hide();
        }
        
        public virtual void Destory()
        {
//            Debug.Log("Clear"+GetType().Name);
            this.Disable();
            view.Clear();
            this.DestroyThis();
        }        

        #endregion

        protected abstract void Load();        


        #region 在子类可以通过view["path"]访问所有节点

        protected override void Create(string path)
        {
//            Debug.Log("Create: "+GetType());
            base.Create(path);
            load_all_object(m_TransFrom.gameObject,"");
        }

        public Dictionary<string, GameObject> view = new Dictionary<string, GameObject>();

        void load_all_object(GameObject root, string path) {
            foreach (Transform tf in root.transform) {
                if (this.view.ContainsKey(path + tf.gameObject.name)) {
                    Debug.LogWarning("发现同名UI节点:" + path + tf.gameObject.name + "!");
                    continue;
                }

//                Debug.Log("加载UI物体： " + path + tf.gameObject.name);
                this.view.Add(path + tf.gameObject.name, tf.gameObject);
                load_all_object(tf.gameObject, path + tf.gameObject.name + "/");
            }

        }        
        
        
        protected Transform GetTransform(string path)
        {
            if (!view.ContainsKey(path))
                throw new Exception("不包含这个路径：" + path);
            return view[path].transform;
        }
        protected T GetComponent<T>(string path) where T : Component
        {
            if (!view.ContainsKey(path))
                throw new Exception("不包含这个路径：" + path);
            return view[path].GetComponent<T>();
        }

        #endregion

        #region 添加UnityUI回调

        protected void add_button_listener(string view_name, UnityAction onclick)
        {
//            Debug.Log(view_name);
            Button bt = view[view_name].GetComponent<Button>();
            if (bt == null) {
                Debug.LogWarning("UI_manager add_button_listener: not Button Component!");
                return;
            }
            
            bt.onClick.AddListener(onclick);
        }

        protected void add_slider_listener(string view_name, UnityAction<float> on_value_changle)
        {
            Slider s = view[view_name].GetComponent<Slider>();
            if (s == null) {
                Debug.LogWarning("UI_manager add_slider_listener: not Slider Component!");
                return;
            }

            s.onValueChanged.AddListener(on_value_changle);
        }

        #endregion
        
    }
}

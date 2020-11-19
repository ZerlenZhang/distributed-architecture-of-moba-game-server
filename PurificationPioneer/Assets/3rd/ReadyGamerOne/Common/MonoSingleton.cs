using System;
using UnityEngine;

namespace ReadyGamerOne.Common
{
    /// <summary>
    /// MonoBehavior单例类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T>:
        MonoBehaviour
        where T:MonoSingleton<T>
    {
        private static T _instance;
        
        public static T Instance {
            get {
                
                //如果有实例，直接返回
                if (_instance != null) 
                    return _instance;
                
                //如果没有实例，尝试从场景中找一个
                _instance = FindObjectOfType(typeof(T)) as T;
                
                if (_instance != null) 
                    return _instance;
                
                //实在没有，创建
                var obj = new GameObject
                {
                    hideFlags = HideFlags.DontSave, 
                    name = typeof(T).Name
                };

                var script  = (T)obj.AddComponent(typeof(T));
                script.OnStateIsNull();
                
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (gameObject.activeSelf == false||enabled==false)
                return;
            
            if (_instance == null)
            {
                OnStateIsNull();
            }
            else if (_instance == this)
            {
                OnStateIsSelf();
            }
            else
            {
                OnStateIsOthers();
            }
        }

        protected virtual void OnStateIsNull()
        {
            _instance = this as T;
        }

        protected virtual void OnStateIsSelf()
        {
            
        }

        protected virtual void OnStateIsOthers()
        {
            
        }


        protected virtual void Update()
        {
            
        }

    }
}
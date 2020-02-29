using System;
using UnityEngine;
namespace ReadyGamerOne.Common
{
    /// <summary>
    /// MonoBehavior单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour
        where T : MonoSingleton<T>
    {
        protected static T _instance { get; private set; } = null;
        public static T Instance {
            get {
                if (_instance) return _instance;
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance) return _instance;
                
//                Debug.Log("新建："+typeof(T).Name);

                var obj = new GameObject()
                {
                    name = typeof(T).Name,
                    hideFlags = HideFlags.DontSave
                };
                
                _instance = (T)obj.AddComponent(typeof(T));

                if (!_instance)
                    throw new Exception("怎么还是空？"+typeof(T).Name);
                        
                DontDestroyOnLoad(obj);
                return _instance;
            }
        }

        protected virtual void Awake() {
            if (_instance == null)
            {
//                Debug.Log("赋值全局单例:"+GetType().Name+" " + GetHashCode());
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else if(_instance!=this)
            {
                Debug.LogWarning(this.GetType().Name+"重复生成，销毁当前物体："+ GetHashCode());
                GameObject.Destroy(this.gameObject);
            }
            else
            {
                //print("instance is this, do nothing");
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}

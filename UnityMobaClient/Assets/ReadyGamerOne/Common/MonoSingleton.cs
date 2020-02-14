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
        protected static T _instance = null;
        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null) {
//                        Debug.Log("新建："+typeof(T).Name);
                        GameObject obj = new GameObject();
                        _instance = (T)obj.AddComponent(typeof(T));
                        obj.hideFlags = HideFlags.DontSave;
                        // obj.hideFlags = HideFlags.HideAndDontSave;
                        obj.name = typeof(T).Name;
                    }
                }
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
//                Debug.Log(this.GetType().Name+"重复生成，销毁当前物体："+ GetHashCode());
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}

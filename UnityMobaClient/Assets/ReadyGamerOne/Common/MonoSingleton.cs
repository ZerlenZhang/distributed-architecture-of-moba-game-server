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
            DontDestroyOnLoad(this.gameObject);
            if (_instance == null)
            {
                Debug.Log("赋值全局单例:"+GetType().Name+" " + GetHashCode());
                _instance = this as T;
            }
            else
            {
                Debug.Log("销毁当前物体：" + GetHashCode());
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}

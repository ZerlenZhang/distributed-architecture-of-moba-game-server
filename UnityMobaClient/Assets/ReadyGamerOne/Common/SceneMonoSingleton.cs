using UnityEngine;

namespace ReadyGamerOne.Common
{
    public class SceneMonoSingleton<T>:
        MonoBehaviour
        where T:SceneMonoSingleton<T>
    {
        private static T _instance;

        protected void Awake()
        {
            _instance = this as T;
        }

        public static T Instance => _instance;

    }
}
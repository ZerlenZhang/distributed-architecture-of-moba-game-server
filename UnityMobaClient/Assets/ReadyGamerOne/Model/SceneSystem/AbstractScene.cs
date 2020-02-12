using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ReadyGamerOne.Model.SceneSystem
{
    
    public abstract class AbstractSceneInfo
    {
        
        #region static_All_Instances
        
        public static event Action onAnySceneLoad;
        public static event Action onAnySceneUnLoaded;

        private static Dictionary<string, AbstractSceneInfo> sceneDis = new Dictionary<string, AbstractSceneInfo>();
        
        public static void RegisterSceneInfo<T>(string sceneName) where T : AbstractSceneInfo, new()
        {
            var scene = new T();
            scene.sceneName = sceneName;
            sceneDis.Add(sceneName, scene);
        }
        
        public static AbstractSceneInfo GetScene(string name)
        {
            if (!sceneDis.ContainsKey(name))
            {
                if (SceneManager.GetSceneByName(name).IsValid())
                {
                    Debug.LogWarning("没有注册场景："+name+" 但是，Unity中Build了这个场景");
                    return new DefaultSceneInfo()
                    {
                        sceneName = name
                    };
                }

                throw new Exception("没有注册这个场景，并且名字无效————" + name);
            }
                
            return sceneDis[name];
        }


        #endregion
        
        public string sceneName;

        public event Action onSceneLoad;
        public event Action onSceneUnLoad;

        /// <summary>
        /// 加载场景时调用
        /// </summary>
        protected internal virtual void OnSceneLoad()
        {
            Debug.Log("OnSceneLoad  "+sceneName);
            onAnySceneLoad?.Invoke();
            onSceneLoad?.Invoke();  
        }

        /// <summary>
        /// 离开场景时调用
        /// </summary>
        protected internal virtual void OnSceneUnLoaded()
        {
            Debug.Log("OnSceneUnLoad  "+sceneName);
            onSceneUnLoad?.Invoke();
            onAnySceneUnLoaded?.Invoke();
        }
    }

    public class DefaultSceneInfo : AbstractSceneInfo
    {
    }
}
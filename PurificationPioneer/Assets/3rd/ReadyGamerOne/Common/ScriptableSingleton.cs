using System;
using ReadyGamerOne.Utility;

#if UNITY_EDITOR
using Directory = UnityEngine.Windows.Directory;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace ReadyGamerOne.Common
{
    public class ScriptableSingleton<T>
        :ScriptableObject
        where T:ScriptableSingleton<T>
    {
        private static string _assetName;

        private static string AssetName
        {
            get
            {

                if (string.IsNullOrEmpty(_assetName))
                {
                    var type = typeof(T);
                    var attribute = type.GetAttribute<ScriptableSingletonInfoAttribute>();
                    if (null == attribute || string.IsNullOrEmpty(attribute.assetName))
                        _assetName = type.Name;
                    else
                        _assetName = attribute.assetName;                    
                }
                
                return _assetName;
            }
        }
        
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.Load<T>("GlobalAssets/"+AssetName);
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    var path = "Assets/Resources/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    _instance = CreateInstance<T>();
                    AssetDatabase.CreateAsset(_instance, path+"/"+AssetName+".asset");
                }
#endif
                if (_instance == null)
                {
                    throw new Exception("初始化失败");
                }

                return _instance;
            }
        }
    }
}
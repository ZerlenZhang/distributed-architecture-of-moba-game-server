using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ReadyGamerOne.ScriptableObjects
{
    public class PrefUtil:ScriptableObject
    {
        private static PrefUtil _instance;
        public static PrefUtil Instance
        {
            get
            {
                if (!_instance)
                { 
                    _instance = Resources.Load<PrefUtil>("GlobalAssets/GlobalConstStrings");
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = CreateInstance<PrefUtil>();
                    var path = "Assets/Resources/GlobalAssets";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    AssetDatabase.CreateAsset(_instance, path+"/GlobalConstStrings.asset");
                }
#endif
                if (_instance == null)
                    throw new System.Exception("初始化失败");

                return _instance;
            }
        }

        public List<string> constStrings = new List<string>();

        [SerializeField]
        public List<PrefItem> prefItems = new List<PrefItem>();


        public void SetString(string key,string value)
        {
            if (-1 == Find(key))
                prefItems.Add(new PrefItem
                {
                    key=key
                });

            prefItems[Find(key)].value = value;
        }

        public string GetString(string key,string defaultValue)
        {
            if (-1 == Find(key))
                return defaultValue;

            return prefItems[Find(key)].value;
        }

        public int Find(string key)
        {
            var index = 0;
            foreach(var pf in prefItems)
            {
                if (pf.key == key)
                    return index;
                index++;
            }
            return -1;
        }

    }
}
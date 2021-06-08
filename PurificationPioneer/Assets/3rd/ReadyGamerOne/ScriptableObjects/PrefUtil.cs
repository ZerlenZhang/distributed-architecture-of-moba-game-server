using System.Collections.Generic;
using ReadyGamerOne.Common;
using UnityEngine;

namespace ReadyGamerOne.ScriptableObjects
{
    [ScriptableSingletonInfo("GlobalConstStrings")]
    public class PrefUtil: Common.ScriptableSingleton<PrefUtil>
    {

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
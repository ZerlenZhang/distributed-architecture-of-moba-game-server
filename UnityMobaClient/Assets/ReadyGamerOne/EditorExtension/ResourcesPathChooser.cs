using System;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [Serializable]
    public class ResourcesPathChooser
    {
        [SerializeField] private int selectedIndex;
        public string Path;

        public string Name
        {
            get
            {
                var currentName = Path;
                if (currentName.Contains("/"))
                {
                    var select = currentName.LastIndexOf('/') + 1;
                    currentName = currentName.Substring(select, currentName.Length-select);
                    return currentName;
                }
                else
                    return Path;
            }
        }
    }
}
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace ReadyGamerOne.ScriptableObjects
{
    [Serializable]
    public class ConstStringChooser
    {
#if UNITY_EDITOR

        public static string GetShowText(SerializedProperty property)
        {
            var prop = property.FindPropertyRelative("selectedIndex");
            return PrefUtil.Instance.constStrings[prop.intValue];
        }
#endif
        
        
        [SerializeField] private int selectedIndex;
        public string StringValue => PrefUtil.Instance.constStrings[selectedIndex];
        public void SetIndex(int index)
        {
            selectedIndex = index;
        }
        
        
        
        
    }
}
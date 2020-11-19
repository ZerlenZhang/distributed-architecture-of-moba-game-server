using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [Serializable]
    public class StringChooser
    {
#if UNITY_EDITOR
        public static string GetShowTextFromSerializedProperty(SerializedProperty property)
        {
            return  property.FindPropertyRelative("values").GetArrayElementAtIndex(property.FindPropertyRelative("selectedIndex").intValue).stringValue;

        }
#endif

        
        //private static Type typeToShow;
        [SerializeField] protected string[] values;
        [SerializeField] protected int selectedIndex;
        public string StringValue => values[selectedIndex];

        public StringChooser(Type type)
        {
            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            if (this.values == null)
                this.values = new string[fieldInfos.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = fieldInfos[i].GetValue(null) as string;
            }
        }
        public StringChooser(IList<string> strs)
        {
            if(values==null)
                values = new string[strs.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = strs[i];
            }

        }
    }
}
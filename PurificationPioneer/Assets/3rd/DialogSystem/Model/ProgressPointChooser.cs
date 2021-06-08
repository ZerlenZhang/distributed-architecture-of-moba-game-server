using System;
using DialogSystem.ScriptObject;
#if UNITY_EDITOR
    
using UnityEditor;
#endif
using UnityEngine;

namespace DialogSystem.Model
{
    [Serializable]
    public class ProgressPointChooser
    {
        
#pragma warning disable 649

        public static bool ShowLabel = true;

#if UNITY_EDITOR
        public static string GetShowTextFromSerializedProperty(SerializedProperty property)
        {
            var text = "设置的进度值为：";
            var valueTypeProp = property.FindPropertyRelative("valueType");
            if (valueTypeProp.enumValueIndex == 0)
                text+= property.FindPropertyRelative("value").floatValue.ToString();
            else
            {
                var info = DialogProgressAsset.Instance.DialogProgressPoints[
                    property.FindPropertyRelative("selectedIndex").intValue];
                text += info.name + $"【{info.value}】";
            }

            return text;
        }
                
#endif

        
        public ValueChooser.ValueType valueType;
        [SerializeField] private int selectedIndex;
        [SerializeField] private float value;
        public float Value
        {
            get
            {
                if (valueType == ValueChooser.ValueType.Var)
                {
                    if (DialogProgressAsset.Instance.DialogProgressPoints.Count <= selectedIndex)
                        throw new Exception("ProgressPointChooser选取的Index不当");
                    return DialogProgressAsset.Instance.DialogProgressPoints[selectedIndex].value;                    
                }

                return value;

            }
        }
        
        
#pragma warning restore 649
    }

}
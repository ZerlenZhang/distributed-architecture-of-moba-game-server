using System.Collections.Generic;
using DialogSystem.ScriptObject;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(ProgressPointChooser))]
    public class ProgressPointChooserDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var indexProp = property.FindPropertyRelative("selectedIndex");
            var valueTypeProp = property.FindPropertyRelative("valueType");
            var valueProp = property.FindPropertyRelative("value");
            var list = new List<string>();

            var left = 0.3f;
            valueTypeProp.enumValueIndex = EditorGUI.Popup(position.GetLeft(left), valueTypeProp.enumValueIndex,
                valueTypeProp.enumNames);
            position = position.GetRight(1 - left);
            var index = 0;
            if (valueTypeProp.enumValueIndex == 1)//使用DialogProgressAsset中的值
            {
                foreach (var VARIABLE in DialogProgressAsset.Instance.DialogProgressPoints)
                {
                    if (string.IsNullOrEmpty(VARIABLE.name))
                    {
                        Debug.LogError("DialogProgressAsset中关键点名不能为空");
                        continue;
                    }
                    list.Add(VARIABLE.name+$"【{VARIABLE.value}】");
                }

                if(ProgressPointChooser.ShowLabel)
                    indexProp.intValue = EditorGUI.Popup(position.GetRectAtIndex(index++), "剧情关键点", indexProp.intValue, list.ToArray());
                else
                {
                    indexProp.intValue = EditorGUI.Popup(position.GetRectAtIndex(index++), indexProp.intValue, list.ToArray());
                }                
            }
            else//使用常量
            {
                if (ProgressPointChooser.ShowLabel)
                    valueProp.floatValue =
                        EditorGUI.FloatField(position.GetRectAtIndex(index++), "剧情关键点", valueProp.floatValue);
                else
                {
                    valueProp.floatValue =
                        EditorGUI.FloatField(position.GetRectAtIndex(index++),  valueProp.floatValue);
                }
            }

        }
    }
}
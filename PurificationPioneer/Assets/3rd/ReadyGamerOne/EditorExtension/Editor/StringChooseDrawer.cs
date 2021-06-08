using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.EditorExtension
{
    [CustomPropertyDrawer(typeof(StringChooser))]
    public class StringChooseDrawer : PropertyDrawer
    {
        private List<string> values;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = property.FindPropertyRelative("selectedIndex").intValue;
            if (values == null)
            {
                values = new List<string>();
                var vs = property.FindPropertyRelative("values");
                Assert.IsTrue( vs!=null&&vs.isArray);
                string ans = "";
   //             Debug.Log(property.displayName+" "+"size: "+vs.arraySize);
                for (int i = 0; i < vs.arraySize; i++)
                {
                    values.Add(vs.GetArrayElementAtIndex(i).stringValue);
                    ans += values[i]+" ";
                }
//                Debug.Log("执行！！"+property.displayName+" "+ans);
            }

            //base.OnGUI(position, property, label);
            EditorGUI.BeginChangeCheck();
            //得到描述在列表中的下标

            index = EditorGUI.Popup(position, property.displayName, index, values.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("selectedIndex").intValue = index;

                //根据下标从id列表中找出技能id进行赋值

                //Debug.Log(property.FindPropertyRelative("selectedValue").stringValue);

            }
        }
    }
}
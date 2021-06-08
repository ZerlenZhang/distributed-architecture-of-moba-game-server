using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(DialogProgressPoint))]
    public class DialogProgressPointDrawer:UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            var left = 0.1f;
            var s=new GUIStyle()
            {
                fontSize = 20
            };
            EditorGUI.LabelField(position.GetLeft(left), property.FindPropertyRelative("index").intValue.ToString(),s);


            position = position.GetRight(1 - left);

            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("name"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("value"));
        }
    }
}
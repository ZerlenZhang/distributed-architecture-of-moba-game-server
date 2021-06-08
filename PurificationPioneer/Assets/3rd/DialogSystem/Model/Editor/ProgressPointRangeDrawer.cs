using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(ProgressPointRange))]
    public class ProgressPointRangeDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2 * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = ProgressPointChooser.ShowLabel;
            ProgressPointChooser.ShowLabel = false;
            var left = 0.4f;
            EditorGUI.LabelField(position.GetLeft(left), "剧情关键点范围");

            var index = 0;
            position = position.GetRight(1 - left);
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("min"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("max"));
            ProgressPointChooser.ShowLabel = value;
        }
    }
}
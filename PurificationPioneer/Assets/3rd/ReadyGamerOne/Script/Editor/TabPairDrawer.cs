using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.Script.Editor
{
    [CustomPropertyDrawer(typeof(TabPair))]
    public class TabPairDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 3 * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            var indexProp = property.FindPropertyRelative("index");
            EditorGUI.LabelField(position.GetRectAtIndex(index++), indexProp.displayName, indexProp.intValue.ToString());
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("tab"));
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("page"));
        }
    }
}
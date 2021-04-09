using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(VarUnitInfo))]
    public class VarUnitInfoDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("VarName"));
            ArgChooser.typeChangAble = false;
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("ArgChooser"));


        }
    }
}
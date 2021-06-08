using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(SkipChoice))]
    public class SkipChoiceDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 4 * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            var r = position.GetRectAtIndex(index++);
            EditorGUI.LabelField(r.GetLeft(.4f),"enable");
            var enableProp = property.FindPropertyRelative("enable");
            EditorGUI.PropertyField(r.GetRight(0.6f), enableProp);
            if(ValueChooser.GetArgType(property.FindPropertyRelative("enable"))!=ArgType.Bool)
                EditorGUILayout.HelpBox("这里必须为Bool类型",MessageType.Error);
            
            
            var text = property.FindPropertyRelative("text");
            var targetDialogInfoAssetProp = property.FindPropertyRelative("targetDialogInfoAsset");
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), text);
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), targetDialogInfoAssetProp);
            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("willBack"));
        }
    }
}
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(MessageTrigger))]
    public class MessageTriggerDrawer:PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            var height = 0;
            height+=2;
            var argCount = property.FindPropertyRelative("argCount").intValue;

            height += argCount;

            return height * EditorGUIUtility.singleLineHeight;

            
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var index = 0;
            var argCount = property.FindPropertyRelative("argCount").intValue;
            

            EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("messageToTick"));
            
            argCount = EditorGUI.Popup(position.GetRectAtIndex(index++), "argCount",argCount,
                new[] {"0", "1", "2"});
            
            ArgChooser.typeChangAble = true;
            
            switch (argCount)
            {
                case 1:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++).GetRight(.9f),
                        property.FindPropertyRelative("arg1"));
                    break;
                case 2:
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++).GetRight(.9f),
                        property.FindPropertyRelative("arg1"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++).GetRight(.9f),
                        property.FindPropertyRelative("arg2"));
                    break;
            }

            property.FindPropertyRelative("argCount").intValue = argCount;


        }
    }
}
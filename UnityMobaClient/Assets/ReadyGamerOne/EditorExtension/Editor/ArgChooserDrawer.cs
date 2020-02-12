using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace ReadyGamerOne.EditorExtension
{
    [CustomPropertyDrawer(typeof(ArgChooser))]
    public class ArgChooserDrawer:PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var argType = property.FindPropertyRelative("argType");

            if (ArgChooser.typeChangAble)
            {
                argType.enumValueIndex = EditorGUI.Popup(position.GetLeft(ArgChooser.argTypeWidth), argType.enumValueIndex, argType.enumNames);

                var argValueWidth = 1 - ArgChooser.argTypeWidth;
                switch (argType.enumValueIndex)
                {
                    case 0:    // Int
                        var intArgProp = property.FindPropertyRelative("IntArg");
                        intArgProp.intValue = EditorGUI.IntField(position.GetRight(argValueWidth), intArgProp.intValue);
                        break;
                    case 1:    // Float
                        var floatArgProp = property.FindPropertyRelative("FloatArg");
                        floatArgProp.floatValue =
                            EditorGUI.FloatField(position.GetRight(argValueWidth), floatArgProp.floatValue);
                        break;
                    case 2:    // String
                        var strArgProp = property.FindPropertyRelative("StringArg");
                        strArgProp.stringValue = EditorGUI.TextField(position.GetRight(argValueWidth), strArgProp.stringValue);
                        break;
                    case 3:    // bool
                        var boolArgProp = property.FindPropertyRelative("BoolArg");
                        boolArgProp.boolValue = EditorGUI.Toggle(position.GetRight(argValueWidth), boolArgProp.boolValue);
                        break;
                    case 4://Vector3
                        var vec3Prop = property.FindPropertyRelative("Vector3Arg");
                        vec3Prop.vector3Value =
                            EditorGUI.Vector3Field(position.GetRight(argValueWidth), "", vec3Prop.vector3Value);
                        break;
                }                
            }
            else
            {
                switch (argType.enumValueIndex)
                {
                    case 0:    // Int
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("IntArg"));
                        break;
                    case 1:    // Float
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("FloatArg"));
                        break;
                    case 2:    // String
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("StringArg"));
                        break;
                    case 3:
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("BoolArg"));
                        break;
                    case 4:
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("Vector3Arg"));
                        break;
                }   
            }
            

        }
    }
}
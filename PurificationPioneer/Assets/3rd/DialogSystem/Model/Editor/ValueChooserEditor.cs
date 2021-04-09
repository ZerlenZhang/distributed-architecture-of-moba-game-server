using System;
using System.Collections.Generic;
using System.Linq;
using DialogSystem.ScriptObject;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(ValueChooser))]
    public class ValueChooserEditor : PropertyDrawer
    {

        private static float ArgValueWidth => 1 - ValueChooser.valueTypeWidth - ValueChooser.argTypeWidth;

        private AbstractDialogInfoAsset _abstractDialogSystem;
        private SerializedProperty indexProp;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _abstractDialogSystem = property.FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue as AbstractDialogInfoAsset;

            if (_abstractDialogSystem == null)
                throw new Exception("abstractAbstractDialogInfoAsset is null");
            
            indexProp = property.FindPropertyRelative("selectedIndex");

            var valueTypeProp = property.FindPropertyRelative("valueType");
            valueTypeProp.enumValueIndex = EditorGUI.Popup(position.GetLeft(ValueChooser.valueTypeWidth),
                valueTypeProp.enumValueIndex, valueTypeProp.enumNames);            
            
            if (valueTypeProp.enumValueIndex == 0) // Value
            {
                ArgChooser.argTypeWidth = ValueChooser.argTypeWidth / (1 -ValueChooser. valueTypeWidth);
                ArgChooser.typeChangAble = true;

                EditorGUI.PropertyField(position.GetRight(1 - ValueChooser.valueTypeWidth),
                    property.FindPropertyRelative("ArgChooser"));
            }
            else // Var
            {
                var argType = property.FindPropertyRelative("argType");

                argType.enumValueIndex =
                    EditorGUI.Popup(position.GetLeft(ValueChooser.valueTypeWidth + ValueChooser.argTypeWidth, ValueChooser.valueTypeWidth),
                        argType.enumValueIndex, argType.enumNames);
                
                
                Dictionary<string, string> ans = null;
                switch (argType.enumValueIndex)
                {
                    case 0: // Int
                        ans = _abstractDialogSystem.GetValueStrings(ArgType.Int);

                        if (ans.Count == 0)
                        {
                            break;
                        }
                        indexProp.intValue = EditorGUI.Popup(position.GetRight(ArgValueWidth), indexProp.intValue,
                            ans.Keys.ToArray());
                        break;
                    case 1: // Float
                        ans = _abstractDialogSystem.GetValueStrings(ArgType.Float);
                        if (ans.Count == 0)
                        {
                            break;
                        }
                        indexProp.intValue = EditorGUI.Popup(position.GetRight(ArgValueWidth), indexProp.intValue,
                            ans.Keys.ToArray());
                        break;
                    case 2: // String
                        ans =_abstractDialogSystem.GetValueStrings(ArgType.String);
                        if (ans.Count == 0)
                        {
                            //property.FindPropertyRelative("StrArg").stringValue = "";
                            break;
                        }
                        indexProp.intValue = EditorGUI.Popup(position.GetRight(ArgValueWidth), indexProp.intValue,
                            ans.Keys.ToArray());
                        break;
                    case 3: // bool
                        ans = _abstractDialogSystem.GetValueStrings(ArgType.Bool);
                        if (ans.Count == 0)
                        {
                            break;
                        }
                        indexProp.intValue = EditorGUI.Popup(position.GetRight(ArgValueWidth), indexProp.intValue,
                            ans.Keys.ToArray());
                        break;
                }
            }
        }

    }
}
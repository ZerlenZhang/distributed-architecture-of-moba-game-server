using System.Linq;
using DialogSystem.ScriptObject;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace DialogSystem.Model.Editor
{
    [CustomPropertyDrawer(typeof(DialogUnitInfo))]
    public class DialogUnitDrawer : PropertyDrawer
    {
        private string[] compare = new[] {"==", "!=", "<", ">", ">=", "<="};
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sp = property.FindPropertyRelative("UnitType");
            var choicesProp = property.FindPropertyRelative("Choices");
            switch (EnumUtil.GetEnumValue<UnitType>(sp.enumValueIndex))
            {
               // case 11://Choos
               //     return 6*EditorGUIUtility.singleLineHeight;
                default:
                    return 10 * EditorGUIUtility.singleLineHeight;
            }
        }

        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            
            var index = 0;
            var dialogSystem = property.FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue as AbstractDialogInfoAsset;


            #region LeftPart
            
            var sp = property.FindPropertyRelative("UnitType");            
            GUIStyle titleStyle = new GUIStyle()
            {
                fontSize = 18,
                alignment = TextAnchor.UpperLeft
            };

            var leftPart = position.GetLeft(0.15f);
            
            // UnitType类型
            EditorGUI.LabelField(leftPart.GetUp(0.6f),sp.enumNames[sp.enumValueIndex],titleStyle);
            // ID
            EditorGUI.LabelField(leftPart.GetBottom(.5f),"ID:   "+property.FindPropertyRelative("id").intValue.ToString());
            
            #endregion

            position = position.GetRight(.85f);

            var text = "";
            bool error = false;

            var name = CharacterChooser.GetShowTextFromSerializedProperty(property.FindPropertyRelative("character"));

            name = name ?? "未设置名字 ";

            //EditorGUI.ObjectField(position.GetRectAtIndex(index++), property.FindPropertyRelative("character"));

            switch (EnumUtil.GetEnumValue<UnitType>(sp.enumValueIndex))
            {
                case 10: //Model.UnitType.Word

                    #region Word
                    
                    text =name+ ":\"" +
                           property.FindPropertyRelative("words").stringValue+"\"";

                    break;
                    #endregion
                case 11: //Model.UnitType.Choose
                    #region Choose
                    text =name+ ":\"" +
                          property.FindPropertyRelative("title").stringValue+"\"";
                    break;                    

                    #endregion
                case 12: //Model.UnitType.Narrator
                    #region Narrator

                    text = property.FindPropertyRelative("wordToNarrator").stringValue;
                    
                    break;

                #endregion
                case 13: //Model.UnitType.ExWord
                    #region ExWord
                    text =name+ ":\"" +
                          property.FindPropertyRelative("words").stringValue+"\"";
                    break;
                #endregion
                case 20:
                    #region IF

                    var vc1Prop = property.FindPropertyRelative("vc1");
                    var vc2Prop = property.FindPropertyRelative("vc2");
                    var ans= dialogSystem.GetValueStrings((ArgType)ValueChooser.GetArgTypeFromSerializedProperty(vc1Prop));

                    var vc1Type = vc1Prop.FindPropertyRelative("valueType");
                    var vc2Type = vc2Prop.FindPropertyRelative("valueType");
                    if ((vc1Type.enumValueIndex==1||vc2Type.enumValueIndex==1)&& ans.Count == 0 || (ValueChooser.GetArgTypeFromSerializedProperty(vc1Prop)!=ValueChooser.GetArgTypeFromSerializedProperty(vc2Prop)))
                    {
                        error = true;
                        text = "注意！！！  条件设置异常！！！";
//                        text += ValueChooser.GetArgTypeFromSerializedProperty(vc1Prop);
//                        text += "  ";
//                        text += ValueChooser.GetArgTypeFromSerializedProperty(vc2Prop);
                        break;
                    }

                    text += ValueChooser.GetShowTextFromSerializedProperty(vc1Prop);
                    text += compare[property.FindPropertyRelative("compareType").intValue];
                    text += ValueChooser.GetShowTextFromSerializedProperty(vc2Prop);

                

                    break;                    

                    #endregion
                case 24: //Model.UnitType.Skip
                    text = "SkipNum=" + property.FindPropertyRelative("skipNum").intValue;
                    break;
                case 60:
                    #region RunSetVarUnit

                    if (dialogSystem.VarAsset.varInfos.Count == 0)
                    {
                        var rect = position.GetRectAtIndex(index++);
                        EditorGUI.HelpBox(rect,"现在 Assets/DialogSystemAssets/GlobalVar.asset 中没有变量",MessageType.Error);
                        break;
                    }
                    
                    var varTypeR = position.GetRectAtIndex(index++);

                    var varInfos = dialogSystem.GetValueInfos();
                    var keyArray = varInfos.Keys.ToArray();

                    var varIndexProp = property.FindPropertyRelative("varIndex");
                    
                    var varToSetProp = property.FindPropertyRelative("varToSet");

                    text = keyArray[varIndexProp.intValue] + " = " +
                           ArgChooser.GetShowTextFromSerializedProperty(varToSetProp);

                    break;
                    #endregion
                case 50: //Model.UnitType.Message
                    #region Message

                    var strchooser=property.FindPropertyRelative("workMessageTrigger").FindPropertyRelative("messageToTick");
                    text = "Tick Message : " + StringChooser.GetShowTextFromSerializedProperty(strchooser);
                    break;                    

                    #endregion
                case 31:
                    #region Wait

                    text = "等待 " + property.FindPropertyRelative("waitTime").floatValue + " s";
                    
                    break;

                    #endregion
                case 32: //Model.UnitType.FadeIn
                    #region FadeIn

                    text = "画面逐渐正常";
                    
                    break;

                    #endregion
                case 33: //Model.UnitType.FadeOut
                    #region FadeOut

                    text = "画面逐渐变色";
                    break;

                    #endregion
                case 40:
                    #region Panel
                    
                    var operateType = property.FindPropertyRelative("panelType");
                    switch (operateType.enumValueIndex)
                    {
                        case 0://Push
                            text = "Push " + StringChooser.GetShowTextFromSerializedProperty(
                                       property.FindPropertyRelative("panelChooser"));
                            break;
                        case 1://Pop
                            text = "Pop Panel";
                            break;
                    }
                    break;
                    

                    #endregion
                case 41:
                    #region Scene

                    if (property.FindPropertyRelative("useSceneMgr").boolValue &&
                        property.FindPropertyRelative("loadBack").boolValue)
                        text = "LoadBack";
                    else
                        text = "LoadScene: " +
                           StringChooser.GetShowTextFromSerializedProperty(
                               property.FindPropertyRelative("sceneNameChooser"));
                    
                    break;                    

                    #endregion
                
                
                case 70:

                    #region Jump

                    var targetAssetProp = property.FindPropertyRelative("targetAsset");
                    if (targetAssetProp.objectReferenceValue)
                        text = "跳转：" + (targetAssetProp.objectReferenceValue as AbstractDialogInfoAsset).name;
                    else
                    {
                        error = true;
                        text = "跳转对象未设置";
                    }
                    break;
                #endregion
                case 90://Model.UnitType.Progress

                    text += ProgressPointChooser.GetShowTextFromSerializedProperty(
                        property.FindPropertyRelative("progressValueToSet"));
                    break;
                
                
                case 80://Model.UnitType.VarInfo
                    text = "运行时打印变量信息";
                    break;
                case 81://Model.UnitType.Print
                    text = "Print:" + property.FindPropertyRelative("wordToPrint").stringValue;
                    break;
                case 82://Model.UnitType.ProcessInfo
                    text = "运行时打印进度信息";
                    break;
                default:
                    break;
            }
            
            
            if(error)
                EditorGUI.HelpBox(position,text,MessageType.Error);
            else
                EditorGUI.LabelField(position,text);
        }
    }
}
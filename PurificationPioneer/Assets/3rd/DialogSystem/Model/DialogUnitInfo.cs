using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DialogSystem.ScriptObject;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Model.SceneSystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using ReadyGamerOne.View.Effects;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
    
using UnityEditor;
using UnityEditorInternal;
#endif

namespace DialogSystem.Model
{
    public enum UnitType
    {
        Null = 0,
        Word = 10,
        Choose = 11,
        Narrator=12,
        ExWord=13,
        
        If=20,
        Else=21,
        EndIf=22,
        End=23,
        Skip=24,
        
        //闪烁
        Shining=30,
        Wait=31,
        FadeIn=32,
        FadeOut=33,
        Audio=34,
        
        
        Panel=40,//切换Panel
        Scene=41,//切换场景
        
        Message = 50,
        Event=51,
        
        SetVar=60,
        
        Jump=70,//跳转对话资源
        
        VarInfo=80,//显示变量信息
        Print=81,
        ProcessInfo=82,//显示当前进度信息
        
        Progress=90, //设置当前进度
        
        Camera=100 //相机移动
        
    }

    public enum ShowType
    {
        Dialog,
        Caption
    }

    public enum NarratorType
    {
        Color,
        Image,
        Object
    }

    public enum AudioType
    {
        BGM,
        Effect,
        StopBGM,
    }

    [Serializable]
    public class DialogUnitInfo
    {
#pragma warning disable 649

        #region Static
        
        private static Dictionary<string, List<string>> messageCache = new Dictionary<string, List<string>>();

        private static void SetAssetMessage(string message,string assetName)
        {
//            Debug.Log("DialogUnitInfo_AddAssetMessage:" + assetName);
            if (!messageCache.ContainsKey(message))
            {
                messageCache.Add(message,new List<string>());
            }

            if (!messageCache[message].Contains(assetName))
                messageCache[message].Add(assetName);
        }

        private static bool GetAssetMessage(string message, string assetName)
        {
            if (!messageCache.ContainsKey(message))
            {
                return false;
            }

            if (!messageCache[message].Contains(assetName))
                return false;

            messageCache[message].Remove(assetName);
            return true;
        }

        static DialogUnitInfo()
        {
            CEventCenter.AddListener<string>(Scripts.DialogSystem.EndThisDialogUnit,
                (assetName) =>
                {
                    SetAssetMessage(Scripts.DialogSystem.EndThisDialogUnit, assetName);
                });
        }        

        #endregion

        #region Editor

#if UNITY_EDITOR
        private ReorderableList skipChoiceList;
        public ReorderableList GetReorderableList(SerializedProperty property)
        {
                    skipChoiceList = new ReorderableList(property.serializedObject,property, true, true, true, true)
                    {
                        elementHeight = 4 * EditorGUIUtility.singleLineHeight
                    };

                    skipChoiceList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "选项"); };
                    
                    //绘制单个元素
                    skipChoiceList.drawElementCallback =
                        (rect, index, isActive, isFocused) =>
                        {
                            var choiceProp = property.GetArrayElementAtIndex(index);
                            var enableProp=choiceProp.FindPropertyRelative("enable");
                            var assetProp = enableProp.FindPropertyRelative("abstractAbstractDialogInfoAsset");
                            if (assetProp.objectReferenceValue == null)
                            {
                                assetProp.objectReferenceValue = abstractAbstractDialogInfoAsset;
                                enableProp.FindPropertyRelative("valueType").enumValueIndex = 0;
                                enableProp.FindPropertyRelative("argType").enumValueIndex = 3;
                                var argChooserProp=enableProp.FindPropertyRelative("ArgChooser");
                                argChooserProp.FindPropertyRelative("argType").enumValueIndex = 3;
                                argChooserProp.FindPropertyRelative("BoolArg").boolValue = true;
                            }
                            EditorGUI.PropertyField(rect,choiceProp);
                        };
                    return skipChoiceList;
        }
        public void onDrawMoreInfo(SerializedProperty property,Rect position)
        {
            #region Shit

            var index = 0;
            var dialogSystem = abstractAbstractDialogInfoAsset;
            
            GUIStyle style = new GUIStyle()
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
            };
            
            EditorGUI.LabelField(position.GetRectFromIndexWithHeight(ref index,20+5),"详细信息",style);
            
            EditorGUI.LabelField(position.GetRectAtIndex(index++),"类型",UnitType.ToString());
            EditorGUI.LabelField(position.GetRectAtIndex(index++), "ID", id.ToString());


            var characterProp = property.FindPropertyRelative("character");
            var msgProp=property.FindPropertyRelative("workMessageTrigger");
            var sceneNameChooserProp = property.FindPropertyRelative("sceneNameChooser");
            var panelNameChooserProp = property.FindPropertyRelative("panelChooser");
            var audioNameChooserProp = property.FindPropertyRelative("m_AudioName");

            if (abstractAbstractDialogInfoAsset.refreshStringChooser)
            {
                if(abstractAbstractDialogInfoAsset.MessageType!=null)
                    EditorUtil.InitSerializedStringArray(msgProp.FindPropertyRelative("messageToTick.values"),
                        abstractAbstractDialogInfoAsset.MessageType);
                if(abstractAbstractDialogInfoAsset.SceneType!=null)
                    EditorUtil.InitSerializedStringArray(sceneNameChooserProp.FindPropertyRelative("values"), 
                        abstractAbstractDialogInfoAsset.SceneType);
                if(abstractAbstractDialogInfoAsset.PanelType!=null)
                    EditorUtil.InitSerializedStringArray(panelNameChooserProp.FindPropertyRelative("values"),
                        abstractAbstractDialogInfoAsset.PanelType);
                if(abstractAbstractDialogInfoAsset.AudioType!=null)
                    EditorUtil.InitSerializedStringArray(audioNameChooserProp.FindPropertyRelative("values"),
                        abstractAbstractDialogInfoAsset.AudioType);
                    
            }            

            #endregion

            
            switch ((int)UnitType)
            {
                case 10: //Model.UnitType.Word
                    #region Word
                    
                    var showTypeProp = property.FindPropertyRelative("showType");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), showTypeProp);
                    var prefabIndexProp = property.FindPropertyRelative("wordPrefabIndex");
                    switch (showTypeProp.enumValueIndex)
                    {
                        case 0://Dialog   
                            var fromNullProp = property.FindPropertyRelative("fromNull");
                            EditorGUI.BeginChangeCheck();
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++),fromNullProp);

                            if (fromNullProp.boolValue)
                            {
                                if (EditorGUI.EndChangeCheck())
                                {
                                    prefabIndexProp.intValue = 2;
                                }
                            }
                            else
                            {
                                EditorGUI.PropertyField(position.GetRectAtIndex(index++), characterProp);
                            }
                            
                            if(abstractAbstractDialogInfoAsset.DialogWordUiKeys==null)
                                EditorGUI.HelpBox(position.GetRectAtIndex(index++),"未设置DialogWordUiPaths",MessageType.Error);
                            else
                                prefabIndexProp.intValue = EditorGUI.Popup(position.GetRectAtIndex(index++), "预制体",
                                    prefabIndexProp.intValue, abstractAbstractDialogInfoAsset.DialogWordUiKeys);            
                            break;
                        case 1://Caption
                            if(abstractAbstractDialogInfoAsset.CaptionWordUiKeys==null)
                                EditorGUI.HelpBox(position.GetRectAtIndex(index++),"未设置CaptionWordUiPaths",MessageType.Error);
                            else
                                prefabIndexProp.intValue = EditorGUI.Popup(position.GetRectAtIndex(index++), "预制体",
                                    prefabIndexProp.intValue, abstractAbstractDialogInfoAsset.CaptionWordUiKeys); 
                            
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), characterProp);
                            break;
                    }

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("words"));
                    break;                    

                    #endregion
                case 11: //Model.UnitType.Choose
                    #region Choose


                    //InitSerializedStringArray(characterProp
                    //    .FindPropertyRelative("values"), abstractAbstractDialogInfoAsset.SpeakerType);

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), characterProp);
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("title"));

                    var timeLimitProp = property.FindPropertyRelative("timeLimit");
                    
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),timeLimitProp);
                    if (timeLimitProp.boolValue)
                    {
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                            property.FindPropertyRelative("limitTime"));
                        var defaultIndexProp = property.FindPropertyRelative("defaultIndex");

                        var strArr = new string[this.Choices.Count];
                        var intArr = new int[this.Choices.Count];
                        for (var i = 0; i < Choices.Count; i++)
                        {
                            strArr[i] = i.ToString();
                            intArr[i] = i;
                        }

                        defaultIndexProp.intValue = EditorGUI.IntPopup(position.GetRectAtIndex(index++), "默认选择的Index",
                            defaultIndexProp.intValue, strArr, intArr);
                    }

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("sendToExWord"));
                    
                    
                    var choicesProp = property.FindPropertyRelative("Choices");
                    if (GUILayout.Button("清空选项列表"))
                    {
                        choicesProp.arraySize = 0;
                    }         
                    var list = GetReorderableList(choicesProp);           
                    list.DoLayoutList();

                    
                    break;                    

                    #endregion
                case 12: //Model.UnitType.Narrator
                    #region Narrator

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("wordToNarrator"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("narratorSpeed"));

                    var narratorTypeProp = property.FindPropertyRelative("m_NarratorType");
                    var narratorColorProp = property.FindPropertyRelative("m_NarratorColor");
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),narratorTypeProp);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (narratorTypeProp.enumValueIndex == 0)
                        {
                            narratorColorProp.colorValue=Color.black;
                        }
                        else
                        {
                            narratorColorProp.colorValue=Color.white;
                        }
                    }
                    
                    switch (narratorTypeProp.enumValueIndex)
                    {
                        case 0:// Color
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), narratorColorProp);
                            break;
                        case 1:// Image
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                                property.FindPropertyRelative("m_NarratorImage"));
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), narratorColorProp);
                            break;
                        case 2:// Object
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                                property.FindPropertyRelative("m_NarratorObject"));
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                                property.FindPropertyRelative("m_NarratorImage"));
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), narratorColorProp);
                            break;
                    }
                    
                    var enableFadeOutPorp = property.FindPropertyRelative("enableFadeOut");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),enableFadeOutPorp);
                    if (enableFadeOutPorp.boolValue)
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                            property.FindPropertyRelative("narratorTextFadeOutTime"));
                    
                    
                    break;

                #endregion
                case 13: //Model.UnitType.ExWord
                    #region ExWord

                    //InitSerializedStringArray(characterProp
                    //    .FindPropertyRelative("values"), abstractAbstractDialogInfoAsset.SpeakerType);
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("needInput"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), characterProp);
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("words"));
                    break;
                #endregion
                case 20:
                    #region IF

                    ValueChooser.argTypeWidth = 0.3f;
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("vc1"));
                    var compareProp = property.FindPropertyRelative("compareType");
                    compareProp.intValue = EditorGUI.Popup(position.GetRectAtIndex(index++), compareProp.intValue,
                        new[] {"==", "!=", "<", ">", ">=", "<="});
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("vc2"));

                    break;                    

                    #endregion
                case 24: //Model.UnitType.Skip
                    #region Skip

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("skipNum"));
                    break;                    

                    #endregion
                case 60:
                    #region RunSetVarUnit

                    if (dialogSystem.VarAsset.varInfos.Count == 0)
                    {
                        var rect = position.GetRectAtIndex(index++);
                        rect.height *= 4;
                        EditorGUI.HelpBox(rect,"现在 Assets/DialogSystemAssets/GlobalVar.asset 中没有变量",MessageType.Error);
                        break;
                    }
                    
                    var varTypeR = position.GetRectAtIndex(index++);

                    var varInfos = dialogSystem.GetValueInfos();
                    var keyArray = varInfos.Keys.ToArray();

                    var varIndexProp = property.FindPropertyRelative("varIndex");
                    varIndexProp.intValue = EditorGUI.Popup(varTypeR.GetLeft(0.3f), varIndexProp.intValue,
                        keyArray);
                    
                    var varToSetProp = property.FindPropertyRelative("varToSet");
                    
                    ArgChooser.typeChangAble = false;
                    varToSetProp.FindPropertyRelative("argType").enumValueIndex =
                        (int) varInfos[keyArray[varIndexProp.intValue]].argType;
                    EditorGUI.PropertyField(varTypeR.GetRight(0.7f), varToSetProp);
                    
                    break;
                    #endregion
                case 50: //Model.UnitType.Message
                    #region Message
                    ValueChooser.argTypeWidth = 0.235f;
                    EditorGUI.PropertyField(position.GetRectFromIndexWithHeight(ref index,EditorGUI.GetPropertyHeight(msgProp)), msgProp);
                    break;                    

                    #endregion
                case 51:  
                    #region Event

                    
                    var eventProp = property.FindPropertyRelative("myEvent");
                    EditorGUI.PropertyField(
                        position.GetRectFromIndexWithHeight(ref index, EditorGUI.GetPropertyHeight(eventProp)),
                        eventProp);
                    break;

                    #endregion
                case 52:
                    #region RunPrintUnit

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("wordToPrint"));
                    break;                    

                    #endregion
                case 30:
                    #region Shining

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("shiningObject"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("times"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("shiningDeltaTime"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("lightTime"));
                    break;                    

                    #endregion
                case 31:
                    #region Wait

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("waitTime"));
                    
                    break;

                    #endregion
                case 32: //Model.UnitType.FadeIn
                    #region FadeIn

                    
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("fadeInTime"));
                    break;

                    #endregion
                case 33: //Model.UnitType.FadeOut
                    #region FadeOut

                    
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),property.FindPropertyRelative("fadeOutTime"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), property.FindPropertyRelative("finalColor"));
                    break;
                

                    #endregion
                
                case 34: //Model.UnitType.Audio

                    var audioTypeProp = property.FindPropertyRelative("m_AudioType");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), audioTypeProp);
                    if (audioTypeProp.enumValueIndex != (int) AudioType.StopBGM)
                    {
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++), audioNameChooserProp);
                    }
                    
                    break;
                
                case 40:
                    #region Panel
                    
                    var operateType = property.FindPropertyRelative("panelType");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++), operateType);
                    switch (operateType.enumValueIndex)
                    {
                        case 0://Push
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++),panelNameChooserProp);
                            var fadeProp = property.FindPropertyRelative("fade");
                            //fadeProp.boolValue = EditorGUI.Toggle(position.GetRectAtIndex(index++),"是否渐隐", fadeProp.boolValue);
                            EditorGUI.PropertyField(position.GetRectAtIndex(index++), fadeProp);
                            if (fadeProp.boolValue)
                            {
                                var fadeValue = property.FindPropertyRelative("floatValue");
                                fadeValue.floatValue =
                                    EditorGUI.Slider(position.GetRectAtIndex(index++), "渐隐时间",fadeValue.floatValue, 0, 5f);
                            }
                            break;
                        case 1://Pop
                            break;
                    }
                    break;
                    

                    #endregion
                case 41:
                    #region Scene

                    var mgr = property.FindPropertyRelative("useSceneMgr");
                    var backProp = property.FindPropertyRelative("loadBack");
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),mgr);
                    if(mgr.boolValue)
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++), backProp);
                    if(mgr.boolValue && !backProp.boolValue)
                        EditorGUI.PropertyField(position.GetRectAtIndex(index++),sceneNameChooserProp);
                    break;                    

                    #endregion
                case 70://Model.UnitType.Jump
                    #region Jump

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("targetAsset"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("willBack"));

                    break;
                    #endregion
                case 80://Model.UnitType.VarInfo
                    #region VarInfo

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("varInfoHelpText"));
                    break;                    

                    #endregion
                case 81://Model.UnitType.Print
                    #region Print

                    EditorGUI.PropertyField(position.GetRectAtIndex(index),
                        property.FindPropertyRelative("wordToPrint"));                    

                    break;
                    #endregion
                case 90://Model.UnitType.Progress
                    #region Progress

                    var pos = position.GetRectAtIndex(index++);
                    EditorGUI.LabelField(pos.GetLeft(0.4f), "将要设定的全局进度值");
                    var value = ProgressPointChooser.ShowLabel;
                    ProgressPointChooser.ShowLabel = false;
                    EditorGUI.PropertyField(pos.GetRight(0.6f),
                        property.FindPropertyRelative("progressValueToSet"));
                    ProgressPointChooser.ShowLabel = value;
                    break;

                    

                    #endregion
                case 100://Model.UnitType.Camera
                    #region Camera

                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("positionOffset"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("targetOrigSize"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("cameraMoveTime"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("cameraMachinePath"));
                    EditorGUI.PropertyField(position.GetRectAtIndex(index++),
                        property.FindPropertyRelative("cameraWaitTime"));

                    break;
                    #endregion
                default:
                    break;
            }
        }

#endif        

        #endregion
        
        
        #region Every

        /// <summary>
        /// 物体上的DialogSystem脚本
        /// </summary>
        public AbstractDialogInfoAsset abstractAbstractDialogInfoAsset;
        /// <summary>
        /// 单元类型
        /// </summary>
        public UnitType UnitType;


        /// <summary>
        /// ID
        /// </summary>
        public int id;

        #endregion

        public string SpeakerName => (character).CharacterName;

        #region Word

        public ShowType showType;
        

        public int wordPrefabIndex = 0;
        
        public bool fromNull = false;
        /// <summary>
        /// 讲述人
        /// </summary>
        public CharacterChooser character;
        /// <summary>
        /// 讲述人说的话
        /// </summary>
        public string words;

        public IEnumerator RunWordUnit()
        {
            abstractAbstractDialogInfoAsset.CreateWordUI(this);
            while (!GetAssetMessage(Scripts.DialogSystem.EndThisDialogUnit, abstractAbstractDialogInfoAsset.name))
            {
//                Debug.Log("woc");
                yield return null;
            }
        }

        #endregion

        #region ExWord

        public bool needInput = false;

        public IEnumerator RunExWordUnit()
        {
            //如果需要用户输入才能下一步，这里就循环
            if(needInput)
                while (!abstractAbstractDialogInfoAsset.CanGoToNext())
                    yield return null;
            
            CEventCenter.BroadMessage(Scripts.DialogSystem.ExternWord, this);

            while (!GetAssetMessage(Scripts.DialogSystem.EndThisDialogUnit, abstractAbstractDialogInfoAsset.name))
                yield return null;
        }

        #endregion


        #region Choose

        /// <summary>
        /// 选择的题目
        /// </summary>
        [SerializeField] internal string title;

        public List<SkipChoice> Choices;

        public bool timeLimit = false;
        public float limitTime = 1.0f;
        public int defaultIndex = 0;
        public bool sendToExWord = false;

        public IEnumerator RunChooseUnit()
        {
           abstractAbstractDialogInfoAsset.CreateChooseUi(this);

           while (!GetAssetMessage(Scripts.DialogSystem.EndThisDialogUnit, abstractAbstractDialogInfoAsset.name))
           {
               if (Scripts.DialogSystem.GetAssetMessage(Scripts.DialogSystem.ChooseNotBack,abstractAbstractDialogInfoAsset.name,false))
               {
                   yield break;
               }
               yield return null;
           }
        }
        
        #endregion

        #region Narrator
        
        

        public string wordToNarrator;
        public float narratorSpeed;
        public bool enableFadeOut = true;
        public float narratorTextFadeOutTime = 1.0f;

        public NarratorType m_NarratorType;
        public Color m_NarratorColor=Color.black;
        public Sprite m_NarratorImage;
        public GameObject m_NarratorObject;
        
        
        public IEnumerator RunNarratorUnit()
        {
            if(abstractAbstractDialogInfoAsset.CreateNarratorUI==null)
                throw new Exception("你没有重写CreateNarratorUI属性！");
            abstractAbstractDialogInfoAsset.CreateNarratorUI.Invoke(this);
                        
            while (!GetAssetMessage(Scripts.DialogSystem.EndThisDialogUnit, abstractAbstractDialogInfoAsset.name))
                yield return null;
        }
        
        #endregion


        #region Jump

        public AbstractDialogInfoAsset targetAsset;
        public bool willBack = true;

        public IEnumerator RunJumpUnit()
        {
            if (willBack == false)
                CEventCenter.BroadMessage(Scripts.DialogSystem.ChooseNotBack, abstractAbstractDialogInfoAsset.name);
            yield return Scripts.DialogSystem.RunDialog(targetAsset);
        }

        #endregion
        
        
        #region Message

        /// <summary>
        /// 消息触发器
        /// </summary>
        [SerializeField] private MessageTrigger workMessageTrigger;

        public void RunMessageUnit()
        {
            this.workMessageTrigger.TickMessage();
        }

        #endregion


        #region VarInfo

        public string varInfoHelpText;
        
        public string GetVarInfo()
        {
            var text = "所在对话资源名:\t" + abstractAbstractDialogInfoAsset.name+"\n";
            text += "变量资源ID:\t\t" + abstractAbstractDialogInfoAsset.VarAsset.GetInstanceID() + "\n";
            text += "变量信息：\n";
            foreach (var VARIABLE in abstractAbstractDialogInfoAsset.VarAsset.varInfos)
            {
                text += VARIABLE.ArgChooser.argType + "\t" +VARIABLE.ArgChooser.GetValueString() + ":\t" 
                         + VARIABLE.VarName+ "\n";
            }

            return text;
        }

        public void ShowVarInfo()
        {
            var text = "《变量信息";
            if (!string.IsNullOrEmpty(varInfoHelpText))
                text += "——"+varInfoHelpText+"》\n";
            else
            {
                text += "》\n";
            }
            text += GetVarInfo() + "\n";
            
            Debug.Log(text);
        }

        #endregion
        

        #region SetVar

        [SerializeField] private int varIndex;

        [SerializeField] private ArgChooser varToSet;

        public void RunSetVarUnit()
        {
            abstractAbstractDialogInfoAsset.VarAsset.varInfos[varIndex].ArgChooser.SetValue(varToSet);
        }
        

        #endregion


        #region IF

        [SerializeField] private ValueChooser vc1;
        [SerializeField] private ValueChooser vc2;
        /// <summary>
        /// 0  ==
        /// 1  !=
        /// 2  <
        /// 3  >
        /// 4  >=
        /// 5  <=
        /// </summary>
        [SerializeField] private int compareType;

        public bool Complare()
        {
            switch (compareType)
            {
                case 0:
                    return vc1 == vc2;
                case 1:
                    return vc1 != vc2;
                case 2:
                    return vc1 < vc2;
                case 3:
                    return vc1 > vc2;
                case 4:
                    return vc1 >= vc2;
                case 5:
                    return vc1 <= vc2;
            }

            throw new Exception("比较异常");
        }
        
        #endregion

        #region Skip

        public int skipNum = 1;

        #endregion

        
        #region Action


        [SerializeField] private UnityEvent myEvent;

        public void RunEventUnit()
        {
            myEvent.Invoke();
        }

        #endregion
        
        #region Print


        [SerializeField] private string wordToPrint;

        public void RunPrintUnit()
        {
            Debug.Log(wordToPrint);
        }

        #endregion

        #region Shining

        [SerializeField] private GameObject shiningObject;
        [SerializeField] private int times;
        [SerializeField] private float shiningDeltaTime;
        [SerializeField] private float lightTime;

        public IEnumerator Shining()
        {
            if(shiningObject==null)
                yield break;
            if(shiningObject.activeSelf)
                shiningObject.SetActive(false);
            for (var i = 0; i < times; i++)
            {
                shiningObject.SetActive(true);
                yield return new WaitForSeconds(lightTime);
                shiningObject.SetActive(false);
                yield return new WaitForSeconds(Random.Range(0,shiningDeltaTime));
            }
            
        }
        
        
        #endregion

        #region Audio

        public AudioType m_AudioType;
        public StringChooser m_AudioName;

        public void RunAudio()
        {
            switch (m_AudioType)
            {
                case AudioType.Effect:
                    AudioMgr.Instance.PlayEffect(m_AudioName.StringValue);
                    break;
                case AudioType.BGM:
                    AudioMgr.Instance.PlayBgm(m_AudioName.StringValue);
                    break;
                default:
                    throw new NotImplementedException($"还没做:{m_AudioType}");
            }
        }

        #endregion

        #region FadeIn

        public float fadeInTime=1.0f;

        public IEnumerator FadeIn()
        {
            yield return PanelMgr.FadeIn(fadeInTime);
        }
        
        #endregion

        #region FadeOut


        public float fadeOutTime=1.5f;
        public Color finalColor=Color.black;

        public IEnumerator FadeOut()
        {
            yield return PanelMgr.FadeOut(fadeOutTime, finalColor);
        }
        
        #endregion

        #region Wait

        public float waitTime;

        public IEnumerator WaitForTime()
        {
            if (abstractAbstractDialogInfoAsset.affectedByTimeScale)
                yield return new WaitForSeconds(waitTime);
            else
            {
                for (var i = 0; i < waitTime / 0.02f; i++)
                    yield return null;
            }

        }

        #endregion

        #region Panel

        
        public enum PanelOperateType
        {
            Push,
            Pop
        }

        public PanelOperateType panelType;
        public bool fade;
        public float floatValue;
        public StringChooser panelChooser;

        public void RunPanelUnit()
        {
            switch (panelType)
            {
                case DialogUnitInfo.PanelOperateType.Pop:
                    PanelMgr.PopPanel();
                    break;
                case DialogUnitInfo.PanelOperateType.Push:
                    Debug.Log("DialogUnitInfo_RunPanelUnit_Switch_PushPanel");
                    if(fade)
                        PanelMgr.PushPanel(panelChooser.StringValue,new FadeInOut(floatValue));
                    else
                    {
                        PanelMgr.PushPanel(panelChooser.StringValue);
                    }
                    break;
            }
        }

        #endregion

        #region Scene

        [SerializeField] private bool useSceneMgr = true;
        [SerializeField] private bool loadBack = false;
        [SerializeField] private StringChooser sceneNameChooser;

        public IEnumerator RunSceneUnit()
        {
//            Debug.Log("DialogUnitInfo_RunSceneUnit_Before");
            if (useSceneMgr)
            {
                if(loadBack)
                    yield return SceneMgr.LoadBack();
                else
                    yield return SceneMgr.LoadSceneCoroutine(sceneNameChooser.StringValue);
            }
            
//            Debug.Log("DialogUnitInfo_RunSceneUnit_End");

            throw new Exception("you must useSceneMgr");
        }
        

        #endregion

        #region Progress


        public ProgressPointChooser progressValueToSet;

        public void RunProgressUnit()
        {
            //保证就算走原来情节，进度不会倒退
            if (progressValueToSet.Value > DialogProgressAsset.Instance.CurrentProgress)
            {
                Debug.Log("设置游戏进度："+progressValueToSet.Value);
                DialogProgressAsset.Instance.CurrentProgress = progressValueToSet.Value+0.01f;
            }
        }

        #endregion

        #region ProgressInfo

        public void RunProgressInfoUnit()
        {
            var min = float.MinValue;
            var max = float.MaxValue;
            int minIndex=-1, maxIndex=-1;
            foreach (var VARIABLE in DialogProgressAsset.Instance.DialogProgressPoints)
            {
                if (VARIABLE.value > min && VARIABLE.value < DialogProgressAsset.Instance.CurrentProgress)
                {
                    min = VARIABLE.value;
                    minIndex = VARIABLE.index;
                }

                if (VARIABLE.value > DialogProgressAsset.Instance.CurrentProgress && VARIABLE.value < max)
                {
                    max = VARIABLE.value;
                    maxIndex = VARIABLE.index;
                }
            }

            var ans = "";
            ans += "《游戏进度信息》\n";
            if(minIndex!=-1)
                ans += DialogProgressAsset.Instance.DialogProgressPoints[minIndex].name +
                       $"【{DialogProgressAsset.Instance.DialogProgressPoints[minIndex].value}】\n";

            ans += "当前进度：" + DialogProgressAsset.Instance.CurrentProgress + "\n";
            
            if(maxIndex!=-1)
                 ans+=DialogProgressAsset.Instance.DialogProgressPoints[maxIndex].name +
                     $"【{DialogProgressAsset.Instance.DialogProgressPoints[maxIndex].value}】\n";

            Debug.Log(ans);
        }

        #endregion

        #region Camera

        public Vector3 positionOffset;
        public float targetOrigSize=1;
        public float cameraMoveTime;
        public string cameraMachinePath = "CM vcam1";
        public float cameraWaitTime = 0.5f;
        public IEnumerator RunCameraUnit()
        {
            var cameraMachine= Camera.main.transform.Find(cameraMachinePath).gameObject;
            cameraMachine.SetActive(false);
            var goTime = cameraMoveTime /2f;
            var backTime = cameraMoveTime - goTime;
            var startSize = Camera.main.orthographicSize;
            var startPos = Camera.main.transform.position;
            for (var timer = 0f; timer < goTime; timer += Time.deltaTime)
            {
                Camera.main.transform.position = Vector3.Lerp(startPos, startPos + positionOffset, timer / goTime);
                Camera.main.orthographicSize = Mathf.Lerp(startSize, targetOrigSize, timer / goTime);
                yield return null;
            }
            
            yield return new WaitForSeconds(cameraWaitTime);
            
            for (var timer = 0f; timer < backTime; timer += Time.deltaTime)
            {
                Camera.main.transform.position = Vector3.Lerp(startPos + positionOffset, startPos, timer / backTime);
                Camera.main.orthographicSize = Mathf.Lerp(targetOrigSize, startSize, timer / backTime);
                yield return null;
            }
            

            Camera.main.transform.position = startPos;
            Camera.main.orthographicSize = startSize;
            cameraMachine.SetActive(true);
        }

        #endregion
        
        


#pragma warning restore 649
    }
}
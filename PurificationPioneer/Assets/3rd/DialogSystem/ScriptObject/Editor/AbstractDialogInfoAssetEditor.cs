using DialogSystem.Model;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ReadyGamerOne.Editor;

namespace DialogSystem.ScriptObject.Editor
{
    [CustomEditor(typeof(AbstractDialogInfoAsset),true)]
    public class AbstractDialogInfoAssetEditor:UnityEditor.Editor
    {
        #region DialogUnitInfoList

        private ReorderableList dialogUnitList;
        private float offset = 0.1f;
//        private int _intOffset = 0;

        #endregion
        
        
        private int selectedItem = 1;
        private SerializedProperty wordFinishTimeProp;
        private SerializedProperty affectedByTimeProp;
        private SerializedProperty dialogUnitListProp;
        private SerializedProperty refreshProp;
        private SerializedProperty canPlayerMoveProp;
        private int selectIndex = -1;
        private Vector2 scrolPos;
        private Vector2 detailPos;
        
        private AbstractDialogInfoAsset _abstractDialogInfoAsset;
        
        
        private void OnEnable()
        {
            this._abstractDialogInfoAsset=target as AbstractDialogInfoAsset;
            
            this.wordFinishTimeProp = serializedObject.FindProperty("wordFinishTime");
            this.refreshProp = serializedObject.FindProperty("refreshStringChooser");
            this.affectedByTimeProp = serializedObject.FindProperty("affectedByTimeScale");
            this.canPlayerMoveProp = serializedObject.FindProperty("canMoveOnTalking");

            
            #region DialogUnitList

            foreach (var VARIABLE in _abstractDialogInfoAsset.DialogUnits)
            {
                if (VARIABLE.abstractAbstractDialogInfoAsset == null)
                {
                    VARIABLE.abstractAbstractDialogInfoAsset = _abstractDialogInfoAsset;
                }
            }

            dialogUnitListProp = serializedObject.FindProperty("DialogUnits");


            dialogUnitList = new ReorderableList(serializedObject, dialogUnitListProp, true, true, true, true)
            {
                elementHeight = 2 * EditorGUIUtility.singleLineHeight
            };


            //绘制单个元素
            dialogUnitList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    var dialogUnit = dialogUnitListProp.GetArrayElementAtIndex(index);
                    var type = _abstractDialogInfoAsset.DialogUnits[index];

                    EditorGUI.PropertyField(rect.GetRight(1 - _abstractDialogInfoAsset.GetIntOffset(index) * offset), dialogUnit);
                };

            //背景色
            dialogUnitList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
            {
                GUI.backgroundColor = isFocused ? new Color(1f, 1f, 0.05f) : Color.white;
            };

            dialogUnitList.onSelectCallback = (list) => { this.selectIndex = list.index; };

            dialogUnitList.onAddDropdownCallback = (rect, list) =>
            {
                var menu = new GenericMenu();
                var enumIndex = -1;
                foreach (var value in EnumUtil.GetValues<UnitType>())
                {   
                    enumIndex++;
                    if ((UnitType) value == UnitType.Panel && _abstractDialogInfoAsset.PanelType == null)
                        continue;
                    if ((UnitType) value == UnitType.Scene && _abstractDialogInfoAsset.SceneType == null) 
                        continue;
                    if ((UnitType) value == UnitType.Message && _abstractDialogInfoAsset.MessageType == null)
                        continue;
                    
                    var itemName = "";
                    if (value >=10  && value < 20)
                    {
                        itemName += "Dialog/";
                    }
                    else if (value >= 20 && value < 30)
                    {
                        itemName += "Logic/";
                    }else if (value >= 40 && value < 50)
                    {
                        itemName += "Game/";
                    }
                    else if (value >= 60 && value <70)
                    {
                        itemName += "Var/";
                    }
                    else if (value >= 50 && value < 60)
                    {
                        itemName += "Trigger/";
                    }
                    else if (value >= 30&&value<40)
                        itemName += "Effect/";
                    else if (value >= 80 && value < 90)
                        itemName += "Debug/";
                        
                    menu.AddItem(new GUIContent(itemName + (UnitType)value), false, OnAddUnitCallBack,enumIndex);

                }

                menu.ShowAsContext();
            };

            //头部
            dialogUnitList.drawHeaderCallback = (rect) =>
                EditorGUI.LabelField(rect, dialogUnitListProp.displayName);

            #endregion
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            scrolPos = GUILayout.BeginScrollView(scrolPos, false, true);
            selectedItem = GUILayout.SelectionGrid(selectedItem, new[] {"基本属性","对话系统"}, 2);

            switch (selectedItem)
            {
                case 0:
                    var style=new GUIStyle();
                    style.fontSize = 30;
                    style.alignment = TextAnchor.MiddleCenter;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(_abstractDialogInfoAsset.GetType().Name,style);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(canPlayerMoveProp);
                    EditorGUILayout.PropertyField(wordFinishTimeProp);
                    EditorGUILayout.PropertyField(affectedByTimeProp);
                    EditorGUILayout.PropertyField(refreshProp);
                    EditorGUILayout.Space();
                    if(_abstractDialogInfoAsset.MessageType!=null)
                        EditorGUILayout.LabelField("MessageType",_abstractDialogInfoAsset.MessageType.FullName);
                    if (_abstractDialogInfoAsset.PanelType != null)
                        EditorGUILayout.LabelField("PanelType", _abstractDialogInfoAsset.PanelType.FullName);
                    if (_abstractDialogInfoAsset.SceneType != null)
                        EditorGUILayout.LabelField("SceneType", _abstractDialogInfoAsset.SceneType.FullName);
                    break;
                case 1:
                    
                    dialogUnitList.DoLayoutList();
                    break;
            }
            GUILayout.EndScrollView();

            if (selectIndex != -1 && selectIndex <dialogUnitListProp.arraySize)
            {

                var prop = dialogUnitListProp.GetArrayElementAtIndex(selectIndex);
                this.detailPos = GUILayout.BeginScrollView(this.detailPos ,GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                var rect = GUILayoutUtility.GetRect(100, EditorGUI.GetPropertyHeight(prop),
                    GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                _abstractDialogInfoAsset.DialogUnits[selectIndex].onDrawMoreInfo(prop, rect);
                GUILayout.EndScrollView();
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnAddUnitCallBack(object type)
        {
            int index = dialogUnitList.serializedProperty.arraySize;
            dialogUnitList.serializedProperty.arraySize++;
            
            dialogUnitList.index = index;
            selectIndex = index;

            var dialogUnit = dialogUnitList.serializedProperty.GetArrayElementAtIndex(index);

            dialogUnit.FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue = _abstractDialogInfoAsset;

            var unitTypeProp = dialogUnit.FindPropertyRelative("UnitType");
            unitTypeProp.enumValueIndex = (int) type;
            switch (EnumUtil.GetEnumValue<UnitType>((int)type))
            {
                case 20: // If
                    dialogUnit.FindPropertyRelative("vc1").FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue =
                        _abstractDialogInfoAsset;
                    dialogUnit.FindPropertyRelative("vc2").FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue =
                        _abstractDialogInfoAsset;
                    break;
                case 50: // Message
                    EditorUtil.InitSerializedStringArray(dialogUnit.FindPropertyRelative("workMessageTrigger")
                            .FindPropertyRelative("messageToTick").FindPropertyRelative("values"),
                        _abstractDialogInfoAsset.MessageType);
                    dialogUnit.FindPropertyRelative("workMessageTrigger").FindPropertyRelative("arg1")
                            .FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue =
                        _abstractDialogInfoAsset;
                    dialogUnit.FindPropertyRelative("workMessageTrigger").FindPropertyRelative("arg2")
                            .FindPropertyRelative("abstractAbstractDialogInfoAsset").objectReferenceValue =
                        _abstractDialogInfoAsset;
                    break;
                case 40://Panel
                    EditorUtil.InitSerializedStringArray(dialogUnit.FindPropertyRelative("panelChooser").FindPropertyRelative("values"),_abstractDialogInfoAsset.PanelType);
                    break;
                case 41://Scene
                    EditorUtil.InitSerializedStringArray(dialogUnit.FindPropertyRelative("sceneNameChooser").FindPropertyRelative("values"),_abstractDialogInfoAsset.SceneType);
                    break;
            }


            dialogUnit.FindPropertyRelative("id").intValue = _abstractDialogInfoAsset.GetID();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
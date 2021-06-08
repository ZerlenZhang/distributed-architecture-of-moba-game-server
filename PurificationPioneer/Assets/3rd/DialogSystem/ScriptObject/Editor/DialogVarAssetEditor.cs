using System;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DialogSystem.ScriptObject.Editor
{
    [CustomEditor(typeof(DialogVarAsset))]
    public class DialogVarAssetEditor:UnityEditor.Editor
    {
        private Vector2 pos;
        [MenuItem("ReadyGamerOne/DialogSystem/ShowDialogVars")]
        public static void CreateAsset()
        {
            Selection.activeInstanceID = DialogVarAsset.Instance.GetInstanceID();
        }
        
        private ReorderableList varList;

        private void OnEnable()
        {
            #region VarList

            var varListProp = serializedObject.FindProperty("varInfos");
            //new ReorderableList(globalVars, typeof(VarUnitInfo), false, true, true, true);
            this.varList = new ReorderableList(serializedObject, varListProp, false, true, true, true);
            if (this.varList == null)
                throw new Exception("wdnmd");

            varList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 4;
            //绘制单个元素
            varList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    var varInfoProp = varListProp.GetArrayElementAtIndex(index);
                    var left = 0.1f;
                    var s=new GUIStyle()
                    {
                        fontSize = 20
                    };
                    EditorGUI.LabelField(rect.GetLeft(left), index.ToString(),s);

                    EditorGUI.PropertyField(rect.GetRight(1-left), varInfoProp);
                };


            //添加单元下拉菜单
            varList.onAddDropdownCallback = (rect, list) =>
            {
                var menu = new GenericMenu();
                int index = 0;
                foreach (var unitType in Enum.GetNames(typeof(ArgType)))
                {
                    menu.AddItem(new GUIContent(unitType), false, OnAddVarCallBack, index++);
                }

                menu.ShowAsContext();
            };
            #endregion
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            pos = EditorGUILayout.BeginScrollView(pos);
            varList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnAddVarCallBack(object type)
        {
            var index = varList.serializedProperty.arraySize;
            varList.serializedProperty.arraySize++;
            varList.index = index;

            var newVar = varList.serializedProperty.GetArrayElementAtIndex(index);
            newVar.FindPropertyRelative("VarName").stringValue = "";
            newVar.FindPropertyRelative("ArgChooser").FindPropertyRelative("argType").enumValueIndex = (int) type;
            serializedObject.ApplyModifiedProperties();
        }

    }
    
    
}
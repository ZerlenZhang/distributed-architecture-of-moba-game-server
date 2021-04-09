using UnityEditor;
using DialogSystem.ScriptObject;
using ReadyGamerOne.Utility;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DialogSystem.Scripts.Editor
{
    [UnityEditor.CustomEditor(typeof(DialogSystem))]
    public class DialogSystemEditor:UnityEditor.Editor
    {
        private DialogSystem dialogSystem;
        private SerializedProperty dialogInfoAssetProp;

        private ReorderableList dialogUnitList;

        private void OnEnable()
        {
            this.dialogSystem = target as DialogSystem;
            this.dialogInfoAssetProp = serializedObject.FindProperty("DialogInfoAssets");

            dialogUnitList = new ReorderableList(serializedObject, dialogInfoAssetProp, true, true, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight
            };


            //绘制单个元素
            dialogUnitList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    var dialogUnit = dialogInfoAssetProp.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, dialogUnit);
                };

            //背景色
            dialogUnitList.drawElementBackgroundCallback = (rect, index, isActive, isFocused) =>
            {
                GUI.backgroundColor = isFocused ? Color.yellow : Color.white;
            };
            //头部
            dialogUnitList.drawHeaderCallback = (rect) =>
                EditorGUI.LabelField(rect, dialogInfoAssetProp.displayName);


        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            var list = EditorUtil.GetDragObjectsFromArea<AbstractDialogInfoAsset>("拖拽 SimpleDialogInfoAsset 资源到这里添加到List中去");
            
            foreach(var asset in list)
            {               
                if (!dialogSystem.DialogInfoAssets.Contains(asset))
                    dialogSystem.DialogInfoAssets.Add(asset);
            }

            if (list.Count != 0)
            {
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            
            dialogUnitList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            
        }
    }
}

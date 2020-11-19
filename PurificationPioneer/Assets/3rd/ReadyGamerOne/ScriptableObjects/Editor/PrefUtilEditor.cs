using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ReadyGamerOne.ScriptableObjects.Editor
{
    [CustomEditor(typeof(PrefUtil))]
    public class PrefUtilEditor:UnityEditor.Editor
    {
        private Vector2 pos;
        [MenuItem("ReadyGamerOne/Show/ConstStrings")]
        public static void CreateAsset()
        {
            Selection.activeInstanceID = PrefUtil.Instance.GetInstanceID();
        }


        private ReorderableList varList;
        private PrefUtil asset;
        private void OnEnable()
        {
            asset=target as PrefUtil;
            
            var varListProp = serializedObject.FindProperty("constStrings");
            this.varList = new ReorderableList(serializedObject, varListProp, false, true, true, true);
            //绘制单个元素
            varList.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.PropertyField(rect, varListProp.GetArrayElementAtIndex(index));
                };

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            pos = EditorGUILayout.BeginScrollView(pos);
            varList.DoLayoutList();
            EditorGUILayout.EndScrollView();

            foreach (var VARIABLE in asset.constStrings)
            {
                if (string.IsNullOrEmpty(VARIABLE))
                {
                    EditorGUILayout.HelpBox("字符串不得为空！！！",MessageType.Error);
                }
            }
            
            if(GUILayout.Button("Clear PrefUtil"))
                asset.prefItems.Clear();
            foreach(var kv in asset.prefItems)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(kv.key);
                GUILayout.FlexibleSpace();
                GUILayout.Label(kv.value);
                GUILayout.EndHorizontal();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
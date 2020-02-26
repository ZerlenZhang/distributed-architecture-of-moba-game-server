using System;
using ReadyGamerOne.View;
using UnityEditor;
using UnityEditorInternal;

namespace ReadyGamerOne.Script.Editor
{
    [CustomEditor(typeof(TabBar))]
    public class TabBarEditor:UnityEditor.Editor
    {
        private ReorderableList list;
        private SerializedProperty listProp;
        private TabBar tb;
        private void OnEnable()
        {
            this.tb=target as TabBar;
            listProp = serializedObject.FindProperty("tabPairs");
            list = new ReorderableList(serializedObject, listProp, false, true, true, true);
            list.elementHeight = 3 * EditorGUIUtility.singleLineHeight;
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var prop = listProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, prop);
            };
            list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, listProp.displayName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            list.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                CheckTabs();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void CheckTabs()
        {
            for (var i = 0; i < listProp.arraySize; i++)
            {
                var prop = listProp.GetArrayElementAtIndex(i);
                if (prop.FindPropertyRelative("tab").objectReferenceValue != null
                    && prop.FindPropertyRelative("page").objectReferenceValue != null)
                {
                    prop.FindPropertyRelative("index").intValue = i;
                }
                else if( i>0 && !CheckTabAtIndex(i-1))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                }
            }
        }

        private bool CheckTabAtIndex(int index)
        {
            var prop = listProp.GetArrayElementAtIndex(index);
            return prop.FindPropertyRelative("tab").objectReferenceValue != null && prop.FindPropertyRelative("page").objectReferenceValue != null;
        }
    }
}
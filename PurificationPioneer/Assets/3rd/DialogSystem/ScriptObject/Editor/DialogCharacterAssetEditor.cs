using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DialogSystem.ScriptObject.Editor
{
    [CustomEditor(typeof(DialogCharacterAsset))]
    public class DialogCharacterAssetEditor:UnityEditor.Editor
    {
        [MenuItem("ReadyGamerOne/DialogSystem/ShowDialogCharacters")]
        public static void CreateAsset()
        {
            Selection.activeInstanceID = DialogCharacterAsset.Instance.GetInstanceID();
        }


        private Vector2 pos;
        private SerializedProperty nameListProp;
        private DialogCharacterAsset targetObject;
        private ReorderableList list;
        private void OnEnable()
        {
            this.targetObject=target as DialogCharacterAsset;
            this.nameListProp = serializedObject.FindProperty("characterNames");
            
            list=new ReorderableList(serializedObject,nameListProp,true,true,true,true);

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var itemProp = nameListProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, itemProp);
            };

            list.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "角色名字");


        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            pos = EditorGUILayout.BeginScrollView(pos);
            list.DoLayoutList();
            EditorGUILayout.EndScrollView();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
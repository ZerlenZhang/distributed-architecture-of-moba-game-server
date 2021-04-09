using UnityEditor;
using UnityEditorInternal;

namespace DialogSystem.ScriptObject.Editor
{
    [CustomEditor(typeof(DialogProgressAsset))]
    public class DialogProgressAssetEditor:UnityEditor.Editor
    {
        [MenuItem("ReadyGamerOne/DialogSystem/ShowDialogProgressPoints")]
        public static void CreateAsset()
        {
            Selection.activeInstanceID = DialogProgressAsset.Instance.GetInstanceID();
        }
        
        
        
        private SerializedProperty progressPointProp;
        private ReorderableList progressPointList;
        private DialogProgressAsset _dialogProgressAsset;
        private SerializedProperty currentProgressValueProp;
        private void OnEnable()
        {
            this._dialogProgressAsset=target as DialogProgressAsset;
            this.progressPointProp = serializedObject.FindProperty("DialogProgressPoints");
            this.currentProgressValueProp = serializedObject.FindProperty("currentProgress");
            this.progressPointList = new ReorderableList(serializedObject, progressPointProp, false, true, true, true);

            this.progressPointList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;

            this.progressPointList.drawElementCallback = (rect, index, isActive, isFocus) =>
            {
                EditorGUI.PropertyField(rect, progressPointProp.GetArrayElementAtIndex(index));
            };

            this.progressPointList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "剧情关键点"); };

            this.progressPointList.onAddCallback = (list) =>
            {
                var index = progressPointProp.arraySize;
                progressPointProp.InsertArrayElementAtIndex(index);
                var ele = progressPointProp.GetArrayElementAtIndex(index);
                ele.FindPropertyRelative("index").intValue = index;
                ele.FindPropertyRelative("name").stringValue = "";
                ele.FindPropertyRelative("value").floatValue = 0f;
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(currentProgressValueProp);
            progressPointList.DoLayoutList();
            foreach (var VARIABLE in _dialogProgressAsset.DialogProgressPoints)
            {
                if (string.IsNullOrEmpty(VARIABLE.name))
                {
                    EditorGUILayout.HelpBox("进度关键点名字不能为空！！", MessageType.Error);
                    break;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
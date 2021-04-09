using UnityEditor;

namespace DialogSystem.ScriptObject.Editor
{
    [CustomEditor(typeof(DialogSettings))]
    public class DialogSettingsEditor:UnityEditor.Editor
    {
        [MenuItem("ReadyGamerOne/DialogSystem/ShowDialogSettings")]
        public static void CreateAsset()
        {
            Selection.activeInstanceID = DialogSettings.Instance.GetInstanceID();
        }

        private SerializedProperty showDialogStackInfoProp;

        private void OnEnable()
        {
            showDialogStackInfoProp = serializedObject.FindProperty("ShowDialogStackInfo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("请注意！！！");
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("1、切换Scene的时候不能调用CEventCenter.Clear");
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(showDialogStackInfoProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
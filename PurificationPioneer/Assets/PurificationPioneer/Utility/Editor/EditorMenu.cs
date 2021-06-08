using DialogSystem.ScriptObject;
using PurificationPioneer.Const;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PurificationPioneer.Utility
{
    public class EditorMenu
    {
        private const string ExpectedScenePath = "Assets/Scenes/PurificationPioneer/Welcome.unity";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.EnteredEditMode && EditorPrefs.GetBool(PrefUtil.ShouldRefreshSceneKey,false))
                {
                    var currentScene = SceneManager.GetActiveScene();
                    var lastScenePath = EditorPrefs.GetString(PrefUtil.LastScenePathKey);
                    EditorPrefs.SetBool(PrefUtil.ShouldRefreshSceneKey, false);
                    if (!string.IsNullOrEmpty(lastScenePath) && lastScenePath != currentScene.path)
                    {
                        EditorSceneManager.OpenScene(lastScenePath, OpenSceneMode.Single);
                    }
                }

                if (change == PlayModeStateChange.ExitingEditMode)
                {
                    ResetDialogVars();
                }
            };
        }
        
        [MenuItem("净化先锋/从Welcome场景启动 &#P")]
        private static void EnterPlayerMode()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning($"已经处于运行模式，无法重复启动");
                return;
            }

            var currentScene = SceneManager.GetActiveScene();

            if (currentScene.path == ExpectedScenePath)
            {
                EditorPrefs.SetBool(PrefUtil.ShouldRefreshSceneKey, false);
                EditorApplication.EnterPlaymode();
                return;
            }
            
            if (currentScene.isDirty)
            {
                EditorSceneManager.SaveOpenScenes();
            }
            
            EditorPrefs.SetBool(PrefUtil.ShouldRefreshSceneKey, true);
            EditorPrefs.SetString(PrefUtil.LastScenePathKey, currentScene.path);
            EditorSceneManager.OpenScene(ExpectedScenePath, OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }

        [MenuItem("净化先锋/重置Dialog变量")]
        private static void ResetDialogVars()
        {
            DialogVarAsset.Instance.varInfos[0].SetValue(false);
            DialogVarAsset.Instance.varInfos[1].SetValue(true);
            DialogVarAsset.Instance.varInfos[2].SetValue(true);
            DialogProgressAsset.Instance.SetProgress(1);
        }
    }
}
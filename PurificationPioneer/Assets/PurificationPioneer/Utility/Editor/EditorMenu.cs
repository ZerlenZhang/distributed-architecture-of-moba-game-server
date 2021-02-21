using PurificationPioneer.Const;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PurificationPioneer.Utility
{
    public class EditorMenu
    {

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                if (change == PlayModeStateChange.EnteredEditMode)
                {
                    var currentScene = SceneManager.GetActiveScene();
                    var lastScenePath = EditorPrefs.GetString(PrefUtil.LastScenePathKey);
                    if (!string.IsNullOrEmpty(lastScenePath) && lastScenePath != currentScene.path)
                    {
                        EditorSceneManager.OpenScene(lastScenePath, OpenSceneMode.Single);
                    }
                }
            };
        }
        
        [MenuItem("净化先锋/从Welcome场景启动 &P")]
        private static void EnterPlayerMode()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning($"已经处于运行模式，无法重复启动");
                return;
            }

            var expectedScenePath = $"Assets/Scenes/Welcome.unity";
            var currentScene = SceneManager.GetActiveScene();

            if (currentScene.path == expectedScenePath)
            {
                EditorApplication.EnterPlaymode();
                return;
            }
            
            if (currentScene.isDirty)
            {
                EditorSceneManager.SaveOpenScenes();
            }
            
            EditorPrefs.SetString(PrefUtil.LastScenePathKey, currentScene.path);
            var scene = EditorSceneManager.OpenScene(expectedScenePath, OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }
    }
}
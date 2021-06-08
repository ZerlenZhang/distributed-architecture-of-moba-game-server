namespace ReadyGamerOne.EditorExtension
{
    using UnityEngine;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    // Author: JohannesMP (2018-08-12)
    //
    // A wrapper that provides the means to safely serialize Scene Asset References.
    // 
    // Internally we serialize an Object to the SceneAsset which only exists at editor time.
    // Any time the object is serialized, we store the path provided by this Asset (assuming it was valid).
    // 
    // This means that, come build time, the string path of the scene asset is always already stored, which if 
    // the scene was added to the build settings means it can be loaded.
    // 
    // It is up to the user to ensure the scene exists in the build settings so it is loadable at runtime.
    // To help with this, a custom PropertyDrawer displays the scene build settings state.
    // 
    // Known issues:
    //     -   When reverting back to a prefab which has the asset stored as null, Unity will show the property 
    //         as modified despite having just reverted. This only happens the fist time, and reverting again 
    //         fixes it. Under the hood the state is still always valid, and serialized correctly regardless.


    /// <summary>
    /// A wrapper that provides the means to safely serialize Scene Asset References.
    /// </summary>
    [System.Serializable]
    public class SceneField : ISerializationCallbackReceiver
    {
    #if UNITY_EDITOR
        // What we use in editor to select the scene
        [SerializeField] private Object sceneAsset = null;
        bool IsValidSceneAsset
        {
            get
            {
                if (sceneAsset == null)
                    return false;
                return sceneAsset.GetType().Equals(typeof(SceneAsset));
            }
        }
    #endif

        // This should only ever be set during serialization/deserialization!
        [SerializeField]
        private string scenePath = string.Empty;

        // Use this when you want to actually have the scene path
        public string ScenePath
        {
            get
            {
    #if UNITY_EDITOR
                // In editor we always use the asset's path
                return GetScenePathFromAsset();
    #else
                // At runtime we rely on the stored path value which we assume was serialized correctly at build time.
                // See OnBeforeSerialize and OnAfterDeserialize
                return scenePath;
    #endif
            }
            set
            {
                scenePath = value;
    #if UNITY_EDITOR
                sceneAsset = GetSceneAssetFromPath();
    #endif
            }
        }

        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.ScenePath;
        }

        // Called to prepare this data for serialization. Stubbed out when not in editor.
        public void OnBeforeSerialize()
        {
    #if UNITY_EDITOR
            HandleBeforeSerialize();
    #endif
        }

        // Called to set up data for deserialization. Stubbed out when not in editor.
        public void OnAfterDeserialize()
        {
    #if UNITY_EDITOR
            // We sadly cannot touch assetdatabase during serialization, so defer by a bit.
            EditorApplication.update += HandleAfterDeserialize;
    #endif
        }



    #if UNITY_EDITOR
        private SceneAsset GetSceneAssetFromPath()
        {
            if (string.IsNullOrEmpty(scenePath))
                return null;
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        }

        private string GetScenePathFromAsset()
        {
            if (sceneAsset == null)
                return string.Empty;
            return AssetDatabase.GetAssetPath(sceneAsset);
        }

        private void HandleBeforeSerialize()
        {
            // Asset is invalid but have Path to try and recover from
            if (IsValidSceneAsset == false && string.IsNullOrEmpty(scenePath) == false)
            {
                sceneAsset = GetSceneAssetFromPath();
                if (sceneAsset == null)
                    scenePath = string.Empty;

                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
            // Asset takes precendence and overwrites Path
            else
            {
                scenePath = GetScenePathFromAsset();
            }
        }

        private void HandleAfterDeserialize()
        {
            EditorApplication.update -= HandleAfterDeserialize;
            // Asset is valid, don't do anything - Path will always be set based on it when it matters
            if (IsValidSceneAsset)
                return;

            // Asset is invalid but have path to try and recover from
            if (string.IsNullOrEmpty(scenePath) == false)
            {
                sceneAsset = GetSceneAssetFromPath();
                // No asset found, path was invalid. Make sure we don't carry over the old invalid path
                if (sceneAsset == null)
                    scenePath = string.Empty;

                if (Application.isPlaying == false)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
    #endif
    }

    
}
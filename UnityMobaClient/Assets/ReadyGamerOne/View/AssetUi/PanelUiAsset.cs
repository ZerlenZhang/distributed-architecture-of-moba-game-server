using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR

#endif
namespace ReadyGamerOne.View.AssetUi
{
    public class PanelUiAsset:BaseUiAsset
    {
        #region Editor

#if UNITY_EDITOR

        [MenuItem("ReadyGamerOne/Create/UI/PanelUiAsset")]
        public static void CreateAsset()
        {
            string[] strs = Selection.assetGUIDs;

            string path = AssetDatabase.GUIDToAssetPath(strs[0]);

            if (path.Contains("."))
            {
                path=path.Substring(0, path.LastIndexOf('/'));
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var targetFullPath = path + "/NewPanelUiAsset";

            if (File.Exists(targetFullPath + ".asset"))
                targetFullPath += "(1)";
            
            AssetDatabase.CreateAsset(CreateInstance<PanelUiAsset>(), targetFullPath + ".asset");
            AssetDatabase.Refresh();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath<PanelUiAsset>(targetFullPath + ".asset");
        }
        
#endif        

        #endregion

        #region Static

        public static event Action<PanelUiAsset> onPanelShow;
        public static event Action<PanelUiAsset> onPanelHide;
        public static event Action<PanelUiAsset> onPanelDestory;        

        #endregion

        public PanelUiAsset target;
        public KeyCode key = KeyCode.Space;

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKeyDown(key))
                PanelAssetMgr.PushPanel(target);
        }

        protected override void OnShow()
        {
        
            base.OnShow();
            onPanelShow?.Invoke(this);
        }

        protected override void OnHide()
        {
            base.OnHide();
            onPanelHide?.Invoke(this);
        }

        public override void DestoryObj(PointerEventData data = null)
        {
            base.DestoryObj(data);
            onPanelDestory?.Invoke(this);
        }
    }
}
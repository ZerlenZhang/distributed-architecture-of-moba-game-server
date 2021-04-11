using System;
using DialogSystem.Model;
using DialogSystem.ScriptObject;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;

namespace PurificationPioneer.Dialog
{
    public class PpDialogAsset:AbstractDialogInfoAsset
    {
#if UNITY_EDITOR
        [MenuItem("净化先锋/Create/DialogSystem/PpDialogAsset #&I")]
        public static void CreateAsset()
        {
            EditorUtil.CreateAsset<PpDialogAsset>("PpDialogAsset");
        }         
#endif

        public override Action<bool> SetPlayerMovable => state => GlobalVar.IsPlayerInControl = state;

        public override Func<bool> IfInteract => () => 
            Input.GetKeyDown(GameSettings.Instance.InteractKey);
        public override Func<bool> CanGoToNext => ()=> Input.GetKeyDown(GameSettings.Instance.DialogContinueKey)
            || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);


        public override Type MessageType => typeof(Message);
        public override Type PanelType => typeof(PanelName);
        public override Type SceneType => typeof(SceneName);


        #region Resources Cofigs

        public override string ChooseUiKeys => DialogName.CaptionChooseUi;

        public override Action<DialogUnitInfo> CreateChooseUi => unit => new PpCaptionChooseUi(unit);

        public override string[] CaptionWordUiKeys => new[]
        {
            DialogName.CaptionWordUi,
        };


        public override Action<DialogUnitInfo> CreateWordUI => unit => new PpCaptionWordUi(unit);


        public override string NarratorUiKeys => DialogName.CaptionNarratorUi;

        public override Action<DialogUnitInfo> CreateNarratorUI => unit => new PpNarratorUi(unit);        

        #endregion

    }
}
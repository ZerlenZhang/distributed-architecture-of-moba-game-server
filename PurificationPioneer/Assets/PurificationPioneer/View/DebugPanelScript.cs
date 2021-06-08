using PurificationPioneer.Const;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class DebugPanelScript : MonoBehaviour
    {
        public Toggle debugModeToggle;
        public Toggle enableSocketLogToggle;
        public Toggle enableProtoLogToggle;
        public Toggle closeSocketOnAnyException;

        public Toggle enableInputLogToggle;
        public Toggle enableMoveLogToggle;
        public Toggle ifShowGizmosToggle;
        public Toggle enableFrameSyncLogToggle;
        
        
        public Button startTestBtn;

        public void InitSettings()
        {
            enableFrameSyncLogToggle.isOn = GameSettings.Instance.EnableFrameSyncLog;
            enableInputLogToggle.isOn = GameSettings.Instance.EnableInputLog;
            enableMoveLogToggle.isOn = GameSettings.Instance.EnableMoveLog;
            ifShowGizmosToggle.isOn = GameSettings.Instance.IfShowGizmos;
            debugModeToggle.isOn = GameSettings.Instance.DeveloperMode;
            enableSocketLogToggle.isOn = GameSettings.Instance.EnableSocketLog;
            closeSocketOnAnyException.isOn = GameSettings.Instance.CloseSocketOnAnyException;
            enableProtoLogToggle.isOn = GameSettings.Instance.EnableProtoLog;
            startTestBtn.onClick.AddListener(
                () => PanelMgr.PushPanel(PanelName.WakeUpPanel));
        }
    }
}
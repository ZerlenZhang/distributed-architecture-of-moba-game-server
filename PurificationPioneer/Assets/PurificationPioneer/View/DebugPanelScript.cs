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
        public Button startTestBtn;

        public void InitSettings()
        {
            debugModeToggle.isOn = GameSettings.Instance.DebugMode;
            enableSocketLogToggle.isOn = GameSettings.Instance.EnableSocketLog;
            closeSocketOnAnyException.isOn = GameSettings.Instance.CloseSocketOnAnyException;
            enableProtoLogToggle.isOn = GameSettings.Instance.EnableProtoLog;
            startTestBtn.onClick.AddListener(
                () => PanelMgr.PushPanel(PanelName.WelcomePanel));
        }
    }
}
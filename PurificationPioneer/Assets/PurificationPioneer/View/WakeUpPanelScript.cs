using PurificationPioneer.Const;
using ReadyGamerOne.View;
using UnityEngine;

namespace PurificationPioneer.View
{
    public class WakeUpPanelScript : MonoBehaviour
    {
        public void BeginGame()
        {
            PanelMgr.PushPanel(PanelName.WelcomePanel);
        }
    }
}
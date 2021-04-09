using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.View;

namespace PurificationPioneer.Script
{
    public class WelcomeSceneMgr:PpSceneMgr<WelcomeSceneMgr>
    {
        protected override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            PanelMgr.PushPanel(PanelName.WelcomePanel);
#elif UNITY_STANDALONE_WIN
            if (GameSettings.Instance.DeveloperMode)
            {
                PanelMgr.PushPanel(PanelName.DebugPanel);
            }
            else
            {
                PanelMgr.PushPanel(PanelName.WelcomePanel);
            }            
#elif UNITY_ANDROID
            PanelMgr.PushPanel(PanelName.DebugPanel);
#endif
        }
    }
}
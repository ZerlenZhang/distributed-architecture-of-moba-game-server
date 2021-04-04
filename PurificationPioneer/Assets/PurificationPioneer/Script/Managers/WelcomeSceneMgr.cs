using PurificationPioneer.Const;
using PurificationPioneer.Global;
using ReadyGamerOne.View;

namespace PurificationPioneer.Script
{
    public class WelcomeSceneMgr:PpSceneMgr<WelcomeSceneMgr>
    {
        protected override void Start()
        {
            base.Start();
            
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            PanelMgr.PushPanel(PanelName.WelcomePanel);
#elif UNITY_ANDROID
            PanelMgr.PushPanel(PanelName.DebugPanel);
#endif
        }
    }
}
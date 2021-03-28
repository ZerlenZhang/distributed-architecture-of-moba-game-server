using PurificationPioneer.Const;
using PurificationPioneer.Global;
using ReadyGamerOne.View;

namespace PurificationPioneer.Script
{
    public class DebugSceneMgr:PpSceneMgr<DebugSceneMgr>
    {
        protected override void Start()
        {
            base.Start();

            if (!GlobalVar.IsUserLoginIn)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                PanelMgr.PushPanel(PanelName.WelcomePanel);
#elif UNITY_ANDROID
                PanelMgr.PushPanel(PanelName.DebugPanel);
#endif
            }
            else
            {
                PanelMgr.PushPanel(PanelName.HomePanel);
            }
        }
    }
}
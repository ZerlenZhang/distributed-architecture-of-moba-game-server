using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;

namespace PurificationPioneer.Script
{
    public class WelcomeSceneMgr:PpSceneMgr<WelcomeSceneMgr>
    {
        public StringChooser m_Bgm = new StringChooser(typeof(AudioName));
        protected override void Start()
        {
            base.Start();

#if UNITY_EDITOR
            PanelMgr.PushPanel(PanelName.WakeUpPanel);
#elif UNITY_STANDALONE_WIN
            if (GameSettings.Instance.DeveloperMode)
            {
                PanelMgr.PushPanel(PanelName.DebugPanel);
            }
            else
            {
                PanelMgr.PushPanel(PanelName.WakeUpPanel);
            }            
#elif UNITY_ANDROID
            PanelMgr.PushPanel(PanelName.DebugPanel);
#endif
            
            AudioMgr.Instance.PlayBgm(m_Bgm.StringValue);
        }
    }
}
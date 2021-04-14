using PurificationPioneer.Const;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;

namespace PurificationPioneer.Script
{
    public class MainCityMgr:PpSceneMgr<MainCityMgr>
    {
        public StringChooser m_Bgm = new StringChooser(typeof(AudioName));
        protected override void Start()
        {
            base.Start();
            PanelMgr.PushPanel(PanelName.MainCityPanel);
            AudioMgr.Instance.PlayBgm(m_Bgm.StringValue);
        }
    }
}
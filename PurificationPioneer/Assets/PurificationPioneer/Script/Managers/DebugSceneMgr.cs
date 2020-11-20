using PurificationPioneer.Const;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.View;

namespace PurificationPioneer.Script
{
    public class DebugSceneMgr:BaseSceneMgr<DebugSceneMgr>
    {
        public StringChooser firstPanel=new StringChooser(typeof(PanelName));

        protected override void Start()
        {
            base.Start();
            PanelMgr.PushPanel(firstPanel.StringValue);
        }
    }
}
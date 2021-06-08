using PurificationPioneer.Const;
using ReadyGamerOne.View;

namespace PurificationPioneer.Script
{
    public class GameEndSceneMgr:PpSceneMgr<GameEndSceneMgr>
    {
        protected override void Start()
        {
            base.Start();
            PanelMgr.PushPanel(PanelName.GameEndPanel);
        }
    }
}
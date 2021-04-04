using PurificationPioneer.Const;
using ReadyGamerOne.View;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class HomeSceneMgr : PpSceneMgr<HomeSceneMgr>
    {
        protected override void Start()
        {
            base.Start();
            PanelMgr.PushPanel(PanelName.HomePanel);
        }
    }
}
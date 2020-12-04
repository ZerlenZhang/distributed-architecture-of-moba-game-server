using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;

namespace PurificationPioneer.Script
{
    public class BattleSceneMgr:BaseSceneMgr<BattleSceneMgr>
    {
        protected override void Start()
        {
            base.Start();
            LogicProxy.Instance.StartGameReq(GlobalVar.Uname);
        }
    }
}
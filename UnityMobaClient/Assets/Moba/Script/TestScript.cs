using gprotocol;
using Moba.Network;
using ReadyGamerOne.Network;
using ReadyGamerOne.Script;
using UnityEngine;

namespace Moba.Script
{
    public class TestScript:MonoBehaviour
    {
        private void OnAuthPackage(CmdPackageProtocol.CmdPackage pk)
        {
            switch ((ChatCmd)pk.cmdType)
            {
                case ChatCmd.eLoginRes:
                    print("LoginRes.Status:" + CmdPackageProtocol.ProtobufDeserialize<LoginRes>(pk.body).status);
                    break;
            }
        }
        
        private void Start()
        {
            NetworkMgr.Instance.AddCmdPackageListener((int)ServiceType.Auth,OnAuthPackage);

            MainLoop.Instance.ExecuteLater(
                () =>
                {
                    NetworkMgr.Instance.SendProtobufCmd((int) ServiceType.Auth, (int) ChatCmd.eLoginReq, null);
                },
                3);
        }
        
        
    }
}
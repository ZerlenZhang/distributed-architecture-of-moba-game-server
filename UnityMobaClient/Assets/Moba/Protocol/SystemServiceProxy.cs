using gprotocol;
using Moba.Const;
using Moba.Global;
using Moba.Network;
using ReadyGamerOne.Common;
using ReadyGamerOne.Network;
using UnityEngine;

namespace Moba.Protocol
{
    public class SystemServiceProxy:Singleton<SystemServiceProxy>
    {
        public SystemServiceProxy()
        {
            NetworkMgr.Instance.AddCmdPackageListener(
                (int)ServiceType.System,
                OnServerCmdCallback);
        }

        private void OnServerCmdCallback(CmdPackageProtocol.CmdPackage pkg)
        {
//            Debug.Log("???");
            switch ((MobaCmd)pkg.cmdType)
            {
                case MobaCmd.eGetMobaInfoRes:
                    OnGetMobaInfoReturn(pkg);
                    break;
                case MobaCmd.eRecvLoginBonuesRes:
                    OnRecvLoginBonuesReturn(pkg);
                    break;
            }
        }

        private void OnRecvLoginBonuesReturn(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<RecvLoginBonuesRes>(pkg.body);
            if (null == res)
            {
                return;
            }
            Debug.Log("RecvLoginBonues.Status:" + res.status);

            if (res.status != Responce.Ok)
                return;
            
            NetInfo.RecvLoginBonues();
            CEventCenter.BroadMessage(Message.SyncUgameInfo);
        }

        private void OnGetMobaInfoReturn(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<GetMobaInfoRes>(pkg.body);
            if (null == res)
            {
                return;
            }

            if (res.status != Responce.Ok)
            {
                Debug.LogWarning("GetMobaInfoRes.status: " + res.status);
                return;
            }
            
            Debug.Log("获取MobaGame信息");

            NetInfo.SaveUgameInfo(res.uinfo);
            
            CEventCenter.BroadMessage(Message.SyncUgameInfo);
            CEventCenter.BroadMessage(Message.GetUgameInfoSuccess);
        }

        public void RecvLoginBonues()
        {
            NetworkMgr.Instance.TcpSendProtobufCmd(
                (int)ServiceType.System,
                (int)MobaCmd.eRecvLoginBonuesReq);
        }


        /// <summary>
        /// 加载游戏信息
        /// </summary>
        public void LoadMobaInfo()
        {
            NetworkMgr.Instance.TcpSendProtobufCmd(
                (int)ServiceType.System,
                (int)MobaCmd.eGetMobaInfoReq);
        }
    }
}
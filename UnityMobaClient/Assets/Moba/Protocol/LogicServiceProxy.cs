using gprotocol;
using Moba.Const;
using Moba.Global;
using Moba.Network;
using ReadyGamerOne.Common;
using ReadyGamerOne.Network;
using UnityEngine;

namespace Moba.Protocol
{
    public class LogicServiceProxy:Singleton<LogicServiceProxy>
    {
        public LogicServiceProxy()
        {
            NetworkMgr.Instance.AddCmdPackageListener(
                (int)ServiceType.Logic,
                OnLogicServerReturn);
        }

        private void OnLogicServerReturn(CmdPackageProtocol.CmdPackage pkg)
        {
            Debug.Log("Callback");
            switch ((LogicCmd)pkg.cmdType)
            {
                case LogicCmd.eLoginLogicRes:
                    OnLoginLogicReturn(pkg);
                    break;
                case LogicCmd.eEnterZoneRes:
                    OnEnterZoneReturn(pkg);
                    break;
                case LogicCmd.eEnterRoom:
                    OnEnterRoom(pkg);
                    break;
                case LogicCmd.ePlayerEnterRoom:
                    OnPlayerEnterRoom(pkg);
                    break;
                case LogicCmd.ePlayerExitRoom:
                    OnPlayerExitRoom(pkg);
                    break;
                case LogicCmd.eExitRoomRes:
                    OnExitRoomReturn(pkg);
                    break;
                case LogicCmd.eGameStart:
                    OnGameStart(pkg);
                    break;
            }
        }

        private void OnGameStart(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<GameStart>(pkg.body);
            if (null == res)
                return;

            foreach (var hero in res.heros)
            {
                Debug.Log("HeroID: " + hero);
            }
            
            CEventCenter.BroadMessage(Message.GameStart);
        }

        private void OnPlayerExitRoom(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<PlayerExitRoom>(pkg.body);
            if (null == res)
                return;

            Debug.Log("player leave " + res.seatid);
            
            for (int i = 0; i < NetInfo.otherPlayers.Count; i++)
            {
                if (NetInfo.otherPlayers[i].seatid == res.seatid)
                {
                    CEventCenter.BroadMessage(Message.PlayerExitRoom,i);
                    NetInfo.otherPlayers.RemoveAt(i);
                    return;
                }
            }
            
        }

        private void OnExitRoomReturn(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<ExitRoomRes>(pkg.body);
            if (null == res)
                return;
            Debug.Log("exit room States: " + res.status);
            if (res.status == Responce.Ok)
            {
                NetInfo.SetZoneId(-1);
                CEventCenter.BroadMessage(Message.LeaveRoomSuccess);
            }
        }

        private void OnPlayerEnterRoom(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<PlayerEnterRoom>(pkg.body);
            if (null == res)
                return;
            
            Debug.Log("other player come: " + res.unick);
            //添加其他玩家信息
            NetInfo.otherPlayers.Add(res);
            //广播消息
            CEventCenter.BroadMessage(Message.PlayerEnterRoom, res);
        }

        private void OnEnterRoom(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<EnterRoom>(pkg.body);
            if (null == res)
                return;
            

            Debug.Log("Enter room: [" + res.zoneid + " : " + res.roomid+"]");
        }

        private void OnEnterZoneReturn(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<EnterZoneRes>(pkg.body);
            if (null == res)
                return;
            if (res.status != Responce.Ok)
            {
                Debug.Log("EnterZoneRes.Status:" + res.status);
                return;
            }

            Debug.Log("Enter zone success");
        }

        private void OnLoginLogicReturn(CmdPackageProtocol.CmdPackage pkg)
        {
            var res = CmdPackageProtocol.ProtobufDeserialize<LoginLogicRes>(pkg.body);
            if (null == res)
                return;
            if (res.status != Responce.Ok)
            {
                Debug.Log("LoginLogicRes.Status:" + res.status);
                return;
            }
            
            CEventCenter.BroadMessage(Message.LoginLogicServerSuccess);
            
        }
        
        public void LoginLogicServer()
        {
            NetworkMgr.Instance.SendProtobufCmd(
                (int)ServiceType.Logic,
                (int)LogicCmd.eLoginLogicReq);
        }

        public void EnterZone(int zoneId)
        {
            if (zoneId < ZoneId.Sgyd || zoneId > ZoneId.Assy)
                return;
            var req = new EnterZoneReq
            {
                zoneid = zoneId,
            };
            NetworkMgr.Instance.SendProtobufCmd(
                (int) ServiceType.Logic,
                (int) LogicCmd.eEnterZoneReq,
                req);
            Debug.Log("Send zondId: " + zoneId);
        }

        public void ExitRoom()
        {
            NetworkMgr.Instance.SendProtobufCmd(
                (int) ServiceType.Logic,
                (int) LogicCmd.eExitRoomReq);
        }
    }
}
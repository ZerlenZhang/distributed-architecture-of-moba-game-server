using PurificationPioneer.Const;
using PurificationPioneer.Network.Const;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.Network.Proxy
{
    public class LogicProxy:Singleton<LogicProxy>
    {
        private bool logicServerConnected = false;
        public LogicProxy()
        {
            NetworkMgr.Instance.AddNetMsgListener(ServiceType.Logic,OnLogicCmd);
        }

        private void OnLogicCmd(CmdPackageProtocol.CmdPackage package)
        {
            switch (package.cmdType)
            {
                case LogicCmd.StopMatchRes:
                    var stopMatchRes = CmdPackageProtocol.ProtobufDeserialize<StopMatchRes>(package.body);
                    if (stopMatchRes == null)
                    {
                        Debug.LogError($"stopMatchRes is null");
                        return;
                    }
                    if (stopMatchRes.status == Response.Ok)
                    {
#if DebugMode
                        if(GameSettings.Instance.EnableProtoLog)
                            Debug.Log($"[StopMatchRes]取消匹配");
#endif
                        CEventCenter.BroadMessage(Message.OnStopMatch);
                    }
                    
                    break;
                case LogicCmd.RemoveMatcherTick:
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[RemoveMatcherTick]有玩家走了");
#endif
                    CEventCenter.BroadMessage(Message.OnRemovePlayer);
                    break;
                case LogicCmd.AddMatcherTick:
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[AddMatcherTick]新玩家来了");
#endif
                    CEventCenter.BroadMessage(Message.OnAddPlayer);
                    break;
                    
                case LogicCmd.StartMatchRes:
                    var startMatchRes = CmdPackageProtocol.ProtobufDeserialize<StartMatchRes>(package.body);
                    if (startMatchRes == null)
                    {
                        Debug.LogError($"startMatchRes is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[StartMatchRes]开始匹配");
#endif
                    CEventCenter.BroadMessage(Message.OnStartMatch,startMatchRes);
                    break;
                case LogicCmd.LoginLogicRes:
                    var loginLogicRes = CmdPackageProtocol.ProtobufDeserialize<LoginLogicRes>(package.body);
                    if (loginLogicRes == null)
                    {
                        Debug.LogError($"loginLogicRes is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[LoginLogicRes]登陆逻辑服务器成功");
#endif
                    //开启Udp接收
                    GameSettings.Instance.SetUdpServerIp(loginLogicRes.udp_ip);
                    GameSettings.Instance.SetUdpServerPort(loginLogicRes.udp_port);
                    NetworkMgr.Instance.SetupUdp(
                        (status)=>
                        {
                            logicServerConnected = status;
                            if(logicServerConnected)
                                Debug.Log("Udp服务建立成功");
                            else 
                                Debug.LogWarning("Udp服务建立失败");
                        });
                    break;
                case LogicCmd.UdpTestRes:
                    var udpTestRes = CmdPackageProtocol.ProtobufDeserialize<UdpTestRes>(package.body);
                    if (udpTestRes == null)
                    {
                        Debug.LogError($"UdpTestRes is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[UdpTestRes]{udpTestRes.content}");
#endif
                    break;
            }
        }
        
        /// <summary>
        /// 登陆逻辑服务器
        /// </summary>
        public void Login()
        {
            NetworkMgr.Instance.TcpSendProtobuf(
                ServiceType.Logic,
                LogicCmd.LoginLogicReq);     
        }

        /// <summary>
        /// 测试Udp
        /// </summary>
        /// <param name="content"></param>
        public void TestUdp(string content)
        {
            if(!logicServerConnected)
                Debug.LogError($"尚未登陆逻辑服务器");
            
            var udpTestReq = new UdpTestReq
            {
                content = content,
            };
            NetworkMgr.Instance.UdpSendProtobuf(
                ServiceType.Logic,
                LogicCmd.UdpTestReq,
                udpTestReq);
        }

        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="uname"></param>
        public void StartMatch(string uname)
        {
            if(!logicServerConnected)
                Debug.LogError($"尚未登陆逻辑服务器");
            
            var startMatchReq = new StartMatchReq
            {
                uname = uname
            };
            NetworkMgr.Instance.TcpSendProtobuf(
                ServiceType.Logic,
                LogicCmd.StartMatchReq,
                startMatchReq);
        }

        public void TryStopMatch(string uname)
        {
            if(!logicServerConnected)
                Debug.LogError($"尚未登陆逻辑服务器");
            var stopMatchReq = new StopMatchReq
            {
                uname = uname,
            };
            NetworkMgr.Instance.TcpSendProtobuf(
                ServiceType.Logic,
                LogicCmd.StopMatchReq,
                stopMatchReq);
        }
    }
}
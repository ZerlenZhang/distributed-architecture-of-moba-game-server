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
                case LogicCmd.StartMatchRes:
#if UNITY_EDITOR
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[StartMatchRes]开始匹配");
#endif
                    CEventCenter.BroadMessage(Message.OnStartMatch);
                    break;
                case LogicCmd.LoginLogicRes:
                    var loginLogicRes = CmdPackageProtocol.ProtobufDeserialize<LoginLogicRes>(package.body);
                    if (loginLogicRes == null)
                    {
                        Debug.LogError($"loginLogicRes is null");
                        return;
                    }
#if UNITY_EDITOR
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[LoginLogicRes]登陆逻辑服务器成功");
#endif
                    
                    //开启Udp接收
                    GameSettings.Instance.SetUdpServerIp(loginLogicRes.udp_ip);
                    GameSettings.Instance.SetUdpServerPort(loginLogicRes.udp_port);
                    NetworkMgr.Instance.SetupUdp(
                        ()=>
                        {
                            logicServerConnected = true;
                            Debug.Log("Udp服务建立成功");
                        });
                    break;
                case LogicCmd.UdpTestRes:
                    var udpTestRes = CmdPackageProtocol.ProtobufDeserialize<UdpTestRes>(package.body);
                    if (udpTestRes == null)
                    {
                        Debug.LogError($"UdpTestRes is null");
                        return;
                    }
#if UNITY_EDITOR
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[UdpTestRes]{udpTestRes.content}");
#endif
                    break;
            }
        }
        
        public void Login()
        {
            if (logicServerConnected)
                return;
            NetworkMgr.Instance.TcpSendProtobuf(
                ServiceType.Logic,
                LogicCmd.LoginLogicReq);     
        }

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
    }
}
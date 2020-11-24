using PurificationPioneer.Const;
using PurificationPioneer.Network.Const;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.Network.Proxy
{
    public class AuthProxy:Singleton<AuthProxy>
    {
        public AuthProxy()
        {
            NetworkMgr.Instance.AddNetMsgListener(ServiceType.Auth,OnAuthCmd);
        }
        
        private void OnAuthCmd(CmdPackageProtocol.CmdPackage msg)
        {
            switch (msg.cmdType)
            {
                case AuthCmd.UserLoginRes:
                    var loginRes = CmdPackageProtocol.ProtobufDeserialize<UserLoginRes>(msg.body);
                    if (null != loginRes)
                    {
                        if (loginRes.status == Response.Ok)
                        {
                            //登陆成功
#if UNITY_EDITOR
                            if (GameSettings.Instance.EnableProtoLog)
                            {
                                Debug.Log($"[UserLoginRes]-{loginRes.uinfo.unick} 上线啦");
                            }
#endif
                            CEventCenter.BroadMessage(Message.OnUserLogin,loginRes.uinfo);
                        }
                        else
                        {
                            Debug.LogError($"UserLoginRes status unexpected: {loginRes.status}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"UserLoginRes is null");
                    }
                    break;
            }
        }

        public void Login(string uname, string pwd)
        {
            var loginReq=new UserLoginReq
            {
                uname = uname,
                pwd = pwd,
            };
            NetworkMgr.Instance.TcpSendProtobuf(ServiceType.Auth, AuthCmd.UserLoginReq, loginReq);
        }
        
        
    }
}
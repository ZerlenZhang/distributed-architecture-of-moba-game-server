using PurificationPioneer.Const;
using PurificationPioneer.Global;
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
#if DebugMode
                            if (GameSettings.Instance.EnableProtoLog)
                            {
                                Debug.Log($"[UserLoginRes]-{loginRes.uinfo.unick} 上线啦, heros:{loginRes.uinfo.heros}");
                            }
#endif
                            //保存信息并广播
                            GlobalVar.SaveUserInfo(loginRes.uinfo);
                            CEventCenter.BroadMessage(Message.OnUserLogin);
                        }
                        else
                        {
                            CEventCenter.BroadMessage(Message.OnResponseError, loginRes.status);
                        }
                    }
                    else
                    {
                        Debug.LogError($"UserLoginRes is null");
                    }
                    break;
                
                case AuthCmd.UserRegisteRes:
                    var userRegisteRes=CmdPackageProtocol.ProtobufDeserialize<UserRegisteRes>(msg.body);
                    if (null == userRegisteRes)
                    {
                        Debug.LogError("userRegisteRes is null");
                    }
                    else
                    {
                        if (userRegisteRes.status == Response.Ok)
                        {
#if DebugMode
                            if (GameSettings.Instance.EnableProtoLog)
                            {
                                Debug.Log($"[UserRegisteRes]-{userRegisteRes.uinfo.unick} 注册上线啦, heros:{userRegisteRes.uinfo.heros}");
                            }                            
#endif
                            //保存信息并广播
                            GlobalVar.SaveUserInfo(userRegisteRes.uinfo);
                            CEventCenter.BroadMessage(Message.OnUserLogin);
                        }
                        else
                        {
                            CEventCenter.BroadMessage(Message.OnResponseError, userRegisteRes.status);
                        }
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

        public void Register(string uname, string pwd, string unick)
        {
            Debug.Log("发送注册消息");
            var registeReq = new UserRegisteReq
            {
                uname = uname,
                pwd = pwd,
                unick = unick
            };
            NetworkMgr.Instance.TcpSendProtobuf(ServiceType.Auth, AuthCmd.UserRegisteReq, registeReq);
        }
    }
}
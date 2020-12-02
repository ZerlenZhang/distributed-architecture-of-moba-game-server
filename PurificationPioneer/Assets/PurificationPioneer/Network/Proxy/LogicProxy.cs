using PurificationPioneer.Const;
using PurificationPioneer.Global;
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
                #region LogicCmd.StartLoadGame
                case LogicCmd.StartLoadGame:
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[StartLoadGame] 开始加载游戏");
#endif
                    break;
                #endregion

                #region LogicCmd.ForceSelect
                case LogicCmd.ForceSelect:
                    var forceSelect = CmdPackageProtocol.ProtobufDeserialize<ForceSelect>(package.body);
                    if (null == forceSelect)
                    {
                        Debug.LogError($"ForceSelect is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[ForceSelect] 强制选择默认英雄");
#endif
                    break;

                #endregion
                
                #region LogicCmd.UpdateSelectTimer
                case LogicCmd.UpdateSelectTimer:
                    var updateSelectTimer = CmdPackageProtocol.ProtobufDeserialize<UpdateSelectTimer>(package.body);
                    if (null == updateSelectTimer)
                    {
                        Debug.LogError($"UpdateSelectTimer is null");
                        return;
                    }

                    CEventCenter.BroadMessage(Message.OnUpdateSelectTimer, updateSelectTimer);
                    break;
                #endregion

                #region LogicCmd.SelectHeroRes
                case LogicCmd.SelectHeroRes:
                    var selectHeroRes = CmdPackageProtocol.ProtobufDeserialize<SelectHeroRes>(package.body);
                    if (null == selectHeroRes)
                    {
                        Debug.LogError($"SelectHeroRes is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[SelectHeroRes] SeatId[{selectHeroRes.seatId}] 选择了 HeroId[{selectHeroRes.hero_id}]");
#endif
                    CEventCenter.BroadMessage(Message.OnSelectHero, selectHeroRes);
                    
                    break;
                #endregion

                #region LogicCmd.SubmitHeroRes

                case LogicCmd.SubmitHeroRes:
                    var submitHeroRes = CmdPackageProtocol.ProtobufDeserialize<SubmitHeroRes>(package.body);
                    if(null==submitHeroRes)
                    {
                        Debug.LogError($"SubmitHeroRes is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[SubmitHeroRes] SeatId[{submitHeroRes.seatId}] 锁定英雄]");
#endif
                    if(submitHeroRes.seatId==GlobalVar.SeatId)
                        GlobalVar.SetSubmit(true);
                    CEventCenter.BroadMessage(Message.OnSubmitHero, submitHeroRes);
                    
                    break;

                #endregion
                
                #region LogicCmd.FinishMatchTick
                case LogicCmd.FinishMatchTick:
                    var finishMatchTick = CmdPackageProtocol.ProtobufDeserialize<FinishMatchTick>(package.body);
                    if (null == finishMatchTick)
                    {
                        Debug.LogError($"FinishMatchTick is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[FinishMatchTick]匹配完成({finishMatchTick.matchers.Count})，开始英雄选择");
#endif
                    GlobalVar.OnFinishMatchTick(finishMatchTick);
                    CEventCenter.BroadMessage(Message.OnFinishMatch);
                    
                    break;
                #endregion
                
                #region LogicCmd.StopMatchRes

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

                #endregion

                #region LogicCmd.RemoveMatcherTick

                case LogicCmd.RemoveMatcherTick:
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[RemoveMatcherTick]有玩家走了");
#endif
                    CEventCenter.BroadMessage(Message.OnRemovePlayer);
                    break;                 

                #endregion

                #region LogicCmd.AddMatcherTick

                case LogicCmd.AddMatcherTick:
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[AddMatcherTick]新玩家来了");
#endif
                    CEventCenter.BroadMessage(Message.OnAddPlayer);
                    break;                      

                #endregion

                #region LogicCmd.StartMatchRes

                case LogicCmd.StartMatchRes:
                    var startMatchRes = CmdPackageProtocol.ProtobufDeserialize<StartMatchRes>(package.body);
                    if (startMatchRes == null)
                    {
                        Debug.LogError($"startMatchRes is null");
                        return;
                    }
#if DebugMode
                    if(GameSettings.Instance.EnableProtoLog)
                        Debug.Log($"[StartMatchRes]开始匹配:{startMatchRes.current}/{startMatchRes.max}");
#endif
                    CEventCenter.BroadMessage(Message.OnStartMatch,startMatchRes);
                    break;                    

                #endregion

                #region LogicCmd.LoginLogicRes

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

                #endregion

                #region LogicCmd.UdpTestRes

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

                #endregion
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

        /// <summary>
        /// 取消匹配
        /// </summary>
        /// <param name="uname"></param>
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


        /// <summary>
        /// 选中英雄
        /// </summary>
        /// <param name="uname"></param>
        /// <param name="heroId"></param>
        public void SelectHero(string uname, int heroId)
        {
            var selectHeroReq = new SelectHeroReq
            {
                uname = uname,
                hero_id = heroId,
            };
            NetworkMgr.Instance.TcpSendProtobuf(
                ServiceType.Logic,
                LogicCmd.SelectHeroReq,
                selectHeroReq);
        }

        /// <summary>
        /// 锁定英雄
        /// </summary>
        /// <param name="uname"></param>
        public void SubmitHero(string uname)
        {
            NetworkMgr.Instance.TcpSendProtobuf(
                ServiceType.Logic,
                LogicCmd.SubmitHeroReq,
                new SubmitHeroReq{uname = uname});
        }
    }
}
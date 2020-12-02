using System;
using UnityEngine;
using System.Collections.Generic;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using ReadyGamerOne.Network;
using ReadyGamerOne.Utility;
using UnityEngine.Assertions;

namespace PurificationPioneer.Network
{
    public class NetworkMgr:GlobalMonoSingleton<NetworkMgr>
    {
        #region 网络事件处理_监听于分发

        //事件监听委托
        public delegate void ReceiveCmdPackageCallback(CmdPackageProtocol.CmdPackage package);
        
        //事件队列
        private Queue<CmdPackageProtocol.CmdPackage> msgQueue = new Queue<CmdPackageProtocol.CmdPackage>();
        private object queueLock=new object();
        
        //监听者字典
        private Dictionary<int, ReceiveCmdPackageCallback> msgListeners =
            new Dictionary<int, ReceiveCmdPackageCallback>();

        /// <summary>
        /// 添加监听者
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="func"></param>
        public void AddNetMsgListener(int sType, ReceiveCmdPackageCallback func)
        {
            if (this.msgListeners.ContainsKey(sType))
                this.msgListeners[sType] += func;
            else
                this.msgListeners.Add(sType, func);
        }

        /// <summary>
        /// 移除监听者
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="func"></param>
        public void RemoveNetMsgListener(int sType, ReceiveCmdPackageCallback func)
        {
            if (!msgListeners.ContainsKey(sType))
                return;
            msgListeners[sType] -= func;
            if (msgListeners[sType] == null)
                msgListeners.Remove(sType);

        }


        protected override void Update()
        {
            base.Update();   
            lock (this.queueLock)
            {
                while (this.msgQueue.Count > 0)
                {// 有事件，广播消息
                    var pk = msgQueue.Dequeue();
                    if (msgListeners.ContainsKey(pk.serviceType))
                    {
                        msgListeners[pk.serviceType]?.Invoke(pk);
                    }
                }
            }
        }

        #endregion

        private TcpHelper tcp;
        
        private UdpHelper udp;
        private string udpServerIp;
        private int udpServerPort;

        private bool errorInside = false;

        #region 发送数据

        public void SetupUdp(Action<bool> onFinishSetup=null)
        {

            var localPort = NetUtil.GetUdpPort();
            GameSettings.Instance.SetUdpLocalPort(localPort);
            



            udp = new UdpHelper(
                GameSettings.Instance.UdpServerIp,
                GameSettings.Instance.UdpServerPort,
                GameSettings.Instance.UdpLocalPort,
                OnError,
                OnRecvCmd,
                GameSettings.Instance.MaxUdpPackageSize,
                () => GameSettings.Instance.EnableSocketLog,
                () =>
                {
                    GameSettings.Instance.SetUdpLocalIp(udp.LocalIp);
#if DebugMode
                    if (GameSettings.Instance.EnableProtoLog)
                    {
                        Debug.Log($"UdpServer[{GameSettings.Instance.UdpServerIp}:{GameSettings.Instance.UdpServerPort}, " +
                                  $"Local[{GameSettings.Instance.UdpLocalIp}:{GameSettings.Instance.UdpLocalPort}]");
                    }
#endif                    
                    onFinishSetup?.Invoke(!errorInside);
                });

        }

        public void UdpSendProtobuf(int serviceType, int cmdType, ProtoBuf.IExtensible body = null)
        {
            if (udp == null)
            {
                Debug.LogError("Udp is null");
                return;
            }
            if (!udp.IsValid)
            {
                Debug.LogError($"Udp状态异常");
                return;
            }
            var cmdPackage = CmdPackageProtocol.PackageProtobuf(serviceType, cmdType, body);
            if (cmdPackage == null)
                return;

            this.udp.Send(cmdPackage);
        }
        
        public void TcpSendJson(int serviceType, int cmdType, string jsonStr)
        {           
            if (tcp == null || !tcp.IsValid)
            {
                Debug.LogError("tcp 状态异常，无法使用");
                return;
            }
            var cmdPackage = CmdPackageProtocol.PackageJson(serviceType, cmdType, jsonStr);
            if (cmdPackage == null)
                return;
            var tcpPackage = TcpProtocol.Pack(cmdPackage);
            if (tcpPackage == null)
                return;

            tcp.Send(tcpPackage);
        }

        public void TcpSendProtobuf(int serviceType, int cmdType, ProtoBuf.IExtensible body=null)
        {
            if (tcp == null || !tcp.IsValid)
            {
                Debug.LogError("tcp 状态异常，无法使用");
                return;
            }
            var cmdPackage = CmdPackageProtocol.PackageProtobuf(serviceType, cmdType, body);
            Assert.IsNotNull(cmdPackage);
            var tcpPackage = TcpProtocol.Pack(cmdPackage);
            Assert.IsNotNull(tcpPackage);

            tcp.Send(tcpPackage);
        }

        #endregion

        #region Private

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnStateIsNull()
        {
            base.OnStateIsNull();
            
            tcp=new TcpHelper(
                GameSettings.Instance.GatewayIp,
                GameSettings.Instance.GatewayPort,
                OnError,
                OnRecvCmd,
                GameSettings.Instance.MaxTcpPackageSize,
                GameSettings.Instance.MaxWaitTime,
                ()=>GameSettings.Instance.EnableSocketLog);

            Application.quitting += CloseSocket;
        }
        
        protected virtual void OnDestroy()
        {
            if (GameSettings.Instance.EnableSocketLog)
            {
                Debug.Log("OnDestroy_关闭链接");
            }
            this.CloseSocket();
        }

        
        /// <summary>
        /// 断开链接
        /// </summary>
        private void CloseSocket()
        {
            tcp?.CloseReceiver();
            tcp = null;

            udp?.CloseReceiver();
            udp = null;
        }

        /// <summary>
        /// 任何异常都调用
        /// </summary>
        /// <param name="o"></param>
        private void OnError(object o)
        {
            this.errorInside = true;
            Debug.Log(o);
            if (GameSettings.Instance.CloseSocketOnAnyException)
            {
                CloseSocket();
            }
        }

        /// <summary>
        /// 接收到命令
        /// </summary>
        /// <param name="pkgData"></param>
        /// <param name="rawDataStart"></param>
        /// <param name="rawDataLen"></param>
        private void OnRecvCmd(byte[] pkgData, int rawDataStart, int rawDataLen)
        {
            CmdPackageProtocol.CmdPackage msg;
            
            //解析
            if (!CmdPackageProtocol.UnpackProtobuf(pkgData, rawDataStart, rawDataLen, out msg))
                return;

            if (null == msg)
                return;

#if DebugMode
            if (GameSettings.Instance.EnableSocketLog)
            {
                Debug.Log($"收到CmdPackage:{{sType-{msg.serviceType},cType-{msg.cmdType}}}");
            }
#endif

            //将收到消息放到事件队列
            lock (queueLock)
            {
                this.msgQueue.Enqueue(msg);
            }
        }        

        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using gprotocol;
using ReadyGamerOne.Common;
using ReadyGamerOne.Network;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Network
{
    public class NetworkMgr:MonoSingleton<NetworkMgr>
    {
        

        #region Tcp

        public string tcpServerIp="127.0.0.1";
        public int tcpPort=6080;
        private Socket tcpSocket;
        public int maxTcpPackageSize = 4096;
        private TcpProtocol.TcpReceiver _tcpReceiver;

        //事件队列
        private Queue<CmdPackageProtocol.CmdPackage> packageQueue = new Queue<CmdPackageProtocol.CmdPackage>();
        private object queueLock=new object();
        //事件监听
        public delegate void ReceiveCmdPackageCallback(CmdPackageProtocol.CmdPackage package);
        private Dictionary<int, ReceiveCmdPackageCallback> eventListeners =
            new Dictionary<int, ReceiveCmdPackageCallback>();        

        #endregion

        #region Udp

        public string udpServerIp = "127.0.0.1";
        public int udpPort = 8800;
        private IPEndPoint udpRemotePoint;
        private Socket udpSocket;
        private Thread udpRecvThread;
        private byte[] udpRecvBuff=new byte[1024*8];
        #endregion

        public string localIp = "127.0.0.1";
        public int localPort = 8800;
        
        
        /// <summary>
        /// 添加监听者
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="func"></param>
        public void AddCmdPackageListener(int sType, ReceiveCmdPackageCallback func)
        {
            if (this.eventListeners.ContainsKey(sType))
                this.eventListeners[sType] += func;
            else
                this.eventListeners.Add(sType, func);
        }

        /// <summary>
        /// 移除监听者
        /// </summary>
        /// <param name="sType"></param>
        /// <param name="func"></param>
        public void RemoveCmdPackageListener(int sType, ReceiveCmdPackageCallback func)
        {
            if (!eventListeners.ContainsKey(sType))
                return;
            eventListeners[sType] -= func;
            if (eventListeners[sType] == null)
                eventListeners.Remove(sType);

        }
        
        
        
        #region 发送数据

        public void UdpSendProtobufCmd(int serviceType, int cmdType, ProtoBuf.IExtensible body = null)
        {
            var cmdPackage = CmdPackageProtocol.PackageProtobuf(serviceType, cmdType, body);
            if (cmdPackage == null)
                return;
            
            try
            {
                this.udpSocket.BeginSendTo(
                    cmdPackage,
                    0,
                    cmdPackage.Length,
                    SocketFlags.None,
                    this.udpRemotePoint,
                    OnUdpSend, 
                    this.udpSocket);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }
        

        public void SendJsonCmd(int serviceType, int cmdType, string jsonStr)
        {           
            var cmdPackage = CmdPackageProtocol.PackageJson(serviceType, cmdType, jsonStr);
            if (cmdPackage == null)
                return;
            var tcpPackage = TcpProtocol.Pack(cmdPackage);
            if (tcpPackage == null)
                return;

            this.tcpSocket.BeginSend(
                tcpPackage,
                0,
                tcpPackage.Length,
                SocketFlags.None,
                OnTcpSend, this.tcpSocket);
        }

        public void TcpSendProtobufCmd(int serviceType, int cmdType, ProtoBuf.IExtensible body=null)
        {
            var cmdPackage = CmdPackageProtocol.PackageProtobuf(serviceType, cmdType, body);
            if (cmdPackage == null)
                return;
            var tcpPackage = TcpProtocol.Pack(cmdPackage);
            if (tcpPackage == null)
                return;

            try
            {
                this.tcpSocket.BeginSend(
                    tcpPackage,
                    0,
                    tcpPackage.Length,
                    SocketFlags.None,
                    OnTcpSend, this.tcpSocket);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        private void OnUdpSend(IAsyncResult ar)
        {
            try
            {
                var client = ar.AsyncState as Socket;
                client.EndSendTo(ar);
            }
            catch (Exception e)
            {
                
                OnError(e);
                throw;
            }
        }

        private void OnTcpSend(IAsyncResult ar)
        {
            try
            {
                var client = ar.AsyncState as Socket;
                Assert.IsNotNull(client);
                client?.EndSend(ar);
                print("发送成功");
            }
            catch (Exception e)
            {
                OnError(e);
                throw;
            }
        }       
        
        
        #endregion

        #region Monobahavior

        protected override void Awake()
        {
//            Debug.Log("Network_Awake");
            base.Awake();
            TcpInit();
            
            UdpInit();
        }

        protected virtual void Update()
        {
            lock (this.queueLock)
            {
                while (this.packageQueue.Count > 0)
                {// 有事件，广播消息
                    var pk = packageQueue.Dequeue();
                    if (eventListeners.ContainsKey(pk.serviceType))
                    {
                        eventListeners[pk.serviceType].Invoke(pk);
                    }
                }
            }
        }
        protected virtual void OnDestroy()
        {
            Debug.Log("关闭链接");
            this.CloseSocket();
        }        

        #endregion

        #region Tcp_Func

        private void TcpInit()
        {
            try
            {
                Assert.IsFalse(string.IsNullOrEmpty(this.tcpServerIp));
                Assert.IsTrue(0 != tcpPort);
                
                tcpSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                
                var ip = IPAddress.Parse(this.tcpServerIp);
                var ipEndPoint = new IPEndPoint(ip, this.tcpPort);
                var result = tcpSocket.BeginConnect(
                    ipEndPoint,
                    OnConnectCallback,
                    this.tcpSocket);
                var success = result.AsyncWaitHandle.WaitOne(5000, true);
                if (!success)
                {
                    OnError("链接超时");
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }        
        
        
        /// <summary>
        /// Tcp连接成功回调
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                Assert.IsNotNull(socket);
                socket.EndConnect(ar);

                _tcpReceiver=new TcpProtocol.TcpReceiver(socket,OnRecvCmd,maxTcpPackageSize,OnTcpReceiveError);
                
                Debug.Log("connect success");
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }
        
        /// <summary>
        /// Tcp接收数据异常得回调
        /// </summary>
        /// <param name="e"></param>
        private void OnTcpReceiveError(Exception e)
        {
            Debug.Log(e);
            CloseSocket();
        }
        #endregion

        #region Udp_Func

        void UdpInit()
        {
            this.udpRemotePoint = new IPEndPoint(
                IPAddress.Parse(this.udpServerIp), udpPort);
            //创建udpSocket
            try
            {
                this.udpSocket=new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Dgram,
                    ProtocolType.Udp);
                //绑定本地端口
                var localPoint = new IPEndPoint(IPAddress.Parse(localIp), localPort);
                this.udpSocket.Bind(localPoint);
                
                //启动线程收取Udp数据
                this.udpRecvThread = new Thread(UdpRecvThead);
                this.udpRecvThread.Start();
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }
        
        
        /// <summary>
        /// UDP收数据线程
        /// </summary>
        private void UdpRecvThead()
        {
            while (true)
            {
                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                var recvd = this.udpSocket.ReceiveFrom(this.udpRecvBuff,ref remote);
                this.OnRecvCmd(this.udpRecvBuff, 0, recvd);
            }
        }        

        #endregion
        

        /// <summary>
        /// 任何异常都调用
        /// </summary>
        /// <param name="o"></param>
        private void OnError(object o)
        {
            Debug.Log(o);
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

            //将收到消息放到事件队列
            lock (queueLock)
            {
                this.packageQueue.Enqueue(msg);
            }
        }
        
        /// <summary>
        /// 断开链接
        /// </summary>
        private void CloseSocket()
        {
            _tcpReceiver?.CloseReceiver();
            _tcpReceiver = null;
            if (this.tcpSocket != null && this.tcpSocket.Connected)
            {
                this.tcpSocket.Close();
                this.tcpSocket = null;
            }

            if (null != this.udpRecvThread)
            {
                udpRecvThread.Interrupt();
                udpRecvThread.Abort();
                udpRecvThread = null;
            }
            
            if (this.udpSocket != null && this.udpSocket.Connected)
            {
                this.udpSocket.Close();
                this.udpSocket = null;
            }
            
            
        }
    }
}
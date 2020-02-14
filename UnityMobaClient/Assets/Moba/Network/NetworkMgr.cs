using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using gprotocol;
using ReadyGamerOne.Common;
using ReadyGamerOne.Network;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Network
{
    public class NetworkMgr:MonoSingleton<NetworkMgr>
    {
        public string serverIp="127.0.0.1";
        public int serverPort=6080;
        public int maxTcpPackageSize = 4096;
        
        
        private Socket clientSocket;
        private TcpProtocol.TcpReceiver _tcpReceiver;

        //事件队列
        private Queue<CmdPackageProtocol.CmdPackage> packageQueue = new Queue<CmdPackageProtocol.CmdPackage>();
        private object queueLock=new object();
        //事件监听
        public delegate void ReceiveCmdPackageCallback(CmdPackageProtocol.CmdPackage package);
        private Dictionary<int, ReceiveCmdPackageCallback> eventListeners =
            new Dictionary<int, ReceiveCmdPackageCallback>();


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

        public void SendJsonCmd(int serviceType, int cmdType, string jsonStr)
        {           
            var cmdPackage = CmdPackageProtocol.PackageJson(serviceType, cmdType, jsonStr);
            if (cmdPackage == null)
                return;
            var tcpPackage = TcpProtocol.Pack(cmdPackage);
            if (tcpPackage == null)
                return;

            this.clientSocket.BeginSend(
                tcpPackage,
                0,
                tcpPackage.Length,
                SocketFlags.None,
                OnAfterSend, this.clientSocket);
        }

        public void SendProtobufCmd(int serviceType, int cmdType, ProtoBuf.IExtensible body)
        {
            var cmdPackage = CmdPackageProtocol.PackageProtobuf(serviceType, cmdType, body);
            if (cmdPackage == null)
                return;
            var tcpPackage = TcpProtocol.Pack(cmdPackage);
            if (tcpPackage == null)
                return;

            try
            {
                this.clientSocket.BeginSend(
                    tcpPackage,
                    0,
                    tcpPackage.Length,
                    SocketFlags.None,
                    OnAfterSend, this.clientSocket);
            }
            catch (Exception e)
            {
                OnConnectError(e);
            }
        }

        private void OnAfterSend(IAsyncResult ar)
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
                OnConnectError(e);
                throw;
            }
        }        

        #endregion

         
        protected override void Awake()
        {
//            Debug.Log("Network_Awake");
            base.Awake();
            ConnectServer();
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

        #region 链接

        private void ConnectServer()
        {
            try
            {
                Assert.IsFalse(string.IsNullOrEmpty(this.serverIp));
                Assert.IsTrue(0 != serverPort);
                clientSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                
                var ip = IPAddress.Parse(this.serverIp);
                var ipEndPoint = new IPEndPoint(ip, this.serverPort);
                var result = clientSocket.BeginConnect(
                    ipEndPoint,
                    OnConnectCallback,
                    this.clientSocket);
                var success = result.AsyncWaitHandle.WaitOne(5000, true);
                if (!success)
                {
                    OnConnectError("链接超时");
                }
            }
            catch (Exception e)
            {
                OnConnectError(e);
            }
        }

        private void OnConnectError(object o)
        {
            Debug.Log(o);
        }        

        #endregion


        private void OnReceiveError(Exception e)
        {
            Debug.Log(e);
            CloseSocket();
        }

        private void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                Assert.IsNotNull(socket);
                socket.EndConnect(ar);

                _tcpReceiver=new TcpProtocol.TcpReceiver(socket,OnRecvCmd,maxTcpPackageSize,OnReceiveError);
                
                Debug.Log("connect success");
            }
            catch (Exception e)
            {
                OnConnectError(e);
            }
        }
        
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
            if (this.clientSocket != null && this.clientSocket.Connected)
            {
                this.clientSocket.Close();
            }
        }
    }
}
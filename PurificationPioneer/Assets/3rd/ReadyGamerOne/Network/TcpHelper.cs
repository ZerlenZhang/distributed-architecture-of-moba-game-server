using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Network
{
    public class TcpHelper
    {
            #region Private

            private readonly int maxPackageSize;

            private Socket clientSocket;
            
            private int recved;
            private byte[] longPkg;
            private int longPkgSize;
            
            
            private Thread recvThread;
            private byte[] _recvBuf;

            private Action<byte[], int, int> onRecvCmd;
            private Action<Exception> onException;
            private Func<bool> ifEnableSocketLog = () => true;

            #endregion
            
            public string Ip { get; }
            public int Port { get; }

            public bool IsValid => clientSocket != null && clientSocket.Connected;
            
            public TcpHelper(string tcpServerIp, int tcpPort, Action<Exception> onException, Action<byte[],int,int> onRecvCmd, int maxTcpPackageSize, int maxWaitTime, Func<bool> enableSocketLog=null)
            {
                Assert.IsFalse(string.IsNullOrEmpty(tcpServerIp));
                Assert.IsTrue(0 != tcpPort);
                Assert.IsNotNull(onException);
                Assert.IsNotNull(onRecvCmd);

                if (null != enableSocketLog)
                    this.ifEnableSocketLog = enableSocketLog;

                this.Ip = tcpServerIp;
                this.Port = tcpPort;
                
                this.onException = onException;
                this.onRecvCmd = onRecvCmd;
                this.maxPackageSize = maxTcpPackageSize;

                
                clientSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                
                var ip = IPAddress.Parse(tcpServerIp);
                var ipEndPoint = new IPEndPoint(ip, tcpPort);
                var result = clientSocket.BeginConnect(
                    ipEndPoint,
                    OnConnectCallback,
                    clientSocket);
                var success = result.AsyncWaitHandle.WaitOne(maxWaitTime, true);
                if (!success)
                {
                    this.onException?.Invoke(new Exception("链接超时"));
                }
            }
            


            /// <summary>
            /// 关闭接收器
            /// </summary>
            public void CloseReceiver()
            {
                longPkg = null;
                _recvBuf = null;
                if (null != recvThread)
                {
                    recvThread.Interrupt();
                    recvThread.Abort();
                }
                recvThread = null;
                
                if (this.clientSocket != null && this.clientSocket.Connected)
                {
                    this.clientSocket.Close();
                }
                this.clientSocket = null;
#if UNITY_EDITOR
                if(ifEnableSocketLog())
                    Debug.Log($"Tcp[{Ip}:{Port}] 关闭连接");
#endif
            }


            /// <summary>
            /// 发送内容
            /// </summary>
            /// <param name="content"></param>
            public void Send(byte[] content)
            {
                try
                {
                    this.clientSocket.BeginSend(
                        content,
                        0,
                        content.Length,
                        SocketFlags.None,
                        OnTcpSend, this.clientSocket);
#if UNITY_EDITOR
                    if(ifEnableSocketLog())
                        Debug.Log($"Tcp[{Ip}:{Port}] 发送数据[{content.Length}]");
#endif
                }
                catch (Exception e)
                {
                    onException(e);
                }

                
            }
            
            

            #region Private
            
            private void OnTcpSend(IAsyncResult ar)
            {
                try
                {
                    var client = ar.AsyncState as Socket;
                    Assert.IsNotNull(client);
                    client?.EndSend(ar);
                }
                catch (Exception e)
                {
                    onException(e);
                }
            }      

            private void BeginReceive()
            {
                this._recvBuf=new byte[this.maxPackageSize];
                this.recvThread=new Thread(RecvThread);
                this.recvThread.Start();
#if UNITY_EDITOR
                if(ifEnableSocketLog())
                    Debug.Log($"Tcp[{Ip}:{Port}] 开始接收");
#endif
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
#if UNITY_EDITOR
                    if(ifEnableSocketLog())
                        Debug.Log($"Tcp[{Ip}:{Port}] 建立连接");
#endif
                    BeginReceive();
                }
                catch (Exception e)
                {
                    this.onException(e);
                }
            }
            
            private void OnRecvTcpData()
            {
                var pkgData = this.longPkg ?? this._recvBuf;

                while (this.recved > 0)
                {
                    int pkgSize;
                    int headerSize;
                    if (!TcpProtocol.ReadHeader(pkgData, pkgData.Length, out pkgSize, out headerSize))
                        break;
                    if (this.recved < pkgSize)
                        break;

                    var rawDataStart = headerSize;
                    var rawDataLen = pkgSize - headerSize;
//                    Debug.Log("Tcp包长：" + pkgSize + "包头："+headerSize);
                    
#if UNITY_EDITOR
                    if(ifEnableSocketLog())
                        Debug.Log($"Tcp[{Ip}:{Port}] 接收到数据包，[{pkgSize}]");
#endif
                    this.onRecvCmd(pkgData, rawDataStart, rawDataLen);

                    if (this.recved > pkgSize)
                    {
                        this._recvBuf=new byte[maxPackageSize];
                        Array.Copy(pkgData, pkgSize, this._recvBuf, 0, this.recved - pkgSize);
                    }

                    this.recved -= pkgSize;

                    if (this.recved == 0 && this.longPkg != null)
                    {
                        this.longPkg = null;
                        this.longPkgSize = 0;
                    }
                }
            }
            
            private void RecvThread()
            {
                while (true)
                {
                    if (!this.clientSocket.Connected)
                    {
#if UNITY_EDITOR
                        if(ifEnableSocketLog())
                            Debug.Log($"Tcp[{Ip}:{Port}] 连接断开，退出接收");
#endif
                        break;
                    }

                    try
                    {
                        var recvLen = 0;
                        if (this.recved < maxPackageSize)
                        {
                            recvLen = this.clientSocket.Receive(this._recvBuf,this.recved, maxPackageSize-this.recved,SocketFlags.None);
                        }
                        else
                        {// 大包
                            if (this.longPkg == null)
                            {// 尚未分配内存
                                int pkgSize;
                                int headSize;
                                TcpProtocol.ReadHeader(this._recvBuf, this.recved, out pkgSize, out headSize);
                                this.longPkgSize = pkgSize;
                                this.longPkg = new byte[longPkgSize];
                                Array.Copy(this._recvBuf, 0, this.longPkg, 0, this.recved); 
                            }

                            recvLen = this.clientSocket.Receive(this.longPkg, this.recved, this.longPkgSize - this.recved,
                                SocketFlags.None);
                        }
                    
                        if (recvLen > 0)
                        {// 收到数据长度
                            this.recved += recvLen;
                            OnRecvTcpData();
                        }
                    }
                    catch (Exception e)
                    {
                        if(this.onException!=null) 
                            this.onException(e);
                        else
                            throw e;
                        break;
                    }
                }

                Debug.LogWarning("退出接收线程");
            }            

            #endregion
    }
}
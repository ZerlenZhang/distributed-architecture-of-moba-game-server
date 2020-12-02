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

            private readonly int MaxBufferSize;

            private Socket clientSocket;
            
            private int usedBufferLength;
            private byte[] longPkg;
            private int longPkgSize;
            
            
            private Thread recvThread;
            private byte[] receiveBuffer;

            private Action<byte[], int, int> onRecvCmd;
            private Action<Exception> onException;
            private Func<bool> ifEnableSocketLog = () => true;

            #endregion
            
            public string Ip { get; }
            public int Port { get; }

            public bool IsValid => clientSocket != null && clientSocket.Connected;
            
            public TcpHelper(string tcpServerIp, int tcpPort, Action<Exception> onException, Action<byte[],int,int> onRecvCmd, int maxTcpBufferSize, int maxWaitTime, Func<bool> enableSocketLog=null)
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
                this.MaxBufferSize = maxTcpBufferSize;

                
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
                receiveBuffer = null;
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
#if DebugMode
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
#if DebugMode
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
                this.receiveBuffer=new byte[this.MaxBufferSize];
                this.recvThread=new Thread(RecvThread);
                this.recvThread.Start();
#if DebugMode
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
#if DebugMode
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
                var dataBuffer = this.longPkg ?? this.receiveBuffer;
                
                while (this.usedBufferLength > 0)
                {
                    if (!TcpProtocol.ReadHeader(dataBuffer, dataBuffer.Length,
                        out var pkgSize, 
                        out var headerSize))
                    {
                        break;
                    }
                    
                    if (this.usedBufferLength < pkgSize)
                    {
                        break;
                    }

                    var rawDataStart = headerSize;
                    var rawDataLen = pkgSize - headerSize;
#if DebugMode
                    if(ifEnableSocketLog())
                        Debug.Log($"Tcp[{Ip}:{Port}] 接收到数据包，[{pkgSize}]");
#endif
                    this.onRecvCmd(dataBuffer, rawDataStart, rawDataLen);

                    if (this.usedBufferLength > pkgSize)
                    {//粘包
                        Array.Copy(dataBuffer, pkgSize, dataBuffer, 0, this.usedBufferLength - pkgSize);
                    }
                    
                    this.usedBufferLength -= pkgSize;
                    
                    if (this.usedBufferLength == 0 && this.longPkg != null)
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
#if DebugMode
                        if(ifEnableSocketLog())
                            Debug.Log($"Tcp[{Ip}:{Port}] 连接断开，退出接收");
#endif
                        break;
                    }

                    try
                    {
                        var recvLen = 0;
                        if (this.usedBufferLength < MaxBufferSize)
                        {
                            recvLen = this.clientSocket.Receive(this.receiveBuffer,this.usedBufferLength, MaxBufferSize-this.usedBufferLength,SocketFlags.None);
                        }
                        else
                        {// 大包
                            Debug.LogWarning($"出现大包！");
                            if (this.longPkg == null)
                            {// 尚未分配内存
                                TcpProtocol.ReadHeader(this.receiveBuffer, this.usedBufferLength, out var pkgSize, out var headSize);
                                this.longPkgSize = pkgSize;
                                this.longPkg = new byte[longPkgSize];
                                Array.Copy(this.receiveBuffer, 0, this.longPkg, 0, this.usedBufferLength); 
                            }

                            recvLen = this.clientSocket.Receive(this.longPkg, this.usedBufferLength, this.longPkgSize - this.usedBufferLength,
                                SocketFlags.None);
                        }
                    
                        if (recvLen > 0)
                        {// 收到数据长度
                            this.usedBufferLength += recvLen;
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
using System;
using System.Net.Sockets;
using System.Threading;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Network
{
    public static class TcpProtocol
    {
        public const int HeadSize = 2;
        
        /// <summary>
        /// 将byte数组打包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Pack(byte[] data)
        {
            var len = data.Length;
            if (len > 65535 - 2)
                return null;

            var cmdLen = len + HeadSize;
            var cmd = new byte[cmdLen];
            cmd.WriteUShortLe((ushort)cmdLen);
            cmd.WriteBytes(data, HeadSize);

            return cmd;
        }

        /// <summary>
        /// 读取Tcp包头信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLen"></param>
        /// <param name="pkgSize"></param>
        /// <param name="headSize"></param>
        /// <returns></returns>
        public static bool ReadHeader(byte[] data, int dataLen, out int pkgSize, out int headSize)
        {
            pkgSize = 0;
            headSize = 0;
            if (dataLen < 2)
                return false;

            pkgSize = data.ReadUShortLe();
            headSize = 2;
            return true;
        }
        
        
        /// <summary>
        /// Tcp协议接收者
        /// 新建线程接收
        /// 关闭时需要调用CloseReceiver
        /// 自动维护Tcp拆包，粘包，
        /// </summary>
        public class TcpReceiver
        {
            private readonly int RecvLen;

            private Socket clientSocket;
            
            private int recved;
            private byte[] longPkg;
            private int longPkgSize;
            
            
            private Thread recvThread;
            private byte[] _recvBuf;

            private Action<byte[], int, int> onRecvCmd;
            private Action<Exception> onException;
            
            /// <summary>
            /// Tcp消息接收者
            /// </summary>
            /// <param name="client"></param>
            /// <param name="onRecvCmd">byte元数据，数据包起始索引，数据包长度</param>
            /// <param name="maxTcpPackageSize"></param>
            /// <param name="onException">异常处理函数</param>
            public TcpReceiver(Socket client, Action<byte[],int,int> onRecvCmd, int maxTcpPackageSize=4096, Action<Exception> onException=null)
            {
                Assert.IsNotNull(client);
                this.clientSocket = client;
                Assert.IsNotNull(onRecvCmd);
                this.RecvLen = maxTcpPackageSize;
                this._recvBuf=new byte[this.RecvLen];
                this.onRecvCmd = onRecvCmd;
                this.onException = onException;
                
                this.recvThread=new Thread(RecvThread);
                this.recvThread.Start();
            }
            
            private void OnRecvTcpData()
            {
                var pkgData = this.longPkg ?? this._recvBuf;

                while (this.recved > 0)
                {
                    int pkgSize;
                    int headerSize;
                    if (!ReadHeader(pkgData, pkgData.Length, out pkgSize, out headerSize))
                        break;
                    if (this.recved < pkgSize)
                        break;

                    var rawDataStart = headerSize;
                    var rawDataLen = pkgSize - headerSize;
//                    Debug.Log("Tcp包长：" + pkgSize + "包头："+headerSize);
                    
                    this.onRecvCmd(pkgData, rawDataStart, rawDataLen);

                    if (this.recved > pkgSize)
                    {
                        this._recvBuf=new byte[RecvLen];
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
                        Debug.Log("没有链接，退出接收");
                        break;
                    }

                    try
                    {
                        var recvLen = 0;
                        if (this.recved < RecvLen)
                        {
                            recvLen = this.clientSocket.Receive(this._recvBuf,this.recved, RecvLen-this.recved,SocketFlags.None);
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
                    recvThread = null;
                }
            }
            
        }
    }
}
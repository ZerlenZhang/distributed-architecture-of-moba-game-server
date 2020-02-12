using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ReadyGamerOne.Common;
using UnityEngine;

namespace Moba.Network
{
    public class NetworkMgr:MonoSingleton<NetworkMgr>
    {
        public string serverIp;
        public int serverPort;

        private Socket clientSocket;
        private bool isConnected;


        private Thread recvThread;
        private byte[] recv_buf=new byte[4096];
        
        

        protected override void Awake()
        {
            base.Awake();
            ConnectServer();
            Invoke("CloseSocket", 5);
        }

        void ConnectServer()
        {
            try
            {
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

        private void OnConnectCallback(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                socket?.EndConnect(ar);

                isConnected = true;
                this.recvThread = new Thread(RecvThread);
                this.recvThread.Start();
                
                OnConnectError("connect success");
            }
            catch (Exception e)
            {
                OnConnectError(e);
                isConnected = false;
            }
        }


        private void RecvThread()
        {
            if (!this.isConnected)
                return;
            while (true)
            {
                if (!this.clientSocket.Connected)
                    break;

                try
                {
                    var recvLen = this.clientSocket.Receive(this.recv_buf);
                    if (recvLen > 0)
                    {// 收到数据长度
                        
                    }
                }
                catch (Exception e)
                {
                    OnConnectError(e);
//                    this.clientSocket.Shutdown(SocketShutdown.Both);
                    this.clientSocket.Close();
                    this.isConnected = false;
                    break;
                }
            }
        }

        private void CloseSocket()
        {
            if (!this.isConnected)
            {
                return;
            }

            this.recvThread?.Abort();

            if (this.clientSocket != null && this.clientSocket.Connected)
            {
                this.clientSocket.Close();
            }
        }
    }
}
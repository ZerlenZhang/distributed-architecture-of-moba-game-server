using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace ReadyGamerOne.Utility
{
    public class NetUtil
    {
        /// <summary>
        /// 获取空闲端口
        /// </summary>
        /// <returns></returns>
        public static int GetUdpPort()
        {
            int port = -1;
            for (var i = 8000; i <= 12000; i++)
            {
                if (IsPortOccupiedFunc(i) == false)
                {
                    port = i;
                    break;
                }
            }
            return port;
        }
    
        /// <summary>
        /// 判断指定端口号是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool IsPortOccupiedFunc(int port)
        {
            bool result = false;
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("netstat", "-an")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true
                    }
                };
                p.Start();
                string output = p.StandardOutput.ReadToEnd().ToLower();
                string ip1 = "127.0.0.1";
                string ip2 = "0.0.0.0";
                System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                var ipList = new List<string> {ip1, ip2};
                ipList.AddRange(addressList.Select(t => t.ToString()));
                
                if (ipList.Any(t => output.IndexOf(t + ":" + port.ToString(), StringComparison.Ordinal) >= 0))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("NetUtil.IsPortOccupedFun1 port :" + ex);
            }
            return result;
        }
        
        /// <summary>
        /// 获取页面html
        /// </summary>
        /// <param name="url">请求的地址</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static void HttpGetPageHtmlAsync(string url, string encoding,Action<string> onFinish)
        {
            using (var webClient = new WebClient())
            {
                var encode = Encoding.GetEncoding(encoding);
                webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36");
                webClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                webClient.DownloadDataAsync(new Uri(url));
                webClient.DownloadDataCompleted += (sender, args) =>
                {
                    onFinish(encode.GetString(args.Result));
                };
            }
        }
        
        /// <summary>
        /// 从html中通过正则找到ip信息(只支持ipv4地址)
        /// </summary>
        /// <param name="pageHtml"></param>
        /// <returns></returns>
        public static string GetIpFromHtml(string pageHtml)
        {
            //验证ipv4地址
            string reg = @"(?:(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))\.){3}(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))";
            string ip = "";
            Match m = Regex.Match(pageHtml, reg);
            if (m.Success)
            {
                ip = m.Value;
            }
            return ip;
        }

        /// <summary>
        /// 获取当前Ip
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static void GetCurrentIpv4Async(
            Action<string> onUseIp,
            string url="http://www.net.cn/static/customercare/yourip.asp",
            string encoding="gbk")
        {
            HttpGetPageHtmlAsync(url, encoding,
                content =>
                {
                    onUseIp(GetIpFromHtml(content));
                });
        }
        
        /// <summary>
        /// 获取本机所有ip地址
        /// </summary>
        /// <param name="netType">"InterNetwork":ipv4地址，"InterNetworkV6":ipv6地址</param>
        /// <returns>ip地址集合</returns>
        public static List<string> GetLocalIpAddress(AddressFamily netType)
        {
            string hostName = Dns.GetHostName();     //获取主机名称 
            IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址 
 
            List<string> IPList = new List<string>();

            for (int i = 0; i < addresses.Length; i++)
            {
                if (addresses[i].AddressFamily == netType)
                {
                    IPList.Add(addresses[i].ToString());
                }
            }
            return IPList;
        }

        
        private class UdpSetupWorker
        {
            private int curIndex = 0;
            private List<string> ipList;
            private Socket socket;
            private int localPort;
            private IPEndPoint targetIpEndPoint;
            public bool IsFinished { get; private set; } = false;
            public UdpSetupWorker(IPEndPoint targetIpEndPoint, int localPort)
            {
                this.localPort = localPort;
                this.targetIpEndPoint = targetIpEndPoint;
                ipList=GetLocalIpAddress(AddressFamily.InterNetwork);
            }

            public void Start(byte[] initBytes, Action recvThread, Action<string, Socket, Thread> onGetSuitableIp)
            {
                if (curIndex >= ipList.Count)
                {
                     IsFinished = true;
                     onGetSuitableIp(null,null,null);
                     return;
                }

                var curIp = ipList[curIndex++];
                var curPort = NetUtil.GetUdpPort();
                Assert.IsNull(this.socket);

                this.socket=new Socket(
                 AddressFamily.InterNetwork,
                 SocketType.Dgram,
                 ProtocolType.Udp);
                //绑定本地端口
                var localPoint = new IPEndPoint(IPAddress.Parse(curIp), curPort);
                this.socket.Bind(localPoint);

                try
                {
                 this.socket.BeginSendTo(initBytes, 0, initBytes.Length, SocketFlags.None,
                     targetIpEndPoint,
                     (iar) =>
                     {
                         bool error = false;
                         try
                         {
                             this.socket.EndSendTo(iar);
                         }
                         catch
                         {
                             // Debug.Log($"[CallBack][CurIndex:{curIndex-1}][{curIp}:{curPort}]{e}");
                             error = true;
                         }
                         finally
                         {
                             if (!error)
                             {
                                 IsFinished = true;
                                 var thread = new Thread(new ThreadStart(recvThread));
                                 onGetSuitableIp(curIp, this.socket, thread);
                                 thread.Start();
                             }
                             else
                             {
                                 socket.Dispose();
                                 socket.Close();
                                 socket = null;
                                 Start(initBytes, recvThread, onGetSuitableIp);
                             }
                         }
                     }, null);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[BeginSendTo][CurIndex:{curIndex-1}][{curIp}:{curPort}]{e}");
                    Start(initBytes,recvThread,onGetSuitableIp);
                }
            }
        }
        public static void SetupUdp(IPEndPoint targetIpEndPoint, int localPort,Action recvThread, Action<string, Socket, Thread> onSetUpUdp, byte[] initBytes=null)
        {
            new UdpSetupWorker(targetIpEndPoint, localPort).Start(initBytes ?? new byte[0], recvThread, onSetUpUdp);
        }
    }     
}
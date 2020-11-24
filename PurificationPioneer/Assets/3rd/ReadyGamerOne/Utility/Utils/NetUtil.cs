using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace ReadyGamerOne.Utility
{
    public class NetUtil
    {
        public static int GetUdpPort()
        {
            int port = -1;
            for (int i = 8000; i <= 12000; i++)
            {
                if (IsPortOccupedFun1(i) == false)
                {
                    port = i;
                    break;
                }
            }

            if (port != -1)
            {
                //Debug.Log("find port :" + port);
            }
            return port;
        }
    
        /// <summary>
        /// 判断指定端口号是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool IsPortOccupedFun1(int port)
        {
            bool result = false;
            try
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo("netstat", "-an");
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                string output = p.StandardOutput.ReadToEnd().ToLower();
                string ip1 = "127.0.0.1";
                string ip2 = "0.0.0.0";
                System.Net.IPAddress[] addressList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                List<string> ipList = new List<string>();
                ipList.Add(ip1);
                ipList.Add(ip2);
                for (int i = 0; i < addressList.Length; i++)
                {
                    ipList.Add(addressList[i].ToString());
                }
                for (int i = 0; i < ipList.Count; i++)
                {
                    //报告指定 Unicode 字符或字符串在此实例中的第一个匹配项的从零开始的索引。 如果未在此实例中找到该字符或字符串，则此方法返回 -1。
                    //if (output.IndexOf("tcp " + ipList[i] + ":" + port.ToString()) >= 0)
                    //if (output.IndexOf(port.ToString()) >= 0)
                    if (output.IndexOf(ipList[i] + ":" + port.ToString()) >= 0)
                    {
                        result = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("NetUtil.IsPortOccupedFun1 port :" + ex);
            }
            return result;
        }
    }
}
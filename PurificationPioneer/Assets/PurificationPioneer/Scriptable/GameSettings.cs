using ReadyGamerOne.Common;
using ReadyGamerOne.Utility;
using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    public class GameSettings:ScriptableSingleton<GameSettings>
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("净化先锋/开发者选项")]
        private static void ShowInspector()
        {
            UnityEditor.Selection.activeInstanceID = Instance.GetInstanceID();
        }
        public void LogIp()
        {
            NetUtil.GetCurrentIpv4Async(ip => Debug.Log(ip));
            Debug.Log("Test");
        }
#endif
        [Header("IP和端口配置")]
        [SerializeField] private string gatewayIp = "121.196.178.141";
        public string GatewayIp => gatewayIp;

        [SerializeField] private int gatewayPort = 6080;
        public int GatewayPort => gatewayPort;

        #region UdpServerIp

        private string udpServerIp;
        public string UdpServerIp => udpServerIp;
        public void SetUdpServerIp(string ip) => udpServerIp = ip;        

        #endregion

        #region UdpServerPort

        private int udpServerPort;
        public int UdpServerPort => udpServerPort;
        public void SetUdpServerPort(int port) => udpServerPort = port;        

        #endregion

        #region UdpLocalIp

        private string udpLocalIp;
        public string UdpLocalIp => udpLocalIp;
        public void SetUdpLocalIp(string ip) => udpLocalIp = ip;        

        #endregion
        
        #region UdpLocalPort

        private int udpLocalPort;
        public int UdpLocalPort => udpLocalPort;
        public void SetUdpLocalPort(int port) => udpLocalPort = port;        

        #endregion

        
        [Header("全局变量")]
        [SerializeField] private int maxTcpPackageSize = 4096;
        public int MaxTcpPackageSize => maxTcpPackageSize;
        
        [SerializeField] private int maxUdpPackageSize = 4096;
        public int MaxUdpPackageSize => maxUdpPackageSize;

        [SerializeField] private int maxWaitTime = 5000;
        public int MaxWaitTime => maxWaitTime;

        [Header("调试开关")]
        [SerializeField] private bool closeSocketOnAnyExpection = true;
        public bool CloseSocketOnAnyExpection => closeSocketOnAnyExpection;
        [SerializeField] private bool enableSocketLog = true;
        public bool EnableSocketLog => enableSocketLog;
        [SerializeField] private bool enableProtoLog = true;

        public bool EnableProtoLog => enableProtoLog;
        
        
        [Header("作弊模式")]
        [SerializeField] private bool debugMode = true;
        public bool DebugMode => debugMode;
        
        [SerializeField] private string debugAccount = "developer";
        public string DebugAccount => debugAccount;
        
        [SerializeField] private string debugPassword = "developer_password";
        public string DebugPassword => debugPassword;
        
    }
}
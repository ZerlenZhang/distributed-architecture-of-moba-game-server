using System.Net;
using System.Net.Sockets;
using DG.Tweening;
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
            Debug.Log("Part_1");
            foreach (var ip in NetUtil.GetLocalIpAddress(AddressFamily.InterNetwork))
            {
                Debug.Log($"LocalIp: {ip}");
            }

            Debug.Log("Part_2");
            NetUtil.GetCurrentIpv4Async(ip => Debug.Log(ip));
                        
        }
#endif

        #region IP和端口配置

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

        #endregion
        
        #region Socket设置

        [Header("Socket设置")]
        [SerializeField] private int maxTcpPackageSize = 4096;
        public int MaxTcpPackageSize => maxTcpPackageSize;
        
        [SerializeField] private int maxUdpPackageSize = 4096;
        public int MaxUdpPackageSize => maxUdpPackageSize;

        [SerializeField] private int maxWaitTime = 5000;
        public int MaxWaitTime => maxWaitTime;        

        #endregion

        #region 调试开关

        [Header("调试开关")] 
        [SerializeField] private int netMsgTimes = 2;

        public int NetMsgTimes => netMsgTimes;
        
        #region CloseSocketOnAnyException
        [SerializeField] private bool closeSocketOnAnyException = true;
        public bool CloseSocketOnAnyException => closeSocketOnAnyException;
        public void SetCloseSocketOnAnyException(bool value) => closeSocketOnAnyException = value;
        #endregion

        #region EnableSocketLog

        [SerializeField] private bool enableSocketLog = true;
        public bool EnableSocketLog => enableSocketLog;
        public void SetEnableSocketLog(bool value) => enableSocketLog = value;        

        #endregion

        #region EnableProtoLog

        [SerializeField] private bool enableProtoLog = true;
        public bool EnableProtoLog => enableProtoLog;
        public void SetEnableProtoLog(bool value) => enableProtoLog = value;        

        #endregion

        #region EnableFrameSyncLog

        [SerializeField] private bool enableFrameSyncLog = true;

        public bool EnableFrameSyncLog => enableFrameSyncLog;

        #endregion

        #region EnableInputLog

        [SerializeField] private bool enableInputLog = true;

        public bool EnableInputLog => enableInputLog;

        #endregion

        #endregion
        
        #region Ui设置

        [Header("Ui设置")]
        [SerializeField] private Ease matchPanelEaseType = Ease.InCirc;
        public Ease MatchPanelEaseType => matchPanelEaseType;
        [SerializeField] private float matchPanelEaseTime = 1;
        public float MatchPanelEaseTime=>matchPanelEaseTime;
        

        #endregion
        
        #region 作弊模式

        [Header("作弊模式")]
        
        #region DebugMode

        [SerializeField] private bool debugMode = true;
        public bool DebugMode => debugMode;
        public void SetDebugMode(bool value) => debugMode = value;        

        #endregion
        
        [SerializeField] private string debugAccount = "developer";
        public string DebugAccount => debugAccount;
        
        [SerializeField] private string debugPassword = "developer_password";
        public string DebugPassword => debugPassword;        

        #endregion

        #region 输入配置

        [Header("PC端攻击键")] [SerializeField] private KeyCode _attackKey = KeyCode.Mouse0;

        public KeyCode AttackKey => _attackKey;
        [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

        public KeyCode JumpKey => _jumpKey;
        #endregion
    }
}
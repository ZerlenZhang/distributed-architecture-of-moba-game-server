using DG.Tweening;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.Scriptable
{
    public class GameSettings:ScriptableSingleton<GameSettings>
    {
        #region Editor

#if UNITY_EDITOR
        [UnityEditor.MenuItem("净化先锋/开发者选项")]
        private static void ShowInspector()
        {
            UnityEditor.Selection.activeInstanceID = Instance.GetInstanceID();
        }
#endif        

        #endregion

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

        [SerializeField] private bool _enableInputLog = true;

        public bool EnableInputLog => _enableInputLog;

        #endregion

        #region EnableBulletLog

        [SerializeField] private bool enableBulletLog = false;

        public bool EnableBulletLog => enableBulletLog;

        #endregion

        #region EnableMoveLog

        [SerializeField] private bool _enableMoveLog;

        public bool EnableMoveLog => _enableMoveLog;

        #endregion

        #region EnablePhysicsLog

        [SerializeField] private bool enablePhysicsLog;

        public bool EnablePhysicsLog => enablePhysicsLog;

        #endregion
        
        #endregion
        
        #region Ui设置

        [Header("Ui设置")]
        [SerializeField] private Ease matchPanelEaseType = Ease.InCirc;
        public Ease MatchPanelEaseType => matchPanelEaseType;
        [SerializeField] private float matchPanelEaseTime = 1;
        public float MatchPanelEaseTime=>matchPanelEaseTime;
        

        #endregion
        
        #region 开发者模式

        [Header("开发者模式")]
        
        #region DebugMode

        [SerializeField] private bool developerMode = true;
        public bool DeveloperMode => developerMode;
        public void SetDeveloperMode(bool value) => developerMode = value;        

        #endregion

        [SerializeField] private bool workAsAndroid = false;

        public bool WorkAsAndroid => workAsAndroid;
        
        [SerializeField] private string debugAccount = "developer";
        public string DebugAccount => debugAccount;
        
        [SerializeField] private string debugPassword = "developer_password";
        public string DebugPassword => debugPassword;        

        #endregion

        #region 输入配置

        [Header("PC端攻击键")] [SerializeField] private KeyCode _attackKey = KeyCode.Mouse0;

        [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

        [SerializeField] private KeyCode _heroFirstSkillKey = KeyCode.Q;
        [SerializeField] private KeyCode _heroSecondSkillKey = KeyCode.E;
        [SerializeField] private KeyCode _weaponFirstSkillKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode _weaponSecondSkillKey = KeyCode.R;
        
        [SerializeField] private KeyCode _gameStateKey = KeyCode.Tab;
        

        public KeyCode AttackKey => _attackKey;

        public KeyCode HeroFirstSkillKey => _heroFirstSkillKey;

        public KeyCode HeroSecondSkillKey => _heroSecondSkillKey;

        public KeyCode WeaponFirstSkillKey => _weaponFirstSkillKey;

        public KeyCode WeaponSecondSkillKey => _weaponSecondSkillKey;
        public KeyCode GameStateKey => _gameStateKey;

        public KeyCode JumpKey => _jumpKey;
        #endregion
        
        #region Gizmos设置

        [Header("Gizmos设置")] [SerializeField] private int defaultStateFontSize = 20;

        public int DefaultStateFontSize => defaultStateFontSize;

        #endregion

        #region Physics

        [Header("Physics")] [SerializeField] private float _physicsDeltaTime = 0.02f;
        public float PhysicsDeltaTime => _physicsDeltaTime;
        [SerializeField] private int _maxCollisionCount = 8;
        public int MaxCollisionCount => _maxCollisionCount;
        [SerializeField] private float defaultStaticFriction = 0.2f;
        public float DefaultStaticFriction => defaultStaticFriction;
        [SerializeField] private float defaultDynamicFriction = 0.1f;
        public float DefaultDynamicFriction => defaultDynamicFriction;
        [SerializeField] private float defaultBounciness = 0;
        public float DefaultBounciness => defaultBounciness;
        [SerializeField] private float _minDetectableDistance = 0.001f;

        public float MinDetectableDistance => _minDetectableDistance;

        #endregion

        #region Painting

        [Header("Painting")] [SerializeField]private float _brushScaler = 10.0f;

        public float BrushScaler => _brushScaler;

        #endregion
    }
}
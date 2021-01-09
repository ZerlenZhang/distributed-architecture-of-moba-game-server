using PurificationPioneer.Scriptable;
using UnityEngine;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 子弹基类
    /// </summary>
    /// <typeparam name="TBullet">子弹类型</typeparam>
    /// <typeparam name="TBulletStrategy">子弹策略类型</typeparam>
    /// <typeparam name="TBulletConfig">子弹配置类型</typeparam>
    /// <typeparam name="TBulletState">子弹状态类型</typeparam>
    public abstract class AbstractFrameSyncBullet<TBullet, TBulletStrategy, TBulletConfig, TBulletState> 
        : MonoBehaviour, IFrameSyncUnit
        where TBullet:AbstractFrameSyncBullet<TBullet, TBulletStrategy, TBulletConfig, TBulletState>
        where TBulletStrategy:class, IBulletStrategy<TBullet, TBulletConfig, TBulletState>,new()
        where TBulletState: class, IBulletState
        where TBulletConfig: class, IBulletConfig
    {
        public LayerMask attackLayer;
        
        /// <summary>
        /// 配置类型，内部只读不写
        /// </summary>
        private TBulletConfig BulletConfig => _bulletConfig;
        /// <summary>
        /// 子弹状态，每个子弹都要有一个不同的
        /// </summary>
        private TBulletConfig _bulletConfig;
        private TBulletState _bulletState;
        /// <summary>
        /// 策略类型一个就够了
        /// </summary>
        private static readonly TBulletStrategy BulletStrategy = new TBulletStrategy();

        /// <summary>
        /// 在新的逻辑帧到来之前，已经经过的时间
        /// </summary>
        private float _timeSinceLastLogicFrame;
        
        /// <summary>
        /// bullet已经存活的逻辑时长
        /// </summary>
        private float _bulletLogicAgeTime;

        private float _bulletRenderAgeTime;

        /// <summary>
        /// 是否已经被销毁
        /// </summary>
        private bool _isDestroyed = false;

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// 子弹是否需要继续飞行
        /// </summary>
        private bool _isBulletRunning = true;
        
        /// <summary>
        /// 子弹初始化
        /// </summary>
        /// <param name="bulletConfig"></param>
        /// <param name="bulletState"></param>
        public void Initialize(TBulletConfig bulletConfig, TBulletState bulletState)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            _isDestroyed = false;
            _isBulletRunning = true;
            _bulletLogicAgeTime = 0;
            _bulletRenderAgeTime = 0;
            _timeSinceLastLogicFrame = 0;
            _bulletConfig = bulletConfig;
            _bulletState = bulletState;
            bulletState.RendererPosition = bulletState.LogicPosition;
            FrameSyncMgr.AddFrameSyncUnit(this);
#if DebugMode
            if(GameSettings.Instance.EnableBulletLog)
                BattleSceneMgr.Instance.eventOnGameState += OnBulletStateGUI;
#endif
        }

        private void Update()
        {
            if (!_isInitialized || !BulletStrategy.IsBulletActivate(this as TBullet) || !_isBulletRunning)
                return;

            var deltaTime = Time.deltaTime;
            
            _timeSinceLastLogicFrame += deltaTime;
            _bulletRenderAgeTime += deltaTime;
            
            if (_bulletRenderAgeTime > BulletConfig.MaxLife)
            {
                _timeSinceLastLogicFrame -= _bulletRenderAgeTime - BulletConfig.MaxLife;
            }

            //更新子弹渲染状态
            BulletStrategy.UpdateStateOnRendererFrame(_timeSinceLastLogicFrame, BulletConfig, ref _bulletState);

            if (_bulletRenderAgeTime > BulletConfig.MaxLife)
            {
                _isBulletRunning = false;
            }
        }

        #region Debug

        private float lastCallTime;

        protected virtual void OnBulletStateGUI(GUIStyle defaultGuiStyle)
        {
            GUILayout.Label($"Bullet[{InstanceId}] tickDelta\t{Time.timeSinceLevelLoad-lastCallTime}", defaultGuiStyle);
        }

        #endregion
        

        #region IFrameSyncUnit

        public int InstanceId => GetInstanceID();
        public void OnLogicFrameUpdate(float deltaTime)
        {
            //debug
            lastCallTime = Time.timeSinceLevelLoad;
            
            //bullet logic
            _timeSinceLastLogicFrame = 0;
            _bulletLogicAgeTime += deltaTime;
            
            if (_bulletLogicAgeTime > BulletConfig.MaxLife)
                deltaTime -= (_bulletLogicAgeTime - BulletConfig.MaxLife);

            //更新子弹逻辑状态
            BulletStrategy.UpdateStateOnLogicFrame(deltaTime, BulletConfig, ref _bulletState);
            
            //子弹击中物体
            if (HitTest(_bulletState, BulletConfig))
            {
                return;
            }
            
            //子弹寿命结束
            if(_bulletLogicAgeTime>=BulletConfig.MaxLife)
            {
                DestroyBullet();
            }
            
#if DebugMode
            else if (GameSettings.Instance.EnableBulletLog)
            {
                Debug.Log($"[AbstractFrameSyncBullet] BulletLife: {_bulletLogicAgeTime}/{_bulletConfig.MaxLife}");
            }
#endif
        }

        #endregion
        

        /// <summary>
        /// 子弹击中物体的检测和处理
        /// </summary>
        /// <param name="currentBulletState"></param>
        /// <param name="bulletConfig"></param>
        /// <returns></returns>
        protected abstract bool HitTest(TBulletState currentBulletState, TBulletConfig bulletConfig);

        #region 子弹的销毁

        /// <summary>
        /// 销毁子弹
        /// </summary>
        protected void DestroyBullet()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;
#if DebugMode
            if (GameSettings.Instance.EnableBulletLog)
            {
                Debug.Log($"[AbstractFrameSyncBullet] DestroyBullet");
            }
#endif
            OnBulletDestroy();
            BulletStrategy.DisableBullet(this as TBullet);
        }

        protected virtual void OnBulletDestroy(bool destroyBySceneLoad=false)
        {
            _isInitialized = false;
            FrameSyncMgr.RemoveFrameSyncUnit(this);
#if DebugMode
            if(GameSettings.Instance.EnableBulletLog)
                BattleSceneMgr.Instance.eventOnGameState -= OnBulletStateGUI;
#endif
        }
        
        private void OnDestroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;
            OnBulletDestroy(true);
        }        

        #endregion
        
        

    }
} 
using PurificationPioneer.Data;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PurificationPioneer.Script
{
    public class DirectionFrameSyncBullet:
        AbstractFrameSyncBullet<DirectionFrameSyncBullet,DirectionBulletStrategy,BulletData,DirectionBulletState>
    {
        public void Init(int bulletId, Vector3 position, Vector3 direction)
        {
            var state = new DirectionBulletState(transform)
            {
                LogicPosition = position,
                Direction = direction
            };
            Initialize(CsvMgr.GetData<BulletData>(bulletId.ToString()),state);
        }
        
        protected override bool HitTest(DirectionBulletState currentBulletState, BulletData bulletConfig)
        {
            return false;
        }
    }
    
    
    public class DirectionBulletState:IBulletState
    {
        private readonly Transform _transform;
        public DirectionBulletState(Transform transform) => _transform = transform;
        public Vector3 LogicPosition { get; set; }

        public Vector3 RendererPosition
        {
            get=>_transform.position;
            set=>_transform.position=value;
        }

        public Vector3 Direction
        {
            get => _transform.forward;
            set => _transform.forward = value;
        }
    }

    public class DirectionBulletStrategy : 
        IBulletStrategy<DirectionFrameSyncBullet, BulletData, DirectionBulletState>
    {
        public void DisableBullet(DirectionFrameSyncBullet frameSyncBullet)
        {
#if DebugMode
            if (GameSettings.Instance.EnableBulletLog)
            {
                Debug.Log($"[DirectBullet] Destroy!");
            }
#endif
            Object.Destroy(frameSyncBullet.gameObject);
        }

        public bool IsBulletActivate(DirectionFrameSyncBullet frameSyncBullet)
        {
            return frameSyncBullet.gameObject.activeSelf;
        }

        public void UpdateStateOnRendererFrame(float timeSinceLastLogicFrame, BulletData bulletConfig,
            ref DirectionBulletState bulletState)
        {
            bulletState.RendererPosition = bulletState.LogicPosition +
                                   bulletState.Direction * (timeSinceLastLogicFrame * bulletConfig.speed);
        }

        public void UpdateStateOnLogicFrame(float deltaTime, BulletData bulletConfig, ref DirectionBulletState bulletState)
        {
            bulletState.LogicPosition = bulletState.LogicPosition +
                                        bulletState.Direction * (deltaTime * bulletConfig.speed);
                                        
        }
    }
    
}
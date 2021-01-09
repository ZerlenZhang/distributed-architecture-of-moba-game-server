using Es.InkPainter;
using PurificationPioneer.Data;
using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PurificationPioneer.Script
{
    public class DirectionFrameSyncBullet:
        AbstractFrameSyncBullet<DirectionFrameSyncBullet,DirectionBulletStrategy,BulletData,DirectionBulletState>
    {
        
        
        public BrushConfigAsset brushConfig;
        private RaycastHit[] HitInfos=new RaycastHit[GlobalVar.MaxHitInfoCount];
        public void Init(int bulletId, Vector3 position, Vector3 direction)
        {
            var state = new DirectionBulletState(transform)
            {
                LogicPosition = position,
                Direction = direction
            };
            var config = CsvMgr.GetData<BulletData>(bulletId.ToString());
            transform.localScale = Vector3.one * config.radius;
            Initialize(config,state);
        }
        
        protected override bool HitTest(DirectionBulletState currentBulletState, BulletData bulletConfig)
        {
            var hitCount =
                Physics.RaycastNonAlloc(
                    new Ray(currentBulletState.LogicPosition, currentBulletState.Direction),
                    HitInfos,
                    bulletConfig.radius,
                    attackLayer);
                // Physics.SphereCastNonAlloc(
                // currentBulletState.LogicPosition,
                // bulletConfig.radius, 
                // currentBulletState.Direction, 
                // HitInfos, 
                // 0.01f, 
                // attackLayer);

            if (hitCount == 0)
                return false;
            
            for (var i = 0; i < hitCount; i++)
            {
                var hitInfo = HitInfos[i];
                var canvas = hitInfo.collider.GetComponent<InkCanvas>();
                if (canvas != null)
                {
                    canvas.Paint(brushConfig.brush, hitInfo);
                    // Debug.Log($"[{hitCount}]Paint!{hitInfo.collider.name}");
                }
            }
            DestroyBullet();
            return true;
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
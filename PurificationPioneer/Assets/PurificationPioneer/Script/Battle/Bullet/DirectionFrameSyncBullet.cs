using Es.InkPainter;
using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PurificationPioneer.Script
{
   
    public class DirectionFrameSyncBullet:
        AbstractFrameSyncBullet<
            DirectionFrameSyncBullet,
            DirectionBulletStrategy,
            DirectionBulletConfigAsset,
            DirectionBulletState>
    {
        private RaycastHit[] HitInfos=new RaycastHit[GlobalVar.MaxHitInfoCount];

        public override bool Initialize(DirectionBulletConfigAsset bulletConfig, DirectionBulletState bulletState)
        {
            if (!base.Initialize(bulletConfig, bulletState))
                return false;
            transform.localScale = Vector3.one * bulletConfig.Radius;
            return true;
        }

        protected override bool HitTest(DirectionBulletState currentBulletState, DirectionBulletConfigAsset bulletConfig)
        {
            var hitCount =
                Physics.RaycastNonAlloc(
                    new Ray(currentBulletState.LogicPosition, currentBulletState.Direction),
                    HitInfos,
                    bulletConfig.Radius,
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
                    canvas.Paint(bulletConfig.BrushConfig.brush, hitInfo);
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
        IBulletStrategy<DirectionFrameSyncBullet, DirectionBulletConfigAsset, DirectionBulletState>
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

        public void UpdateStateOnRendererFrame(float timeSinceLastLogicFrame, DirectionBulletConfigAsset bulletConfig,
            ref DirectionBulletState bulletState)
        {
            bulletState.RendererPosition = bulletState.LogicPosition +
                                   bulletState.Direction * (timeSinceLastLogicFrame * bulletConfig.Speed);
        }

        public void UpdateStateOnLogicFrame(float deltaTime, DirectionBulletConfigAsset bulletConfig, ref DirectionBulletState bulletState)
        {
            bulletState.LogicPosition += bulletState.Direction * (deltaTime * bulletConfig.Speed);
                                        
        }
    }
    
}
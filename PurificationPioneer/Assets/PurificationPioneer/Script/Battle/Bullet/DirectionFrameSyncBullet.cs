using Es.InkPainter;
using PurificationPioneer.Global;
using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.Assertions;
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

    public class DirectionBulletState
    {
        private readonly PpRigidbody _rigidbody;
        private IPpRigidbodyState _rigidbodyState;
        private Vector3 _logicPosition;
        /// <summary>
        /// 保存当前状态，
        /// </summary>
        public void SaveState()
        {
            _logicPosition = _rigidbody.Position;
            _rigidbody.GetStateNoAlloc(ref _rigidbodyState);
        }

        /// <summary>
        /// 应用上次保存的状态并模拟一段时间
        /// </summary>
        /// <param name="deltaTime"></param>
        public void ApplyAndSimulate(float deltaTime)
        {
            _rigidbody.ApplyRigidbodyState(_rigidbodyState);
            _rigidbody.Simulate(deltaTime);
        }
        public DirectionBulletState(PpRigidbody rigidbody, Vector3 dir, Vector3 logicPosition)
        {
            _rigidbody = rigidbody;
            Assert.IsTrue(_rigidbody);

            _logicPosition = logicPosition;
            _rigidbody.Position = logicPosition;
            Direction = dir;
            _rigidbodyState = _rigidbody.GetState();
        }

        public Vector3 LogicPosition => _logicPosition;

        public Vector3 Direction
        {
            get => _rigidbody.transform.forward;
            private set => _rigidbody.transform.forward = value;
        }
        public Vector3 Velocity
        {
            set => _rigidbody.Velocity = value;
        }
        public float Radius
        {
            set=>_rigidbody.transform.localScale = Vector3.one *value;
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

        public void OnInitBulletState(DirectionFrameSyncBullet bullet, DirectionBulletState bulletState,
            DirectionBulletConfigAsset bulletConfig)
        {
            bulletState.Radius = bulletConfig.Radius;
            bulletState.Velocity = bulletState.Direction * bulletConfig.Speed;
            bulletState.SaveState();
        }

        public void OnRendererFrame(float timeSinceLastLogicFrame, DirectionBulletConfigAsset bulletConfig,
            ref DirectionBulletState bulletState)
        {
            bulletState.ApplyAndSimulate(timeSinceLastLogicFrame);
        }

        public void OnLogicFrame(float deltaTime, DirectionBulletConfigAsset bulletConfig, 
            ref DirectionBulletState bulletState)
        {
            bulletState.ApplyAndSimulate(deltaTime);
            bulletState.SaveState();
        }
    }
}
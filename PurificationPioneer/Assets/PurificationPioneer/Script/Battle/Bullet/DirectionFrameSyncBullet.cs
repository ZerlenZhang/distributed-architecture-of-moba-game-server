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
        protected override bool HitTest(DirectionBulletState currentBulletState, DirectionBulletConfigAsset bulletConfig)
        {
            var rig = GetComponent<PpRigidbody>();
            if (rig)
            {
                var hit = false;
                
                rig.RigidbodyHelper.CastAroundNoAlloc(
                    hitInfo =>
                    {
                        var canvas = hitInfo.collider.GetComponent<InkCanvas>();
                        if (canvas != null)
                        {
                            hit = true;
                            // Debug.Log($"[BulletPos-{currentBulletState.LogicPosition}][HitPoint-{hitInfo.point}]");
                            canvas.Paint(bulletConfig.BrushConfig.brush, hitInfo);
                            // Debug.Log($"子弹碰到并涂色！：{hitInfo.collider.name}");
                        }else
                            Debug.LogWarning($"子弹碰到但没有涂色：{hitInfo.collider.name}");
                    },attackLayer,currentBulletState.LogicPosition-rig.Position);
                
                // rig.RigidbodyHelper.RaycastNoAlloc(
                //     currentBulletState.Direction,
                //     bulletConfig.Radius*1.5f,
                //     hitInfo =>
                //     {
                //         var canvas = hitInfo.collider.GetComponent<InkCanvas>();
                //         if (canvas != null)
                //         {
                //             hit = true;
                //             canvas.Paint(bulletConfig.BrushConfig.brush, hitInfo);
                //             // Debug.Log($"子弹碰到并涂色！：{hitInfo.collider.name}");
                //         }else
                //             Debug.LogWarning($"子弹碰到但没有涂色：{hitInfo.collider.name}");
                //     },currentBulletState.LogicPosition,attackLayer);
                if (hit)
                {
                    DestroyBullet();
                    return true;
                }

                return false;
            }

            return false;

            //
            // var hitCount =
            //     Physics.RaycastNonAlloc(
            //         new Ray(currentBulletState.LogicPosition, currentBulletState.Direction),
            //         HitInfos,
            //         bulletConfig.Radius*1.5f,
            //         attackLayer);
            // if (hitCount == 0)
            // {
            //     return false;
            // }
            //
            // for (var i = 0; i < hitCount; i++)
            // {
            //     var hitInfo = HitInfos[i];
            //     var canvas = hitInfo.collider.GetComponent<InkCanvas>();
            //     if (canvas != null)
            //     {
            //         canvas.Paint(bulletConfig.BrushConfig.brush, hitInfo);
            //         // Debug.Log($"子弹碰到并涂色！：{hitInfo.collider.name}");
            //     }else
            //         Debug.LogWarning($"子弹碰到但没有涂色：{hitInfo.collider.name}");
            // }
            // DestroyBullet();
            // return true;
        }
    }

    public class DirectionBulletState
    {
        private readonly PpRigidbodyState _rigidbodyState;
        public DirectionBulletState(PpRigidbody rigidbody,Vector3 dir,Vector3 initPosition)
        {
            _rigidbody = rigidbody;
            Assert.IsTrue(_rigidbody);
            _rigidbodyState = PpPhysics.GetRigidbodyState(_rigidbody,true);
            RendererPosition = initPosition;
            Direction = dir;
            Assert.IsNotNull(_rigidbodyState);
        }     
        private readonly PpRigidbody _rigidbody;

        public Vector3 LogicPosition => _rigidbodyState.Position;
        private Vector3 RendererPosition
        {
            set => _rigidbody.InternalPosition = value;
        }
        
        public Vector3 Velocity
        {
            set => _rigidbody.InternalVelocity = value;
        }
        public Vector3 Direction
        {
            get => _rigidbody.transform.forward;
            private set => _rigidbody.transform.forward = value;
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
            
            // bulletState.SaveState();
        }

        public void OnRendererFrame(float timeSinceLastLogicFrame, DirectionBulletConfigAsset bulletConfig,
            ref DirectionBulletState bulletState)
        {
            // bulletState.ApplyAndSimulate(timeSinceLastLogicFrame);
        }

        public void OnLogicFrame(float deltaTime, DirectionBulletConfigAsset bulletConfig, 
            ref DirectionBulletState bulletState)
        {
            // bulletState.ApplyAndSimulate(deltaTime);
            // bulletState.SaveState();
        }
    }
}
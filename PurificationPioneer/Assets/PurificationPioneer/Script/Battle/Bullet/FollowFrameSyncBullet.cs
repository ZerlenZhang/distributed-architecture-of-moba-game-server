using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class FollowFrameSyncBullet:AbstractFrameSyncBullet<
        FollowFrameSyncBullet,
        FollowFrameSyncBulletStrategy,
        FollowFrameSyncBulletConfigAsset,
        FollowFrameSyncBulletState>
    {
        protected override bool HitTest(FollowFrameSyncBulletState currentBulletState, FollowFrameSyncBulletConfigAsset bulletConfig)
        {
            return false;
        }
    }


    public class FollowFrameSyncBulletState
    {
        public Vector3 LastVelocity
        {
            get => _rigidbodyState.Velocity;
        }

        public Vector3 Velocity
        {
            set => _rigidbody.Velocity = value;
        }

        public Vector3 InternalVelocity
        {
            set => _rigidbody.InternalVelocity = value;
        }

        public Vector3 Forward
        {
            set => _rigidbody.transform.forward = value;
            get => _rigidbody.transform.forward;
        }
        
        public Vector3 LogicPosition => _rigidbodyState.Position;

        public Transform Target => _target;
        
        

        public FollowFrameSyncBulletState(PpRigidbody rigidbody, Vector3 direction, Vector3 initPosition, Transform target)
        {
            _rigidbody = rigidbody;
            Assert.IsTrue(_rigidbody);
            Forward = direction;
            _target = target;
            _rigidbody.Position = initPosition;
            Assert.IsTrue(_target);
            _rigidbodyState = PpPhysics.GetRigidbodyState(_rigidbody, true);
        }
        private PpRigidbodyState _rigidbodyState;
        private readonly PpRigidbody _rigidbody;
        private Transform _target;
    }

    public class FollowFrameSyncBulletStrategy : 
        IBulletStrategy<FollowFrameSyncBullet, FollowFrameSyncBulletConfigAsset
        , FollowFrameSyncBulletState>
    {
        public void DisableBullet(FollowFrameSyncBullet bullet)
        {
#if DebugMode
            if (GameSettings.Instance.EnableBulletLog)
            {
                Debug.Log($"[FollowBullet] Destroy!");
            }
#endif
            Object.Destroy(bullet.gameObject);
        }

        public bool IsBulletActivate(FollowFrameSyncBullet bullet)
        {
            return bullet.gameObject.activeSelf;
        }

        public void OnInitBulletState(FollowFrameSyncBulletState bulletState,
            FollowFrameSyncBulletConfigAsset bulletConfig)
        {
            bulletState.InternalVelocity = bulletState.Forward * bulletConfig.InitSpeedScale;
        }

        public void OnRendererFrame(float timeSinceLastLogicFrame, FollowFrameSyncBulletConfigAsset bulletConfig,
            ref FollowFrameSyncBulletState bulletState)
        {
        }

        public void OnLogicFrame(float deltaTime, FollowFrameSyncBulletConfigAsset bulletConfig,
            ref FollowFrameSyncBulletState bulletState)
        {
            var target = bulletState.Target;

            var expectedVelocity = (target.position - bulletState.LogicPosition).normalized * bulletConfig.MaxSpeed;
                
            var velocityChange = expectedVelocity - bulletState.LastVelocity; 
            
            if (velocityChange.magnitude > bulletConfig.MaxVelocityChange)
            {
                velocityChange = velocityChange.normalized * bulletConfig.MaxVelocityChange;
            }
            
            var newVelocity=bulletState.LastVelocity + velocityChange;

            bulletState.Velocity = newVelocity;
            bulletState.Forward = newVelocity;
        }
    }
}
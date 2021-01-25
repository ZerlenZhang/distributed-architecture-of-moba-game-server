using System;
using System.Collections.Generic;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class PpRigidbodyHelper:IDisposable
    {
        private HashSet<PpColliderHelper> _selfColliders;
        private HashSet<PpColliderHelper> _selfTriggers;
        private HashSet<Collider> _selfTriggersAndColliders;
        private PpRigidbody _rigidbody;
        private RaycastHit[] _cache;

        private Vector3 _positionFix;
        private int _detectLayer;
        
        public int DetectLayer => _detectLayer;

        public void FixPosition(Vector3 fix)
        {
            _positionFix = fix;
            _rigidbody.Position += fix;
        }

        public Vector3 GetLastPositionFix() => _positionFix;

        public HashSet<PpColliderHelper> SelfColliders => _selfColliders;

        public HashSet<PpColliderHelper> SelfTriggers => _selfTriggers;

        public HashSet<Collider> SelfTriggersAndColliders => _selfTriggersAndColliders;

        public PpRigidbodyHelper(PpRigidbody rigidbody,int maxRaycastHitCount)
        {
            Assert.IsTrue(rigidbody);

            _rigidbody = rigidbody;
            _cache = new RaycastHit[maxRaycastHitCount];
            _selfColliders = new HashSet<PpColliderHelper>();
            _selfTriggers = new HashSet<PpColliderHelper>();
            _selfTriggersAndColliders = new HashSet<Collider>();
            
            foreach (var collider in rigidbody.GetComponents<Collider>())
            {
                _selfTriggersAndColliders.Add(collider);
                if (collider.isTrigger)
                    _selfTriggers.Add(collider.GetHelper(this));
                else
                    _selfColliders.Add(collider.GetHelper(this));
            }
            for (var i = 0; i < 32; i++)
            {
                var ignore = Physics.GetIgnoreLayerCollision(i, rigidbody.transform.gameObject.layer);
                if(!ignore)
                    _detectLayer |= 1 << i;
            }
#if DebugMode
            if (GameSettings.Instance.EnablePhysicsLog)
            {
                Debug.Log($"[PpRigidbodyHelper] {rigidbody.name} Collider-{_selfColliders.Count}, Trigger-{_selfTriggers.Count}");
            }
#endif
        }

        /// <summary>
        /// 获取向某一个方向上最大移动距离
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        public float TryMoveDistance(Vector3 movement)
        {
            var length = movement.magnitude;
            // foreach (var ppColliderHelper in _selfColliders)
            // {
            //     var count = ppColliderHelper.Collider.CastNoAlloc(movement, length, DetectLayer, _cache);
            //     for (var i = 0; i < count; i++)
            //     {
            //         var hitInfo = _cache[i];
            //         if(hitInfo.collider.isTrigger)
            //             continue;
            //         length = Mathf.Min(hitInfo.distance, length);
            //     }
            // }
            return length;
        }
        
        public void Dispose()
        {
            foreach (var colliderHelper in _selfColliders)
            {
                PpPhysics.ReleaseCollider(colliderHelper.Collider);
            }
            foreach (var colliderHelper in _selfTriggers)
            {
                PpPhysics.ReleaseCollider(colliderHelper.Collider);
            }

            _rigidbody = null;
            _selfColliders = null;
            _selfTriggers = null;
            _selfTriggersAndColliders = null;
        }

        public void DrawSelfGizmos()
        {
            foreach (var collider in _selfTriggersAndColliders)
            {
                collider.DrawSelfGizmos();
            }
        }
        
        /// <summary>
        /// 对自己造成物理效果
        /// </summary>
        /// <param name="selfColliderHelper"></param>
        /// <param name="other"></param>
        /// <param name="collision"></param>
        public void CausePhysicsEffectToSelf(PpColliderHelper selfColliderHelper,Collider other, PpCollision collision)
        {
            var otherRig = other.GetComponent<PpRigidbody>();
            if (otherRig)
            {
                //刚体相撞，动量定理、物理材质……
                Debug.LogWarning($"尚未实现刚体碰撞逻辑 [{selfColliderHelper.Collider.name}-{other.name}]");
            }
            else
            {
                //碰到固定的碰撞体

                if (selfColliderHelper.LastColliderDic.TryGetValue(other, out var lastCollision))
                {
                    collision.CopyValues(lastCollision);
                }
                else
                {
                    var normal = collision.ExpectedMovement.normalized;
                    
                    var srcSpeedProjected = Vector3.Project(_rigidbody.Velocity, normal);
                    var velocityChange = -srcSpeedProjected * (1 + _rigidbody.Bounciness);

                    var srcAccelerationProjected = Vector3.Project(_rigidbody.Acceleration, normal);
                    var accelerationChange = -srcAccelerationProjected;

                    collision.Normal = normal;
                    collision.DeltaAcceleration = accelerationChange;
                    collision.DeltaVelocity = velocityChange;
                    _rigidbody.Velocity += velocityChange;
                    _rigidbody.Acceleration += accelerationChange;            
                }

            }
        }

        public void RemovePhysicsEffectFromSelf(PpCollision collision)
        {
            _rigidbody.Acceleration -= collision.DeltaAcceleration;
        }
    }
}
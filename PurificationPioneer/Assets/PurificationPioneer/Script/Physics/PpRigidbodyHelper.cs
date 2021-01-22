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
        private HashSet<ColliderHelper> _selfColliders;
        private HashSet<ColliderHelper> _selfTriggers;
        private HashSet<Collider> _selfTriggersAndColliders;

        public HashSet<ColliderHelper> SelfColliders => _selfColliders;

        public HashSet<ColliderHelper> SelfTriggers => _selfTriggers;

        public HashSet<Collider> SelfTriggersAndColliders => _selfTriggersAndColliders;
        
        private PpRigidbody _body;
        private Collider[] _cache;
        private int collisionLayer;

        public PpRigidbodyHelper(PpRigidbody rigidbody, int maxCollisionCount=8)
        {
            Assert.IsTrue(rigidbody);

            for (var i = 0; i < 32; i++)
            {
                var ignore = Physics.GetIgnoreLayerCollision(i, rigidbody.transform.gameObject.layer);
                if(!ignore)
                    collisionLayer |= 1 << i;
            }
            
            this._body = rigidbody;
            this._cache = new Collider[maxCollisionCount];
            _selfColliders = new HashSet<ColliderHelper>();
            _selfTriggers = new HashSet<ColliderHelper>();
            _selfTriggersAndColliders = new HashSet<Collider>();
            
            foreach (var collider in rigidbody.GetComponents<Collider>())
            {
                _selfTriggersAndColliders.Add(collider);
                if (collider.isTrigger)
                    _selfTriggers.Add(collider.GetHelper(rigidbody));
                else
                    _selfColliders.Add(collider.GetHelper(rigidbody));
            }
#if DebugMode
            if (GameSettings.Instance.EnablePhysicsLog)
            {
                Debug.Log($"[PpRigidbodyHelper] {rigidbody.name} Collider-{_selfColliders.Count}, Trigger-{_selfTriggers.Count}");
            }
#endif
        }

        /// <summary>
        /// 获取交叉体中的所有触发器和碰撞体，不会包含自身
        /// </summary>
        /// <param name="results"></param>
        public void GetOverlapColliderAndTrigger(HashSet<Collider> results)
        {
            foreach (var collider in _selfTriggersAndColliders)
            {
                var count = collider.GetOverlapNoAlloc(collisionLayer, _cache);
                for (var i = 0; i < count; i++)
                {
                    //此处需要去除自身
                    var other = _cache[i];
                    if (!_selfTriggersAndColliders.Contains(other))
                        results.Add(other);
                }
            }
        }

        /// <summary>
        /// 计算当前刚体在周围所有碰撞体交互中应该移动的最小距离
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        public Vector3 Calculate(Vector3? currentPosition=null)
        {
            var ans = Vector3.zero;

            foreach (var colliderHelper in _selfColliders)
            {
                ans += colliderHelper.Calculate(_cache, currentPosition, _selfTriggersAndColliders);
            }
            
            return ans;
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
            
            _selfColliders = null;
            _selfTriggers = null;
            _selfTriggersAndColliders = null;
            _cache = null;
            _body = null;
        }

        public void DrawSelfGizmos()
        {
            foreach (var collider in _selfTriggersAndColliders)
            {
                collider.DrawSelfGizmos();
            }
        }
    }
}
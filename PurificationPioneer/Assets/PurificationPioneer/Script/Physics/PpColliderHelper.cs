using System;
using System.Collections.Generic;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class PpColliderHelper:IDisposable
    {
        #region public properties

        public bool IsTrigger => _collider.isTrigger;
        public int SelfLayer => _collider.transform.gameObject.layer;
        public Collider Collider => _collider;        

        #endregion

        #region private fields

        private Collider _collider;
        private PpRigidbodyHelper _rigidbodyHelper;

        //当前物理帧检测到的触发器和碰撞体
        private Dictionary<Collider,PpRaycastHit> _currentTriggerDic;
        private Dictionary<Collider,PpRaycastHit> _currentCollisionDic;
        //上一物理帧检测到的触发器和碰撞体
        private Dictionary<Collider,PpRaycastHit> _lastTriggerDic;
        private Dictionary<Collider,PpRaycastHit> _lastCollisionDic;        

        #endregion

        /// <summary>
        /// 执行碰撞逻辑
        /// </summary>
        /// <param name="hitInfo"></param> 
        public void AddCurrentCollision(PpRaycastHit hitInfo)
        {
            var otherCollider = hitInfo.Collider;
            if (_currentCollisionDic.ContainsKey(otherCollider))
            {
                return;
            }
            
            _currentCollisionDic.Add(otherCollider, hitInfo);
        }

        /// <summary>
        /// 添加当前触发信息
        /// </summary>
        /// <param name="hitInfo"></param>
        /// <exception cref="Exception"></exception>
        public void AddCurrentTrigger(PpRaycastHit hitInfo)
        {
            var otherCollider = hitInfo.Collider;
            if (!otherCollider.isTrigger)
            {
                throw new Exception($"Collider[{otherCollider.name}] 不是触发器");
            }
            if (_currentTriggerDic.ContainsKey(otherCollider))
            {
                throw new Exception($"重复触发物体：{otherCollider.name}");
            }
            _currentTriggerDic.Add(otherCollider, hitInfo);
            
        }
        
        private void BroadCollisionEvent(Collider other, bool shouldCallOther, string message, PpRaycastHit args)
        {
            _collider.SendMessage(message, args, SendMessageOptions.DontRequireReceiver);
            if (shouldCallOther && other)
            {
                // Debug.Log($"BroadMessage: {message}");
                var temp = args.CreateWithNewCollider(_collider);
                other.SendMessage(message, temp,SendMessageOptions.DontRequireReceiver);
            }
        }
        private void BroadTriggerEvent(Collider other, bool shouldCallOther, string message)
        {
            _collider.SendMessage(message, other,SendMessageOptions.DontRequireReceiver);
            if(shouldCallOther && other)
                other.SendMessage(message, _collider,SendMessageOptions.DontRequireReceiver);
        }
        /// <summary>
        /// 将这一逻辑帧的触发器和碰撞器数据缓存
        /// </summary>
        public void UpdateTriggersAndCollidersSet(RaycastHit[] cache,HashSet<Collider> ignoreSet)
        {
            #region UpdateNormal
            
            // GetCastAround(cache,
            //     hitInfo =>
            //     {
            //         if (hitInfo.collider.isTrigger)
            //         {
            //             if(_currentTriggerDic.TryGetValue(hitInfo.collider,out var triggerHit))
            //             {
            //                 // Debug.LogWarning($"[UpdateNormal][Normal-{hitInfo.normal}][Collider-{hitInfo.collider.name}]");
            //                 triggerHit.UpdateValues(hitInfo);
            //             }
            //         }else if (_currentCollisionDic.TryGetValue(hitInfo.collider, out var collisionHit))
            //         {
            //             // Debug.LogWarning($"[UpdateNormal][Normal-{hitInfo.normal}][Collider-{hitInfo.collider.name}]");
            //             collisionHit.UpdateValues(hitInfo);
            //         }
            //         
            //     },ignoreSet);
            
            #endregion
            
            #region Exit

            foreach (var kv in _lastCollisionDic)
            {
                var otherCollider = kv.Key;
                if (_currentCollisionDic.ContainsKey(otherCollider)) 
                    continue;
                //Collision Exit
                // Debug.Log($"[{PpPhysics.physicsFrameId}]Exit:{otherCollider.name}->{_collider.name}");

                BroadCollisionEvent(otherCollider,
                    otherCollider.IsNeedCallEvent(),
                    PpPhysics.OnCollisionExitMsg, kv.Value);
                _rigidbodyHelper.RemovePhysicsEffectFromSelf(kv.Value);
            }

            foreach (var kv in _lastTriggerDic)
            {
                var otherCollider = kv.Key;
                if (_currentTriggerDic.ContainsKey(otherCollider))
                    continue;
            
                //Trigger Exit
                BroadTriggerEvent(otherCollider,
                    otherCollider.IsNeedCallEvent(),
                    PpPhysics.OnTriggerExitMsg);
                
            }
                        

            #endregion

            #region Stay and Enter

            foreach (var kv in _currentCollisionDic)
            {
                var otherCollider = kv.Key;
                var hitInfo = kv.Value;
                //碰撞物理效果 和 事件回调
                if (_lastCollisionDic.TryGetValue(otherCollider, out _))
                {
                    //Collision Stay
                    // Debug.Log($"[{PpPhysics.physicsFrameId}]Stay:{otherCollider.name}->{_collider.name}");
                    BroadCollisionEvent(otherCollider,
                        otherCollider.IsNeedCallEvent(),
                        PpPhysics.OnCollisionStayMsg, hitInfo);
                }
                else
                {
                    // Debug.Log($"[{PpPhysics.physicsFrameId}]Enter:{otherCollider.name}->{_collider.name}");
                    //Collision Enter
                    BroadCollisionEvent(otherCollider,
                        otherCollider.IsNeedCallEvent(),
                        PpPhysics.OnCollisionEnterMsg, hitInfo);
                    _rigidbodyHelper.AddPhysicsEffectToSelf(this, otherCollider, hitInfo);
                }
            }

            foreach (var kv in _currentTriggerDic)
            {
                var otherCollider = kv.Key;
                //物理事件触发
                if (_lastTriggerDic.TryGetValue(otherCollider, out _))
                {
                    //Collider Stay
                    BroadTriggerEvent(otherCollider,
                        otherCollider.IsNeedCallEvent(),
                        PpPhysics.OnTriggerStayMsg);
                }
                else
                {
                    //Collider Enter
                    BroadTriggerEvent(otherCollider,
                        otherCollider.IsNeedCallEvent(),
                        PpPhysics.OnTriggerEnterMsg);
                }
            }
            
            #endregion

            // 更新
            _lastCollisionDic.Clear();
            ObjectUtil.SwapReference(ref _lastCollisionDic, ref _currentCollisionDic);
            _lastTriggerDic.Clear();
            ObjectUtil.SwapReference(ref _lastTriggerDic, ref _currentTriggerDic);
        }
        internal PpColliderHelper(Collider collider, PpRigidbodyHelper rigidbodyHelper)
        {
            Assert.IsTrue(collider);
            Assert.IsNotNull(rigidbodyHelper);
            _rigidbodyHelper = rigidbodyHelper;
            _collider = collider;
            _lastTriggerDic = new Dictionary<Collider,PpRaycastHit>();
            _lastCollisionDic = new Dictionary<Collider,PpRaycastHit>();
            _currentTriggerDic = new Dictionary<Collider,PpRaycastHit>();
            _currentCollisionDic = new Dictionary<Collider,PpRaycastHit>();
        }

        /// <summary>
        /// 检测周围碰撞体
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="onHitOther"></param>
        /// <param name="ignoreSet"></param>
        /// <param name="centerOffset"></param>
        public void GetCastAround(RaycastHit[] cache, Action<RaycastHit> onHitOther, HashSet<Collider> ignoreSet=null, Vector3? centerOffset=null)
        {
            //直接检测
            _collider.CastActionNoAlloc(Vector3.down, GameSettings.Instance.MinDetectableDistance, _rigidbodyHelper.DetectLayer, cache, onHitOther, ignoreSet,centerOffset);

        }

        public void Dispose()
        {
            _currentCollisionDic = null;
            _currentTriggerDic = null;
            _lastTriggerDic = null;
            _lastCollisionDic = null;
            _collider = null;
            _rigidbodyHelper = null;
        }
    }

    public static class ColliderExtension
    {
        /// <summary>
        /// 如果当前collider物体带有PpRigidbody组件，会获得其helper并返回true
        /// </summary>
        /// <param name="self"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool TryGetPpColliderHelper(this Collider self, out PpColliderHelper helper)
        {
            helper = null;
            if (!self)
            {
                throw new Exception($"Collider is null");
            }
            
            var rig = self.GetComponent<PpRigidbody>();

            if (rig && rig.Enable)
            {
                helper = self.GetHelper(rig.RigidbodyHelper);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否应该代为调用此collider的物理事件
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsNeedCallEvent(this Collider self)
        {
            if (!self || !self.enabled)
                return false;
            if (!self.gameObject || !self.gameObject.activeSelf)
                return false;
            var rig = self.GetComponent<PpRigidbody>();
            return !rig;
        }
    }

}
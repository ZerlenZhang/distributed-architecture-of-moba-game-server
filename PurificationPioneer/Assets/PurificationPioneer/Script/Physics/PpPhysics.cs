using System;
using System.Collections.Generic;
using System.Linq;
using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public enum PpPhysicsSimulateOptions
    {
        BroadEvent,
        NoEvent,
    }
    public static class PpPhysics
    {
        private static Collider[] _cache;
        
        #region ColliderState

        private static Dictionary<Collider, PpColliderHelper> _colliderStateDic;

        /// <summary>
        /// 获取Collider状态
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static PpColliderHelper GetColliderHelper(Collider collider, PpRigidbodyHelper rigidbodyHelper)
        {
            Assert.IsTrue(collider);
            Assert.IsNotNull(rigidbodyHelper);
            if (!_colliderStateDic.ContainsKey(collider))
            {
                _colliderStateDic.Add(collider,new PpColliderHelper(collider,rigidbodyHelper));
            }
            
            return _colliderStateDic[collider];
            
        }
 
        public static void ReleaseCollider(Collider collider)
        {
            if (_colliderStateDic.TryGetValue(collider,out var helper))
            {
                helper.Dispose();
                _colliderStateDic.Remove(collider);
            }
        }

        public static void ClearCollider()
        {
            foreach (var colliderState in _colliderStateDic)
            {
                colliderState.Value.Dispose();
            }
            _colliderStateDic.Clear();
        }        

        #endregion

        #region Rigidbody

        private static Dictionary<PpRigidbody, PpRigidbodyHelper> _rigidbodyHelpers;
        public static PpRigidbodyHelper GetRigidbodyHelper(PpRigidbody rigidbody)
        {
            Assert.IsTrue(rigidbody);
            if (!_rigidbodyHelpers.ContainsKey(rigidbody))
            {
                _rigidbodyHelpers.Add(rigidbody,new PpRigidbodyHelper(rigidbody,GameSettings.Instance.MaxCollisionCount));
            }

            if (!_rigidbodyStates.ContainsKey(rigidbody))
            {
                _rigidbodyStates.Add(rigidbody,new PpRigidbodyState(rigidbody));
            }

            return _rigidbodyHelpers[rigidbody];
        }

        public static void ReleaseRigidbody(PpRigidbody rigidbody)
        {
            if (_rigidbodyHelpers.TryGetValue(rigidbody,out var helper))
            {
                helper.Dispose();
                _rigidbodyHelpers.Remove(rigidbody);
            }

            if (_rigidbodyStates.TryGetValue(rigidbody, out _))
            {
                _rigidbodyStates.Remove(rigidbody);
            }
        }

        public static void ClearRigidbody()
        {
            foreach (var ppRigidbodyHelper in _rigidbodyHelpers)
            {
                ppRigidbodyHelper.Value.Dispose();
            }
            _rigidbodyHelpers.Clear();
        }
        #endregion

        #region 状态的保存和恢复

        private static Dictionary<PpRigidbody, PpRigidbodyState> _rigidbodyStates;

        public static void SaveWorldState()
        {
            foreach (var kv in _rigidbodyHelpers)
            {
                var rigidbody = kv.Key;
                Assert.IsTrue(_rigidbodyStates.ContainsKey(rigidbody));
                _rigidbodyStates[rigidbody].SaveValue(rigidbody);
            }
        }

        public static PpRigidbodyState GetRigidbodyState(PpRigidbody rigidbody,bool autoAdd=false)
        {
            if (_rigidbodyStates.TryGetValue(rigidbody, out var state))
            {
                return state;
            }

            if (autoAdd)
            {
                GetRigidbodyHelper(rigidbody);
                return _rigidbodyStates[rigidbody];
            }

            throw new Exception($"[PpPhysics] GetRigidbodyState 没有这个状态: {rigidbody.name}");
        }

        public static void ApplyWorldState()
        {
            foreach (var kv in _rigidbodyHelpers)
            {
                var rigidbody = kv.Key;
                if (_rigidbodyStates.TryGetValue(rigidbody, out var state))
                {
                    rigidbody.ApplyRigidbodyState(state);
                }
                else
                {
                    throw new Exception($"[PpPhysics] ApplyWorldState: 没有这个刚体的状态：{rigidbody.name}");
                }
            }
        }

        #endregion

        public static void SetupPhysicsWorld()
        {
            _colliderStateDic = new Dictionary<Collider, PpColliderHelper>();
            _rigidbodyHelpers = new Dictionary<PpRigidbody, PpRigidbodyHelper>();
            _rigidbodyStates = new Dictionary<PpRigidbody, PpRigidbodyState>();
            _cache = new Collider[GameSettings.Instance.MaxCollisionCount];
        }
        
        public static void Simulate(float time,PpPhysicsSimulateOptions options)
        {
            for (var i = 0f; i < time; i += Time.fixedDeltaTime)
            {
                // 1. 所有刚体开始运动
                foreach (var kv in _rigidbodyHelpers)
                {
                    var rigidbody = kv.Key;
                    if(!rigidbody.gameObject.activeSelf || 
                       !rigidbody.Enable ||
                       rigidbody.IsKinematic)
                        continue;
                    rigidbody.Simulate(time);
                }
                
                // 2. 刚体位置修正
                // 遍历每一个刚体
                foreach (var ppRigidbodyHelper in _rigidbodyHelpers)
                {
                    var rigidbody = ppRigidbodyHelper.Key;
                    var helper = ppRigidbodyHelper.Value;
                    var bodyMovement = Vector3.zero;
                    //遍历刚体每一个Collider上交叉的每一个碰撞体
                    foreach (var colliderHelper in helper.SelfColliders)
                    { 
                        //更新当前物理帧交叉体
                        colliderHelper.UpdateCurrentTriggersAndCollidersSet(
                            _cache,
                            helper.SelfTriggersAndColliders);

                        //计算最小修正
                        bodyMovement += colliderHelper.Calculate(
                            colliderHelper.CurrentColliderDic.Keys,
                            (other, dir, distance) =>
                            {
                                var ans = dir.normalized * distance;
                                var otherRig = other.GetComponent<PpRigidbody>();
                                if (otherRig && !otherRig.IsKinematic)
                                {
                                    ans *= rigidbody.Mass / (rigidbody.Mass + otherRig.Mass);
                                }

                                //保存修正值
                                colliderHelper.CurrentColliderDic[other].ExpectedMovement = ans;
                                return ans;
                            });
                    }
                    
                    helper.FixPosition(bodyMovement);
                    // Debug.Log($"修正 {rigidbody.name} {bodyMovement}");
                }
                
                // 3. 物理效果触发
                foreach (var ppRigidbodyHelper in _rigidbodyHelpers)
                {
                    var rigidbody = ppRigidbodyHelper.Key;
                    var helper = ppRigidbodyHelper.Value;
                    //遍历刚体每一个Collider上交叉的每一个碰撞体
                    foreach (var colliderHelper in helper.SelfColliders)
                    {

                        foreach (var kv in colliderHelper.LastColliderDic)
                        {
                            if (!colliderHelper.CurrentColliderDic.TryGetValue(kv.Key, out _))
                            {
                                helper.RemovePhysicsEffectFromSelf(kv.Value);
                            }
                        }
                        foreach (var kv in colliderHelper.CurrentColliderDic)
                        {
                            helper.CausePhysicsEffectToSelf(colliderHelper, kv.Key, kv.Value);
                        }
                    }
                }

                // 4. 碰撞事件回调
                if (options == PpPhysicsSimulateOptions.BroadEvent)
                {
                    foreach (var ppRigidbodyHelper in _rigidbodyHelpers)
                    {
                        var rigidbody = ppRigidbodyHelper.Key;
                        var helper = ppRigidbodyHelper.Value;
                        
                        //遍历刚体每一个Collider上交叉的每一个碰撞体
                        foreach (var colliderHelper in helper.SelfColliders)
                        {
                            foreach (var kv in colliderHelper.CurrentColliderDic)
                            {
                                var otherCollider = kv.Value.Collider;
                                var shouldCallOther = IsShouldCallOther(otherCollider);
                                if (colliderHelper.LastColliderDic.ContainsKey(otherCollider))
                                {
                                    BroadCollisionEvent(colliderHelper.Collider, otherCollider, shouldCallOther,"PpOnCollisionStay",kv.Value);
                                }
                                else
                                {
                                    BroadCollisionEvent(colliderHelper.Collider, otherCollider, shouldCallOther,"PpOnCollisionEnter",kv.Value);
                                }
                            } 

                            foreach (var kv in colliderHelper.LastColliderDic)
                            {
                                var otherCollider = kv.Value.Collider;
                                var shouldCallOther = IsShouldCallOther(otherCollider);
                                if (!colliderHelper.CurrentColliderDic.ContainsKey(otherCollider))
                                {                        
                                    BroadCollisionEvent(colliderHelper.Collider, otherCollider, shouldCallOther,"PpOnCollisionExit",kv.Value);
                                }
                            }
                            
                            colliderHelper.UpdateLastTriggersAndCollidersSet();
                        }

                        // foreach (var triggerHelper in helper.SelfTriggers)
                        // {
                        //     foreach (var other in triggerHelper.CurrentColliderSet.Union(triggerHelper.CurrentTriggerSet))
                        //     {
                        //         var shouldCallOther = !other.GetComponent<PpRigidbody>();
                        //         if (triggerHelper.LastColliderSet.Contains(other))
                        //         {                           
                        //             SendMessage(triggerHelper.Collider, other, shouldCallOther,"PpOnTriggerStay");
                        //         }
                        //         else
                        //         {
                        //             SendMessage(triggerHelper.Collider, other, shouldCallOther,"PpOnTriggerEnter");
                        //         }
                        //     }
                        //
                        //     foreach (var other in triggerHelper.LastColliderSet.Union(triggerHelper.LastTriggerSet))
                        //     {
                        //         var shouldCallOther = !other.GetComponent<PpRigidbody>();
                        //         if (!triggerHelper.CurrentTriggerSet.Contains(other))
                        //         {
                        //             SendMessage(triggerHelper.Collider, other, shouldCallOther,"PpOnTriggerExit");
                        //         }
                        //     }
                        //     
                        //     triggerHelper.UpdateLastTriggersAndCollidersSet();
                        // }
                    }                      
                }
              
            }
        }

        public static void OnGui(GUIStyle defaultStyle)
        {
            GUILayout.Label($"Physics RigidbodyHelpers-{_rigidbodyHelpers.Count}, RigidbodyState-{_rigidbodyStates.Count}, ColliderStates-{_colliderStateDic.Count}",defaultStyle);
        }

        private static bool IsShouldCallOther(Collider collider)
        {
            if (!collider)
                return false;
            var rig = collider.GetComponent<PpRigidbody>();
            if (!rig)
                return true;
            // if (rig.IsKinematic)
            //     return true;
            return false;
        }

        private static void BroadCollisionEvent(Collider self, Collider other, bool shouldCallOther, string message,
            PpCollision args)
        {
            
            self.SendMessage(message, args,SendMessageOptions.DontRequireReceiver);
            if (shouldCallOther)
            {
                // Debug.Log($"BroadMessage: {message}");
                var temp = new PpCollision(
                    self, 
                    args.Normal, 
                    Vector3.zero,
                    Vector3.zero, 
                    Vector3.zero);
                other.SendMessage(message, temp,SendMessageOptions.DontRequireReceiver);
            }
        }
        private static void BroadTriggerEvent(Collider self,Collider other, bool shouldCallOther, string message)
        {
            self.SendMessage(message, other,SendMessageOptions.DontRequireReceiver);
            if(shouldCallOther && other)
                other.SendMessage(message, self,SendMessageOptions.DontRequireReceiver);
        }
    }

    public static class Extension
    {
        public static PpRigidbodyHelper GetHelper(this PpRigidbody self)
        {
            return PpPhysics.GetRigidbodyHelper(self);
        }

        public static PpColliderHelper GetHelper(this Collider self, PpRigidbodyHelper rigidbodyHelper) => PpPhysics.GetColliderHelper(self,rigidbodyHelper);
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public static class PpPhysics
    {
        private static Collider[] _cache;
        
        #region ColliderState

        private static Dictionary<Collider, ColliderHelper> _colliderStateDic;

        /// <summary>
        /// 获取Collider状态
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static ColliderHelper GetColliderHelper(Collider collider, PpRigidbody rigidbody)
        {
            Assert.IsTrue(collider && rigidbody);
            if (!_colliderStateDic.ContainsKey(collider))
            {
                _colliderStateDic.Add(collider,new ColliderHelper(collider,rigidbody));
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

            return _rigidbodyHelpers[rigidbody];
        }

        public static void ReleaseRigidbody(PpRigidbody rigidbody)
        {
            if (_rigidbodyHelpers.TryGetValue(rigidbody,out var helper))
            {
                helper.Dispose();
                _rigidbodyHelpers.Remove(rigidbody);
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

        public static void SetupPhysicsWorld()
        {
            _colliderStateDic = new Dictionary<Collider, ColliderHelper>();
            _rigidbodyHelpers = new Dictionary<PpRigidbody, PpRigidbodyHelper>();
            _cache = new Collider[GameSettings.Instance.MaxCollisionCount];
        }
        
        public static void Simulate(float time)
        {
            // 1. 所有刚体开始运动
            foreach (var ppRigidbody in _rigidbodyHelpers)
            {
                var rigidbody = ppRigidbody.Key;
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
                    //清空寄存器
                    colliderHelper.ClearMovementValues();
                    
                    //更新当前物理帧交叉体
                    colliderHelper.UpdateCurrentTriggersAndCollidersSet(
                        _cache,
                        helper.SelfTriggersAndColliders);

                    //计算最小修正
                    bodyMovement += colliderHelper.Calculate(
                        colliderHelper.CurrentColliderSet,
                        (other, dir, distance) =>
                        {
                            var ans = dir.normalized * distance;
                            var otherRig = other.GetComponent<PpRigidbody>();
                            if (otherRig && !otherRig.IsKinematic)
                            {
                                ans *= rigidbody.Mass / (rigidbody.Mass + otherRig.Mass);
                            }

                            //保存修正值
                            colliderHelper.SetValue(other, ans);
                            return ans;
                        });
                }

                rigidbody.Position += bodyMovement;
            }
            
            // 3. 物理效果触发
            foreach (var ppRigidbodyHelper in _rigidbodyHelpers)
            {
                var rigidbody = ppRigidbodyHelper.Key;
                var helper = ppRigidbodyHelper.Value;
                
                //遍历刚体每一个Collider上交叉的每一个碰撞体
                foreach (var colliderHelper in helper.SelfColliders)
                {
                    foreach (var collider_movement in colliderHelper.MovementDic)
                    {
                        var other = collider_movement.Key;
                        var otherRig = other.GetComponent<PpRigidbody>();
                        if (otherRig)
                        {
                            //刚体相撞，动量定理、物理材质……
                            throw new NotImplementedException();
                        }
                        else
                        {
                            //碰到固定的碰撞体，将当前物体速度，
                            var normal = collider_movement.Value;
                            var srcSpeedProjected = Vector3.Project(rigidbody.Velocity, normal);
                            rigidbody.Velocity += -srcSpeedProjected;
                        }
                    }
                }
            }
            
            // 4. 碰撞事件回调
            foreach (var ppRigidbodyHelper in _rigidbodyHelpers)
            {
                var rigidbody = ppRigidbodyHelper.Key;
                var helper = ppRigidbodyHelper.Value;
                
                //遍历刚体每一个Collider上交叉的每一个碰撞体
                foreach (var colliderHelper in helper.SelfColliders)
                {
                    foreach (var other in colliderHelper.CurrentColliderSet)
                    {
                        var shouldCallOther = !other.GetComponent<PpRigidbody>();
                        if (colliderHelper.LastColliderSet.Contains(other))
                        {
                            SendMessage(colliderHelper.Collider, other, shouldCallOther,"PpOnCollisionStay");
                        }
                        else
                        {
                            SendMessage(colliderHelper.Collider, other, shouldCallOther,"PpOnCollisionEnter");
                        }
                    }

                    foreach (var other in colliderHelper.LastColliderSet)
                    {
                        var shouldCallOther = !other.GetComponent<PpRigidbody>();
                        if (!colliderHelper.CurrentColliderSet.Contains(other))
                        {                        
                            SendMessage(colliderHelper.Collider, other, shouldCallOther,"PpOnCollisionExit");
                        }
                    }
                    
                    colliderHelper.UpdateLastTriggersAndCollidersSet();
                }

                foreach (var triggerHelper in helper.SelfTriggers)
                {
                    foreach (var other in triggerHelper.CurrentColliderSet.Union(triggerHelper.CurrentTriggerSet))
                    {
                        var shouldCallOther = !other.GetComponent<PpRigidbody>();
                        if (triggerHelper.LastColliderSet.Contains(other))
                        {                           
                            SendMessage(triggerHelper.Collider, other, shouldCallOther,"PpOnTriggerStay");
                        }
                        else
                        {
                            SendMessage(triggerHelper.Collider, other, shouldCallOther,"PpOnTriggerEnter");
                        }
                    }

                    foreach (var other in triggerHelper.LastColliderSet.Union(triggerHelper.LastTriggerSet))
                    {
                        var shouldCallOther = !other.GetComponent<PpRigidbody>();
                        if (!triggerHelper.CurrentTriggerSet.Contains(other))
                        {
                            SendMessage(triggerHelper.Collider, other, shouldCallOther,"PpOnTriggerExit");
                        }
                    }
                    
                    triggerHelper.UpdateLastTriggersAndCollidersSet();
                }
            }
        }

        public static void OnGui(GUIStyle defaultStyle)
        {
            GUILayout.Label($"Physics Rigidbody-{_rigidbodyHelpers.Count}, Collider-{_colliderStateDic.Count}",defaultStyle);
        }


        private static void SendMessage(Collider self,Collider other, bool shouldCallOther, string message)
        {
            self.SendMessage(message, other,SendMessageOptions.DontRequireReceiver);
            if(shouldCallOther)
                other.SendMessage(message, self,SendMessageOptions.DontRequireReceiver);
        }
    }

    public static class Extension
    {
        public static PpRigidbodyHelper GetHelper(this PpRigidbody self) => PpPhysics.GetRigidbodyHelper(self);
        public static ColliderHelper GetHelper(this Collider self, PpRigidbody rigidbody) => PpPhysics.GetColliderHelper(self,rigidbody);
        
    }
}
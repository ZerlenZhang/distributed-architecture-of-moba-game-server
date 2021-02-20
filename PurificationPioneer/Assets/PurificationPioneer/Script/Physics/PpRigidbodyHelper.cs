﻿using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class PpRigidbodyHelper:IDisposable
    {
        #region private fields

        private HashSet<PpColliderHelper> _selfColliders;
        private HashSet<PpColliderHelper> _selfTriggers;
        private HashSet<Collider> _selfTriggersAndColliders;
        private PpRigidbody _rigidbody;
        private RaycastHit[] _cache;
        private RaycastHit[] _cache2;
        private int _detectLayer;        

        #endregion

        #region public properties

        public int DetectLayer => _detectLayer;
        
        #endregion


        public PpRigidbodyHelper(PpRigidbody rigidbody,int maxRaycastHitCount)
        {
            Assert.IsTrue(rigidbody);

            _rigidbody = rigidbody;
            _cache = new RaycastHit[maxRaycastHitCount];
            _cache2 = new RaycastHit[maxRaycastHitCount];
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

            _detectLayer = _rigidbody.gameObject.GetUnityDetectLayer();
#if DebugMode
            if (GameSettings.Instance.EnablePhysicsLog)
            {
                Debug.Log($"[PpRigidbodyHelper Constructor] {rigidbody.name} Collider-{_selfColliders.Count}, Trigger-{_selfTriggers.Count}");
            }
#endif
        }

        /// <summary>
        /// 是否在地上
        /// </summary>
        /// <param name="dectectDistance"></param>
        /// <returns></returns>
        public bool IfOnGround(float dectectDistance)
        {
            foreach (var ppColliderHelper in _selfColliders)
            {
                var count = ppColliderHelper.Collider.CastNoAlloc(Vector3.down, dectectDistance, DetectLayer, _cache);
                if (count > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取向某一个方向上最大移动距离
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        public float TryMoveDistance(Vector3 movement, PpPhysicsSimulateOptions options)
        {
            var length = movement.magnitude;
            var isLogicFrame = options == PpPhysicsSimulateOptions.BroadEvent;
            var isDirValid = Mathf.Abs(length) > Mathf.Epsilon;
            
            var dir =  isDirValid? movement.normalized : Vector3.down;
            
#if DebugMode
            var hitObjInfo = new StringBuilder();
#endif
            PpColliderHelper closestSelfColliderHelper = null;
            RaycastHit? closestOtherHit = null;
            //记录擦过的平面
            var flatRaycastHits = new Dictionary<RaycastHit?, (PpColliderHelper,Vector3)>();
            
            //先计算真实移动距离
            foreach (var ppColliderHelper in _selfColliders)
            {
                bool hit = false;
                
                //debug
                // var debugMsg = new StringBuilder();  
                // debugMsg.Append($"[Debug][{PpPhysics.physicsFrameId}]\n");
                // for (var i = 0; i < 12; i++)
                // {
                //     var debugDir = new Vector3(
                //         Mathf.Cos(i * 30), 0, Mathf.Sin(i * 30));
                //     var debugHit = false;
                //     ppColliderHelper.Collider.CastActionNoAlloc(debugDir, 1, DetectLayer, _cache,
                //         hitInfo =>
                //         {
                //             if(hitInfo.collider.isTrigger)
                //                 return;
                //             debugHit = true;
                //             debugMsg.Append(
                //                 $"[Index-{i}][Dir-{debugDir}][Name-{hitInfo.collider.name}][法线-{hitInfo.normal}][角度-{Vector3.Angle(hitInfo.normal, dir)}]\n");
                //
                //         },SelfTriggersAndColliders);
                //     if (!debugHit)
                //     {
                //         debugMsg.Append($"[Index-{i}][Dir-{debugDir}] 无法检测\n");
                //     }
                // }
                // Debug.Log(debugMsg);
                
                
                ppColliderHelper.Collider.CastActionNoAlloc(dir,length,DetectLayer,_cache,
                    hitInfo =>
                    {
                        if (hitInfo.collider.isTrigger)
                            return;
                        hit = true;
#if DebugMode
                        hitObjInfo.Append($"{hitInfo.collider.name},");
#endif
                        if (hitInfo.distance < length || !isDirValid)
                        {// 发现障碍物
                            
                            if (isLogicFrame) 
                            {
                                //[Stay]如果正在擦过平面，就直接返回
                                if (_rigidbody.TryGetForce(hitInfo.collider, out var force))
                                {
                                    var angle =Vector3.Angle(force, dir);
                                    if ( Mathf.Abs(90-angle)< Mathf.Epsilon || !isDirValid)
                                    {
                                        flatRaycastHits.Add(hitInfo,(ppColliderHelper,force.normalized));
                                        return;
                                    }
                                }
                                
                                //[Enter]如果将要进入一个平面，不能用此平面去检测closestHit
                                var breakAll = false;
                                ppColliderHelper.GetCastAround(_cache2,
                                    tempOtherHit =>
                                    {
                                        if (tempOtherHit.collider == hitInfo.collider)
                                        {
                                            var angle = Vector3.Angle(tempOtherHit.normal, dir);
                                            if (Mathf.Abs(90 - angle) < Mathf.Epsilon)
                                            {
                                                Debug.LogWarning(
                                                    $"[{PpPhysics.physicsFrameId}][取消拦截-{tempOtherHit.collider.name}]");
                                                flatRaycastHits.Add(tempOtherHit,
                                                    (ppColliderHelper, tempOtherHit.normal));
                                                breakAll = true;
                                            }
                                        }
                                    }, _selfTriggersAndColliders, hitInfo.distance * dir);

                                if (breakAll)
                                    return;
                            }

                            if (isDirValid)
                            {
                                length = hitInfo.distance;
                                closestOtherHit = hitInfo;
                                closestSelfColliderHelper = ppColliderHelper;                                
                            }
                        }
                    },_selfTriggersAndColliders);

                // if (isLogicFrame && isDirValid && !hit)
                // {
                //     Debug.Log($"[{PpPhysics.physicsFrameId}] [Collider-{ppColliderHelper.Collider.name}][Dir-{dir}][没碰到东西]");
                // }
            }
            
            if (isLogicFrame)
            {
#if DebugMode
                if (GameSettings.Instance.EnablePhysicsLog)
                {
                    var msg = new StringBuilder();
                    msg.Append($"[{PpPhysics.physicsFrameId}] [Pos-{_rigidbody.Position}]");

                    if (closestOtherHit.HasValue)
                    {
                        msg.Append($"[拦截物体-{closestOtherHit.Value.collider.name}, 拦截法线-{closestOtherHit.Value.normal}，[角度-{Vector3.Angle(closestOtherHit.Value.normal,dir)}]");
                    }

                    msg.Append($"[Dir-{dir}][Result-{length}][Src-{movement.magnitude}], [{hitObjInfo}]");
                    
                    if (Mathf.Abs(length) <= Mathf.Epsilon)
                    {
                        msg.Append($"[ZeroLength] "); 
                        //msg.Append($"[Self-{_rigidbody.name}][DetectLayer-{Convert.ToString(DetectLayer, 2)}]");
                    }
                    Debug.Log(msg);                    
                }
#endif
                //擦过的平面
                foreach (var kv in flatRaycastHits)
                {
                    var otherHit = kv.Key;
                    var selfColliderHelper = kv.Value.Item1;
                    var normal = kv.Value.Item2;
                    // Debug.LogWarning($"[{PpPhysics.physicsFrameId}] 正在擦过平面 [Other-{otherHit.Value.collider.name}][Length-{length}][Self-{_rigidbody.name}]");
                    selfColliderHelper.AddCurrentCollision(new PpRaycastHit(otherHit));
                    if (otherHit.Value.collider.TryGetPpColliderHelper(out var otherHelper))
                    {
                        otherHelper.AddCurrentCollision(new PpRaycastHit(closestOtherHit,selfColliderHelper.Collider,normal));
                    }
                }
                // 记录碰撞信息
                if (closestOtherHit.HasValue)
                {
                    closestSelfColliderHelper.AddCurrentCollision(new PpRaycastHit(closestOtherHit));
                    if (closestOtherHit.Value.collider.TryGetPpColliderHelper(out var otherHelper))
                    {
                        otherHelper.AddCurrentCollision(new PpRaycastHit(closestOtherHit,closestSelfColliderHelper.Collider));
                    }
                }

                
                // 记录触发器信息
                foreach (var collider in _selfTriggersAndColliders)
                {
                    var rigidbodyHelper = collider.GetHelper(this);
                    collider.CastActionNoAlloc(dir,length,DetectLayer,_cache,
                        hitInfo =>
                        {
                            if(!rigidbodyHelper.IsTrigger && !hitInfo.collider.isTrigger)
                                return;

                            rigidbodyHelper.AddCurrentTrigger(new PpRaycastHit(hitInfo));
                            if (hitInfo.collider.TryGetPpColliderHelper(out var otherHelper))
                            {
                                otherHelper.AddCurrentTrigger(new PpRaycastHit(hitInfo,collider));
                            }
                            
                        },_selfTriggersAndColliders);
                }
            }
            
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
        /// <param name="raycastHit"></param>
        public void AddPhysicsEffectToSelf(PpColliderHelper selfColliderHelper,Collider other, PpRaycastHit raycastHit)
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
                var normal = raycastHit.Normal;
                
                var srcSpeedProjected = Vector3.Project(_rigidbody.Velocity, normal);
                var velocityChange = -srcSpeedProjected * (1 + _rigidbody.Bounciness);
                
                _rigidbody.Velocity += velocityChange;
                _rigidbody.AddInteract(raycastHit,selfColliderHelper.Collider);
            }
        }

        public void RemovePhysicsEffectFromSelf(PpRaycastHit raycastHit)
        {
            _rigidbody.RemoveInteract(raycastHit.Collider);
        }

        public void UpdateTriggersAndCollidersSet()
        {
            foreach (var colliderHelper in _selfColliders)
            {
                colliderHelper.UpdateTriggersAndCollidersSet(_cache,_selfTriggersAndColliders);
            }
            foreach (var colliderHelper in _selfTriggers)
            {
                colliderHelper.UpdateTriggersAndCollidersSet(_cache,_selfTriggersAndColliders);
            }
        }
    }
}
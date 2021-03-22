using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
        public float TryMoveDistance(Vector3 movement, PpPhysicsSimulateOptions options,float minDetectableDistance)
        {
            var length = movement.magnitude;
            var isLogicFrame = options == PpPhysicsSimulateOptions.BroadEvent;
            var isDirValid = Mathf.Abs(length) > Mathf.Epsilon;
            
            var dir =  movement.normalized;

            if (!isDirValid)
            {
                dir=Vector3.down;
                length = minDetectableDistance;
            }
            
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
#if DebugMode
                if (GameSettings.Instance.EnablePhysicsLog && isLogicFrame)
                {
                    //debug
                    var debugMsg = new StringBuilder();  
                    debugMsg.Append($"[Debug][{PpPhysics.physicsFrameId}]\n");
                    var dirList = new List<Vector3>();
                    dirList.Add(Vector3.down);
                    for (var i = 0; i < 12; i++)
                    {
                        dirList.Add(new Vector3(
                            Mathf.Cos(i * 30), 0, Mathf.Sin(i * 30)));
                    }
                    for (var i = 0; i < dirList.Count; i++)
                    {
                        var debugDir = dirList[i];
                        var debugHit = false;
                        var distance = i == 0 ? minDetectableDistance : 1;
                        ppColliderHelper.Collider.CastActionNoAlloc(debugDir, distance, DetectLayer, _cache,
                            hitInfo =>
                            {
                                if(hitInfo.collider.isTrigger)
                                    return;
                                debugHit = true;
                                debugMsg.Append(
                                    $"[Index-{i}][Dir-{debugDir}][Name-{hitInfo.collider.name}][法线-{hitInfo.normal}][角度-{Vector3.Angle(hitInfo.normal, dir)}]\n");
                    
                            },_selfTriggersAndColliders);
                        if (!debugHit)
                        {
                            debugMsg.Append($"[Index-{i}][Dir-{debugDir}] 无法检测\n");
                        }
                    }
                    Debug.Log(debugMsg);                    
                    
                }
#endif

                ppColliderHelper.Collider.CastActionNoAlloc(dir,length,DetectLayer,_cache,
                    hitInfo =>
                    {
                        if (hitInfo.collider.isTrigger)
                            return;
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
                                            if (Mathf.Abs(90 - angle) < minDetectableDistance || Mathf.Abs(180-angle)<minDetectableDistance)
                                            {
                                                if(!flatRaycastHits.Any(kv=>kv.Key.HasValue && kv.Key.Value.collider==tempOtherHit.collider))
                                                {
                                                    flatRaycastHits.Add(tempOtherHit, (ppColliderHelper, tempOtherHit.normal));
                                                    breakAll = true;                                                    
                                                }
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


                if (isLogicFrame)
                {
                    ppColliderHelper.GetCastAround(_cache,
                        tempOtherHit =>
                        {
                            if (!tempOtherHit.collider.isTrigger)
                            {
                                var angle = Vector3.Angle(tempOtherHit.normal, dir);
                                // Debug.Log($"[{PpPhysics.physicsFrameId}][Extra][Hit-{tempOtherHit.collider.name}][Normal-{tempOtherHit.normal}][Angle-{angle}]");
                                if (Mathf.Abs(90 - angle) < minDetectableDistance || Mathf.Abs(180-angle)<minDetectableDistance)
                                {
                                    if(!flatRaycastHits.Any(kv=>kv.Key.HasValue && kv.Key.Value.collider==tempOtherHit.collider))
                                    {
#if DebugMode
                                        hitObjInfo.Append($"《{tempOtherHit.collider.name}》,");
#endif
                                        // Debug.LogWarning($"[{PpPhysics.physicsFrameId}][取消拦截-{tempOtherHit.collider.name}]");
                                        flatRaycastHits.Add(tempOtherHit,
                                            (ppColliderHelper, tempOtherHit.normal));
                                    }
                                }
                            }
                        }, _selfTriggersAndColliders, length * dir);
                }
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
            }
            
            return length;
        }

        public void CastActionNoAlloc(Vector3 dir, float distance, Action<RaycastHit> onHitOther, int? detectLayerOverride=null, Vector3? centerOffset=null)
        {
            var layer = detectLayerOverride ?? DetectLayer;
            foreach (var selfCollider in _selfTriggersAndColliders)
            {
                selfCollider.CastActionNoAlloc(dir,distance,layer,_cache,onHitOther,_selfTriggersAndColliders,centerOffset);
            }
        }

        public void CastAroundNoAlloc(Action<RaycastHit> onHitOther, int? detectLayerOverride=null, Vector3? centerOffset=null)
        {
            CastActionNoAlloc(Vector3.down, GameSettings.Instance.MinDetectableDistance, onHitOther, detectLayerOverride, centerOffset);
        }

        public void RaycastNoAlloc(Vector3 dir, float distance, Action<RaycastHit> onHitOther, Vector3? startPosOverride=null, LayerMask? detectLayerOverride=null)
        {
            var layer = detectLayerOverride ?? DetectLayer;
            var start = startPosOverride ?? _rigidbody.Position;
            var hitCount =
                Physics.RaycastNonAlloc(
                    new Ray(start, dir),
                    _cache,
                    distance,
                    layer);
            if (hitCount == 0)
            {
                return;
            }

            for (var i = 0; i < hitCount; i++)
            {
                var hitInfo = _cache[i];
                if (_selfTriggersAndColliders.Contains(hitInfo.collider))
                    continue;
                onHitOther.Invoke(hitInfo);
            }
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
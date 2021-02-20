using System;
using System.Collections.Generic;
using System.Text;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class DebugScript : MonoBehaviour
    {
        public Transform start;
        public Transform end;
        public LayerMask layerMask;
        public float sphereRadius = 0.5f;

        public string key;

        public GameObject moveObj;

        public GameObject castAroundObj;
        public float castDistance = Mathf.Epsilon;


        #region private

        #region properties

        private HashSet<Collider> _castAroundObjColliders;
        public HashSet<Collider> CastAroundObjColliders
        {
            get
            {
                if (null == _castAroundObjColliders)
                {
                    _castAroundObjColliders = new HashSet<Collider>();
                    foreach (var collider in castAroundObj.GetComponents<Collider>())
                    {
                        _castAroundObjColliders.Add(collider);
                    }
                }

                return _castAroundObjColliders;
            }
        }

        private RaycastHit[] _raycastCache;

        public RaycastHit[] RaycastCache
        {
            get
            {
                if (null == _raycastCache)
                {
                    _raycastCache=new RaycastHit[GameSettings.Instance.MaxCollisionCount];
                }

                return _raycastCache;
            }
        }        

        #endregion


        #region methods

        
        private bool RaycastDir(Vector3 moveDir,out RaycastHit? hit,float distance=1f)
        {
            RaycastHit? closestHit=null;
            var length = distance;
            var detectLayer = castAroundObj.GetUnityDetectLayer();
            foreach (var collider in CastAroundObjColliders)
            {
                if(collider.isTrigger)  
                    continue;
                
                
                collider.CastActionNoAlloc(
                    moveDir, 
                    length,
                    detectLayer,
                    RaycastCache,
                    hitInfo=>
                    {
                        if (hitInfo.collider.isTrigger)
                            return;
                        if (hitInfo.distance < length)
                        {
                            length = hitInfo.distance;
                            closestHit = hitInfo;
                            Debug.Log($"Dir: {moveDir}, 法线：{hitInfo.normal}, 角度：{Vector3.Angle(hitInfo.normal,moveDir)}");
                        }
                    },
                    CastAroundObjColliders);
                
            }

            hit = closestHit;

            
            return hit.HasValue;
        }


        #endregion
        
        #endregion


        [ContextMenu("测试HashSet")]
        private void TestHashSet()
        {
            var msg = new StringBuilder();
            var set = new HashSet<int>();
            set.Add(0);
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(0);
            foreach (var i in set)
            {
                msg.Append($"{i} ");
            }

            Debug.Log(msg);
        }


        [ContextMenu("掉下去")]
        private void DropDown()
        {
            var distance = float.MaxValue;
            var detectLayer = castAroundObj.GetUnityDetectLayer();
            foreach (var collider in CastAroundObjColliders)
            {
                if(collider.isTrigger)  
                    continue;
                
                collider.CastActionNoAlloc(
                    Vector3.down, 
                    distance,
                    detectLayer,
                    RaycastCache,
                    hitInfo=>
                    {
                        if (!hitInfo.collider.isTrigger)
                        {
                            distance = Mathf.Min(distance, hitInfo.distance);
                        }
                    },
                    CastAroundObjColliders);
                
            }
            
            Assert.IsTrue(Mathf.Abs(distance-float.MaxValue)>=0f);
            castAroundObj.transform.position += distance * Vector3.down;
            Debug.Log($"下降距离：{distance}");
        }
        
        
        [ContextMenu("检查Overlap")]
        public void CastAround()
        {
            var triggerSet = new HashSet<Collider>();
            var colliderSet = new HashSet<Collider>();
            var detectLayer = castAroundObj.GetUnityDetectLayer();
            foreach (var collider in CastAroundObjColliders)
            {
                if(collider.isTrigger)  
                    continue;
                
                collider.CastActionNoAlloc(
                    Vector3.up, 
                    castDistance,
                    detectLayer,
                    RaycastCache,
                    hitInfo=>
                    {
                        if (hitInfo.collider.isTrigger)
                            triggerSet.Add(hitInfo.collider);
                        else
                            colliderSet.Add(hitInfo.collider);
                    },
                    CastAroundObjColliders);
                
            }

            var msg = new StringBuilder();
            msg.Append("《Collider》\n");
            foreach (var colliderHit in colliderSet)
            {
                msg.Append($"{colliderHit.name}\n");
            }

            msg.Append("《Trigger》\n");
            foreach (var triggerHit in triggerSet)
            {
                msg.Append($"{triggerHit.name}\n");
            }
            Debug.Log(msg);
        }

        [ContextMenu("Transform向右移动物体")]
        public void MoveTarget()
        {
            moveObj.transform.position += Vector3.right;
        }

        [ContextMenu("Raycast扫描四周")]
        public void RaycastAround()
        {
            //debug
            var debugMsg = new StringBuilder();  
            debugMsg.Append($"[Debug][{PpPhysics.physicsFrameId}]\n");
            for (var i = 0; i < 12; i++)
            {
                var debugDir = new Vector3(
                    Mathf.Cos(i * 30), 0, Mathf.Sin(i * 30));
                var debugHit = false;
                foreach (var collider in CastAroundObjColliders)
                {
                    collider.CastActionNoAlloc(debugDir, 
                        1, 
                        castAroundObj.GetUnityDetectLayer(), 
                        RaycastCache,
                        hitInfo =>
                        {
                            if(hitInfo.collider.isTrigger)
                                return;
                            debugHit = true;
                            debugMsg.Append(
                                $"[Index-{i}][Dir-{debugDir}][Name-{hitInfo.collider.name}][法线-{hitInfo.normal}][角度-{Vector3.Angle(hitInfo.normal, debugDir)}]\n");

                        },CastAroundObjColliders);                    
                }

                if (!debugHit)
                {
                    debugMsg.Append($"[Index-{i}][Dir-{debugDir}] 无法检测\n");
                }
            }
            Debug.Log(debugMsg);
        }


        [ContextMenu("Raycast向右扫描")]
        public void TryMoveTarget()
        {
            var length = 1f;
            
            var msg = new StringBuilder();
            if (RaycastDir(Vector3.right, out var closestHit,length))
            {
                length = closestHit.Value.distance;
                msg.Append($"[碰到：{closestHit.Value.collider.name}]");
            }
            
            msg.Append($"[移动距离：{length}]");
            
            Debug.Log(msg);
        }

        [ContextMenu("TestAddressable")]
        public void TestAddressable()
        {
            Addressables.LoadAssetAsync<GameObject>(key).Completed += (handler) =>
            {
                if (!handler.IsDone)
                    return;
                var instance = Instantiate(handler.Result, Vector3.zero, Quaternion.identity);
                instance.name = $"[Test Addressable] {key}";
            };
            Addressables.LoadAssetAsync<HeroConfigAsset>("ClassCharacter/Character3").Completed += (handler) =>
            {
                if (handler.IsDone)
                {
                    var config = handler.Result;
                    Debug.Log($"{config.heroName}");
                }
            };
        }
        
        [ContextMenu("Raycast")]
        private void Raycast()
        {
            var dir = end.position - start.position;
            var distance = dir.magnitude;
            
            foreach (var raycast in Physics.RaycastAll(start.position,dir,distance,layerMask))
            {
                Debug.Log($"[RayCast] {raycast.collider.name}");
            }

            foreach (var raycast in Physics.SphereCastAll(start.position,sphereRadius,dir,distance,layerMask))
            {
                Debug.Log($"[SphereCast] {raycast.collider.name}");
            }
        }
        
        
        [ContextMenu("Show CapsuleCollider Direction")]
        private void ShowCapsuleColliderDirection()
        {
            var col = GetComponent<CapsuleCollider>();
            if (col)
            {
                Debug.Log(col.direction);
            }
        }
        
        
        
        
    }
}
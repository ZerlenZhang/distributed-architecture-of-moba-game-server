using System;
using System.Collections.Generic;
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
        internal static int physicsFrameId = 0;
        internal const string OnCollisionEnterMsg = "PpOnCollisionEnter";
        internal const string OnCollisionStayMsg = "PpOnCollisionStay";
        internal const string OnCollisionExitMsg = "PpOnCollisionExit";
        internal const string OnTriggerEnterMsg = "PpOnTriggerEnter";
        internal const string OnTriggerStayMsg = "PpOnTriggerStay";
        internal const string OnTriggerExitMsg = "PpOnTriggerExit";


        #region ColliderHelper

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

        #region RigidbodyHelper

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
        }
        
        public static void Simulate(float time,PpPhysicsSimulateOptions options)
        {
            var physicsDeltaTime = GameSettings.Instance.PhysicsDeltaTime;
            for (var i = 0f; i < time; i += physicsDeltaTime)
            {
                foreach (var kv in _rigidbodyHelpers)
                {
                    var rigidbody = kv.Key;
                    if(!rigidbody.gameObject.activeSelf || 
                       !rigidbody.Enable ||
                       rigidbody.IsKinematic)
                        continue;
                    rigidbody.Simulate(physicsDeltaTime,options);
                }
                
                if (options == PpPhysicsSimulateOptions.BroadEvent)
                {
                    foreach (var kv in _rigidbodyHelpers)
                    {
                        var helper = kv.Value;
                        helper.UpdateTriggersAndCollidersSet();
                    }
                    physicsFrameId++;
                }
            }
        }

        public static void OnGui(GUIStyle defaultStyle)
        {
            GUILayout.Label($"Physics RigidbodyHelpers-{_rigidbodyHelpers.Count}, RigidbodyState-{_rigidbodyStates.Count}, ColliderStates-{_colliderStateDic.Count}",defaultStyle);
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
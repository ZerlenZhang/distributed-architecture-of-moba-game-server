using System;
using System.Collections.Generic;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class ColliderHelper:IDisposable
    {
        public bool IsTrigger => _collider.isTrigger;
        public int SelfLayer => _collider.transform.gameObject.layer;
        public int DetectLayer => _detectLayer;
        public Collider Collider => _collider;

        //当前物理帧检测到的触发器和碰撞体
        private HashSet<Collider> _currentTriggerSet;
        public HashSet<Collider> CurrentColliderSet => _currentColliderSet;
        private HashSet<Collider> _currentColliderSet;
        public HashSet<Collider> CurrentTriggerSet => _currentTriggerSet;
        //上一物理帧检测到的触发器和碰撞体
        private HashSet<Collider> lastTriggerSet;
        public HashSet<Collider> LastColliderSet => lastColliderSet;
        private HashSet<Collider> lastColliderSet;
        public HashSet<Collider> LastTriggerSet => lastTriggerSet;
        
        
        private Collider _collider;
        private PpRigidbody _rigidbody;
        public PpRigidbody Rigidbody => _rigidbody;
        private int _detectLayer;

        internal ColliderHelper(Collider collider, PpRigidbody rigidbody)
        {
            Assert.IsTrue(collider && rigidbody);
            _rigidbody = rigidbody;
            _collider = collider;
            lastTriggerSet = new HashSet<Collider>();
            lastColliderSet = new HashSet<Collider>();
            _currentTriggerSet = new HashSet<Collider>();
            _currentColliderSet = new HashSet<Collider>();
            movementDic = new Dictionary<Collider, Vector3>();
            for (var i = 0; i < 32; i++)
            {
                var ignore = Physics.GetIgnoreLayerCollision(i, rigidbody.transform.gameObject.layer);
                if(!ignore)
                    _detectLayer |= 1 << i;
            }
        }

        #region 寄存相对collider的移动

        private Dictionary<Collider, Vector3> movementDic;
        public Dictionary<Collider, Vector3> MovementDic => movementDic;

        public void SetValue(Collider collider, Vector3 value)
        {
            if (!movementDic.ContainsKey(collider))
                movementDic.Add(collider, value);
            else
                movementDic[collider] = value;
        }

        public Vector3 GetValue(Collider collider)
        {
            Assert.IsTrue(movementDic.ContainsKey(collider));
            return movementDic[collider];
        }

        public void ClearMovementValues()
        {
            movementDic.Clear();
        }

        #endregion

        /// <summary>
        /// 将这一逻辑帧的触发器和碰撞器数据缓存
        /// </summary>
        public void UpdateLastTriggersAndCollidersSet()
        {
            lastColliderSet.Clear();
            foreach (var other in _currentColliderSet)
            {
                lastColliderSet.Add(other);
            }
            lastTriggerSet.Clear();
            foreach (var other in _currentTriggerSet)
            {
                lastTriggerSet.Add(other);
            }
        }

        /// <summary>
        /// 更新当前物理帧检测到的碰撞体和触发器
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="ingoreSet"></param>
        public void UpdateCurrentTriggersAndCollidersSet(Collider[] cache, HashSet<Collider> ingoreSet)
        {
            _currentColliderSet.Clear();
            _currentTriggerSet.Clear();
                    
            GetOverlapNoAlloc(
                cache,
                _currentColliderSet,
                _currentTriggerSet,
                ingoreSet);
        }
        
        /// <summary>
        /// 获取当前碰撞体周围的所有交叉体，按照trigger和collider分开放，会包含自身，建议将自身加入到ignoreSet
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="resultColliders"></param>
        /// <param name="resultTriggers"></param>
        /// <param name="ignoreSet"></param>
        public void GetOverlapNoAlloc(Collider[] cache,HashSet<Collider> resultColliders,HashSet<Collider> resultTriggers, HashSet<Collider> ignoreSet=null)
        {
            var count = _collider.GetOverlapNoAlloc(DetectLayer, cache);
            for (var i = 0; i < count; i++)
            {
                var other = cache[i];
                if(ignoreSet!=null && ignoreSet.Contains(other))
                    continue;
                if (!other.isTrigger && !IsTrigger)
                {
                    resultColliders.Add(other);
                }
                else
                {
                    resultTriggers.Add(other);
                }
            }
        }

        public Vector3 Calculate(IEnumerable<Collider> colliders,Func<Collider,Vector3,float,Vector3> onOperateOtherCollider=null)
        {
            if(IsTrigger)
                return Vector3.zero;
            return _collider.ComputeCurrent(colliders,onOperateOtherCollider);
        }
        
        public Vector3 Calculate(Collider[] cache, Vector3? currentPosition=null, HashSet<Collider> ignoreSet=null)
        {
            if (IsTrigger)
                return Vector3.zero;
            return _collider.ComputeCurrent(DetectLayer, cache, ignoreSet, currentPosition);
        }


        public void Dispose()
        {
            movementDic = null;
            _currentColliderSet = null;
            _currentTriggerSet = null;
            lastTriggerSet = null;
            lastColliderSet = null;
            _collider = null;
            _rigidbody = null;
        }
    }
}
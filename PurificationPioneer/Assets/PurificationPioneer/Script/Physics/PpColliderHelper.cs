using System;
using System.Collections.Generic;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class PpColliderHelper:IDisposable
    {
        public bool IsTrigger => _collider.isTrigger;
        public int SelfLayer => _collider.transform.gameObject.layer;
        public Collider Collider => _collider;

        //当前物理帧检测到的触发器和碰撞体
        public HashSet<Collider> CurrentTriggerSet => _currentTriggerSet;
        private HashSet<Collider> _currentTriggerSet;
        public Dictionary<Collider,PpCollision> CurrentColliderDic => _currentColliderDic;
        private Dictionary<Collider,PpCollision> _currentColliderDic;
        //上一物理帧检测到的触发器和碰撞体
        public HashSet<Collider> LastTriggerSet => lastTriggerSet;
        private HashSet<Collider> lastTriggerSet;
        public Dictionary<Collider,PpCollision> LastColliderDic => _lastColliderDic;
        private Dictionary<Collider,PpCollision> _lastColliderDic;
        
        #region 寄存相对collider的移动

        #endregion
        
        private Collider _collider;
        private PpRigidbodyHelper _rigidbodyHelper;

        internal PpColliderHelper(Collider collider, PpRigidbodyHelper rigidbodyHelper)
        {
            Assert.IsTrue(collider);
            Assert.IsNotNull(rigidbodyHelper);
            _rigidbodyHelper = rigidbodyHelper;
            _collider = collider;
            lastTriggerSet = new HashSet<Collider>();
            _lastColliderDic = new Dictionary<Collider,PpCollision>();
            _currentTriggerSet = new HashSet<Collider>();
            _currentColliderDic = new Dictionary<Collider,PpCollision>();
        }


        /// <summary>
        /// 将这一逻辑帧的触发器和碰撞器数据缓存
        /// </summary>
        public void UpdateLastTriggersAndCollidersSet()
        {
            _lastColliderDic.Clear();
            foreach (var other in _currentColliderDic)
            {
                _lastColliderDic.Add(other.Key, other.Value);
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
            _currentColliderDic.Clear();
            _currentTriggerSet.Clear();
                    
            GetOverlapNoAlloc(
                cache,
                collider=>_currentColliderDic.Add(collider, new PpCollision(collider)),
                trigger=>_currentTriggerSet.Add(trigger),
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
            var count = _collider.GetOverlapNoAlloc(_rigidbodyHelper.DetectLayer, cache);
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
        /// <summary>
        /// 获取当前碰撞体周围的所有交叉体，会包含自身，建议将自身加入到ignoreSet
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="onGetCollider"></param>
        /// <param name="onGetTrigger"></param>
        /// <param name="ignoreSet"></param>
        public void GetOverlapNoAlloc(Collider[] cache,Action<Collider> onGetCollider,Action<Collider> onGetTrigger, HashSet<Collider> ignoreSet=null)
        {
            var count = _collider.GetOverlapNoAlloc(_rigidbodyHelper.DetectLayer, cache);
            for (var i = 0; i < count; i++)
            {
                var other = cache[i];
                if(ignoreSet!=null && ignoreSet.Contains(other))
                    continue;
                if (!other.isTrigger && !IsTrigger)
                {
                    onGetCollider.Invoke(other);
                }
                else
                {
                    onGetTrigger.Invoke(other);
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
            return _collider.ComputeCurrent(_rigidbodyHelper.DetectLayer, cache, ignoreSet, currentPosition);
        }


        public void Dispose()
        {
            _currentColliderDic = null;
            _currentTriggerSet = null;
            lastTriggerSet = null;
            _lastColliderDic = null;
            _collider = null;
            _rigidbodyHelper = null;
        }
    }
}
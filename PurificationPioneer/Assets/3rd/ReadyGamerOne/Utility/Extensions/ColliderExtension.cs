using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReadyGamerOne.Utility
{

    public static class ColliderExtension
    {
        #region Collider

        public static int CastNoAlloc(this Collider self, Vector3 dir, float distance, int layer, RaycastHit[] results)
        {
            if (self is BoxCollider box)
            {
                return Physics.BoxCastNonAlloc(box.GetCenterPosition(), box.size, dir.normalized, results,self.transform.rotation,distance,layer);
            }

            if (self is SphereCollider sphere)
            {
                return Physics.SphereCastNonAlloc(sphere.GetCenterPosition(), sphere.radius, dir.normalized, results, distance,
                    layer);
            }

            if (self is CapsuleCollider capsule)
            {
                capsule.GetTowPoint(out var first, out var second);
                return Physics.CapsuleCastNonAlloc(first, second, capsule.radius, dir.normalized, results, distance, layer);
            }
            
            throw new Exception($"Unexpected collider type: {self}");
        }

        public static void GetCenterPosition(this Collider self)
        {        
            if (self is BoxCollider box)
            {
                box.GetCenterPosition();
            }else if (self is SphereCollider sphere)
            {
                sphere.GetCenterPosition();
            }else if (self is CapsuleCollider capsule)
            {
                capsule.GetCenterPosition();
            }
            else
            {
                throw new Exception($"Unexpected collider type: {self}");
            }
        }
        public static void DrawSelfGizmos(this Collider self)
        {
            if (self is BoxCollider box)
            {
                box.DrawSelfGizmos();
            }else if (self is SphereCollider sphere)
            {
                sphere.DrawSelfGizmos();
            }else if (self is CapsuleCollider capsule)
            {
                capsule.DrawSelfGizmos();
            }
            else
            {
                throw new Exception($"Unexpected collider type: {self}");
            }
        }
        
        /// <summary>
        /// 计算和给定的collider至少移动什么样的距离可以脱离
        /// </summary>
        /// <param name="self"></param>
        /// <param name="colliders"></param>
        /// <param name="onOperateOtherCollider">远离其他collider的回调，（碰撞体，方向，距离）</param>
        /// <returns></returns>
        public static Vector3 ComputeCurrent(this Collider self, IEnumerable<Collider> colliders, Func<Collider,Vector3,float,Vector3> onOperateOtherCollider=null)
        {
            var ans = Vector3.zero;
            if (self.isTrigger)
                return ans;
            foreach (var collider in colliders)
            {
                if(collider.isTrigger)
                    continue;
                if (Physics.ComputePenetration(
                    self, self.transform.position, self.transform.rotation,
                    collider, collider.transform.position, collider.transform.rotation,
                    out var dir, out var distance))
                {
                    if(null!=onOperateOtherCollider)
                        ans += onOperateOtherCollider(collider,dir,distance);
                    else 
                        ans += dir.normalized * distance;
                }
            }

            return ans;
        }
        
        /// <summary>
        /// 计算和周边碰撞体移动多少距离才能脱离碰撞，会包含自身，建议将自身加入到ignoreSet中
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer">检测的层</param>
        /// <param name="cache"></param>
        /// <param name="ignoreSet">需要忽视的碰撞体集合</param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        public static Vector3 ComputeCurrent(this Collider self,int detectLayer, Collider[] cache,
            HashSet<Collider> ignoreSet=null, Vector3? currentPosition=null)
        {
            if (self is BoxCollider box)
            {
                return box.ComputeCurrent(detectLayer,cache,ignoreSet,currentPosition);
            }else if (self is SphereCollider sphere)
            {
                return sphere.ComputeCurrent(detectLayer,cache,ignoreSet,currentPosition);
            }else if (self is CapsuleCollider capsule)
            {
                return capsule.ComputeCurrent(detectLayer,cache,ignoreSet,currentPosition);
            }
            else
            {
                throw new Exception($"Unexpected collider type: {self}");
            }
        }
        
        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身，但不会申请内存
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="results"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        public static int GetOverlapNoAlloc(this Collider self, int detectLayer, Collider[] results,
            Vector3? currentPosition = null)
        {
            if (self is BoxCollider box)
            {
                return box.GetOverlapNoAlloc(detectLayer,results,currentPosition);
            }else if (self is SphereCollider sphere)
            {
                return sphere.GetOverlapNoAlloc(detectLayer,results,currentPosition);
            }else if (self is CapsuleCollider capsule)
            {
                return capsule.GetOverlapNoAlloc(detectLayer,results,currentPosition);
            }
            else
            {
                throw new Exception($"Unexpected collider type: {self}");
            }
        }
        
        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        public static Collider[] GetOverlap(this Collider self, int detectLayer, Vector3? currentPosition = null)
        {
            if (self is BoxCollider box)
            {
                return box.GetOverlap(detectLayer,currentPosition);
            }else if (self is SphereCollider sphere)
            {
                return sphere.GetOverlap(detectLayer,currentPosition);
            }else if (self is CapsuleCollider capsule)
            {
                return capsule.GetOverlap(detectLayer,currentPosition);
            }
            else
            {
                throw new Exception($"Unexpected collider type: {self}");
            }
        }        
        
        #endregion

        #region BoxCollider

        private static Vector3 GetCenterPosition(this BoxCollider self)
        {
            var trans = self.transform;
            return trans.position + (trans.localToWorldMatrix * self.center).ToVector3();
        }

        private static void DrawSelfGizmos(this BoxCollider self)
        {
            Gizmos.DrawWireCube(
                self.GetCenterPosition(),
                self.size);
        }

        /// <summary>
        /// 计算和周边碰撞体移动多少距离才能脱离碰撞，会包含自身，建议将自身加入到ignoreSet中
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer">检测的层</param>
        /// <param name="cache"></param>
        /// <param name="ignoreSet">需要忽视的碰撞体集合</param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static Vector3 ComputeCurrent(this BoxCollider self,int detectLayer, Collider[] cache, HashSet<Collider> ignoreSet=null, Vector3? currentPosition=null)
        {
            if(self.isTrigger)
                return Vector3.zero;
            var ans = Vector3.zero;
            var count = self.GetOverlapNoAlloc(detectLayer, cache,currentPosition);
            for (var i = 0; i < count; i++)
            {
                var other = cache[i];
                if(other.isTrigger)
                    continue;
                if(ignoreSet!=null && ignoreSet.Contains(other))
                    continue;
                if (Physics.ComputePenetration(
                    self, currentPosition ?? self.transform.position, self.transform.rotation,
                    other, other.transform.position, other.transform.rotation,
                    out var dir, out var distance))
                {
                    ans += dir.normalized * distance;
                }
            }
            return ans;
        }
        
        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身，但不会申请内存
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="results"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static int GetOverlapNoAlloc(this BoxCollider self, int detectLayer, Collider[] results, Vector3? currentPosition=null)
        {
            var trans = self.transform;
            return Physics.OverlapBoxNonAlloc(
                currentPosition ?? self.GetCenterPosition(),
                self.size,
                results,
                trans.rotation,
                detectLayer);
        }

        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static Collider[] GetOverlap(this BoxCollider self, int detectLayer, Vector3? currentPosition=null)
        {
            var trans = self.transform;
            return Physics.OverlapBox(
                currentPosition ?? self.GetCenterPosition(),
                self.size,
                trans.rotation,
                detectLayer);
        }
        
        #endregion

        #region CapsuleCollider

        private static Vector3 GetCenterPosition(this CapsuleCollider self)
        {
            var trans = self.transform;
           return trans.position + (trans.localToWorldMatrix * self.center).ToVector3();
        }

        public static float GetRealHeight(this CapsuleCollider self)
        {
            return Mathf.Max(2 * self.radius, self.height);
        }

        public static float GetMiddleLength(this CapsuleCollider self)
        {
            return Mathf.Max(0, self.height - 2 * self.radius);
        }

        public static void GetTowPoint(this CapsuleCollider self, out Vector3 first, out Vector3 second,Vector3? currentPosition=null)
        {
            var trans = self.transform;
            var pos = currentPosition ?? self.GetCenterPosition();
            var halfHeight = self.GetMiddleLength() / 2;
            first = Vector3.zero;
            second=Vector3.zero;
            switch (self.direction)
            {
                case 0:// x axis
                    first = pos - trans.right * halfHeight;
                    second = pos + trans.right * halfHeight;
                    break;
                case 1:// y axis
                    first = pos - trans.up * halfHeight;
                    second = pos + trans.up * halfHeight;
                    break;
                case 2:// z axis
                    first = pos - trans.forward * halfHeight;
                    second = pos + trans.forward * halfHeight;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void DrawSelfGizmos(this CapsuleCollider self)
        {
            self.GetTowPoint(out var first, out var second);
            Gizmos.DrawWireSphere(first, self.radius);
            Gizmos.DrawWireSphere(second, self.radius);

        }
        /// <summary>
        /// 计算和周边碰撞体移动多少距离才能脱离碰撞，会包含自身，建议将自身加入到ignoreSet中
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer">检测的层</param>
        /// <param name="cache"></param>
        /// <param name="ignoreSet">需要忽视的碰撞体集合</param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static Vector3 ComputeCurrent(this CapsuleCollider self, int detectLayer, Collider[] cache, HashSet<Collider> ignoreSet=null, Vector3? currentPosition=null)
        {
            if(self.isTrigger)
                return Vector3.zero;
            var ans = Vector3.zero;
            var count = self.GetOverlapNoAlloc(detectLayer, cache, currentPosition);
            for (var i = 0; i < count; i++)
            {
                var other = cache[i];
                if(other.isTrigger)
                    continue;
                if(ignoreSet!=null && ignoreSet.Contains(other))
                    continue;
                if (Physics.ComputePenetration(
                    self, currentPosition ?? self.transform.position, self.transform.rotation,
                    other, other.transform.position, other.transform.rotation,
                    out var dir, out var distance))
                {
                    ans += dir.normalized * distance;
                }
            }
            return ans;
        }
        
        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身，但不会申请内存
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="results"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static int GetOverlapNoAlloc(this CapsuleCollider self, int detectLayer, Collider[] results, Vector3? currentPosition=null)
        {
            self.GetTowPoint(out var first, out var second, currentPosition);
            
            return Physics.OverlapCapsuleNonAlloc(first, second,
                self.radius, results, detectLayer);
        }
       
        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static Collider[] GetOverlap(this CapsuleCollider self, int detectLayer, Vector3? currentPosition=null)
        {
            self.GetTowPoint(out var first, out var second, currentPosition);
            return Physics.OverlapCapsule(first, second,
                self.radius, detectLayer);
        }
        
        #endregion
        
        #region SphereCollider

        private static Vector3 GetCenterPosition(this SphereCollider self)
        {
            var trans = self.transform;
            return trans.position + (trans.localToWorldMatrix * self.center).ToVector3();
        }

        private static void DrawSelfGizmos(this SphereCollider self)
        {
            Gizmos.DrawWireSphere(
                self.GetCenterPosition(),
                self.radius);
        }
        /// <summary>
        /// 计算和周边碰撞体移动多少距离才能脱离碰撞，会包含自身，建议将自身加入到ignoreSet中
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer">检测的层</param>
        /// <param name="cache"></param>
        /// <param name="ignoreSet">需要忽视的碰撞体集合</param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static Vector3 ComputeCurrent(this SphereCollider self, int detectLayer, Collider[] cache, HashSet<Collider> ignoreSet=null, Vector3? currentPosition=null)
        {
            if(self.isTrigger)
                return Vector3.zero;
            var ans = Vector3.zero;
            var count = self.GetOverlapNoAlloc(detectLayer, cache,currentPosition);
            for (var i = 0; i < count; i++)
            {
                var other = cache[i];
                if(other.isTrigger)
                    continue;
                if(ignoreSet!=null && ignoreSet.Contains(other))
                    continue;
                if (Physics.ComputePenetration(
                    self, currentPosition ?? self.transform.position, self.transform.rotation,
                    other, other.transform.position, other.transform.rotation,
                    out var dir, out var distance))
                {
                    ans += dir.normalized * distance;
                }
            }
            return ans;
        }
        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身，但不会申请内存
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="results"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static int GetOverlapNoAlloc(this SphereCollider self, int detectLayer, Collider[] results, Vector3? currentPosition=null)
        {
            return Physics.OverlapSphereNonAlloc(
                currentPosition ?? self.GetCenterPosition(),
                self.radius,
                results,
                detectLayer);
        }
        /// <summary>
        /// 获取有交叉的所有碰撞体和触发器，会包括自身
        /// </summary>
        /// <param name="self"></param>
        /// <param name="detectLayer"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private static Collider[] GetOverlap(this SphereCollider self,int detectLayer, Vector3? currentPosition=null)
        {
            return Physics.OverlapSphere(
                 currentPosition ?? self.GetCenterPosition(),
                self.radius,
                detectLayer);
        }

        #endregion

    }
}
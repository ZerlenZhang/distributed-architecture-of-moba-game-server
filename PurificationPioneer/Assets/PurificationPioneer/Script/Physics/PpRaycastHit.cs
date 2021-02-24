using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class PpRaycastHit
    {
        private Collider _overrideCollider;
        private Vector3? _normal;
        private RaycastHit?_hitInfo;

        public Collider Collider => _overrideCollider ?? _hitInfo.Value.collider;
        public Vector3 Normal => _normal ?? _hitInfo.Value.normal;
        public float Distance => _hitInfo.Value.distance;
        public Vector3 Point => _hitInfo.Value.point;
        public Vector3 Force { get; set; }
        public float UserTag { get; set; }
        public bool IsColliderOverride => _overrideCollider;
        public bool IsNormalOverride => _normal.HasValue;
        public RaycastHit RaycastHit => _hitInfo.Value;
        public PpRaycastHit(RaycastHit? hitInfo, Collider overrideCollider = null, Vector3? normal=null)
        {
            Assert.IsTrue(hitInfo.HasValue);
            _hitInfo = hitInfo;
            _normal = normal;
            _overrideCollider = overrideCollider;
        }

        public void UpdateValues(RaycastHit? value)
        {
            if (!value.HasValue)
                return;
            if (!_hitInfo.HasValue)
            {
                _hitInfo = value;
                return;
            }
            _normal = value.Value.normal;
        }

        public PpRaycastHit CreateWithNewCollider(Collider newCollider)
        {
            return new PpRaycastHit(_hitInfo, newCollider);
        }
    }
}
using System;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class PpCollision
    {
        public Vector3 DeltaVelocity
        {
            get => _deltaVelocity;
            internal set => _deltaVelocity = value;
        }

        public Vector3 DeltaAcceleration
        {
            get => _deltaAcceleration;
            internal set => _deltaAcceleration = value;
        }
        public Collider Collider => _collider;

        public Vector3 Normal
        {
            get => _normal;
            internal set => _normal = value;
        }

        public Vector3 ExpectedMovement
        {
            get => _expectedMovement;
            internal set => _expectedMovement = value;
        }
        private Vector3 _expectedMovement;
        private Vector3 _deltaAcceleration;
        private Vector3 _deltaVelocity;
        private Collider _collider;
        private Vector3 _normal;

        public void CopyValues(PpCollision other)
        {
            _expectedMovement = other.ExpectedMovement;
            _deltaAcceleration = other.DeltaAcceleration;
            _deltaVelocity = other.DeltaVelocity;
            _collider = other.Collider;
            _normal = other.Normal;
        }

        public PpCollision(Collider collider)
        {
            _collider = collider;
        }

        public PpCollision(Collider collider, Vector3 normal, Vector3 expectedMovement, Vector3 deltaAcceleration,
            Vector3 deltaVelocity)
        {
            _collider = collider;
            _normal = normal;
            _deltaAcceleration = deltaAcceleration;
            _deltaVelocity = deltaVelocity;
            _expectedMovement = expectedMovement;
        }
    }
}
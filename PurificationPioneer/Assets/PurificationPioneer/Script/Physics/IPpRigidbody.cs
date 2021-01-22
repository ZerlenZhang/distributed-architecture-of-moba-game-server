using System;
using UnityEngine;

namespace PurificationPioneer.Script
{
    
    public interface IPpRigidbody
    {
        bool Enable { get; set; }
        bool IsKinematic { get; set; }
        bool UseGravity { get; set; }
        float GravityScale { get; set; }
        float Mass { get; set; }

        #region State

        Vector3 Velocity { get; set; }
        Vector3 Acceleration { get; }
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }        

        #endregion
        
        event Action<Collider> eventOnTriggerEnter;
        event Action<Collider> eventOnTriggerStay;
        event Action<Collider> eventOnTriggerExit;
        event Action<Collider> eventOnColliderEnter;
        event Action<Collider> eventOnColliderStay;
        event Action<Collider> eventOnColliderExit;
    }

    public interface IPpRigidbodyState
    {
        Vector3 Velocity { get; }
        Vector3 Acceleration { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        void SetValue(Vector3 velocity, Vector3 acceleration, Vector3 position, Quaternion rotation);
    }
}
using UnityEngine;

namespace PurificationPioneer.Script
{
    
    public interface IPpRigidbody
    {
        bool IsStatic { get; set; }
        bool UseGravity { get; set; }
        float GravityScale { get; set; }
        float Mass { get; set; }
        Vector3 Velocity { get; set; }
        Vector3 Acceleration { get; }
        Vector3 Position { get; set; }
    }

    public interface IPpRigidbodyState
    {
        Vector3 Velocity { get; }
        Vector3 Acceleration { get; }
        Vector3 Position { get; }
        void SetValue(Vector3 velocity, Vector3 acceleration, Vector3 position);
    }
}
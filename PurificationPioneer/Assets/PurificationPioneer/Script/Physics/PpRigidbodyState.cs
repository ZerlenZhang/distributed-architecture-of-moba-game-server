using UnityEngine;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 刚体状态类，此类的数据，足以设置刚体的状态
    /// </summary>
    public class PpRigidbodyState
    {
        public Vector3 Velocity { get; private set; }
        public Vector3 Acceleration { get;private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public void SetVelocity(Vector3 velocity) => Velocity = velocity;
        public void SetAcceleration(Vector3 acceleration) => Acceleration = acceleration;
        public void SetPosition(Vector3 position) => Position = position;
        public void SetRotation(Quaternion rotation) => Rotation = rotation;
        public void SaveValue(PpRigidbody rigidbody)
        {
            Velocity = rigidbody.Velocity;
            Acceleration = rigidbody.Acceleration;
            Position = rigidbody.Position;
            Rotation = rigidbody.Rotation;
        }
        public void SaveValue(Vector3 velocity, Vector3 acceleration, Vector3 position, Quaternion rotation)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            Position = position;
            Rotation = rotation;
        }

        public PpRigidbodyState(PpRigidbody ppRigidbody)
        {
            Velocity = ppRigidbody.Velocity;
            Acceleration = ppRigidbody.Acceleration;
            Position = ppRigidbody.Position;
            Rotation = ppRigidbody.Rotation;
        }
    }
}
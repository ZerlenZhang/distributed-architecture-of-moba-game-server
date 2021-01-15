using System;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class PpRigidbodyState : IPpRigidbodyState
    {
        public Vector3 Velocity { get; private set; }
        public Vector3 Acceleration { get;private set; }
        public Vector3 Position { get; private set; }
        public void SetValue(Vector3 velocity, Vector3 acceleration, Vector3 position)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            Position = position;
        }

        public PpRigidbodyState(IPpRigidbody ppRigidbody)
        {
            Velocity = ppRigidbody.Velocity;
            Acceleration = ppRigidbody.Acceleration;
            Position = ppRigidbody.Position;
        }
    }
    public class PpRigidbody : MonoBehaviour,IPpRigidbody
    {
        #region IPpRigidbody

        public bool IsStatic { get=>_isStatic; set=>_isStatic=value; }
        public bool UseGravity { get=>_useGravity; set=>_useGravity=value; }
        public float GravityScale { get=>_gravityScale; set=>_gravityScale=value; }
        public Vector3 Velocity { get=>_velocity; set=>_velocity=value; }
        public Vector3 Acceleration { get=>_acceleration; private set=>_acceleration=value; }
        public Vector3 Position { get=>transform.position; set=>transform.position=value; }
        public float Mass { get => _mass; set => _mass = value; }

        #endregion

        [SerializeField] private bool _isStatic;
        [SerializeField] private float _mass=1;
        [SerializeField] private bool _useGravity=true;
        [SerializeField] private float _gravityScale=1;
        [SerializeField] private Vector3 _velocity;
        [SerializeField] private Vector3 _acceleration;

        #region IPpRigidbodyState logic

        public void GetStateNoAlloc(ref IPpRigidbodyState state)
        {
            state.SetValue(Velocity, Acceleration, Position);
        }
        public IPpRigidbodyState GetState()
        {
            return new PpRigidbodyState(this);
        }
        public void ApplyRigidbodyState(IPpRigidbodyState state)
        {
            Position = state.Position;
            Velocity = state.Velocity;
            Acceleration = state.Acceleration;
        }
        

        #endregion
        
        public void AddImpulse(Vector3 impulse, float time)
        {
            var temp = time / Mass;
            this.Velocity += new Vector3(
                impulse.x * temp,
                impulse.y * temp,
                impulse.z * temp
            );
        }

        public void AddImpulse(Vector3 impulse) => AddImpulse(impulse, Time.deltaTime);

        private void Start()
        {
            if (UseGravity)
            {
                Acceleration += Physics.gravity * GravityScale;
            }
        }

        private void Update()
        {
            if (IsStatic)
                return;
            Simulate(Time.deltaTime);
        }

        public void Simulate(float deltaTime)
        {
            for (var timer = 0f; timer < deltaTime; timer += Time.deltaTime)
            {
                Velocity += Acceleration * Time.deltaTime;
                Position += Velocity * Time.deltaTime;                
            }
        }
    }
}
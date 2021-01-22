using System;
using System.Collections.Generic;
using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 刚体状态类，此类的数据，足以设置刚体的状态
    /// </summary>
    public class PpRigidbodyState : IPpRigidbodyState
    {
        public Vector3 Velocity { get; private set; }
        public Vector3 Acceleration { get;private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public void SetValue(Vector3 velocity, Vector3 acceleration, Vector3 position, Quaternion rotation)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            Position = position;
            Rotation = rotation;
        }

        public PpRigidbodyState(IPpRigidbody ppRigidbody)
        {
            Velocity = ppRigidbody.Velocity;
            Acceleration = ppRigidbody.Acceleration;
            Position = ppRigidbody.Position;
            Rotation = ppRigidbody.Rotation;
        }
    }
    
    public class PpRigidbody : MonoBehaviour,IPpRigidbody
    {
        #region IPpRigidbody

        public bool Enable { get=>this.enabled; set=>this.enabled=value; }
        public bool IsKinematic { get=>_isKinematic; set=>_isKinematic=value; }
        public bool UseGravity { get=>_useGravity; set=>_useGravity=value; }
        public float GravityScale { get=>_gravityScale; set=>_gravityScale=value; }
        public Vector3 Velocity { get=>_velocity; set=>_velocity=value; }
        public Vector3 Acceleration { get=>_acceleration; private set=>_acceleration=value; }
        public Vector3 Position { get=>transform.position; set=>transform.position=value; }
        public Quaternion Rotation { get=>transform.rotation; set=>transform.rotation=value; }
        public float Mass { get => _mass; set => _mass = value; }


        #region 碰撞事件
        public event Action<Collider> eventOnTriggerEnter;
        public event Action<Collider> eventOnTriggerStay;
        public event Action<Collider> eventOnTriggerExit;
        public event Action<Collider> eventOnColliderEnter;
        public event Action<Collider> eventOnColliderStay;
        public event Action<Collider> eventOnColliderExit;

        // private CollisionChecker _collisionChecker;
        // private CollisionChecker BodyCollisionChecker
        // {
        //     get
        //     {
        //         if (null == _collisionChecker)
        //         {
        //             _collisionChecker = new CollisionChecker(this,GameSettings.Instance.MaxCollisionCount);
        //         }
        //
        //         return _collisionChecker;
        //     }
        // }        

        #endregion        
        
        #endregion

        [SerializeField] private bool _isKinematic;
        [SerializeField] private float _mass=1;
        [SerializeField] private bool _useGravity=true;
        [SerializeField] private float _gravityScale=1;
        [SerializeField] private Vector3 _velocity;
        [SerializeField] private Vector3 _acceleration;

        public PhysicMaterial physicMaterial;

        private PpRigidbodyHelper _rigidbodyHelper;
        
        
        #region IPpRigidbodyState logic

        public void GetStateNoAlloc(IPpRigidbodyState state)
        {
            state.SetValue(Velocity, Acceleration, Position, Rotation);
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
            Rotation = state.Rotation;
        }
        

        #endregion

        #region AddForce

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

        #endregion

        #region Monobehaviors

        private void Start()
        {
            if (UseGravity)
            {
                Acceleration += Physics.gravity * GravityScale;
            }

            _rigidbodyHelper = this.GetHelper();
        }

        private void OnDestroy()
        {
            PpPhysics.ReleaseRigidbody(this);
            _rigidbodyHelper = null;
        }

#if DebugMode
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && GameSettings.Instance.EnablePhysicsLog)
            {
                _rigidbodyHelper?.DrawSelfGizmos();
            }
        }

#endif
        #endregion

        #region Physics

        public void Simulate(float deltaTime)
        {
            for (var timer = 0f; timer < deltaTime; timer += Time.fixedDeltaTime)
            {
                Velocity += Acceleration * Time.fixedDeltaTime;
                Move(Velocity * Time.fixedDeltaTime);
            }
        }
        
        public void Move(Vector3 movement)
        {
            Position += movement;
        }        

        #endregion
    }
}
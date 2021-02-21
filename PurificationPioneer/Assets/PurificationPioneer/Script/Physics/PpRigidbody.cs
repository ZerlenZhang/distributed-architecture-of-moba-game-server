using System;
using System.Collections.Generic;
using System.Linq;
using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
   
    public class PpRigidbody : MonoBehaviour
    {
        public bool Enable { get=>this.enabled; set=>this.enabled=value; }
        public bool IsKinematic { get=>_isKinematic; set=>_isKinematic=value; }
        public bool UseGravity { get=>_useGravity; set=>_useGravity=value; }
        public float GravityScale { get=>_gravityScale; set=>_gravityScale=value; }
        public Vector3 Velocity{ get=>_velocity; set => _velocity = value; }

        public Vector3 Acceleration { get=>_acceleration; }
        public Vector3 Position { get=>transform.position; set=>transform.position=value; }
        public Quaternion Rotation { get=>transform.rotation; set=>transform.rotation=value; }
        public float Mass { get => _mass; set => _mass = value; }

        public float Bounciness => 
            physicMaterial ? physicMaterial.bounciness : GameSettings.Instance.DefaultBounciness;
        public float StaticFriction =>
            physicMaterial ? physicMaterial.staticFriction : GameSettings.Instance.DefaultStaticFriction;
        public float DynamicFriction =>
            physicMaterial ? physicMaterial.dynamicFriction : GameSettings.Instance.DefaultDynamicFriction;


        #region internal properties

        internal Vector3 InternalVelocity
        {
            set
            {
                Velocity = value;
                PpPhysics.GetRigidbodyState(this,true).SetVelocity(value);
            }
        }

        internal Vector3 InternalAcceleration
        {
            get => Acceleration;
            set
            {
                _acceleration = value;
                PpPhysics.GetRigidbodyState(this,true).SetAcceleration(value);
            }
        }

        internal Vector3 InternalPosition
        {
            set
            {
                Position = value;
                PpPhysics.GetRigidbodyState(this,true).SetPosition(value);
            }
        }

        internal Quaternion InternalRotation
        {
            set
            {
                Rotation = value;
                PpPhysics.GetRigidbodyState(this,true).SetRotation(value);
            }
        }        

        #endregion
        
        #region editor fields

        [SerializeField] private bool _isKinematic;
        [SerializeField] private float _mass=1;
        [SerializeField] private bool _useGravity=true;
        [SerializeField] private float _gravityScale=1;
        [SerializeField] private Vector3 _velocity;
        [SerializeField] private Vector3 _acceleration;

        public PhysicMaterial physicMaterial;        

        #endregion



        private PpRigidbodyHelper _rigidbodyHelper;

        internal PpRigidbodyHelper RigidbodyHelper
        {
            get
            {
                if (null == _rigidbodyHelper)
                {
                    _rigidbodyHelper = this.GetHelper();
                    Assert.IsNotNull(_rigidbodyHelper);
                }
                return _rigidbodyHelper;
            }
        }
        
        
        #region PpRigidbodyState logic

        public void GetStateNoAlloc(PpRigidbodyState state)
        {
            state.SaveValue(Velocity, Acceleration, Position, Rotation);
        }
        public PpRigidbodyState GetState()
        {
            return new PpRigidbodyState(this);
        }
        public void ApplyRigidbodyState(PpRigidbodyState state)
        {
            Position = state.Position;
            Velocity = state.Velocity;
            _acceleration = state.Acceleration;
            Rotation = state.Rotation;
        }
        
        #endregion

        #region AddForce

        #endregion

        #region Monobehaviors
        private void Start()
        {
            if (UseGravity)
            {
                _defaultAcceleration = Physics.gravity * GravityScale;
            }
            InternalAcceleration += _defaultAcceleration;
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
                RigidbodyHelper?.DrawSelfGizmos();
            }
        }

#endif
        #endregion

        #region Physics

        /// <summary>
        /// 没有任何物体接触情况下的加速度
        /// </summary>
        private Vector3 _defaultAcceleration;

        private Dictionary<Collider, PpRaycastHit> _commonForces = new Dictionary<Collider, PpRaycastHit>();
        // private Dictionary<Collider, PpRaycastHit> _physicsForces = new Dictionary<Collider, PpRaycastHit>();
        
        public bool TryGetForce(Collider collider, out Vector3 force)
        {
            if (_commonForces.TryGetValue(collider, out var commonForce))
            {
                force = commonForce.Force;
                return true;
            }

            // if (_physicsForces.TryGetValue(collider, out var physicsForce))
            // {
            //     force = physicsForce.Value;
            //     return true;
            // }

            force=Vector3.zero;
            return false;
        }

        /// <summary>
        /// 添加一个接触
        /// </summary>
        /// <param name="interact"></param>
        /// <param name="selfCollider"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public void AddInteract(PpRaycastHit interact,Collider selfCollider)
        {
            if (_commonForces.ContainsKey(interact.Collider))
            {
                throw new Exception($"重复添加力：from:{interact.Collider.name}, to:{selfCollider.name}");
            }
            // if (_physicsForces.ContainsKey(interact.Collider))
            // {
            //     throw new Exception($"重复添加力：from:{interact.Collider.name}, to:{selfCollider.name}");
            // }

            if (interact.Collider.IsNeedCallEvent())
                _commonForces.Add(interact.Collider, interact);
            else
            {
                throw new NotImplementedException("尚未实现刚体间的交互");
                // _physicsForces.Add(interact.Collider, interact);
            }
            
            UpdateForceState();
        }

        /// <summary>
        /// 移除一个接触
        /// </summary>
        /// <param name="from"></param>
        public void RemoveInteract(Collider from)
        {
            if (_commonForces.TryGetValue(from, out var forceToRemove))
            {
                _commonForces.Remove(from);
                UpdateForceState();
            }
        }

        /// <summary>
        /// 更新受力状态
        /// </summary>
        private void UpdateForceState()
        {
            _acceleration = _defaultAcceleration;
            //计算分力
            var total = 0f;
            foreach (var kv in _commonForces)
            {
                var raycastHit = kv.Value;

                var takeAcceleration =
                    Vector3.Project(
                        _defaultAcceleration,
                        raycastHit.Normal);
                
                total += Vector3.Project(takeAcceleration, _defaultAcceleration).magnitude;
                
            }
            //Fix 分力
            foreach (var kv in _commonForces)
            {
                var raycastHit = kv.Value;
                var takeAcceleration =
                    Vector3.Project(
                        _defaultAcceleration,
                        raycastHit.Normal);
                
                takeAcceleration *= Vector3.Project(takeAcceleration, _defaultAcceleration).magnitude/total;

                
                raycastHit.Force = - Mass * takeAcceleration;

                _acceleration -= takeAcceleration;
            }
        }

        public void AddImpulse(Vector3 impulse, float time)
        {
            var temp = time / Mass;
            this.Velocity += new Vector3(
                impulse.x * temp,
                impulse.y * temp,
                impulse.z * temp
            );
        }

        public void AddImpulse(Vector3 impulse) => AddImpulse(impulse, GameSettings.Instance.PhysicsDeltaTime);        

        public void Simulate(float deltaTime,PpPhysicsSimulateOptions options)
        {
            var physicsDeltaTime = GameSettings.Instance.PhysicsDeltaTime;
            for (var timer = 0f; timer < deltaTime; timer += physicsDeltaTime)
            {
                Velocity += Acceleration * physicsDeltaTime;
                Move(Velocity * physicsDeltaTime,options);
            }
        }

        private void Move(Vector3 movement,PpPhysicsSimulateOptions options)
        {
            Position += movement.normalized * RigidbodyHelper.TryMoveDistance(movement,options,GameSettings.Instance.MinDetectableDistance);
        }

        /// <summary>
        /// 是否在地上
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public bool IfOnGround(float distance=0.01f)
        {
            return _rigidbodyHelper.IfOnGround(distance);
        }

        #endregion
    }
}
using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.Assertions;

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
    
    public class PpRigidbody : MonoBehaviour
    {
        #region IPpRigidbody

        public bool Enable { get=>this.enabled; set=>this.enabled=value; }
        public bool IsKinematic { get=>_isKinematic; set=>_isKinematic=value; }
        public bool UseGravity { get=>_useGravity; set=>_useGravity=value; }
        public float GravityScale { get=>_gravityScale; set=>_gravityScale=value; }
        public Vector3 Velocity{ get=>_velocity; set => _velocity = value; }

        public Vector3 Acceleration { get=>_acceleration; internal set=>_acceleration=value; }
        public Vector3 Position { get=>transform.position; set=>transform.position=value; }
        public Quaternion Rotation { get=>transform.rotation; set=>transform.rotation=value; }
        public float Mass { get => _mass; set => _mass = value; }

        public float Bounciness => 
            physicMaterial ? physicMaterial.bounciness : GameSettings.Instance.DefaultBounciness;
        public float StaticFriction =>
            physicMaterial ? physicMaterial.staticFriction : GameSettings.Instance.DefaultStaticFriction;
        public float DynamicFriction =>
            physicMaterial ? physicMaterial.dynamicFriction : GameSettings.Instance.DefaultDynamicFriction;

        #endregion

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
                Acceleration = value;
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

        private PpRigidbodyHelper RigidbodyHelper
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
        
        
        #region IPpRigidbodyState logic

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
                InternalAcceleration += Physics.gravity * GravityScale;
            }
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
            Position += movement.normalized * RigidbodyHelper.TryMoveDistance(movement);
        }

        #endregion
    }
}
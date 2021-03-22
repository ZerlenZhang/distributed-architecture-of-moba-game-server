using System.Collections.Generic;
using System.Text;
using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    [RequireComponent(typeof(Rigidbody))]
    public class TempRigidbody : MonoBehaviour
    {
#if DebugMode
        public bool enableInitSpeed = false;
        public Vector3 initSpeed = Vector3.right;
        public bool enableAdvance = false;
        public float advanceTime = 3;
        public bool enableStateLogEverySecond = false;
        public bool showTempRigLog;
#endif

        private Rigidbody m_Rigidbody;

        private Rigidbody Rigidbody
        {
            get
            {
                if (null == m_Rigidbody)
                {
                    m_Rigidbody = GetComponent<Rigidbody>();
                }

                return m_Rigidbody;
            }
        }

        private int index = 0;

        public Vector3 Position
        {
            set => Rigidbody.position = value;
            get => Rigidbody.position;
        }

        public float Mass
        {
            get => Rigidbody.mass;
            set => Rigidbody.mass = value;
        }

        public Vector3 Velocity
        {
            get => m_Velocity;
            set
            {
                m_Velocity = value;
            }
        }

        public bool UseGravity
        {
            get => Rigidbody.useGravity;
            set => Rigidbody.useGravity = value;
        }


        private Vector3 m_Velocity;
        private Vector3 m_Acceleration;

        private void Awake()
        {
#if DebugMode
            if (enableInitSpeed)
            {
                m_Velocity = initSpeed;
            }

            if (enableStateLogEverySecond)
            {
                InvokeRepeating("LogSpeed", 0, 1);
            }

            if (enableAdvance)
            {
                Invoke("Advanced", advanceTime);
            }
#endif
            if (UseGravity)
            {
                m_Acceleration += Physics.gravity;
            }
        }

        private void LogSpeed()
        {
            Debug.Log($"[TempPhysics][{Time.timeSinceLevelLoad}][Pos-{Position}][Vel-{Velocity}]");
        }

        private void Advanced()
        {
            Simulate(3);
        }
        

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            Simulate(deltaTime);

            index++;
#if DebugMode
            if (showTempRigLog)
            {
                Debug.Log($"[FixedUpdate][Index-{index}][Vel-{Velocity}]");
            }
#endif
        }

        private Dictionary<Collider, Vector3> m_CollisionDic = new Dictionary<Collider, Vector3>();
        
        private void OnCollisionEnter(Collision other)
        {
            Assert.IsFalse(m_CollisionDic.ContainsKey(other.collider));
            
            Velocity += other.impulse / Mass;
            
            m_CollisionDic.Add(other.collider, Vector3.zero);

            if (other.rigidbody == null)
            {
                //清空这个方向的加速度
                var projectAcceleration = Vector3.Project(m_Acceleration, other.impulse);
                m_Acceleration -= projectAcceleration;
                m_CollisionDic[other.collider] = projectAcceleration;
            }

#if DebugMode
            if (showTempRigLog)
            {
                Debug.Log($"[FixedUpdate][Index-{index}][Enter][Vel-{Velocity}][Impul-{other.impulse}][G-{Physics.gravity*Mass}");
            }
#endif
        }
        
        private void OnCollisionExit(Collision other)
        {
            Assert.IsTrue(m_CollisionDic.ContainsKey(other.collider));
            
            m_Acceleration += m_CollisionDic[other.collider];
            
            m_CollisionDic.Remove(other.collider);
        }

        public void Simulate(float deltaTime)
        {
            var acceleration = m_Acceleration;
            var tryVelocity = Velocity + acceleration * deltaTime;
            var speed = tryVelocity.magnitude;
            
            
            if (Mathf.Abs(speed) < GameSettings.Instance.MinDetectableDistance)
                return;
            
            
            var tryPos = Position + tryVelocity * deltaTime;


            var beforePos = Rigidbody.position;
            Rigidbody.MovePosition(tryPos);
            
            
            
            var changePos = Rigidbody.position - beforePos;
            var deltaPos = tryPos - Position;
            
            var moveTime = changePos.magnitude / speed;

#if DebugMode
            if (showTempRigLog)
            {
                Debug.Log($"[TempRig][index-{index}][accel-{m_Acceleration}][tryVelo-{tryVelocity}][changePos-{changePos}][deltaPos-{deltaPos}]");
            }
#endif

            
            Velocity += acceleration * moveTime;
        }
    }
}
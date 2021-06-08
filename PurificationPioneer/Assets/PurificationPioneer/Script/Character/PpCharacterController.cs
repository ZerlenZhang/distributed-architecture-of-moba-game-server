using System.Collections.Generic;
using System.Linq;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using UnityEngine.Assertions;
using System.Text;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.Script
{
    public class PpCharacterController:
        MonoBehaviour,
        IPpController
    {
        #region const

        private const string AniSpeedScale = "AniSpeed";

        #endregion
        
        #region SerializeFields

        [Tooltip("以本地模式运行")]   
        [SerializeField]
        protected bool m_WorkAsLocal = false;
        
        [Tooltip("摄像机看向的点")]
        [SerializeField]private Transform m_CameraLookPoint;
        
        [Tooltip("玩家移动速度")]        
        [HideInInspector]
        [SerializeField]private float m_MoveSpeed = 3;
        
        [Tooltip("玩家旋转速度")]  
        [SerializeField]
        protected float m_DesiredRotationSpeed = 0.3f;

        [Tooltip("跳跃初始速度")]
        [SerializeField]
        protected float m_JumpSpeed = 10f;

        [Tooltip("重力比率")]
        [SerializeField]
        protected float m_GravatyScale = 1.0f;
        
        [Header("Animation Smoothing")] [SerializeField]
        private float m_AnimationSpeedScale = 1.0f;
        
        [Range(0,1f)]
        [SerializeField]private float m_StartAnimTime = 0.3f;
        [Range(0, 1f)]
        [SerializeField]private float m_StopAnimTime = 0.15f;

        [SerializeField]private float m_AllowPlayerRotation = 0.1f;     
        
        [SerializeField]private float m_VerticalVel = -0.5f;
        
        #endregion

        #region No SerializeFields

        private bool m_Initialized = false;
        private bool m_IsGrounded;
        private bool m_BlockRotationPlayer;
        private bool m_LastAttack;
        
        // component refs
        private Animator m_Animator;
        private Camera m_Camera;
        private CharacterController m_CharacterController;
        protected LocalCameraHelper m_LocalCameraHelper;
        

        // sync
        private CharacterState m_LogicState = CharacterState.Idle;
        private CharacterState m_AniState = CharacterState.Idle;
        private int m_StickX, m_StickY, m_FaceX, m_FaceY, m_FaceZ;
        protected bool m_Attacking, m_Jumping;
        private float m_LogicYVelocity;



#if DebugMode

        private float timer = 0;
        private float time = 1;
        private Vector3 lastPosition;
        
#endif
        #endregion

        public float MoveSpeed
        {
            get => m_MoveSpeed;
            set => m_MoveSpeed = value;
        }

        public float PaintEfficiencyScale
        {
            get => HeroConfig.basePaintEfficiency/100f;
        }
        
        #region Events

        protected virtual void Start()
        {
            if (m_WorkAsLocal)
            {
                UnityAPI.LockMouse();
                
                m_Animator = this.GetComponent<Animator> ();
                if (m_Animator)
                {
                    m_Animator.SetFloat(AniSpeedScale, m_AnimationSpeedScale);
                }
                m_CharacterController = this.GetComponent<CharacterController> ();
                
                m_LocalCameraHelper =
                    ResourceMgr.InstantiateGameObject(LocalAssetName.LocalCamera)
                        .GetComponent<LocalCameraHelper>();
                m_LocalCameraHelper.Init(transform,this.m_CameraLookPoint);
                m_Camera = m_LocalCameraHelper.ActivateCamera;
            }
            else
            {
                CEventCenter.AddListener<PlayerInput>(Message.OnInputPredict, OnInputPredict);
            }
        }

        protected virtual void Update()
        {
            if (m_WorkAsLocal)
            {
                m_Attacking = Input.GetKey(GameSettings.Instance.AttackKey);
                
                if (!m_LastAttack && m_Attacking)
                {
                    m_BlockRotationPlayer = true;
                }else if (m_LastAttack && !m_Attacking)
                {
                    m_BlockRotationPlayer = false;
                }
                if (m_Attacking)
                {
                    RotateToCamera(transform);
                    var face = m_Camera.transform.forward;
                    OnCommonAttack(face.x.ToInt(), face.y.ToInt(), face.z.ToInt());
                }
                
                UpdateAnimation();

                m_IsGrounded = m_CharacterController.isGrounded;
                
                if (m_IsGrounded)
                    m_VerticalVel -= 0;
                else
                    m_VerticalVel -= 1;

                var moveVector = new Vector3(0, m_VerticalVel * .2f * Time.deltaTime, 0);
                m_CharacterController.Move(moveVector);



                m_LastAttack = m_Attacking;
                return;
                
                
                
                void UpdateAnimation()
                {
                    //Calculate Input Vectors
                    var InputX = GlobalVar.IsPlayerInControl ? Input.GetAxis ("Horizontal") : 0;
                    var InputZ = GlobalVar.IsPlayerInControl ?Input.GetAxis ("Vertical") : 0;

                    //Calculate the Input Magnitude
                    var Speed = new Vector2(InputX, InputZ).sqrMagnitude;

                    if (m_Animator)
                    {
                        //Change animation mode if rotation is blocked
                        m_Animator.SetBool("shooting", m_BlockRotationPlayer);

                        //Physically move player
                        if (Speed > m_AllowPlayerRotation) {
                            m_Animator.SetFloat ("Blend", Speed, m_StartAnimTime, Time.deltaTime);
                            m_Animator.SetFloat("X", InputX, m_StartAnimTime/3, Time.deltaTime);
                            m_Animator.SetFloat("Y", InputZ, m_StartAnimTime/3, Time.deltaTime);
                            PlayerMoveAndRotation ();
                        } else if (Speed < m_AllowPlayerRotation) {
                            m_Animator.SetFloat ("Blend", Speed, m_StopAnimTime, Time.deltaTime);
                            m_Animator.SetFloat("X", InputX, m_StopAnimTime/ 3, Time.deltaTime);
                            m_Animator.SetFloat("Y", InputZ, m_StopAnimTime/ 3, Time.deltaTime);
                        }
	                            
                    }
                    else if (Speed > m_AllowPlayerRotation)
                    {
                        PlayerMoveAndRotation ();
                    }

	        
                    void PlayerMoveAndRotation() 
                    {
                        var InputX = GlobalVar.IsPlayerInControl ? Input.GetAxis ("Horizontal"):0;
                        var InputZ = GlobalVar.IsPlayerInControl ?Input.GetAxis ("Vertical"):0;

                        var forward = m_Camera.transform.forward;
                        var right = m_Camera.transform.right;

                        forward.y = 0f;
                        right.y = 0f;

                        forward.Normalize();
                        right.Normalize();

                        var desiredMoveDirection = forward * InputZ + right * InputX;

                        if (m_BlockRotationPlayer == false) {
                            //Camera
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), m_DesiredRotationSpeed);
                            var motion = desiredMoveDirection * Time.deltaTime * MoveSpeed;
                            m_CharacterController.Move(motion);
                        }
                        else
                        {
                            //Strafe
                            var motion = (transform.forward * InputZ + transform.right * InputX) * Time.deltaTime * MoveSpeed;
                            m_CharacterController.Move(motion);
                        }
                    }
                }
            }

            if (!m_Initialized)
                return;
            
            #region Network

            #region Network Input

#if UNITY_EDITOR
            if (Input.GetKeyDown(GameSettings.Instance.JumpKey))
            {
                InputMgr.jump = true;
            }
            if (!GameSettings.Instance.WorkAsAndroid)
            {
                //用户输入
                if (Input.GetKey(GameSettings.Instance.AttackKey))
                    InputMgr.attack = true;
            }
#elif UNITY_STANDALONE_WIN
            //用户输入
            if (Input.GetKeyDown(GameSettings.Instance.JumpKey))
            {
                InputMgr.jump = true;
            }
            if (Input.GetKey(GameSettings.Instance.AttackKey))
                InputMgr.attack = true;
#endif            

            #endregion

            //如果逻辑状态是击飞，眩晕，死亡的状态的话，表现层不变
            if (m_LogicState != CharacterState.Idle
                && m_LogicState != CharacterState.Move)
                return;

            // Camera
            if (m_Attacking)
            {
                RotateToCamera(transform);
            }

            #region Animation
            
            if (m_Animator)
            {
                //Calculate the Input Magnitude
                var inputX = m_StickX.ToFloat();
                var inputZ = m_StickY.ToFloat();
                var inputMagnitude = new Vector2(inputX, inputZ).sqrMagnitude;

                //Change animation mode if rotation is blocked
                m_Animator.SetBool("shooting", m_BlockRotationPlayer);

                //Physically move player
                if (inputMagnitude > m_AllowPlayerRotation) {
                    m_Animator.SetFloat ("Blend", inputMagnitude, m_StartAnimTime, Time.deltaTime);
                    m_Animator.SetFloat("X", inputX, m_StartAnimTime/3, Time.deltaTime);
                    m_Animator.SetFloat("Y", inputZ, m_StartAnimTime/3, Time.deltaTime);
                } else if (inputMagnitude < m_AllowPlayerRotation) {
                    m_Animator.SetFloat ("Blend", inputMagnitude, m_StopAnimTime, Time.deltaTime);
                    m_Animator.SetFloat("X", inputX, m_StopAnimTime/ 3, Time.deltaTime);
                    m_Animator.SetFloat("Y", inputZ, m_StopAnimTime/ 3, Time.deltaTime);
                }                      
            }

      

            #endregion

            //没有移动的话，直接Idle不用管别的
            if (this.m_StickX==0 && this.m_StickY==0 && !m_Jumping)
            {
                if (m_AniState == CharacterState.Move)
                {
                    m_AniState=CharacterState.Idle;
                }
                return;
            }
            
            //有移动的话，切换状态，执行手柄逻辑
            if (m_AniState == CharacterState.Idle)
            {
                m_AniState = CharacterState.Move;
            }

            DoSimulateY(Time.deltaTime);
            DoJoystick(Time.deltaTime);

#if DebugMode
            //debug
            if(GameSettings.Instance.EnableMoveLog)
            {
                timer += Time.deltaTime;
                if (timer > time)
                {
                    timer = 0;
                    var move = transform.position - lastPosition;
                    // Debug.Log($"[PpCharacterController] Move: {move.magnitude} {move}");
                    lastPosition = transform.position;
                }
            }
#endif

            #endregion

        }

        protected virtual void OnDestroy()
        {
            if(!m_WorkAsLocal)
                CEventCenter.RemoveListener<PlayerInput>(Message.OnInputPredict, OnInputPredict);
        }

        #endregion

        private void DoJoystick(float deltaTime)
        {
            //没有移动，将逻辑状态置为Idle
            if (this.m_StickX == 0 && this.m_StickY == 0 && !m_Jumping)
            {
                m_LogicState = CharacterState.Idle;
                return;
            }
            //有移动，将逻辑状态置为Walk
            m_LogicState = CharacterState.Move;


            var desiredMotion = GetDesiredMotion(deltaTime);

            //Camera
            if (!m_BlockRotationPlayer) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMotion.normalized), m_DesiredRotationSpeed);
            }

            m_CharacterController.Move(desiredMotion);
        }
        
        private bool DoSimulateY(float deltaTime)
        {
            var hit = false;
            var currentPos = transform.position;
            var distance = 10;
            var dir = Mathf.Sign(m_LogicYVelocity) * Vector3.up;
            var start = currentPos + dir * -0.001f;
            var end = start + dir * distance;
            
            if (Mathf.Abs(m_LogicYVelocity) <= Mathf.Epsilon)
            {
                // Debug.Log("Direct return");
                var tempAns = Physics.RaycastAll( currentPos + 0.001f * Vector3.up, Vector3.down , GameSettings.Instance.MinDetectableDistance,
                    gameObject.GetUnityDetectLayer());
                if (!tempAns.Any())
                {
                    return false;
                }

                
                return tempAns.Any(raycastHit => raycastHit.collider.gameObject != gameObject);
            }
            
            
            var desiredMotion = GetDesiredMotion(deltaTime);

            desiredMotion.y = m_LogicYVelocity * deltaTime;

            Debug.DrawLine(start, end, Color.red);

            var ans = Physics.RaycastAll(start, dir , distance,
                gameObject.GetUnityDetectLayer());
            if (ans.Any())
            {
                var desiredDistance = desiredMotion.magnitude;
                foreach (var raycastHit in ans)
                {
                    if (raycastHit.collider.gameObject != gameObject 
                        && raycastHit.distance < desiredDistance)
                    {
                        desiredMotion.y = raycastHit.point.y-currentPos.y;
                        // Debug.Log($"撞到: {raycastHit.collider.name}");
                        hit = true;      
                        break;
                    }
                }
            }

            var perfectLogicPos = currentPos + Vector3.up * desiredMotion.y;
            // Debug.Log($"Move: {desiredMotion.y}");
            transform.position = perfectLogicPos; //Vector3.Lerp(currentPos, perfectLogicPos, 0.5f);


            return hit;
        }


        #region IFrameSyncCharacter

        public int SeatId { get; private set; }
        public void SkipCharacterInput(IEnumerable<PlayerInput> inputs)
        {
            SyncLastCharacterInput(inputs);
        }

        public virtual void SyncLastCharacterInput(IEnumerable<PlayerInput> inputs)
        {
            foreach (var playerInput in inputs)
            {
#if DebugMode
                if (GameSettings.Instance.EnableInputLog)
                {
                    var msg = new StringBuilder();
                    msg.Append($"[InputMsg][FrameId-{FrameSyncMgr.FrameId}]");
                    msg.Append($"[Move-({playerInput.moveX},{playerInput.moveY}]");
                    msg.Append($"[Attack-{playerInput.attack}][Jump-{playerInput.jump}]");
                    msg.Append($"[Face-({playerInput.faceX},{playerInput.faceY})]");
                    msg.Append($"[Mouse-({playerInput.mouseX},{playerInput.mouseY})]");
                    Debug.Log(msg);
                }
#endif
                
                this.m_StickX = playerInput.moveX;
                this.m_StickY = playerInput.moveY;
                this.m_FaceX = playerInput.faceX;
                this.m_FaceY = playerInput.faceY;
                this.m_FaceZ = playerInput.faceZ;
                this.m_Attacking = playerInput.attack;
                
                // Attack
                if (!m_LastAttack && m_Attacking)
                {
                    m_BlockRotationPlayer = true;
                }
                else if (m_LastAttack && !m_Attacking)
                {
                    m_BlockRotationPlayer = false;
                }
                m_LastAttack = m_Attacking;
                

                var deltaTime = GlobalVar.LogicFrameDeltaTime.ToFloat();
                
                // Jump
                // if (m_CharacterController.isGrounded)
                {
                    // print(1);
                    if (!m_Jumping && playerInput.jump)
                    {
                        m_Jumping = true;
                        m_LogicYVelocity = m_JumpSpeed;
                    }
                }

                if (DoSimulateY(deltaTime))// 撞到东西
                {
                    m_Jumping = false;
                    m_LogicYVelocity = 0;
                    // Debug.Log("Reset");
                }
                else// 更新速度
                {
                    var acceleration = Physics.gravity.y * m_GravatyScale;
                    var newVel = m_LogicYVelocity + acceleration * deltaTime;
                    m_LogicYVelocity = newVel;
                    // Debug.Log($"Update: {m_LogicYVelocity}");
                }

                var beforePos = transform.position;
                
                DoJoystick(deltaTime);

                var newPos = transform.position;

                if (playerInput.attack)
                {
                    OnCommonAttack(this.m_FaceX,this.m_FaceY,this.m_FaceZ);
                }

#if DebugMode
                if(GameSettings.Instance.EnableMoveLog)
                    Debug.Log($"[GetInput-SyncLast] [Time {deltaTime}] ({this.m_StickX},{this.m_StickY}), 前进：{(newPos - beforePos).magnitude}");                
#endif
            }
        }


        public void OnHandleCurrentCharacterInput(IEnumerable<PlayerInput> inputs)
        {
            foreach (var playerInput in inputs)
            {
                this.m_StickX = playerInput.moveX;
                this.m_StickY = playerInput.moveY;
                this.m_FaceX = playerInput.faceX;
                this.m_FaceY = playerInput.faceY;
                this.m_FaceZ = playerInput.faceZ;
                this.m_Attacking = playerInput.attack;
                
#if DebugMode
                if(GameSettings.Instance.EnableMoveLog)
                    Debug.Log($"[GetInput-Current-{FrameSyncMgr.FrameId}] ({this.m_StickX},{this.m_StickY})");
#endif
                if (this.m_StickX == 0 && this.m_StickY == 0 && !m_Jumping)
                {
                    m_LogicState = CharacterState.Idle;
                }
                else
                {
                    m_LogicState = CharacterState.Move;
                }
            }
        }

        #endregion

        #region IPpController
        public HeroConfigAsset HeroConfig { get; private set; }
        public void InitCharacterController(int seatId, HeroConfigAsset config)
        {
            m_Initialized = true;
            
            SeatId = seatId;
            HeroConfig = config;
            FrameSyncMgr.AddFrameSyncCharacter(this);
            
            //初始化内部参数
            this.m_StickX = 0;
            this.m_StickY = 0;
            m_LogicState = CharacterState.Idle;
            
            m_Animator = this.GetComponent<Animator> ();
            m_CharacterController = this.GetComponent<CharacterController> ();

            Assert.IsTrue(m_CharacterController);

            if (m_Animator)
            {
                m_Animator.SetFloat(AniSpeedScale, m_AnimationSpeedScale);
            }
            
            InitCharacter(GlobalVar.LocalSeatId==SeatId);
            
            
#if DebugMode
            //debug
            lastPosition = transform.position;
            
#endif
        }        

        #endregion

        #region protected

        protected virtual void InitCharacter(bool isLocal)
        {
            m_MoveSpeed = HeroConfig.moveSpeed;
            if (isLocal)
            {
                m_LocalCameraHelper =
                    ResourceMgr.InstantiateGameObject(LocalAssetName.LocalCamera)
                        .GetComponent<LocalCameraHelper>();
                m_LocalCameraHelper.Init(transform,this.m_CameraLookPoint);
                m_Camera = m_LocalCameraHelper.ActivateCamera;

#if UNITY_EDITOR
                if(GameSettings.Instance.AutoSelectLocalPlayer)
                    UnityEditor.Selection.activeInstanceID = this.gameObject.GetInstanceID();
#endif
            }
            else
            {
                var headCanvasUi = ResourceMgr.InstantiateGameObject(LocalAssetName.CharacterHeadCanvas,transform)
                    .GetComponent<CharacterHeadCanvas>();
                headCanvasUi.transform.localPosition = Vector3.up * m_CharacterController.height;
                headCanvasUi.Init(Camera.main, GlobalVar.SeatId_MatcherInfo[SeatId].Unick);
            }
        }


        protected virtual void OnCommonAttack(int faceX, int faceY, int faceZ)
        {
            
        }
        
        #endregion
        
        #region private

        private void OnInputPredict(PlayerInput input)
        {
            if (SeatId != GlobalVar.LocalSeatId)
                return;
            m_StickX = input.moveX;
            m_StickY = input.moveY;
            m_FaceX = input.faceX;
            m_FaceY = input.faceY;
            m_FaceZ = input.faceZ;
            m_Attacking = input.attack;
        }

        private void RotateToCamera(Transform t)
        {
            var forward = m_Camera.transform.forward;
            Quaternion lookAtRotation = Quaternion.LookRotation(forward);
            Quaternion lookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            t.rotation = Quaternion.Slerp(transform.rotation, lookAtRotationOnly_Y, m_DesiredRotationSpeed);
        }

        private Vector3 GetDesiredMotion(float deltaTime)
        {
            if (m_StickX == 0 && m_StickY == 0)
            {
                return Vector3.zero;
            }
            
            var inputX = m_StickX.ToFloat();
            var inputZ = m_StickY.ToFloat();
            var expectedForward = new Vector2(this.m_FaceX, this.m_FaceZ).normalized;
            var inputDir = new Vector2( inputX, inputZ);
            var angle =  Mathf.Sign(inputDir.x) * Vector2.Angle(Vector2.up, inputDir);
            var moveDir = expectedForward.RotateDegree(angle);

            var desiredMoveDirection = new Vector3(moveDir.x, 0, moveDir.y);
            var desiredMotion = Vector3.zero;
            if (m_BlockRotationPlayer == false) {
                desiredMotion = desiredMoveDirection * (deltaTime * MoveSpeed);
                // Debug.Log($"Face:{expectedForward}, Input:{inputDir}, Angle:{angle}, Final: {desiredMoveDirection}");

            }
            else
            {
                //Strafe
                desiredMotion = (transform.forward * inputZ + transform.right  * inputX) * (deltaTime * MoveSpeed);
                // Debug.Log($"DesireDir: {(transform.forward * inputZ + transform.right  * inputX)}");

            }

            return desiredMotion;
        }
        
        #endregion
    }
}
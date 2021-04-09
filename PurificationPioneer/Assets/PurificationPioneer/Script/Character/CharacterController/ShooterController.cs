using System;
using System.Collections.Generic;
using System.Text;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    [RequireComponent(typeof(CharacterController))]
    public class ShooterController : MonoBehaviour,IPpController
    {

        #region SerializeFields

        [Tooltip("以本地模式运行")]   
        [SerializeField] private bool m_WorkAsLocal = false;
        
        [Tooltip("玩家移动速度")]        
        [SerializeField]private float m_MoveSpeed = 1;
        
        
        [Tooltip("玩家旋转速度")]  
        [SerializeField]private float m_DesiredRotationSpeed = 0.3f;
        
        
        [Tooltip("摄像机看向的点")]
        [SerializeField]private Transform m_CameraLookPoint;
        
        [Header("Animation Smoothing")]
        [Range(0,1f)]
        [SerializeField]private float m_StartAnimTime = 0.3f;
        [Range(0, 1f)]
        [SerializeField]private float m_StopAnimTime = 0.15f;

        [SerializeField]private float m_VerticalVel = -0.5f;
        [SerializeField]private float m_AllowPlayerRotation = 0.1f;        
        [SerializeField] private ShootingSystem m_ShootSystem;

        #endregion

        #region No SerializeFields

        private bool m_Initialized = false;
        private bool m_IsGrounded;
        private bool m_BlockRotationPlayer;
        
        // component refs
        private Animator m_Animator;
        private Camera m_Camera;
        private CharacterController m_CharacterController;
        

        // sync
        private CharacterState m_LogicState = CharacterState.Idle;
        private CharacterState m_AniState = CharacterState.Idle;
        private int m_StickX, m_StickY, m_FaceX, m_FaceY, m_FaceZ;
        private bool m_Attack;

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
        
        #region Events

        private void Start()
        {
            if (m_WorkAsLocal)
            {
                UnityAPI.LockMouse();
                
                m_Animator = this.GetComponent<Animator> ();
                m_CharacterController = this.GetComponent<CharacterController> ();
                
                var localCameraHelper =
                    ResourceMgr.InstantiateGameObject(LocalAssetName.LocalCamera)
                        .GetComponent<LocalCameraHelper>();
                localCameraHelper.Init(transform,this.m_CameraLookPoint);
                m_Camera = localCameraHelper.ActivateCamera;

                
                m_ShootSystem.Initialize(
                    () => Input.GetMouseButton(0),
                    true,
                    localCameraHelper.vcam,
                    RotateToCamera,
                    GameSettings.Instance.LeftMaterial);
            }
            else
            {
                CEventCenter.AddListener<PlayerInput>(Message.OnInputPredict, OnInputPredict);
            }
        }

        private void Update()
        {
            if (m_WorkAsLocal)
            {
                InputMagnitude ();

                m_IsGrounded = m_CharacterController.isGrounded;
                if (m_IsGrounded)
                    m_VerticalVel -= 0;
                else
                    m_VerticalVel -= 1;

                var moveVector = new Vector3(0, m_VerticalVel * .2f * Time.deltaTime, 0);
                m_CharacterController.Move(moveVector);
        
                return;
        
                void InputMagnitude() {
                    //Calculate Input Vectors
                    var InputX = Input.GetAxis ("Horizontal");
                    var InputZ = Input.GetAxis ("Vertical");

                    //Calculate the Input Magnitude
                    var Speed = new Vector2(InputX, InputZ).sqrMagnitude;

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
			
			
                    void PlayerMoveAndRotation() 
                    {
                        var InputX = Input.GetAxis("Horizontal");
                        var InputZ = Input.GetAxis("Vertical");

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
                            m_CharacterController.Move(desiredMoveDirection * Time.deltaTime * MoveSpeed);
                        }
                        else
                        {
                            //Strafe
                            m_CharacterController.Move((transform.forward * InputZ + transform.right  * InputX) * Time.deltaTime * MoveSpeed);
                        }
                    }
                }
            }

            if (!m_Initialized)
                return;
            
            #region Network

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

            //如果逻辑状态是击飞，眩晕，死亡的状态的话，表现层不变
            if (m_LogicState != CharacterState.Idle
                && m_LogicState != CharacterState.Move)
                return;
            
            
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
            
            
            
            //没有移动的话，直接Idle不用管别的
            if (this.m_StickX==0 && this.m_StickY==0)
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
                    Debug.Log($"[PpCharacterController] Move: {move.magnitude} {move}");
                    lastPosition = transform.position;
                }                
            }
#endif            

            #endregion

        }

        private void OnDestroy()
        {
            if(!m_WorkAsLocal)
                CEventCenter.RemoveListener<PlayerInput>(Message.OnInputPredict, OnInputPredict);
        }

        #endregion


        
        

        private void DoJoystick(float deltaTime)
        {
            //没有移动，将逻辑状态置为Idle
            if (this.m_StickX == 0 && this.m_StickY == 0)
            {
                m_LogicState = CharacterState.Idle;
                return;
            }
            //有移动，将逻辑状态置为Walk
            m_LogicState = CharacterState.Move;
            
            var expectedForward = new Vector2(this.m_FaceX, this.m_FaceZ).normalized;
            var inputDir = new Vector2(this.m_StickX.ToFloat(), this.m_StickY.ToFloat());
            var angle =  -Mathf.Sign(inputDir.x) * Vector2.Angle(inputDir, Vector2.up);
            var moveDir = expectedForward.RotateDegree(angle);

            var desiredMoveDirection = new Vector3(moveDir.x, 0, moveDir.y);
            
            if (m_BlockRotationPlayer == false) {
                //Camera
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), m_DesiredRotationSpeed);
                m_CharacterController.Move(desiredMoveDirection * (Time.deltaTime * MoveSpeed));
            }
            else
            {
                //Strafe
                var inputX = m_StickX.ToFloat();
                var inputZ = m_StickY.ToFloat();
                m_CharacterController.Move((transform.forward * inputZ + transform.right  * inputX) * (Time.deltaTime * MoveSpeed));
            }
        }


        #region IFrameSyncCharacter

        public int SeatId { get; private set; }
        public void SkipCharacterInput(IEnumerable<PlayerInput> inputs)
        {
            SyncLastCharacterInput(inputs);
        }

        public void SyncLastCharacterInput(IEnumerable<PlayerInput> inputs)
        {
            m_ShootSystem.ApplyAndSimulate();
            m_ShootSystem.SaveState();
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
                this.m_Attack = playerInput.attack;

                var time = GlobalVar.LogicFrameDeltaTime.ToFloat();
                var beforePos = transform.position;
                
                DoJoystick(time);

                var newPos = transform.position;

                if (playerInput.attack)
                {
                    OnCommonAttack(this.m_FaceX,this.m_FaceY,this.m_FaceZ);
                }

                if (playerInput.jump)
                {
                    OnJump();
                }
#if DebugMode
                if(GameSettings.Instance.EnableMoveLog)
                    Debug.Log($"[GetInput-SyncLast] [Time {time}] ({this.m_StickX},{this.m_StickY}), 前进：{(newPos - beforePos).magnitude}");                
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
                this.m_Attack = playerInput.attack;
#if DebugMode
                if(GameSettings.Instance.EnableMoveLog)
                    Debug.Log($"[GetInput-Current-{FrameSyncMgr.FrameId}] ({this.m_StickX},{this.m_StickY})");
#endif
                if (this.m_StickX == 0 && this.m_StickY == 0)
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

            Assert.IsTrue(m_Animator &&  m_CharacterController);
            
            InitCharacter(GlobalVar.LocalSeatId==SeatId);
            
            
#if DebugMode
            //debug
            lastPosition = transform.position;
            
#endif
        }        

        #endregion


        protected virtual void InitCharacter(bool isLocal)
        {
            if (isLocal)
            {
                var localCameraHelper =
                    ResourceMgr.InstantiateGameObject(LocalAssetName.LocalCamera)
                        .GetComponent<LocalCameraHelper>();
                localCameraHelper.Init(transform,this.m_CameraLookPoint);
                m_Camera = localCameraHelper.ActivateCamera;

                m_ShootSystem.Initialize(
                    () => m_Attack,
                    false,
                    localCameraHelper.vcam,
                    RotateToCamera,
                    GlobalVar.LocalSeatId % 2==0
                    ? GameSettings.Instance.LeftMaterial
                    : GameSettings.Instance.RightMaterial);

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

                m_ShootSystem.Initialize(() => m_Attack);
            }
        }


        private void OnCommonAttack(int faceX, int faceY, int faceZ)
        {
            
        }


        private void OnJump()
        {
            
        }


        private void OnInputPredict(PlayerInput input)
        {
            if (SeatId != GlobalVar.LocalSeatId)
                return;
            m_StickX = input.moveX;
            m_StickY = input.moveY;
            m_FaceX = input.faceX;
            m_FaceY = input.faceY;
            m_FaceZ = input.faceZ;
            m_Attack = input.attack;
        }

        
        public void RotateToCamera(Transform t)
        {
            var forward = m_Camera.transform.forward;
            Quaternion lookAtRotation = Quaternion.LookRotation(forward);
            Quaternion lookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            t.rotation = Quaternion.Slerp(transform.rotation, lookAtRotationOnly_Y, m_DesiredRotationSpeed);
        }
    }
}
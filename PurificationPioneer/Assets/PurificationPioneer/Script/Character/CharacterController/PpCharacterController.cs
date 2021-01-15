using System.Collections.Generic;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.Attributes;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class PpCharacterController<T>:
        MonoBehaviour,
        IPpController
        where T:class, IPpAnimator
    {
        #region Inspector properties

        [SerializeField]private Transform cameraLookPoint;
        [SerializeField] protected Transform centerPoint;

        public HeroConfigAsset HeroConfig { get; private set; }
        
        #region MoveSpeed

        /// <summary>
        /// 玩家移动速度
        /// </summary>
        [SerializeField]private float moveSpeed = 1;
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }        

        #endregion
        
        #region CharacterAnimator

        [RequireInterface(typeof(IPpAnimator))]
        [SerializeField] 
        private Object characterAnimator;

        protected T CharacterAnimator => characterAnimator as T;        

        #endregion
        
        #region CharacterController

        [SerializeField] private CharacterController characterController;

        protected CharacterController CharacterController => characterController;
        #endregion        

        #endregion

        #region public API

        /// <summary>
        /// 初始化角色控制器
        /// </summary>
        /// <param name="seatId"></param>
        /// <param name="logicPos"></param>
        public void InitCharacterController(int seatId, Vector3 logicPos, HeroConfigAsset config)
        {
            Assert.IsTrue(cameraLookPoint && centerPoint);
            Assert.IsNotNull(config);
            //帧同步回调初始化
            SeatId = seatId;
            HeroConfig = config;
            FrameSyncMgr.AddFrameSyncCharacter(this);
            
            //初始化内部参数
            this.stick_x = 0;
            this.stick_y = 0;
            CharacterAnimator.LogicToIdle();
            logicPosition = logicPos;
            
            InitCharacter();
            
            if(GlobalVar.LocalSeatId==SeatId)
                InitLocalCharacter();
        }        

        #endregion
        
        #region protected methods

        /// <summary>
        /// 所有角色都要进行的初始化操作
        /// </summary>
        protected virtual void InitCharacter()
        {
            
        }

        /// <summary>
        /// 初始化本地玩家
        /// </summary>
        protected virtual void InitLocalCharacter()
        {
            var localCameraHelper =
                ResourceMgr.InstantiateGameObject(LocalAssetName.LocalCamera)
                    .GetComponent<LocalCameraHelper>();
            localCameraHelper.Init(transform,this.cameraLookPoint);
        }        

        protected virtual void OnAttack(int faceX,int faceY, int faceZ)
        {
            
        }
        #endregion

        #region IFrameSyncWithSeatId

        [SerializeField]private int seatId;
        public int SeatId
        {
            get => seatId;
            private set => seatId = value;
        }

        public void SkipCharacterInput(IEnumerable<PlayerInput> inputs)
        {               
            SyncLastCharacterInput(inputs);
        }

        public void SyncLastCharacterInput(IEnumerable<PlayerInput> inputs)
        {            
            foreach (var playerInput in inputs)
            {
                this.stick_x = playerInput.moveX;
                this.stick_y = playerInput.moveY;
                this.face_x = playerInput.faceX;
                this.face_y = playerInput.faceY;
                this.face_z = playerInput.faceZ;
                this.attack = playerInput.attack;
                
                var before = this.logicPosition;
                this.transform.position = this.logicPosition;
                DoJoystick(GlobalVar.LogicFrameDeltaTime.ToFloat());
                this.logicPosition = this.transform.position;

                if (playerInput.attack)
                {
                    OnAttack(this.face_x,this.face_y,this.face_z);
                }
#if DebugMode
                if(GameSettings.Instance.EnableFrameSyncLog)
                    Debug.Log($"[GetInput-SyncLast-{FrameSyncMgr.FrameId}] ({this.stick_x},{this.stick_y}), 前进：{(this.logicPosition - before).magnitude}");                
#endif

            }
        }

        public void OnHandleCurrentCharacterInput(IEnumerable<PlayerInput> inputs)
        {
            foreach (var playerInput in inputs)
            {
                this.stick_x = playerInput.moveX;
                this.stick_y = playerInput.moveY;
                this.face_x = playerInput.faceX;
                this.face_y = playerInput.faceY;
                this.face_z = playerInput.faceZ;
                this.attack = playerInput.attack;
#if DebugMode
                if(GameSettings.Instance.EnableFrameSyncLog)
                    Debug.Log($"[GetInput-Current-{FrameSyncMgr.FrameId}] ({this.stick_x},{this.stick_y})");
#endif
                if (this.stick_x == 0 && this.stick_y == 0)
                {
                    CharacterAnimator.LogicToIdle();
                }
                else
                {
                    CharacterAnimator.LogicToWalk();
                }
                if(playerInput.attack)
                    OnAttack(this.face_x,this.face_y,this.face_z);
            }
        }        
        
        #endregion
        
        #region Private_Logic

        /// <summary>
        /// 遥感操作
        /// </summary>
        private int stick_x, stick_y, face_x,face_y,face_z;

        private bool attack;
        
        /// <summary>
        /// 逻辑位置
        /// </summary>
        private Vector3 logicPosition;

        protected virtual void Start()
        {
            Assert.IsNotNull(CharacterAnimator);
            Assert.IsNotNull(CharacterController);
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            if (!GameSettings.Instance.WorkAsAndroid)
            {
                //用户输入
                if (Input.GetKey(GameSettings.Instance.AttackKey))
                    InputMgr.attack = true;
            }
#elif UNITY_STANDALONE_WIN
            //用户输入
            if (Input.GetKey(GameSettings.Instance.AttackKey))
                InputMgr.attack = true;
#endif
            
            //如果逻辑状态是击飞，眩晕，死亡的状态的话，表现层不变
            if (CharacterAnimator.LogicState != CharacterState.Idle
                && CharacterAnimator.LogicState != CharacterState.Move)
                return;
            
            //没有移动的话，直接Idle不用管别的
            if (this.stick_x==0 && this.stick_y==0)
            {
                if (CharacterAnimator.AniState == CharacterState.Move)
                {
                    CharacterAnimator.ToIdle();
                }
                return;
            }

            //有移动的话，切换状态，执行手柄逻辑
            if (CharacterAnimator.AniState == CharacterState.Idle)
            {
                CharacterAnimator.ToMove();
            }
            
            DoJoystick(Time.deltaTime);
        }
        

        /// <summary>
        /// 根据stick_x和stick_y，移动deltaTime这么长时间
        /// </summary>
        /// <param name="deltaTime"></param>
        private void DoJoystick(float deltaTime)
        {
            //没有移动，将逻辑状态置为Idle
            if (this.stick_x == 0 && this.stick_y == 0) 
            {
                CharacterAnimator.LogicToIdle();
                return;
            }

            //有移动，将逻辑状态置为Walk
            CharacterAnimator.LogicToWalk();

            var expectedForward = new Vector2(this.face_x, this.face_z).normalized;
            var inputDir = new Vector2(
                this.stick_x.ToFloat(),
                this.stick_y.ToFloat());


            var angle =  -Mathf.Sign(inputDir.x) * Vector2.Angle(inputDir, Vector2.up);
            var moveDir = expectedForward.RotateDegree(angle);

            transform.forward = new Vector3(moveDir.x, 0, moveDir.y);
            
            // Debug.Log($"[Move] expectedForward:{expectedForward}, inputDir:{inputDir}, moveDir:{moveDir}, angle:{angle}");

            var s = this.moveSpeed * deltaTime;
            
            CharacterController.Move(new Vector3(
                s * moveDir.x,
                0, 
                s * moveDir.y));
        }            

        #endregion
    }
}
using System.Collections.Generic;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Utility;
using ReadyGamerOne.Attributes;
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
        /// <summary>
        /// 玩家移动速度
        /// </summary>
        [SerializeField]private float moveSpeed = 1;
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }
        
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
        
        /// <summary>
        /// 初始化角色控制器
        /// </summary>
        /// <param name="seatId"></param>
        /// <param name="logicPos"></param>
        public virtual void InitCharacterController(int seatId, Vector3 logicPos)
        {
            //帧同步回调初始化
            SeatId = seatId;
            FrameSyncMgr.AddListener(this);
            
            //初始化内部参数
            this.stick_x = 0;
            this.stick_y = 0;
            CharacterAnimator.LogicToIdle();
            logicPosition = logicPos;
        }
        
        #region IFrameSyncWithSeatId

        [SerializeField]private int seatId;
        public int SeatId
        {
            get => seatId;
            private set => seatId = value;
        }

        public virtual void SkipPlayerInput(IEnumerable<PlayerInput> inputs)
        {               
            SyncLastPlayerInput(inputs);
        }

        public virtual void SyncLastPlayerInput(IEnumerable<PlayerInput> inputs)
        {            
            foreach (var playerInput in inputs)
            {
                this.stick_x = playerInput.moveX;
                this.stick_y = playerInput.moveY;
                var before = this.logicPosition;
                this.transform.position = this.logicPosition;
                DoJoystick(GlobalVar.LogicFrameDeltaTime.ToFloat());
                this.logicPosition = this.transform.position;
                
                print("前进：" + (this.logicPosition - before).magnitude);                
            }
        }

        public virtual void OnHandleCurrentPlayerInput(IEnumerable<PlayerInput> inputs)
        {
            foreach (var playerInput in inputs)
            {
                this.stick_x = playerInput.moveX;
                this.stick_y = playerInput.moveY;

                if (this.stick_x == 0 && this.stick_y == 0)
                {
                    CharacterAnimator.LogicToIdle();
                }
                else
                {
                    CharacterAnimator.LogicToWalk();
                }
            }
        }        
        
        #endregion

        #region Private_Logic

        /// <summary>
        /// 遥感操作
        /// </summary>
        private int stick_x, stick_y;
        
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
                CharacterAnimator.ToMove(this.stick_x.ToFloat(),this.stick_y.ToFloat(),MoveSpeed);
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

            var localForward = transform.forward;

            var srcDir = new Vector2(
                localForward.x,
                localForward.z);
            var inputDir = new Vector2(
                this.stick_x.ToFloat(),
                this.stick_y.ToFloat());
            
            var moveDir = srcDir.RotateDegree(Vector2.Angle(inputDir, Vector2.up));

            var s = this.moveSpeed * deltaTime;
            
            CharacterController.Move(new Vector3(
                s * moveDir.x,
                0, 
                s * moveDir.y));
        }            

        #endregion
    }
}
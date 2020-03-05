using System;
using gprotocol;
using Moba.Const;
using Moba.Data;
using Moba.Global;
using Moba.Protocol;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Script
{
    enum HeroState
    {
        Walk = 1,
        Free = 2,
        Idle = 3,
        Attack1 = 4,
        Attack2 = 5,
        Attack3 = 6,
        Skill1 = 7,
        Skill2 = 8,
        Die = 9,
    }
    
    
    [RequireComponent(typeof(CharacterController))]
    public class Hero : MonoBehaviour
    {
        /// <summary>
        /// 是否为其他玩家
        /// </summary>
        public bool isGhost = false;

        /// <summary>
        /// 玩家移动速度
        /// </summary>
        public float moveSpeed = 8;

        [Header("世界角度y方向偏移")]
        public float worldDegree = -45;
        
        public SideType side;
        public int seatid = -1;

        private CharacterController cc;

        /// <summary>
        /// 动画状态
        /// </summary>
        private HeroState aniState = HeroState.Idle;
        
        /// <summary>
        /// 遥感操作
        /// </summary>
        private int stick_x, stick_y;
        /// <summary>
        /// 逻辑状态
        /// </summary>
        private HeroState logicState = HeroState.Idle;
        /// <summary>
        /// 逻辑位置
        /// </summary>
        private Vector3 logicPosition = Vector3.zero;

        /// <summary>
        /// 逻辑初始化
        /// </summary>
        /// <param name="logicPos"></param>
        public void LogicInit(Vector3 logicPos)
        {
            this.stick_x = 0;
            this.stick_y = 0;
            logicState = HeroState.Idle;
            logicPosition = logicPos;
        }


        #region Private

        private Animation anim;

        private Vector3 cameraOffset;

        private void Start()
        {
            
            cc = GetComponent<CharacterController>();
            Assert.IsTrue(cc);
            
            if(!isGhost)
            {// 如果是主角，就生成光环
                var ring = ResourceMgr.InstantiateGameObject(
                    EffectName.LightRing,transform);
                
                //调整摄像机
                if (this.side == SideType.Red)
                {// 红色
                    Camera.main.transform.localPosition = new Vector3(262.4f, 82, 112.6f);
                    Camera.main.transform.localEulerAngles = new Vector3(50.7f, 225, 0);
                }
                

                this.cameraOffset = Camera.main.transform.position - transform.position;

                

            }


            anim = GetComponent<Animation>();
                Assert.IsTrue(anim);
            anim.Play("idle");
        }


        private void Update()
        {
            OnJoystickAniUpdate();
        }

        /// <summary>
        /// 遥感动画更新
        /// </summary>
        private void OnJoystickAniUpdate()
        {
            if (this.logicState != HeroState.Idle
                && this.logicState != HeroState.Walk)
                return;

            
            #region 动画_&&_状态
            

            if (this.stick_x==0
                && this.stick_y==0)
            {
                if (aniState == HeroState.Walk)
                {
                    this.anim.CrossFade("idle");
                    this.aniState = HeroState.Idle;                    
                }
                return;
            }

            if (this.aniState == HeroState.Idle)
            {
                this.anim.CrossFade("walk");
                this.aniState = HeroState.Walk;
            }            

            #endregion


            DoJoystick(Time.deltaTime);

            if (!isGhost)
            {
                Camera.main.transform.position = transform.position + cameraOffset;
            }

        }

        private void DoJoystick(float deltaTime)
        {
            #region 逻辑状态

            if (this.stick_x == 0 && this.stick_y == 0) {
                this.logicState = HeroState.Idle;
                return;
            }

            this.logicState = HeroState.Walk;

            #endregion

            
            var rawDir=new Vector2
            {
                x = stick_x/(float)(1<<16),
                y = stick_y/(float)(1<<16),
            };
            var rotateDegree = this.side == SideType.Red
                ? 180 + worldDegree
                : worldDegree;
            var moveDir = rawDir.RotateAngle(rotateDegree);

            #region 移动


            var s = this.moveSpeed * deltaTime;
            var sx = s * moveDir.x; // cos
            var sz = s * moveDir.y; // sin


            this.cc.Move(new Vector3(sx, 0, sz));            

            #endregion

            #region 朝向

            var degree = Mathf.Atan2(moveDir.y, moveDir.x) 
                         * Mathf.Rad2Deg;

            degree = 360 - degree + 90;

            this.transform.localEulerAngles = new Vector3(0, degree, 0);


            #endregion
        }        

        #endregion



        
        
        
        /// <summary>
        /// 更新当前帧逻辑
        /// </summary>
        /// <param name="opt"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnHandlerCurrentLogicFrame(OptionEvent opt)
        {
            switch ((OptType)opt.optype)
            {
                case OptType.JoyStick:
                    this.stick_x = opt.x;
                    this.stick_y = opt.y;

                    if (this.stick_x == 0 && this.stick_y == 0)
                    {
                        this.logicState = HeroState.Idle;
                    }
                    else
                    {
                        this.logicState = HeroState.Walk;
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// 同步之前收到的最后一帧的处理结果
        /// </summary>
        /// <param name="opt"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnAsyncLastLogicFrame(OptionEvent opt)
        {
            switch ((OptType)opt.optype)
            {
                case OptType.JoyStick:
                    this.stick_x = opt.x;
                    this.stick_y = opt.y;
                    var before = this.logicPosition;
                    this.transform.position = this.logicPosition;
                    DoJoystick(LogicConfig.LogicFrameTime);
                    this.logicPosition = this.transform.position;
                    print("前进：" + (this.logicPosition - before).magnitude);
                    break;
            }
        }

        /// <summary>
        /// 跳帧
        /// </summary>
        /// <param name="opt"></param>
        public void OnJumpToNextFrame(OptionEvent opt)
        {
            OnAsyncLastLogicFrame(opt);
        }
    }
}
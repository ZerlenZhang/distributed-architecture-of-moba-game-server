using System;
using Moba.Const;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Script
{
    enum CharacterState
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
    public class CharacterCtrl : MonoBehaviour
    {
        /// <summary>
        /// 是否为其他玩家
        /// </summary>
        public bool isGhost = false;

        public float moveSpeed = 20;

        [Header("世界角度y方向偏移")]
        public float worldDegree = -45;

        private CharacterController cc;

        private CharacterState state = CharacterState.Idle;

        [HideInInspector]
        public Joystick _joystick;

        private Animation anim;

        private Vector3 cameraOffset;

        private void Start()
        {
            this.cameraOffset = Camera.main.transform.position - transform.position;
            
            cc = GetComponent<CharacterController>();
            Assert.IsTrue(cc);
            
            if(!isGhost)
            {// 如果是主角，就生成光环
                var ring = ResourceMgr.InstantiateGameObject(
                    EffectName.LightRing,transform);

                anim = GetComponent<Animation>();
                Assert.IsTrue(_joystick&&anim);
            }

            anim.Play("idle");
        }

        private void Update()
        {
            
            #region 动画_&&_状态
            
            if (this.state != CharacterState.Idle
                && this.state != CharacterState.Walk)
                return;


            if (Math.Abs(this._joystick.TouchDir.x) < 0.01f
                && Math.Abs(this._joystick.TouchDir.y) < 0.01f)
            {
                if (state == CharacterState.Walk)
                {
                    this.anim.CrossFade("idle");
                    this.state = CharacterState.Idle;                    
                }
                return;
            }

            if (this.state == CharacterState.Idle)
            {
                this.anim.CrossFade("walk");
                this.state = CharacterState.Walk;
            }            

            #endregion
            
            
//            print(this._joystick.TouchDir);

            var moveDir = _joystick.TouchDir.RotateAngle(worldDegree);

            #region 移动


            var s = this.moveSpeed * Time.deltaTime;
            var sx = s * moveDir.x; // cos
            var sz = s * moveDir.y; // sin


            this.cc.Move(new Vector3(sx, 0, sz));            

            #endregion

            #region 朝向

            var degree = Mathf.Atan2(moveDir.y, moveDir.x) 
                         * Mathf.Rad2Deg;

            degree = 360 - degree + 90 ;

            this.transform.localEulerAngles = new Vector3(0, degree, 0);


            #endregion

            if (!isGhost)
            {
                Camera.main.transform.position = transform.position + cameraOffset;
            }

        }
    }
}
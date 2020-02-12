using System;
using UnityEngine;

namespace ReadyGamerOne.Rougelike.Person
{
    public abstract class AbstractPersonController:MonoBehaviour
    {
        public AbstractPerson selfPerson;
        
        /// <summary>
        /// 走路速度
        /// </summary>
        public abstract float MoveSpeed { get; set; }
        public virtual Vector3 Velocity3D => Vector3.zero;
        public virtual Vector2 Velocity2D => Vector2.zero;

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            
        }
        
        protected virtual void Update()
        {
        }


        /// <summary>
        /// 设置当前角色是否可以移动
        /// </summary>
        /// <param name="state"></param>
        public abstract void SetMoveable(bool state);

        public virtual void InitController(AbstractPerson self)
        {
            this.selfPerson = self;
        }

        /// <summary>
        /// 此函数如果使用不能调用base.ToDie
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void ToDie()
        {
            throw new NotImplementedException();
        }
    }
}
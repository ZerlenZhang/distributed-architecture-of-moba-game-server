using System;
using ReadyGamerOne.Common;
using UnityEngine;

namespace ReadyGamerOne.Rougelike.Person
{
    public interface IPoolPerson :
        IPerson,
        IPoolable
    {
        void Release();
        event Action<IPoolPerson> onAfterGet;
        event Action<IPoolPerson> onBeforeRealse;
    }
    
    /// <summary>
    /// 自带对象池维护的Person
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PoolPerson<T> : 
        AbstractPerson,
        IPoolPerson
        where T : PoolPerson<T>, new()
    {
        #region Static

        /// <summary>
        /// 对象池，这是整个泛型定义存在的意义!
        /// 泛型对象池可以做到为所有可能的子类各自维护一个对象池
        /// </summary>
        private static ObjPool<T> _objPool= new ObjPool<T>(
            () => new T(),
            obj => obj.OnRecycleToPool(),
            obj => obj.OnGetFromPool());

        public static T GetInstance(Vector3? pos, Transform parent = null)
        {
            var person = _objPool.GetObj();
            if (parent)
                parent.gameObject.transform.SetParent(parent);
            if(pos!=null)
                person.gameObject.transform.position = pos.Value;
            return person;
        }
        
        #endregion
        
        /// <summary>
        /// 重写死亡操作，对象池控制的话就不会销毁，而是回收
        /// </summary>
        public override void LogicKill()
        {
            Release();
        }

        /// <summary>
        /// 重写快速终结操作
        /// </summary>
        public override void Destroy()
        {
            Release();
        }


        public override bool IsAlive
        {
            get
            {
                if (gameObject == null)
                    return false;

                return gameObject.activeSelf && Hp > 0;
            }
        }

        #region 对象池操作，只有在角色出生和死亡时调用

        public virtual void OnRecycleToPool()
        {
            onBeforeRealse?.Invoke(this);
            DisableObject();
        }

        public virtual void OnGetFromPool()
        {
            EnableObject();
            onAfterGet?.Invoke(this);
        }

        public void Release()
        {
            TickOnKillEventAndClearEvent();
            _objPool.ReleaseObj(this as T);
        }

        public event Action<IPoolPerson> onAfterGet;
        public event Action<IPoolPerson> onBeforeRealse;

        #endregion

        #region 禁止子类重载EnableObject和DisableObject，使用OnGetFromPool和OnRecycleToPool代替

        public sealed override void EnableObject()
        {
            base.EnableObject();
        }

        public sealed override void DisableObject()
        {
            base.DisableObject();
        }        

        #endregion

    }
}
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace ReadyGamerOne.Common
{
    /// <summary>
    /// 模板对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjPoor<T>where T:class
    {

        private Queue<T> objCache;

        private event Func<T> onInit; 

        private event Action<T> onGet;

        private event Action<T> onRelease;
        
        public ObjPoor(Func<T> onInit, Action<T> onRelease=null,Action<T> onGet=null)
        {
            this.objCache=new Queue<T>();

            this.onInit += onInit;
            
            if (onGet != null)
                this.onGet += onGet;
            
            if (onRelease != null)
                this.onRelease += onRelease;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public T GetObj()
        {
            T obj=null;
            if (objCache.Count <= 0)
            {
                obj = this.onInit();
                if(obj==null)
                    throw new Exception("对象池对象初始化失败");
            }
            else
            {
                obj = objCache.Dequeue();
            }
            
            this.onGet?.Invoke(obj);
            return obj;
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="obj"></param>
        public void ReleaseObj(T obj)
        {
            Assert.IsTrue(obj!=null);
            this.onRelease?.Invoke(obj);
            objCache.Enqueue(obj);
        }


        public void Clear()
        {
            objCache.Clear();
        }
    }

    /// <summary>
    /// 实现Poolable接口物品的对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PoolableObjectPool<T> :
        ObjPoor<T>
        where T : class, IPoolable, new()
    {
        public PoolableObjectPool() : base(()=>new T(),obj=>obj.OnRecycleToPool() , obj=>obj.OnGetFromPool())
        {
        }
    }
}
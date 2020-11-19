using System;
using ReadyGamerOne.Data;

namespace ReadyGamerOne.Rougelike.Person
{
    public interface IPoolDataPerson :
        IPoolPerson,
        IUseCsvData
    {
    }
    
    /// <summary>
    /// 自带从CSV读取数据的人物，继承此类的Person类需要具有UseCsvData特性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PoolDataPerson<T> :
        PoolPerson<T>,
        IPoolDataPerson
        where T : PoolPerson<T>, new()
    {
        
        #region Fields

        private CsvMgr _personData;

        #endregion


        #region IUseCsvData

        public CsvMgr CsvData => _personData;
        
        public abstract Type DataType { get; }

        /// <summary>
        /// 加载数据，在这个函数中，应该完成对当前角色各项数据的赋值
        /// </summary>
        /// <param name="data"></param>
        public virtual void LoadData(CsvMgr data)
        {
            this._personData = data;
        }

        #endregion
        

        /// <summary>
        /// 从对象池获取时加载数据资源
        /// </summary>
        public override void OnGetFromPool()
        {
            LoadData(CsvMgr.GetData(GetType().Name,DataType));
            base.OnGetFromPool();
        }
    }
}
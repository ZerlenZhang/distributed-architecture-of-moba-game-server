namespace ReadyGamerOne.Common
{
    public interface IPoolable
    {
        /// <summary>
        /// 对象池回收Person时调用
        /// </summary>
        void OnRecycleToPool();
        
        /// <summary>
        /// 对象池加载Person时调用
        /// </summary>
        void OnGetFromPool();
    }
}
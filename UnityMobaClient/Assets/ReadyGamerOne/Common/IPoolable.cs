namespace ReadyGamerOne.Common
{
    public interface IPoolable<T>
        where T:class,new()
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
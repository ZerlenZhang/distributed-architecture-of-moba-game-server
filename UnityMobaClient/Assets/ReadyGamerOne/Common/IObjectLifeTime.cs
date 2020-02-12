namespace ReadyGamerOne.Common
{
    /// <summary>
    /// 与预制体对应的物体
    /// </summary>
    public interface IResourcableObject:
        IUnityGameObject
    {
        /// <summary>
        /// 预制体或资源路径
        /// </summary>
        string ResPath { get; }
        /// <summary>
        /// 实例化物体
        /// </summary>
        void OnInstanciateObject();
        /// <summary>
        /// 销毁物体
        /// </summary>
        void DestroyObject();
        /// <summary>
        /// 激活物体
        /// </summary>
        void EnableObject();
        /// <summary>
        /// 失活物体
        /// </summary>
        void DisableObject();
    }
}
using UnityEngine;

namespace PurificationPioneer.Script
{
    public interface IBulletState
    {
        Vector3 LogicPosition { get; set; }
        Vector3 RendererPosition { set; get; }
    }   
    public interface IBulletConfig
    {
        int MaxLife { get; }
    }
    public interface IBulletStrategy<TBullet, TBulletConfig, TBulletState>
        where TBulletState: class, IBulletState
        where TBulletConfig: class, IBulletConfig
    {
        /// <summary>
        /// 子弹如何销毁
        /// </summary>
        /// <param name="bullet"></param>
        void DisableBullet(TBullet bullet);
        /// <summary>
        /// 子弹是否活动状态
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        bool IsBulletActivate(TBullet bullet);

        /// <summary>
        /// 每个渲染帧帧更新子弹状态
        /// eg: 位置、朝向……
        /// </summary>
        /// <param name="timeSinceLastLogicFrame"></param>
        /// <param name="bulletConfig"></param>
        /// <param name="bulletState"></param>
        void UpdateStateOnRendererFrame(float timeSinceLastLogicFrame, TBulletConfig bulletConfig, ref TBulletState bulletState);
      
        /// <summary>
        /// 每个逻辑帧更新子弹状态
        /// eg: 位置、朝向……
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="bulletConfig"></param>
        /// <param name="bulletState"></param>
        void UpdateStateOnLogicFrame(float deltaTime, TBulletConfig bulletConfig, ref TBulletState bulletState);
    }

}
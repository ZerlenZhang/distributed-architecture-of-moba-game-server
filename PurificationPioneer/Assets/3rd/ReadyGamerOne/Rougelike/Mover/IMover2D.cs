using System;
using UnityEngine;

namespace ReadyGamerOne.Rougelike.Mover
{
    public interface IMover2D
    {
        /// <summary>
        /// 设置和获取位置
        /// </summary>
        Vector3 Position { get; set; }
        
        /// <summary>
        /// 获取和设置速度
        /// </summary>
        Vector2 Velocity { get; set; }

        /// <summary>
        /// 重力实际值
        /// </summary>
        float Gravity { get; set; }
        
        /// <summary>
        /// 重力缩放值
        /// </summary>
        float GravityScale { get; set; }
        
        /// <summary>
        /// 会和什么层发生碰撞碰撞
        /// </summary>
        LayerMask ColliderLayers { get; set; }
        
        /// <summary>
        /// 会和那些层触发Trigger但不碰撞
        /// </summary>
        LayerMask TriggerLayers { get; set; }
        
        /// <summary>
        /// 上方是否有碰撞
        /// </summary>
        bool CollidedUp { get; }
        /// <summary>
        /// 下方是否有碰撞
        /// </summary>
        bool CollidedDown { get; }
        /// <summary>
        /// 左方是否有碰撞
        /// </summary>
        bool CollidedLeft { get; }
        /// <summary>
        /// 右方是否有碰撞
        /// </summary>
        bool CollidedRight { get; }

        event Action<RaycastHit2D> eventOnColliderEnter;
        event Action<RaycastHit2D> eventOnColliderStay;
        event Action<RaycastHit2D> eventOnColliderExit;
        event Action<GameObject> eventOnTriggerEnter;
        event Action<GameObject> eventOnTriggerStay;
        event Action<GameObject> eventOnTriggerExit;
    }
}
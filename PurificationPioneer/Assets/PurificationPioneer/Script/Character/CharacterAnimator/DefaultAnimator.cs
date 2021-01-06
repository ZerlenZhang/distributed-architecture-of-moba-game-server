using UnityEngine;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 默认角色动画机
    /// </summary>
    public class DefaultAnimator : MonoBehaviour,IPpAnimator
    {
        public CharacterState LogicState { get; private set; }
        public CharacterState AniState { get; private set; }

        protected virtual void Start()
        {
            
        }
        
        public virtual void LogicToWalk()
        {
            LogicState = CharacterState.Move;
        }

        public virtual void LogicToIdle()
        {
            LogicState = CharacterState.Idle;
        }

        public virtual void ToMove()
        {
            AniState = CharacterState.Move;
        }

        public virtual void ToIdle()
        {
            AniState = CharacterState.Idle;
        }

        public virtual void ToDie()
        {
            AniState = CharacterState.Die;
        }
    }
}
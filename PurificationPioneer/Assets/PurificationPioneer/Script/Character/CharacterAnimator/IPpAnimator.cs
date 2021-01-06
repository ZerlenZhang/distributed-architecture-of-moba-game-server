namespace PurificationPioneer.Script
{
    public enum CharacterState
    {
        Move = 1,
        Free = 2,
        Idle = 3,
        Attack1 = 4,
        Attack2 = 5,
        Attack3 = 6,
        Skill1 = 7,
        Skill2 = 8,
        Die = 9,
    }
    
    public interface IPpAnimator
    {
        CharacterState LogicState { get;  }
        CharacterState AniState { get;  }

        void LogicToWalk();
        void LogicToIdle();
        
        /// <summary>
        /// 表现层执行移动动画
        /// </summary>
        void ToMove();
        void ToIdle();
        void ToDie();
    }
}
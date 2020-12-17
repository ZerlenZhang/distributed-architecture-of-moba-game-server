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
        /// <param name="x">水平方向移动，0~1</param>
        /// <param name="y">前后方向移动，0~1</param>
        /// <param name="speed">移动速度</param>
        void ToMove(float x,float y, float speed);
        void ToIdle();
        void ToDie();
    }
}
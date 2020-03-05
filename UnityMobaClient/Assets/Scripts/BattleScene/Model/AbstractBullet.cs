using Moba.Data;
using Moba.Global;
using Moba.Script.Building;

namespace DefaultNamespace
{
    public class AbstractBullet : UnityEngine.MonoBehaviour
    {
        protected SideType side;
        protected BulletConfig config;

        public virtual void Init(SideType side)
        {
            this.side = side;
            config = this is MainBullet
                ? LogicConfig.MainBullet
                : LogicConfig.NormalBullet;
        }
        public virtual void OnLogicFrameUpdate(float deltaTime)
        {
            
        }
    }
}
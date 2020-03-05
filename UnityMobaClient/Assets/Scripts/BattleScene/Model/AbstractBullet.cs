using Moba.Data;
using Moba.Global;
using Moba.Script;
using Moba.Script.Building;
using UnityEngine;

namespace DefaultNamespace
{
    
    /*
     * 子弹动画：自己控制
     * 子弹打到谁？ 逻辑帧控制
     * 子弹生命周期；逻辑帧控制
     * 
     */
    public class AbstractBullet : UnityEngine.MonoBehaviour
    {
        protected SideType side;
        protected BulletConfig config;
        protected int activeTime; // ms
        private float passed_time;
        private bool is_running = false;
        private int logicPastTime = 0;
        private Vector3 logicPos;
        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            // 只管播放动画
            if (!is_running)
                return;

            var tempTime = activeTime / 1000f;
            var dt = Time.deltaTime;
            this.passed_time += dt;
            if (this.passed_time > tempTime)
            {
                dt -= passed_time - tempTime;
            }

            //更新子弹显示位置
            var offset = this.transform.forward * config.speed * dt;
            this.transform.position += offset;
            
            if (passed_time > tempTime)
            {
                this.is_running = false;
            }     
        }
        

        public virtual void Init(SideType side)
        {
            this.side = side;
            config = this is MainBullet
                ? LogicConfig.MainBullet
                : LogicConfig.NormalBullet;
        }
        public virtual void OnLogicFrameUpdate(int deltaTime)
        {
            this.logicPastTime += deltaTime;

            if (this.logicPastTime > this.activeTime)
                deltaTime -= (this.logicPastTime - this.activeTime);

            //更新子弹逻辑位置
            var dt = deltaTime / 1000f;
            var offset = this.transform.forward * config.speed * dt;
            this.logicPos += offset;    
            
            //子弹打到人逻辑
            if (HitTest(this.logicPos, offset.magnitude))
            {
                return;
            }
            
            //销毁子弹
            if(this.logicPastTime>=this.activeTime)
            {
                GameZygote.Instance.RemoveBullet(this);
            }            
        }

        private bool HitTest(Vector3 startPos, float distance)
        {
            var hits = Physics.RaycastAll(
                startPos, transform.forward, distance);
            if (hits!=null && hits.Length > 0)
            {
                for (var i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    if (hit.collider.gameObject.layer == (int) ObjType.Hero)
                    {
                        var hero = hit.collider.GetComponent<Hero>();
                        if (hero)
                        {
                            if(hero.side==side)
                                continue;
                            
                            //造成伤害
                            hero.TakeDamage(config.attack);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void ShotTo(Vector3 target )
        {
            this.transform.LookAt(target); // z轴指向了这个位置
            var dir = target - this.transform.position;
            var len = dir.magnitude;
            this.activeTime = (int)len*1000 / config.speed;
            this.passed_time = 0f;
            this.is_running = true;
            this.logicPastTime = 0;
            this.logicPos = transform.position;
        }
    }
}
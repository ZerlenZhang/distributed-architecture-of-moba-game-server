using System;
using Moba.Data;
using Moba.Global;
using UnityEngine;

namespace Moba.Script.Building
{
    public class AbstractTower:MonoBehaviour
    {
        [Header("红色放还是蓝色方，1红，-1蓝")]
        protected SideType side;
        protected TowerConfig config;

        private int nowFps = 0;

        protected virtual void Start()
        {
            this.nowFps = config.shoot_logic_fps;
        }
        public virtual void Init(SideType side)
        {
            this.side = side;
            config = this is MainTower
                ? LogicConfig.MainTower
                : LogicConfig.NormalTower;
        }
        public virtual void OnLogicFrameUpdate(int deltaTime) 
        {
            this.nowFps++;
            if (this.nowFps >= config.shoot_logic_fps)
            {
                this.nowFps = 0;
                DoTowerAi();
            }
        }

        private void DoTowerAi()
        {
            var len =float.MaxValue;
            Hero target=null;
            foreach (var hero in GameZygote.Instance.GetHeros())
            {
                if(hero.side==this.side)
                    continue;
                var dis = (transform.position - hero.transform.position).magnitude;
                if (dis < len)
                {
                    len = dis;
                    target = hero;
                }
            }

            if(target && len < config.attackRadis)
            {
                var cc = target.GetComponent<CharacterController>();
                var targetPos = target.transform.position +
                                new Vector3(0,0.6f * cc.height, 0);
                ShotAt(targetPos);
            }
            
        }

        private void ShotAt(Vector3 targetPos)
        {
            var bullet = GameZygote.Instance.CreateBullet(this.side, GetType());
            bullet.transform.position = transform.Find("point").transform.position;
            bullet.ShotTo(targetPos);
        }
    }
}
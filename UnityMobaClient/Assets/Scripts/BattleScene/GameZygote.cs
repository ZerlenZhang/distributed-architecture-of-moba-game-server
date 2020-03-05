using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using gprotocol;
using Moba.Const;
using Moba.Data;
using Moba.Global;
using Moba.Protocol;
using Moba.Script.Building;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Script
{

    public enum ObjType
    {
        Bullet = 12,
        Hero,
        Tower,
    }
    
    public class GameZygote : MonoSingleton<GameZygote>
    {
        public GameObject[] characters;
        public Transform playerEntry;
        public Transform enemyEntry;
        public float spreadRadis = 4f;

        //main,left,right,front
        public GameObject[] redTowers;
        public List<AbstractTower> _redTowers=new List<AbstractTower>();
        public GameObject[] blueTowers;
        public List<AbstractTower> _blueTowers = new List<AbstractTower>();

        public GameObject mainBulletPrefab;
        public GameObject normalBulletPrefab;
        
        private List<AbstractBullet> bullets=new List<AbstractBullet>();
        
        /// <summary>
        /// 已经同步过的id
        /// </summary>
        private int sync_frameid = 1;

        /// <summary>
        /// 上一帧所有用户操作
        /// </summary>
        private FrameOptionEvent lastOptionEvent;


        private Joystick _joystick;

        private List<Hero> heros = new List<Hero>();

        #region 初始化



        private void PlaceTowers()
        {
            for (var i = 0; i < redTowers.Length; i++)
            {
                var tower = i == 0
                    ? (AbstractTower) redTowers[i].AddComponent<MainTower>()
                    : redTowers[i].AddComponent<NormalTower>();
                tower.Init(SideType.Red);
                _redTowers.Add(tower);
                
                tower  = i == 0
                    ? (AbstractTower) blueTowers[i].AddComponent<MainTower>()
                    : blueTowers[i].AddComponent<NormalTower>();
                tower.Init(SideType.Blue);
                _blueTowers.Add(tower);
            }
        }

        private Hero PlaceHeroAt(PlayerMatchInfo info)
        {
            var uinfo = NetInfo.GetPlayerInfo(info.seatid);
            
            var player = Instantiate(characters[uinfo.usex]);
            player.name = uinfo.unick;

            var center = info.side == -1
                ? playerEntry    // 蓝色方
                : enemyEntry;    // 红色方

            var degree = 180.0f / (LogicConfig.PlayerCount + 1);
            var index = info.seatid / 2;
            var temp = Vector2.right;
            temp = temp.RotateAngle((index + 1) * degree);
            player.transform.position
                = center.position
                  + spreadRadis * temp.x * center.right
                  + spreadRadis * temp.y * center.forward;
            
            var hero = player.AddComponent<Hero>();
            hero.isGhost = NetInfo.seatid != info.seatid;
            hero.side = (SideType)info.side;
            hero.seatid = info.seatid;
            //逻辑初始化
            hero.LogicInit(player.transform.position);
            return hero;

        }

        private Hero GetHero(int seatid)
        {
            foreach (var hero in heros)
            {
                if (hero.seatid == seatid)
                    return hero;
            }

            return null;
        }
                

        #endregion

        #region Monobehavior
        
        private void Start()
        {
            CEventCenter.AddListener<LogicFrame>(Message.OnLogicFrame,OnLogicFrame);

            _joystick = FindObjectOfType<Joystick>();

            PlaceTowers();

            foreach (var matchInfo in NetInfo.playerMatchInfos)
            {
                var hero = PlaceHeroAt(matchInfo);
                heros.Add(hero);
            }
        }

        private void OnDestroy()
        {
            CEventCenter.RemoveListener<LogicFrame>(Message.OnLogicFrame, OnLogicFrame);
        }        

        #endregion
        
        #region 帧同步核心

        private void OnLogicFrame(LogicFrame obj)
        {            
            if (obj.frameid < this.sync_frameid)
                return;

            /*print("未同步的帧：" + obj.unsync_frames.Count);
            var temp = "";
            foreach (var frameOptionEvent in obj.unsync_frames)
            {
                foreach (var opt in frameOptionEvent.opts)
                {
                    temp+=($"optType: {opt.optype}, stick: [{opt.x},{opt.y}]\n");
                }
            }

            print(temp);*/

            // 同步之前收到的最后一帧的处理结果，据此保证所有客户端的处理结果都是一样的
            // 同步客户端上一帧逻辑操作，调整位置，调整后，客户端同步到的就是sync_frame
            if (null != lastOptionEvent)
            {
                this.OnAsyncLastLogicFrame(lastOptionEvent);
            }
            
            // 同步丢掉的帧
            // 从sync_frame 同步到 obj.frameid-1
            foreach (var frame in obj.unsync_frames)
            {
                if (this.sync_frameid >= frame.frameid)
                {// 已经同步
                    continue;
                }
                if (frame.frameid >= obj.frameid)
                {// 无需同步
                    break;
                }
                
                //跳帧
                this.OnJumpToNextFrame(frame);
            }
            
            
            //记录下当前帧操作
            this.sync_frameid = obj.frameid;
            if (obj.unsync_frames.Count > 0)
            {
                //保留最新帧信息
                this.lastOptionEvent = obj.unsync_frames.Last();
                //获取最新帧信息，处理逻辑
                this.OnHandlerCurrentLogicFrame(this.lastOptionEvent);                
            }
            else
            {
                this.lastOptionEvent = null;
            }

            //采集下一帧事件发布给服务器 
            CollectNextFrameOpts();
            
            
        }

        /// <summary>
        /// 跳帧
        /// 不用播放动画
        /// </summary>
        /// <param name="frameOpts"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnJumpToNextFrame(FrameOptionEvent optionEvent)
        {
            foreach (var opt in optionEvent.opts)
            {
                var hero = GetHero(opt.seatid);
                if(!hero)
                {
                    Debug.LogError("找不到这个seatid: "+opt.seatid);
                    continue;
                }
                hero.OnJumpToNextFrame(opt);
            }
            
            OnBulletLogicFrameUpdate();
            
            OnTowerLogicFrameUpdate();
        }

        /// <summary>
        /// 同步之前收到的最后一帧的处理结果
        /// </summary>
        /// <param name="optionEvent"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnAsyncLastLogicFrame(FrameOptionEvent optionEvent)
        {
            //把所有英雄的输入进行数据同步，同步的时间间隔就是帧的时间间隔
            foreach (var opt in optionEvent.opts)
            {
                var hero = GetHero(opt.seatid);
                if(!hero)
                {
                    Debug.LogError("找不到这个seatid: "+opt.seatid);
                    continue;
                }
                hero.OnAsyncLastLogicFrame(opt);

            }
        }

        /// <summary>
        /// 处理最新帧逻辑
        /// </summary>
        /// <param name="frameOptionEvent"></param>
        private void OnHandlerCurrentLogicFrame(FrameOptionEvent optionEvent)
        {
            //把所有英雄输入代入进去，更新数据，进而控制表现
            foreach (var opt in optionEvent.opts)
            {
                var hero = GetHero(opt.seatid);
                if (!hero)
                {
                    Debug.LogError("找不到这个seatid: "+opt.seatid);
                    continue;
                }
                hero.OnHandlerCurrentLogicFrame(opt);
            }
            
            //子弹先走
            OnBulletLogicFrameUpdate();
            
            //塔AI，根据我们处理，进一步处理
            OnTowerLogicFrameUpdate();
        }
        
        /// <summary>
        /// 收集下一帧操作并发送到服务器
        /// </summary>
        private void CollectNextFrameOpts()
        {
            var nfo = new NextFrameOpts
            {
                frameid = this.sync_frameid + 1,
                zoneid = NetInfo.zoneId,
                roomid = NetInfo.roomid,
                seatid = NetInfo.seatid,
            };

            #region 采集操作
            //遥感
            var stick = new OptionEvent
            {
                seatid = NetInfo.seatid,
                optype = (int) OptType.JoyStick,
                x = (int) (this._joystick.TouchDir.x * (1 << 16)),
                y = (int) (this._joystick.TouchDir.y * (1 << 16)),
            };

            nfo.opts.Add(stick);

            //攻击

            //技能


            #endregion

            LogicServiceProxy.Instance.SendNextFrameOpts(nfo);
        }


        private void OnTowerLogicFrameUpdate()
        {
            foreach (var tower in _redTowers)
            {
                tower.OnLogicFrameUpdate(LogicConfig.LogicFrameTime);
            }
            foreach (var tower in _blueTowers)
            {
                tower.OnLogicFrameUpdate(LogicConfig.LogicFrameTime);
            }
        }


        private void OnBulletLogicFrameUpdate()
        {
            for (var i = 0; i < bullets.Count; i++)
            {
                var bullet = bullets[i];
                bullet.OnLogicFrameUpdate(LogicConfig.LogicFrameTime);
            }
        }
        #endregion


        public List<Hero> GetHeros()
        {
            return this.heros;
        }

        public AbstractBullet CreateBullet(SideType side,Type type)
        {
            var isMain = type == typeof(MainTower);
            var prefab = isMain
                ? mainBulletPrefab
                : normalBulletPrefab;
            var obj = Instantiate(prefab);
            obj.transform.SetParent(transform, false);
            var bullet = isMain
                ? (AbstractBullet)obj.AddComponent<MainBullet>()
                : obj.AddComponent<NormalBullet>();
            Assert.IsTrue(bullet);
            bullet.Init(side);
            bullets.Add(bullet);
            return bullet;
        }

        public void RemoveBullet(AbstractBullet bullet)
        {
            if (bullet)
                bullets.Remove(bullet);
            Destroy(bullet.gameObject);
        }
        
        
    }
}
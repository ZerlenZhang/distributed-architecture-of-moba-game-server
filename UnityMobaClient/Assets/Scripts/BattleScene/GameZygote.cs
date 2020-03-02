using System;
using System.Linq;
using gprotocol;
using Moba.Const;
using Moba.Global;
using Moba.Protocol;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using UnityEngine;

namespace Moba.Script
{
    public class GameZygote : UnityEngine.MonoBehaviour
    {
        public GameObject[] characters;
        public Transform playerEntry;
        /// <summary>
        /// 已经同步过的id
        /// </summary>
        private int sync_frame = 1;

        /// <summary>
        /// 上一帧所有用户操作
        /// </summary>
//        private FrameOptionEvent lastOptionEvent;

        private Joystick _joystick;

        private void Start()
        {
            CEventCenter.AddListener<LogicFrame>(Message.OnLogicFrame,OnLogicFrame);
            
            //实例化角色
            var player = Instantiate(characters[NetInfo.usex]);
            player.transform.position = playerEntry.position;
            
            //添加角色控制
            var ctrl = player.AddComponent<CharacterCtrl>();
            ctrl.isGhost = false;
            this._joystick=FindObjectOfType<Joystick>();
            ctrl._joystick = this._joystick;
        }

        private void OnDestroy()
        {
            CEventCenter.RemoveListener<LogicFrame>(Message.OnLogicFrame, OnLogicFrame);
        }

        private void OnLogicFrame(LogicFrame obj)
        {
            if (obj.frameid < this.sync_frame)
                return;

            print("未同步的帧：" + obj.unsync_frames.Count);
            var temp = "";
            foreach (var frameOptionEvent in obj.unsync_frames)
            {
                foreach (var opt in frameOptionEvent.opts)
                {
                    temp+=($"optType: {opt.optype}, stick: [{opt.x},{opt.y}]\n");
                }
            }

            print(temp);
            // 同步前一帧状态 
            //同步客户端上一帧逻辑操作，调整位置，调整后，客户端同步到的就是sync_frame
            
            // 同步收到的帧
            // 从sync_frame 同步到 obj.frameid-1
            
            //获取最后一部操作，根据这一步操作播放动画
            
            //记录下当前帧操作
            this.sync_frame = obj.frameid;
//            this.lastOptionEvent = obj.unsync_frames.Last();
            
            //采集下一帧事件发布给服务器 
            CollectNextFrameOpts();
        }

        private void CollectNextFrameOpts()
        {
            var nfo = new NextFrameOpts
            {
                frameid = this.sync_frame + 1,
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
    }
}
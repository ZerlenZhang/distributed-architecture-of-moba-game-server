using System;
using System.Collections.Generic;
using System.Linq;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Network.Proxy;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public interface IFrameSyncWithSeatId
    {
        int SeatId { get; }
        void SkipPlayerInput(IEnumerable<PlayerInput> inputs);
        void SyncLastPlayerInput(IEnumerable<PlayerInput> inputs);
        void OnHandleCurrentPlayerInput(IEnumerable<PlayerInput> inputs);
    }
    public static class FrameSyncMgr
    {

        #region Public

        /// <summary>
        /// 添加监听者
        /// </summary>
        /// <param name="frameSyncWithSeatId"></param>
        public static void AddListener(IFrameSyncWithSeatId frameSyncWithSeatId)
        {
            if (!ListenerDic.TryGetValue(frameSyncWithSeatId.SeatId, out var listenerList))
            {
                listenerList = new List<IFrameSyncWithSeatId>();
                ListenerDic.Add(frameSyncWithSeatId.SeatId, listenerList);
            }

            if (!listenerList.Contains(frameSyncWithSeatId))
                listenerList.Add(frameSyncWithSeatId);
        }
        /// <summary>
        /// 接收到服务器帧事件的回调
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logicFrameDeltaTime"></param>
        public static void OnFrameSyncTick(LogicFramesToSync msg, int logicFrameDeltaTime)
        {
            //包过时
            if (msg.frameId <= _frameId)
                return;
            
            //同步上一帧处理结果
            if (null != _lastFrameEvent)
                SyncLastFrame(_lastFrameEvent);
            
            //如果现在frameId小于msg.frameId，就快进到最后一帧
            for (var i = 0; i < msg.unsyncFrames.Count-1; i++)
            {
                var logicFrame = msg.unsyncFrames[i];
                SkipLogicFrame(logicFrame);
            }
            
            //更新客户端frameId
            _frameId = msg.frameId;

            //根据最后一帧，控制接下来的显示逻辑
            if (msg.unsyncFrames.Count > 1)
            {
                _lastFrameEvent = msg.unsyncFrames.Last();
                HandleCurrentLogicFrame(_lastFrameEvent);
            }
            else
            {
                _lastFrameEvent = null;
            }
            
            //收集最近输入，发送到服务器
            SendPlayerInput();
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public static void Clear()
        {
            _frameId = 0;
            _lastFrameEvent = null;
            ListenerDic.Clear();
        }

        #endregion

        #region Private

        private static int _frameId = 0;
        public static int FrameId => _frameId;
        private static LogicFrame _lastFrameEvent;

        private static readonly Dictionary<int, List<IFrameSyncWithSeatId>> ListenerDic =
            new Dictionary<int, List<IFrameSyncWithSeatId>>();
        private static void BroadLogicFrame(LogicFrame logicFrame, Action<IFrameSyncWithSeatId,IEnumerable<PlayerInput>> onOperateInputs)
        {
            var groups = logicFrame.inputs
                .GroupBy(input => input.seatId);
            foreach (var seatIdInputs in groups)
            {
                var seatId = seatIdInputs.Key;
                Assert.IsTrue(ListenerDic.ContainsKey(seatId));
                foreach (var listener in ListenerDic[seatId])
                {
                    onOperateInputs(listener, seatIdInputs);
                }
            }
        }

        private static void SendPlayerInput()
        {
            var selfInput = InputMgr.GetInput();
            var inputFrameId = _frameId + 1;
            
            Debug.Log($"SendInput:[{inputFrameId}]({selfInput.moveX},{selfInput.moveY})");
            
            var inputs = new List<PlayerInput>
            {
                selfInput,
            };

            LogicProxy.Instance.SendLogicInput(
                inputFrameId,
                GlobalVar.RoomType,
                GlobalVar.RoomId,
                GlobalVar.SeatId,
                inputs);
            
        }

        private static void HandleCurrentLogicFrame(LogicFrame logicFrame)
        {
            BroadLogicFrame(logicFrame, (
                    listener,
                    inputs)
                    =>listener.OnHandleCurrentPlayerInput(inputs));
        }

        private static void SkipLogicFrame(LogicFrame logicFrame)
        {
            BroadLogicFrame(logicFrame, (
                    listener,
                    inputs)
                =>listener.SkipPlayerInput(inputs));
        }

        private static void SyncLastFrame(LogicFrame logicFrame)
        {
            BroadLogicFrame(logicFrame, (
                    listener,
                    inputs)
                =>listener.SyncLastPlayerInput(inputs));
        }        

        #endregion

    }
}
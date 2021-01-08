using System;
using System.Collections.Generic;
using System.Linq;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 接受帧同步输入的角色需要实现这个接口并加入到监听当中
    /// </summary>
    public interface IFrameSyncCharacter
    {
        /// <summary>
        /// 游戏单位座位号
        /// </summary>
        int SeatId { get; }
        /// <summary>
        /// 跳过某一帧的输入
        /// </summary>
        /// <param name="inputs"></param>
        void SkipCharacterInput(IEnumerable<PlayerInput> inputs);
        /// <summary>
        /// 同步上一帧的输入
        /// </summary>
        /// <param name="inputs"></param>
        void SyncLastCharacterInput(IEnumerable<PlayerInput> inputs);
        /// <summary>
        /// 处理这一帧的输入
        /// </summary>
        /// <param name="inputs"></param>
        void OnHandleCurrentCharacterInput(IEnumerable<PlayerInput> inputs);
    }

    /// <summary>
    /// 随帧同步更新的逻辑单位需要实现这个接口
    /// 比如子弹，自动AI
    /// </summary>
    public interface IFrameSyncUnit
    {
        int InstanceId { get; }
        void OnLogicFrameUpdate(float deltaTime);
    }
    
    public static class FrameSyncMgr
    {
        private static float lastTickTime = 0;
        
        public static void OnFrameSyncStateGUI(GUIStyle defaultGuiStyle)
        {
            GUILayout.Label($"FrameID\t{_frameId}\t",defaultGuiStyle);
            
            var delayFrameCount = FrameId - NewestInputId;
            GUILayout.Label($"DelayFrame\t{delayFrameCount}", defaultGuiStyle);
            
            // GUILayout.Label($"上次逻辑帧间隔\t{Time.timeSinceLevelLoad-lastTickTime}",defaultGuiStyle);
            var delayTime = delayFrameCount * GlobalVar.LogicFrameDeltaTime +
                            (int)((Time.timeSinceLevelLoad - lastTickTime)*1000);
            GUILayout.Label($"Delay\t{delayTime}\tms", defaultGuiStyle);
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
            //debug logic
            lastTickTime = Time.timeSinceLevelLoad;

            if (msg.frameId > _frameId + msg.unsyncFrames.Count)
            {
                throw new Exception($"网络同步异常, msg.frameId[{msg.frameId}] msg.framesCount[{msg.unsyncFrames.Count}] localFrameId[{_frameId}]");
            }

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
            _lastFrameEvent = msg.unsyncFrames.Last();
            HandleCurrentLogicFrame(_lastFrameEvent);
            
            if (_lastFrameEvent.inputs.Count > 0)
            {
                newestInputId = _frameId;
            }

            
            //收集最近输入，发送到服务器
            SendLocalCharacterInput();

            //移除缓存的监听者
            RemoveCacheFrameSyncListeners();
        }
        
        
        
        
        
        
        #region IFrameSyncWithSeatId_监听
        
        /// <summary>
        /// 添加帧同步角色监听者
        /// </summary>
        /// <param name="frameSyncCharacter"></param>
        public static void AddFrameSyncCharacter(IFrameSyncCharacter frameSyncCharacter)
        {
            if (!frameSyncCharacterDic.TryGetValue(frameSyncCharacter.SeatId, out var listenerList))
            {
                listenerList = new List<IFrameSyncCharacter>();
                frameSyncCharacterDic.Add(frameSyncCharacter.SeatId, listenerList);
            }

            if (!listenerList.Contains(frameSyncCharacter))
            {
                listenerList.Add(frameSyncCharacter);
            }
        }

        /// <summary>
        /// 移除帧同步角色监听者
        /// </summary>
        /// <param name="frameSyncCharacter"></param>
        public static void RemoveFrameSyncCharacter(IFrameSyncCharacter frameSyncCharacter)
        {
            _frameSyncCharactersToRemove.Add(frameSyncCharacter.SeatId, frameSyncCharacter);
            // if (frameSyncCharacterDic.TryGetValue(frameSyncCharacter.SeatId, out var characterList))
            // {
            //     characterList.Remove(frameSyncCharacter);
            // }
        }
        
        /// <summary>
        /// 清空帧同步角色监听者
        /// </summary>
        public static void ClearFrameSyncCharacters()
        {
            _frameId = 0;
            _lastFrameEvent = null;
            frameSyncCharacterDic.Clear();
        }
        

        #endregion
        

        #region IFrameSyncUnit_监听

        public static void AddFrameSyncUnit(IFrameSyncUnit frameSyncUnit)
        {
            frameSyncUnitDic.Add(frameSyncUnit.InstanceId, frameSyncUnit);
        }

        public static void RemoveFrameSyncUnit(IFrameSyncUnit frameSyncUnit)
        {
            _frameSyncUnitsToRemove.Add(frameSyncUnit.InstanceId, frameSyncUnit);
        }

        public static void ClearFrameSyncUpdaters()
        {
            frameSyncUnitDic.Clear();
        }
        #endregion
 
        private static int _frameId = 0;
        public static int FrameId => _frameId;

        private static int newestInputId = 0;

        public static int NewestInputId => newestInputId;


        #region Private

        private static LogicFrame _lastFrameEvent;

        #region Private Collections

        private static readonly Dictionary<int, IFrameSyncUnit> frameSyncUnitDic =
            new Dictionary<int, IFrameSyncUnit>();

        private static Dictionary<int, IFrameSyncUnit> _frameSyncUnitsToRemove = new Dictionary<int, IFrameSyncUnit>();
        private static void ForeachFrameSyncUnits(Action<IFrameSyncUnit> onForeach)
        {
            foreach (var id_frameSyncUpdate in frameSyncUnitDic)
            {
                if(_frameSyncUnitsToRemove.ContainsKey(id_frameSyncUpdate.Key))
                    continue;
                onForeach?.Invoke(id_frameSyncUpdate.Value);
            }
        }
        
        private static readonly Dictionary<int, List<IFrameSyncCharacter>> frameSyncCharacterDic =
            new Dictionary<int, List<IFrameSyncCharacter>>();
        private static Dictionary<int, IFrameSyncCharacter> _frameSyncCharactersToRemove =
            new Dictionary<int, IFrameSyncCharacter>();        

        #endregion
        
        /// <summary>
        /// 广播逻辑帧给每个IFrameSyncCharacter
        /// </summary>
        /// <param name="logicFrame"></param>
        /// <param name="onOperateInputs"></param>
        private static void BroadLogicFrame(LogicFrame logicFrame, Action<IFrameSyncCharacter,IEnumerable<PlayerInput>> onOperateInputs)
        {
            var groups = logicFrame.inputs
                .GroupBy(input => input.seatId);
            foreach (var seatIdInputs in groups)
            {
                var seatId = seatIdInputs.Key;
                Assert.IsTrue(frameSyncCharacterDic.ContainsKey(seatId));
                foreach (var listener in frameSyncCharacterDic[seatId])
                {
                    if(_frameSyncCharactersToRemove.ContainsKey(listener.SeatId))
                        continue;
                    onOperateInputs(listener, seatIdInputs);
                }
            }
        }

        /// <summary>
        /// 发送本地玩家输入
        /// </summary>
        private static void SendLocalCharacterInput()
        {
            var selfInput = InputMgr.GetInput();
            
            var inputs = new List<PlayerInput> 
            {
                selfInput,
            };

            for(var i=0;i<GameSettings.Instance.NetMsgTimes;i++)
                LogicProxy.Instance.SendLogicInput(
                    _frameId + 1,
                    GlobalVar.RoomType,
                    GlobalVar.RoomId,
                    GlobalVar.SeatId,
                    inputs);
            
        }
        
        /// <summary>
        /// 处理这一帧的输入
        /// </summary>
        /// <param name="logicFrame"></param>
        private static void HandleCurrentLogicFrame(LogicFrame logicFrame)
        {
            BroadLogicFrame(logicFrame, (
                    character,
                    inputs)
                    =>character.OnHandleCurrentCharacterInput(inputs));
            
            ForeachFrameSyncUnits( 
                unit => 
                    unit.OnLogicFrameUpdate(GlobalVar.LogicFrameDeltaTime.ToFloat()));
        }

        /// <summary>
        /// 跳过某一帧的输入
        /// </summary>
        /// <param name="logicFrame"></param>
        private static void SkipLogicFrame(LogicFrame logicFrame)
        {
            BroadLogicFrame(logicFrame, (
                    character,
                    inputs)
                =>character.SkipCharacterInput(inputs));
            ForeachFrameSyncUnits( 
                unit => 
                    unit.OnLogicFrameUpdate(GlobalVar.LogicFrameDeltaTime.ToFloat()));
        }
        
        /// <summary>
        /// 同步上一帧的输入
        /// </summary>
        /// <param name="logicFrame"></param>
        private static void SyncLastFrame(LogicFrame logicFrame)
        {
            BroadLogicFrame(logicFrame, (
                    character,
                    inputs)
                =>character.SyncLastCharacterInput(inputs));
        }

        /// <summary>
        /// 移除缓冲区里里的帧消息监听者
        /// </summary>
        private static void RemoveCacheFrameSyncListeners()
        {
            foreach (var id_listener in _frameSyncCharactersToRemove)
            {
                frameSyncCharacterDic.Remove(id_listener.Key);
            }
            _frameSyncCharactersToRemove.Clear();
            foreach (var id_unit in _frameSyncUnitsToRemove)
            {
                frameSyncUnitDic.Remove(id_unit.Key);
            }
            _frameSyncUnitsToRemove.Clear();
        }
        
        #endregion
    }
}
using System;
using System.Collections.Generic;
using ReadyGamerOne.Script;
using UnityEngine;

namespace ReadyGamerOne.Utility
{
    /// <summary>
    /// 输入工具类——注意，使用此类，需要将MainLoop放在所有脚本执行顺序之前
    /// 封装双击，长按方法
    /// </summary>
    public static class InputUtil
    {
        #region Private

        private enum KeyState
        {
            Null,   //持续状态    无输入
            Down_1, //单帧状态    第一次按下
            Down_,  //持续状态    第一次按下不松开
            Up_1,   //单帧状态    第一次松开
            Up_,    //持续状态    第一次按下并松开
            Down_2, //单帧状态    第二次按下
            Down_Down_, //持续状态    两次按下，第二下不松开
            Up_2,   //单帧状态    第二次松开
            Up_Up_  //持续状态    第二次松开
        }

        private class KeyInfo
        {
            public KeyState State;
            public float timeBetweenLastTwoClick;
            public float timeAfterLastUp;
            public float timeAfterLastDown;
            public float preTimeAfterLastDown;
        }

        static InputUtil()
        {
            MainLoop.Instance.AddUpdateFunc(FixedUpdate);
            keyDic.Add(KeyCode.U, new KeyInfo());
        }

        private static Dictionary<KeyCode, KeyInfo> keyDic = new Dictionary<KeyCode, KeyInfo>();
        private static Dictionary<int, KeyInfo> mouseDic = new Dictionary<int, KeyInfo>();

        private static void FixedUpdate()
        {
            foreach (var item in keyDic)
            {
                #region 处理状态变化

                #region 主动输入导致的状态变化

                if (Input.GetKeyDown(item.Key))
                {
                    switch (item.Value.State)
                    {
                        case KeyState.Null:
                            item.Value.State = KeyState.Down_1;
                            break;
                        case KeyState.Up_:
                        case KeyState.Up_1:
                        case KeyState.Up_2:
                        case KeyState.Up_Up_:
                            item.Value.State = KeyState.Down_2;
                            break;
                    }
                }
                else if (Input.GetKeyUp(item.Key))
                {
                    switch (item.Value.State)
                    {
                        case KeyState.Null:
                            throw new Exception("不该出现这个情况，异常按键：" + item.Key);
                        case KeyState.Down_:
                        case KeyState.Down_1:
                            item.Value.State = KeyState.Up_1;
                            break;
                        case KeyState.Down_2:
                        case KeyState.Down_Down_:
                            item.Value.State = KeyState.Up_2;
                            break;
                    }
                }

                #endregion

                #region 没有输入时，单帧状态切换到持续状态

                else
                {
                    switch (item.Value.State)
                    {
                        case KeyState.Down_1:
                            item.Value.State = KeyState.Down_;
                            break;
                        case KeyState.Down_2:
                            item.Value.State = KeyState.Down_Down_;
                            break;
                        case KeyState.Up_1:
                            item.Value.State = KeyState.Up_;
                            break;
                        case KeyState.Up_2:
                            item.Value.State = KeyState.Up_Up_;
                            break;
                    }
                }

                #endregion

                #endregion

                #region 执行本状态的逻辑

                switch (item.Value.State)
                {
                    case KeyState.Null:
                        item.Value.timeAfterLastUp = 0;
                        item.Value.timeBetweenLastTwoClick = 0;
                        break;
                    case KeyState.Down_1:
                        item.Value.preTimeAfterLastDown = item.Value.timeAfterLastDown;
                        item.Value.timeAfterLastDown = 0;
                        break;
                    case KeyState.Down_2:
                        item.Value.preTimeAfterLastDown = item.Value.timeAfterLastDown;
                        item.Value.timeAfterLastDown = 0;
                        item.Value.timeBetweenLastTwoClick = item.Value.timeAfterLastUp;
                        break;
                    case KeyState.Up_1:
                    case KeyState.Up_2:
                        item.Value.timeAfterLastUp = 0;
                        break;
                        
                    case KeyState.Up_:
                    case KeyState.Up_Up_:
                    case KeyState.Down_:
                    case KeyState.Down_Down_:
                        item.Value.timeAfterLastUp += Time.deltaTime;
                        item.Value.timeAfterLastDown += Time.deltaTime;
                        break;
                }

                #endregion
            }
                        
            foreach (var item in mouseDic)
            {
                #region 处理状态变化

                #region 主动输入导致的状态变化

                if (Input.GetMouseButtonDown(item.Key))
                {
                    switch (item.Value.State)
                    {
                        case KeyState.Null:
                            item.Value.State = KeyState.Down_1;
                            break;
                        case KeyState.Up_:
                        case KeyState.Up_1:
                        case KeyState.Up_2:
                        case KeyState.Up_Up_:
                            item.Value.State = KeyState.Down_2;
                            break;
                    }
                }
                else if (Input.GetMouseButtonUp(item.Key))
                {
                    switch (item.Value.State)
                    {
                        case KeyState.Null:
                            throw new Exception("不该出现这个情况，异常按键：" + item.Key);
                        case KeyState.Down_:
                        case KeyState.Down_1:
                            item.Value.State = KeyState.Up_1;
                            break;
                        case KeyState.Down_2:
                        case KeyState.Down_Down_:
                            item.Value.State = KeyState.Up_2;
                            break;
                    }
                }

                #endregion

                #region 没有输入时，单帧状态切换到持续状态

                else
                {
                    switch (item.Value.State)
                    {
                        case KeyState.Down_1:
                            item.Value.State = KeyState.Down_;
                            break;
                        case KeyState.Down_2:
                            item.Value.State = KeyState.Down_Down_;
                            break;
                        case KeyState.Up_1:
                            item.Value.State = KeyState.Up_;
                            break;
                        case KeyState.Up_2:
                            item.Value.State = KeyState.Up_Up_;
                            break;
                    }
                }

                #endregion

                #endregion

                #region 执行本状态的逻辑

                switch (item.Value.State)
                {
                    case KeyState.Null:
                        item.Value.timeAfterLastUp = 0;
                        item.Value.timeBetweenLastTwoClick = 0;
                        break;
                    case KeyState.Down_1:
                        item.Value.preTimeAfterLastDown = item.Value.timeAfterLastDown;
                        item.Value.timeAfterLastDown = 0;
                        break;
                    case KeyState.Down_2:
                        item.Value.preTimeAfterLastDown = item.Value.timeAfterLastDown;
                        item.Value.timeAfterLastDown = 0;
                        item.Value.timeBetweenLastTwoClick = item.Value.timeAfterLastUp;
                        break;
                    case KeyState.Up_1:
                    case KeyState.Up_2:
                        item.Value.timeAfterLastUp = 0;
                        break;
                        
                    case KeyState.Up_:
                    case KeyState.Up_Up_:
                    case KeyState.Down_:
                    case KeyState.Down_Down_:
                        item.Value.timeAfterLastUp += Time.deltaTime;
                        item.Value.timeAfterLastDown += Time.deltaTime;
                        break;
                }

                #endregion
            }
        }

        #endregion

        /// <summary>
        /// 是否双击按下某个键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <returns></returns>
        public static bool GetDoubleKeyDown(KeyCode key, float timeBetweenLastTwoClick)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];
            var ans = info.State == KeyState.Down_2 && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick;
            return ans;
        }

        /// <summary>
        /// 是否双击按下某个鼠标键
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <returns></returns>
        public static bool GetMouseDoubleDown(int mouseButton, float timeBetweenLastTwoClick)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            var ans = info.State == KeyState.Down_2 && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick;
            return ans;
        }

        /// <summary>
        /// 是否双击松开某个键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <returns></returns>
        public static bool GetDoubleKeyUp(KeyCode key, float timeBetweenLastTwoClick)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];
            return info.State == KeyState.Up_2 && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick;
        }

        /// <summary>
        /// 获取鼠标双击抬起
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <returns></returns>
        public static bool GetMouseDoubleUp(int mouseButton, float timeBetweenLastTwoClick)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            
            return info.State == KeyState.Up_2 && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick;
        }

        /// <summary>
        /// 是否双击按住某个按键，并且距离最后一次按下时间间隔不足timeAfterLastDown
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <param name="timeAfterLastDown"></param>
        /// <returns></returns>
        public static bool GetDoubleKey(KeyCode key, float timeBetweenLastTwoClick, float timeAfterLastDown)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];

            var ans = info.State == KeyState.Down_Down_ && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick 
                && info.timeAfterLastDown<=timeAfterLastDown;
            
            return ans;
        }

        /// <summary>
        /// 是否双击按住某个鼠标键，并且距离最后一次按下时间间隔不足timeAfterLastDown
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <param name="timeAfterLastDown"></param>
        /// <returns></returns>
        public static bool GetDoubleMouse(int mouseButton, float timeBetweenLastTwoClick, float timeAfterLastDown)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            var ans = info.State == KeyState.Down_Down_ && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick 
                                                        && info.timeAfterLastDown<=timeAfterLastDown;
            
            return ans;
        }
        

        /// <summary>
        /// 是否双击按住某个键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <returns></returns>
        public static bool GetDoubleKey(KeyCode key, float timeBetweenLastTwoClick)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];

            var ans = info.State == KeyState.Down_Down_ && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick;

            return ans;
        }

        /// <summary>
        /// 是否双击按住某个鼠标键
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="timeBetweenLastTwoClick"></param>
        /// <returns></returns>
        public static bool GetDoubleMouse(int mouseButton, float timeBetweenLastTwoClick)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            var ans = info.State == KeyState.Down_Down_ && info.timeBetweenLastTwoClick <= timeBetweenLastTwoClick;

            return ans;
        }

        /// <summary>
        /// 获取按键长按，只触发一次
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool GetLongKeyDown(KeyCode key, float time)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];
            return Math.Abs(info.timeAfterLastDown - time) < 0.01f &&
                (info.State == KeyState.Down_ || info.State == KeyState.Down_Down_);
        }

        /// <summary>
        /// 获取鼠标长按，只触发一次
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool GetLongMouseDown(int mouseButton, float time)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            
            return Math.Abs(info.timeAfterLastDown - time) < 0.01f &&
                   (info.State == KeyState.Down_ || info.State == KeyState.Down_Down_);
        }


        /// <summary>
        /// 获取按键长按以后的抬起，只触发一次
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time">长按时长</param>
        /// <returns></returns>
        public static bool GetLongKeyUp(KeyCode key, ref float time)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }
            var info = keyDic[key];

            if (info.State != KeyState.Up_1 && info.State != KeyState.Up_2) return false;
            
            time = info.timeAfterLastDown;
            return true;

        }

        /// <summary>
        /// 获取鼠标长按以后的抬起，只触发一次
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time">长按时长</param>
        /// <returns></returns>
        public static bool GetLongMouseUp(int mouseButton, ref float time)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];

            if (info.State != KeyState.Up_1 && info.State != KeyState.Up_2)
                return false;

            time = info.timeAfterLastDown;
            return true;
        }
        

        /// <summary>
        /// 获取按键长按，时间超过time后，持续触发
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool GetLongKey(KeyCode key, float time)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];
            return info.timeAfterLastDown >= time &&
                   (info.State == KeyState.Down_ || info.State == KeyState.Down_Down_);
        }

        /// <summary>
        /// 获取按键长按，持续触发
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time">从按下到现在的时间</param>
        /// <returns></returns>
        public static bool GetLongKey(KeyCode key, ref float time)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];
            if (info.State == KeyState.Down_ || info.State == KeyState.Down_Down_)
            {
                time = info.timeAfterLastDown;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取鼠标长按，持续触发
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool GetLongMouse(int mouseButton, float time)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            return info.timeAfterLastDown >= time &&
                   (info.State == KeyState.Down_ || info.State == KeyState.Down_Down_);
        }

        /// <summary>
        /// 获取鼠标长按，持续触发
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time">从按下到现在的时间</param>
        /// <returns></returns>
        public static bool GetLongMouse(int mouseButton, ref float time)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            if (info.State == KeyState.Down_ || info.State == KeyState.Down_Down_)
            {
                time = info.timeAfterLastDown;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取按键频繁按下
        /// </summary>
        /// <param name="key"></param>
        /// <param name="maxIntervalTime">两次按下不得超过的最大允许间隔</param>
        /// <returns></returns>
        public static bool GetFrequentKey(KeyCode key, float maxIntervalTime)
        {
            if (!keyDic.ContainsKey(key))
            {
                keyDic.Add(key, new KeyInfo());
                return false;
            }

            var info = keyDic[key];
            return Math.Abs(info.preTimeAfterLastDown) > 0.01f && info.timeAfterLastDown <= maxIntervalTime &&
                   info.preTimeAfterLastDown <= maxIntervalTime;
        }

        /// <summary>
        /// 获取鼠标键频繁按下
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <param name="maxIntervalTime">两次按下不得超过的最大允许间隔</param>
        /// <returns></returns>
        public static bool GetFrequentMouse(int mouseButton, float maxIntervalTime)
        {
            if (!mouseDic.ContainsKey(mouseButton))
            {
                mouseDic.Add(mouseButton, new KeyInfo());
                return false;
            }

            var info = mouseDic[mouseButton];
            return Math.Abs(info.preTimeAfterLastDown) > 0.01f && info.timeAfterLastDown <= maxIntervalTime &&
                   info.preTimeAfterLastDown <= maxIntervalTime;
        }



        private static void ModifyKeyInfo(KeyCode key, KeyInfo info)
        {
            if (Input.GetKeyUp(key))
            {
                switch (info.State)
                {
                    case KeyState.Null:
                        throw new Exception("不该出现这个情况，异常按键：" + key);
                    case KeyState.Down_:
                    case KeyState.Down_1:
                        info.State = KeyState.Up_1;
                        break;
                    case KeyState.Down_2:
                    case KeyState.Down_Down_:
                        info.State = KeyState.Up_2;
                        break;
                }
            }
        }
        
    }
}
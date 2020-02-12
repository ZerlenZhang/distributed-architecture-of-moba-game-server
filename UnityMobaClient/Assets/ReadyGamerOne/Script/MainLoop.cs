using System;
using System.Collections;
using System.Collections.Generic;
using ReadyGamerOne.Common;
using UnityEngine;

namespace ReadyGamerOne.Script
{
    /// <summary>
    /// 主循环basi
    /// 在外部调用，实现脱离MonoBehavior进行更新
    /// 封装多种协程便于调用
    /// </summary>
    public sealed class MainLoop : MonoSingleton<MainLoop>
    {
        #region Static

        #region PrivateFields

        private static event Action updateEvent;
        private static event Action fixedUpdateEvent;
        private static event Action guiEvent;
        private static event Action startEvent;

        private static List<UpdateTestPair> callBackPairs = new List<UpdateTestPair>();
        

        #endregion

        #region Method

        #region StartHelper

        public void AddStartFunc(Action func)
        {
            startEvent += func;
        }

        public void RemoveStartFunc(Action func)
        {
            startEvent -= func;
        }

        #endregion


        #region UpdateHelper

        public void AddUpdateTest(UpdateTestPair pair)
        {
            if (callBackPairs.Contains(pair))
                Debug.Log("注意！已经包含这个UpdateTestPair！");
            callBackPairs.Add(pair);
        }

        public void RemoveUpdateTest(UpdateTestPair pair)
        {
            callBackPairs.Remove(pair);
        }


        public  void AddUpdateFunc(Action func)
        {
            updateEvent += func;
        }

        public void RemoveUpdateFunc(Action func)
        {
            updateEvent -= func;
        }

        #endregion


        #region FixedUpdateHelper

        public void AddFixedUpdateFunc(Action func)
        {
            fixedUpdateEvent += func;
        }

        public void RemoveFixedUpdateFunc(Action func)
        {
            fixedUpdateEvent -= func;
        }

        #endregion


        #region GUIHelper

        public void AddGUIFunc(Action func)
        {
            guiEvent += func;
        }

        public void RemoveGUIFunc(Action func)
        {
            guiEvent -= func;
        }

        #endregion


        /// <summary>
        /// 清空所有辅助调用的事件
        /// </summary>
        public void Clear()
        {
            updateEvent = null;
            startEvent = null;
            fixedUpdateEvent = null;
            guiEvent = null;
        }

        #endregion
        
        #endregion

        public class UpdateTestPair
        {
            public Func<bool> IsOk;
            public Action func;

            public UpdateTestPair(Func<bool> isok, Action func)
            {
                this.IsOk = isok;
                this.func = func;
            }
        }


        #region 协程（延时调用，间隔调用，一段时间内每帧调用）

        #region 开始关闭协程

        /// <summary>
        /// 开始关闭协程
        /// </summary>
        /// <param CharacterName="Coroutine"></param>
        /// <returns></returns>
        public Coroutine StartCoroutines(IEnumerator Coroutine)
        {
            return StartCoroutine(Coroutine);
        }

        public void StopCoroutines(Coroutine Coroutine)
        {
            StopCoroutine(Coroutine);
        }

        #endregion


        #region 运行直到为真

        /// <summary>
        /// 运行直到为真
        /// </summary>
        /// <param CharacterName="method"></param>
        /// <param CharacterName="endCall"></param>
        /// <returns></returns>
        public Coroutine ExecuteUntilTrue(Func<bool> method, Action endCall = null)
        {
            return StartCoroutine(_ExecuteUntilTrue(method, endCall));
        }

        #endregion


        #region 延时调用

        /// <summary>
        /// 延时调用
        /// </summary>
        /// <param CharacterName="method"></param>          c
        /// <param CharacterName="seconds"></param>
        public Coroutine ExecuteLater(Action method, float seconds)
        {
            return StartCoroutine(_ExecuteLater(method, seconds));
        }

        public Coroutine ExecuteLater<T>(Action<T> method, float seconds, T args)
        {
            return StartCoroutine(_ExecuteLater_T<T>(method, seconds, args));
        }

        #endregion


        #region 间隔调用

        /// <summary>
        /// 间隔调用
        /// </summary>
        /// <param CharacterName="method"></param>
        /// <param CharacterName="times"></param>
        /// <param CharacterName="duringTime"></param>
        public Coroutine ExecuteEverySeconds(Action method, int times, float duringTime)
        {
            return StartCoroutine(_ExecuteSeconds(method, times, duringTime));
        }

        public Coroutine ExecuteEverySeconds<T>(Action<T> method, float times, float duringTime, T args)
        {
            return StartCoroutine(_ExecuteSeconds_T(method, times, duringTime, args));
        }

        public Coroutine ExecuteEverySeconds(Action method, float times, float duringTime, Action endCall)
        {
            return StartCoroutine(_ExecuteSeconds_Action(method, times, duringTime, endCall));
        }

        public Coroutine ExecuteEverySeconds<T>(Action<T> method, float times, float duringTime, T args,
            Action<T> endCall)
        {
            return StartCoroutine(_ExecuteSeconds_Action_T(method, times, duringTime, args, endCall));
        }

        #endregion


        #region 一段时间内每帧调用

        /// <summary>
        /// 一段时间内每帧调用
        /// </summary>
        /// <param CharacterName="method"></param>
        /// <param CharacterName="seconds"></param>
        public Coroutine UpdateForSeconds(Action method, float seconds, Action endCall)
        {
            return StartCoroutine(_UpdateForSeconds_Action(method, seconds, endCall));
        }

        public Coroutine UpdateForSeconds<T>(Action<T> method, float seconds, T arg, Action<T> endCall)
        {
            return StartCoroutine(_UpdateForSeconds_Action_T(method, seconds, arg, endCall));
        }

        public Coroutine UpdateForSeconds(Action method, float seconds, float delay = 0f)
        {
            return StartCoroutine(_UpdateForSeconds(method, seconds, delay));
        }

        public Coroutine UpdateForSeconds<T>(Action<T> method, float seconds, T args, float delay = 0f)
        {
            return StartCoroutine(_UpdateForSeconds_T(method, seconds, args, delay));
        }

        #endregion


        #region 内部调用

        private IEnumerator _ExecuteLater(Action mathdem, float time)
        {
            yield return new WaitForSeconds(time);
            mathdem();
        }

        private IEnumerator _ExecuteLater_T<T>(Action<T> mathdom, float time, T args)
        {
            yield return new WaitForSeconds(time);
            mathdom(args);
        }

        private IEnumerator _ExecuteSeconds(Action mathdom, int times, float duringTime)
        {
            if (times <= 0)
            {
                while (true)
                {
                    yield return new WaitForSeconds(duringTime);
                    mathdom?.Invoke();
                }
            }
            else
            {
                for (int i = 0; i < times; i++)
                {
                    for (var timer = 0f; timer < duringTime; timer += Time.deltaTime)
                        yield return 0;
                    mathdom();
                }                
            }
            

        }

        private IEnumerator _ExecuteSeconds_T<T>(Action<T> mathdom, float times, float duringTime, T args)
        {
            for (int i = 0; i < times; i++)
            {
                for (var timer = 0f; timer < duringTime; timer += Time.deltaTime)
                    yield return 0;
                mathdom(args);
            }
        }

        private IEnumerator _ExecuteSeconds_Action(Action method, float times, float dur, Action endCall)
        {
            for (int i = 0; i < times; i++)
            {
                for (var timer = 0f; timer < dur; timer += Time.deltaTime)
                    yield return 0;
                method();
            }

            endCall();
        }

        private IEnumerator _ExecuteSeconds_Action_T<T>(Action<T> method, float times, float dur, T args,
            Action<T> endCall)
        {
            for (int i = 0; i < times; i++)
            {
                for (var timer = 0f; timer < dur; timer += Time.deltaTime)
                    yield return 0;
                method(args);
            }

            endCall(args);
        }

        private IEnumerator _UpdateForSeconds(Action mathdom, float seconds, float start)
        {
            for (var d = 0f; d < start; d += Time.deltaTime)
                yield return 0;
            for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
            {
                yield return 0;
                mathdom();
            }
        }

        private IEnumerator _UpdateForSeconds_T<T>(Action<T> mathdom, float seconds, T args, float start)
        {
            for (var d = 0f; d < start; d += Time.deltaTime)
                yield return 0;
            for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
            {
                yield return 0;
                mathdom(args);
            }
        }

        private IEnumerator _UpdateForSeconds_Action(Action method, float time, Action endcall)
        {
            for (var timer = 0f; timer < time; timer += Time.deltaTime)
            {
                yield return 0;
                method();
            }

            yield return 0;
            endcall();
        }


        private IEnumerator _UpdateForSeconds_Action_T<T>(Action<T> method, float seconds, T arg, Action<T> endCall)
        {
            for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
            {
                yield return 0;
                method(arg);
            }

            yield return 0;
            endCall(arg);
        }


        private IEnumerator _ExecuteUntilTrue(Func<bool> method, Action endCall)
        {
            while (true)
            {
                if (method())
                {
                    endCall?.Invoke();
                    break;
                }

                yield return 0;
            }
        }

        #endregion

        #endregion


        #region MonoBehavior

        void Start()
        {
            startEvent?.Invoke();
        }

        void Update()
        {
            updateEvent?.Invoke();
            foreach (var pair in callBackPairs)
            {
                if (pair.IsOk())
                    pair.func();
            }
        }

        private void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke();
        }

        void OnGUI()
        {
            guiEvent?.Invoke();
        }

        #endregion


        private void OnDestroy()
        {
            Debug.LogWarning("MainLoop_OnDestory函数被调用！");
        }
    }
}
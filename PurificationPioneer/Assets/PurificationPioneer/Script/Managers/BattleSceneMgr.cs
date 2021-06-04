using System;
using System.Collections;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.Script
{
    public class BattleSceneMgr:PpSceneMgr<BattleSceneMgr>
    {
        public Transform leftPoint;
        public Transform rightPoint;
        public float generateRadius = 3;
        public bool lookMouse = true;
        protected override void Start()
        {
            base.Start();
            
            LogicProxy.Instance.StartGameReq(GlobalVar.Uname);
            
            CEventCenter.AddListener<int>(Message.OnGameStart,OnStartGameSoon);

#if UNITY_EDITOR
            if (GameSettings.Instance.WorkAsAndroid)
            {
                PanelMgr.PushPanel(PanelName.AndroidBattlePanel);
            }
            else
            {                
                if (lookMouse)
                {
                    UnityAPI.LockMouse();
                }
                PanelMgr.PushPanel(PanelName.BattlePanel);
            }
#elif UNITY_ANDROID
            PanelMgr.PushPanel(PanelName.AndroidBattlePanel);
#elif UNITY_STANDALONE_WIN
            PanelMgr.PushPanel(PanelName.BattlePanel);
            if(lookMouse)
                    UnityAPI.LockMouse();
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            PpPhysics.Clear();
            FrameSyncMgr.Clear();
#if UNITY_EDITOR
            if (!GameSettings.Instance.WorkAsAndroid)
            {
                if (lookMouse)
                {
                    UnityAPI.FreeMouse();
                }
            }

#elif UNITY_STANDALONE_WIN
            if(lookMouse)
                    UnityAPI.FreeMouse();
#endif
            CEventCenter.RemoveListener<int>(Message.OnGameStart,OnStartGameSoon);
        }

        /// <summary>
        /// delay秒后游戏开始，此函数用于加载角色
        /// </summary>
        /// <param name="delay"></param>
        private void OnStartGameSoon(int delay)
        {
            StartCoroutine(GenerateCharacters(delay));
        }

        private IEnumerator GenerateCharacters(float delay)
        {
            var times = Mathf.Max(GlobalVar.SeatId_MatcherInfo.Count / 2,1);
            var deltaTime = delay / (1 + times);
            
            var deltaDegree = 360f / times;
            foreach (var kv in GlobalVar.SeatId_MatcherInfo)
            {
                var matcherInfo = kv.Value;
                
                var genPoint = kv.Key % 2 == 1
                    ? leftPoint
                    : rightPoint;
                
                var lookAtPoint = kv.Key % 2 == 0
                    ? leftPoint
                    : rightPoint;

                //get config
                var characterConfig=ResourceMgr.GetAsset<HeroConfigAsset>(
                    AssetConstUtil.GetHeroConfigKey(matcherInfo.HeroId));
                Assert.IsTrue(characterConfig);
                
                //get position
                var generatePos = genPoint.position + new Vector3(
                    this.generateRadius * Mathf.Cos(kv.Key * deltaDegree),
                    0,
                    this.generateRadius * Mathf.Sin(kv.Key * deltaDegree));

                //实例化，初始化
                var player = characterConfig.InstantiateAndInitialize(kv.Key, generatePos);
                player.transform.LookAt(lookAtPoint);
                
                //delay
                if(genPoint==rightPoint)
                    yield return new WaitForSeconds(deltaTime);
            }
        }


        private void OnDrawGizmos()
        {
            if (leftPoint)
            {
                Gizmos.color=Color.blue;
                Gizmos.DrawWireSphere(leftPoint.position, generateRadius);
            }

            if (rightPoint)
            {
                Gizmos.color=Color.red;
                Gizmos.DrawWireSphere(rightPoint.position, generateRadius);
            }
        }
    }
}
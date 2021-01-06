using System.Collections;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Utility;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
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
#if UNITY_ANDROID && !UNITY_EDITOR
            PanelMgr.PushPanel(PanelName.AndroidBattlePanel);
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
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

                //instantiate go
                var characterObj = ResourceMgr.InstantiateGameObject(
                    AssetConstUtil.GetHeroGameObjectKey(matcherInfo.HeroId));
                var ppController = characterObj.GetComponent<IPpController>();

                Assert.IsNotNull(ppController);
                
                //get position
                var generatePos = genPoint.position + new Vector3(
                    this.generateRadius * Mathf.Cos(kv.Key * deltaDegree),
                    0,
                    this.generateRadius * Mathf.Sin(kv.Key * deltaDegree));

                //init settings
                characterObj.transform.position = generatePos;
                ppController.InitCharacterController(kv.Key, generatePos);
                
                //delay
                if(genPoint==rightPoint)
                    yield return new WaitForSeconds(deltaTime);
            }

            // Cursor.visible = false;
            if(lookMouse)
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
using System;
using PurificationPioneer.Const;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class AndroidBattlePanelScript : MonoBehaviour
    {
        public Joystick joystick;
        public AndroidBattleHeroStateUi heroStateUi;
        public AndroidBattleAttackBtnAreaUi heroAttackBtnUi;
        public BattleOptionUi battleOptionUi;
        public Text timeText;

        public void Init()
        {
            heroStateUi.Init();
            heroAttackBtnUi.Init();
            battleOptionUi.SetVisible(false);
        }
        
        
        private void OnEnable()
        {
            CEventCenter.AddListener<int>(Message.OnTimeLosing, OnTimeLosing);
        }

        private void OnDisable()
        {
            CEventCenter.RemoveListener<int>(Message.OnTimeLosing, OnTimeLosing);
        }


        private void OnTimeLosing(int leftSeconds)
        {
            timeText.text = $"{leftSeconds / 60}:{leftSeconds % 60}";
        }

#if DebugMode
        private void Update()
        {
            if (GameSettings.Instance.WorkAsAndroid)
            {
                if (Input.GetKeyDown(GameSettings.Instance.BattleOptionKey))
                {
                    ShowOptions();
                }
            }
        }
#endif
        
        
        public void ShowOptions()
        {
            battleOptionUi.SetVisible(true);
        }
    }
}
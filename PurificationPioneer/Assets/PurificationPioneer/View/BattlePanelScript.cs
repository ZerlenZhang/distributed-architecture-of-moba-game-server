using System;
using PurificationPioneer.Const;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class BattlePanelScript : MonoBehaviour
    {
        public BattlePanelBottomAreaUi bottomAreaUi;
        public BattleOptionUi battleOptionUi;
        public Text timeText;
        
        public void Init()
        {
            bottomAreaUi.Init();
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


        private void Update()
        {
            if (Input.GetKeyDown(GameSettings.Instance.BattleOptionKey))
            {
                ShowOptions();
            }
        }

        public void ShowOptions()
        {
            battleOptionUi.SetVisible(true);
        }
    }
}
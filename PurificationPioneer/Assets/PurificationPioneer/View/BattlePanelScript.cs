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
        public WarInfoAreaUi m_WarInfoAreaUi;
        
        public void Init()
        {
            bottomAreaUi.Init();
            battleOptionUi.SetVisible(false);
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
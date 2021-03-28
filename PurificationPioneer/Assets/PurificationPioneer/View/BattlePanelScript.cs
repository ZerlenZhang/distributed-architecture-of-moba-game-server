using System;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using UnityEngine;

namespace PurificationPioneer.View
{
    public class BattlePanelScript : MonoBehaviour
    {
        public BattlePanelBottomAreaUi bottomAreaUi;
        public BattleOptionUi battleOptionUi;
        
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
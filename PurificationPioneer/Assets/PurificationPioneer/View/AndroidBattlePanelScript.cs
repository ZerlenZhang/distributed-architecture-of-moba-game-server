using PurificationPioneer.Script;
using ReadyGamerOne.Script;
using UnityEngine;

namespace PurificationPioneer.View
{
    public class AndroidBattlePanelScript : MonoBehaviour
    {
        public Joystick joystick;
        public AndroidBattleHeroStateUi heroStateUi;
        public AndroidBattleAttackBtnAreaUi heroAttackBtnUi;
        public BattleOptionUi battleOptionUi;

        public void Init()
        {
            heroStateUi.Init();
            heroAttackBtnUi.Init();
            battleOptionUi.SetVisible(false);
        }
        
        public void ShowOptions()
        {
            battleOptionUi.SetVisible(true);
        }
    }
}
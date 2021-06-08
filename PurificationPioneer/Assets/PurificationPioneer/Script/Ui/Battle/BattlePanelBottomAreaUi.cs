using PurificationPioneer.Global;
using ReadyGamerOne.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class BattlePanelBottomAreaUi : MonoBehaviour
    {
        public Image heroIcon;
        public SuperBloodBar bloodBar;

        public void Init()
        {
            var localHeroConfig = GlobalVar.LocalHeroConfig;

            heroIcon.sprite = localHeroConfig.icon;
            bloodBar.Value = 1;
        }
    }
}
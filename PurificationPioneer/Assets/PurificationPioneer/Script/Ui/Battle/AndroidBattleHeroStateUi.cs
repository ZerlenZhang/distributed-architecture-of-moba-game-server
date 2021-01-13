using PurificationPioneer.Global;
using ReadyGamerOne.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class AndroidBattleHeroStateUi : MonoBehaviour
    {
        public Image heroIcon;
        public Text heroNameText;
        public Text heroLevelText;
        public SuperBloodBar bloodBar;

        public void Init()
        {
            var heroConfig = GlobalVar.LocalHeroConfig;
            heroIcon.sprite = heroConfig.icon;
            heroNameText.text = heroConfig.heroName;
            heroLevelText.text = 1.ToString();
            bloodBar.Value = 1;
        }
    }
}
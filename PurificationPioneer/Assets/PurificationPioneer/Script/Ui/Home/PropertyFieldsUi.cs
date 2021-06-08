using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class PropertyFieldsUi : MonoBehaviour
    {
        public Text hpText;
        public Text defenceText;
        public Text moveSpeedText;
        public Text attackSpeedText;
        public Text attackText;
        public Text paintRateText;
        public Text trustValueText;

        public void Init(HeroConfigAsset config)
        {
            hpText.text = $"{config.baseHp}";
            defenceText.text = $"{config.baseDefence}";
            moveSpeedText.text = $"{config.moveSpeed}";
            attackSpeedText.text = $"{config.attackSpeed}";
            attackText.text = $"{config.baseAttack}";
            paintRateText.text = $"{config.basePaintEfficiency}";
            trustValueText.text = $"{config.trustValue}";
        }
    }
}
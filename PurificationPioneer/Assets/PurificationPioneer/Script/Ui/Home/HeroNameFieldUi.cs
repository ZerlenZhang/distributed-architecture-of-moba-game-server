using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class HeroNameFieldUi : MonoBehaviour
    {
        public Text heroNameText;

        public void Init(HeroConfigAsset config)
        {
            heroNameText.text = config.heroName;
        }
    }
}
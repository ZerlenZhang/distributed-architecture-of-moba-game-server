using System;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class HeroOptionUi : MonoBehaviour
    {
        public Image heroIcon;
        public Button clickBtn;

        public void InitValues(HeroConfigAsset heroConfig, Action<int> onClick)
        {
            this.heroIcon.sprite = heroConfig.icon;
            clickBtn.onClick.AddListener(()=>onClick(heroConfig.characterId));
        }
    }
}
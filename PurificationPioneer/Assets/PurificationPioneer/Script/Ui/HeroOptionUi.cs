using System;
using PurificationPioneer.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class HeroOptionUi : MonoBehaviour
    {
        public Image heroIcon;
        public Button clickBtn;

        public void InitValues(int heroId, Action<int> onClick)
        {
            heroIcon.sprite = AssetConstUtil.GetHeroIcon(heroId);
            clickBtn.onClick.AddListener(()=>onClick(heroId));
        }
    }
}
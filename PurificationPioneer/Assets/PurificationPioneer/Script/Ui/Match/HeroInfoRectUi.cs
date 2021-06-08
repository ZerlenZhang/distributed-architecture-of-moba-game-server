using PurificationPioneer.Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 英雄选择界面，显示人物信息的Ui
    /// </summary>
    public class HeroInfoRectUi : MonoBehaviour
    {
        public RawImage rawImage;
        public Text heroName;


        public void SetInfo(HeroConfigAsset heroConfig)
        {
            var rt = HomeSceneMgr.Instance.UpdateHeroPreview(heroConfig);
            rawImage.texture = rt;
            heroName.text = heroConfig.heroName;
        }
    }
}
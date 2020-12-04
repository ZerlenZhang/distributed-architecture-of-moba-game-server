using PurificationPioneer.Global;
using PurificationPioneer.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 游戏加载界面的每个玩家的Rect
    /// </summary>
    public class LoadingMatcherInfoUi : MonoBehaviour
    {
        public Image bigImage;
        public Image userIcon;
        public Text userNick;
        public Text userLevel;
        public Text userRank;

        public void InitValues(GlobalVar.MatcherInfo matcherInfo)
        {
            bigImage.sprite = AssetConstUtil.GetHeroIcon(matcherInfo.HeroId);
            userIcon.sprite = AssetConstUtil.GetUserIcon(matcherInfo.Uface);
            userNick.text = matcherInfo.Unick;
            userLevel.text = matcherInfo.Ulevel.ToString();
            userRank.text = matcherInfo.Urank.ToString();
        }
    }
}
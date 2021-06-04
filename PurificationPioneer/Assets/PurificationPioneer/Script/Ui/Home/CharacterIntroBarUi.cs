using PurificationPioneer.Const;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    public class CharacterIntroBarUi : MonoBehaviour
    {
        // 现实的几个属性
        public Image m_Icon;
        
        private HeroConfigAsset m_Config;
        
        
        public void Init(int heroId)
        {
            m_Config = ResourceMgr.GetAsset<HeroConfigAsset>(AssetConstUtil.GetHeroConfigKey(heroId));

            // 为显示的属性赋值
            m_Icon.sprite = m_Config.icon;
        }
        
        /// <summary>
        /// 打开详情页
        /// </summary>
        public void ShowMoreInfo()
        {
            PanelMgr.PushPanel(PanelName.CharacterPanel);
            CEventCenter.BroadMessage(Message.ShowHeroInfo, m_Config);
        }
    }
}
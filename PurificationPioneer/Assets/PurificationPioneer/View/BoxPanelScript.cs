using System.Linq;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Script;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
    public class BoxPanelScript : MonoBehaviour
    {
        public ScrollRect scrollRect;

        public void Init()
        {
            if (!GlobalVar.HeroIds.Any())
            {
                CEventCenter.BroadMessage(Message.ShowTip, "未初始化HeroIds");
                return;
            }

            foreach (var heroId in GlobalVar.HeroIds)
            {
                var heroIntroBarUi = ResourceMgr.InstantiateGameObject(UiName.CharacterIntroBarUi, scrollRect.content)
                    .GetComponent<CharacterIntroBarUi>();

                heroIntroBarUi.Init(heroId);
            }
            
        }

        public void Return()
        {
            PanelMgr.PopPanel();
        }
    }
}
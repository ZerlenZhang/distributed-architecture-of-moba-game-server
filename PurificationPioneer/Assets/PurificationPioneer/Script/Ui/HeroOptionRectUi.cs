using System;
using System.Collections.Generic;
using PurificationPioneer.Const;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.Script
{
    /// <summary>
    /// 英雄选择界面，英雄选择区域Ui
    /// </summary>
    public class HeroOptionRectUi : MonoBehaviour
    {
        public ScrollRect heroScrollView;

        public void InitValues(List<int> heroIds, Action<int> onClick_HeroId)
        {
            foreach (var heroId in heroIds)
            {
                var heroOptionUi = ResourceMgr.InstantiateGameObject(UiName.HeroOption, heroScrollView.content)
                    .GetComponent<HeroOptionUi>();
                heroOptionUi.InitValues(heroId, onClick_HeroId);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(heroScrollView.content);
        }
    }
}
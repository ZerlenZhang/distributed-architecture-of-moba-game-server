using System.Collections.Generic;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
	public partial class MatchPanel
	{
		private MatchPanelScript script;
		private Dictionary<int, MatcherRect> seatId_MatcherRect = new Dictionary<int, MatcherRect>();
		partial void OnLoad()
		{
			script = m_TransFrom.GetComponent<MatchPanelScript>();
			//do any thing you want
			
			AddMatcherIcons();
			AddHeroOptions();
		}

		private void AddHeroOptions()
		{
			script.heroOptionRectUi.InitValues(GlobalVar.HeroIds,
				id =>
				{
					if (GlobalVar.IsSubmit)
					{
						Debug.LogError("已经锁定，无法选择");
						return;
					}
					Debug.Log($"Choose Hero[{id}]");
					LogicProxy.Instance.SelectHero(
						GlobalVar.Uname,
						id);
				});
		}

		private void AddMatcherIcons()
		{
			foreach (var matcherInfo in GlobalVar.MatcherInfos)
			{
				var parent = matcherInfo.seatId % 2 == 1 ? script.leftGroup : script.rightGroup;
				var matcherRectUi = ResourceMgr.InstantiateGameObject(UiName.MatcherRect, parent)
					.GetComponent<MatcherRect>();
				
				//初始化属性
				matcherRectUi.InitValues(matcherInfo);
				
				//保存到字典
				seatId_MatcherRect.Add(matcherInfo.seatId, matcherRectUi);
			}

			LayoutRebuilder.ForceRebuildLayoutImmediate(script.leftGroup);
			LayoutRebuilder.ForceRebuildLayoutImmediate(script.rightGroup);

		}

		#region Override

		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener<SelectHeroRes>(Message.OnSelectHero, OnSelectHero);
			CEventCenter.AddListener<SubmitHeroRes>(Message.OnSubmitHero, OnSubmitHero);
			CEventCenter.AddListener<UpdateSelectTimer>(Message.OnUpdateSelectTimer,OnUpdateSelectTimer);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			CEventCenter.RemoveListener<SelectHeroRes>(Message.OnSelectHero, OnSelectHero);
			CEventCenter.RemoveListener<SubmitHeroRes>(Message.OnSubmitHero, OnSubmitHero);
			CEventCenter.RemoveListener<UpdateSelectTimer>(Message.OnUpdateSelectTimer,OnUpdateSelectTimer);
		}


		public override void Destory()
		{
			base.Destory();
			this.seatId_MatcherRect.Clear();
		}		

		#endregion


		#region MessageHandler

		private void OnUpdateSelectTimer(UpdateSelectTimer obj)
		{
			script.heroChooseTimerUi.SetTime(obj.current);
		}
		private void OnSubmitHero(SubmitHeroRes obj)
		{
			if (!seatId_MatcherRect.ContainsKey(obj.seatId))
			{
				Debug.LogError($"[MatchPanel.OnSubmitHero] unexpected seatId:{obj.seatId}");
				return;
			}

			seatId_MatcherRect[obj.seatId].SubmitHeroRes();

			if (obj.seatId == GlobalVar.SeatId)
			{
				script.OnSubmit();
			}
		}

		private void OnSelectHero(SelectHeroRes obj)
		{
			if (!seatId_MatcherRect.ContainsKey(obj.seatId))
			{
				Debug.LogError($"[MatchPanel.OnSelectHero] unexpected seatId:{obj.seatId}");
				return;
			}

			seatId_MatcherRect[obj.seatId].SelectHeroRes(obj);
		}		

		#endregion


	}
}

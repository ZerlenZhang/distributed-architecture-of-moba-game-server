using System.Collections.Generic;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Network.Proxy;
using PurificationPioneer.Script;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.View;
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
					if (GlobalVar.IsLocalSubmit)
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
			foreach (var kv in GlobalVar.SeatId_MatcherInfo)
			{
				var matcherInfo = kv.Value;
				var parent = matcherInfo.SeatId % 2 == 1 ? script.leftGroup : script.rightGroup;
				var matcherRectUi = ResourceMgr.InstantiateGameObject(UiName.MatcherRect, parent)
					.GetComponent<MatcherRect>();
				
				//初始化属性
				matcherRectUi.InitValues(matcherInfo);
				
				//保存到字典
				seatId_MatcherRect.Add(matcherInfo.SeatId, matcherRectUi);
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
			CEventCenter.AddListener(Message.OnStartLoadGame, OnStartLoadGame);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			CEventCenter.RemoveListener<SelectHeroRes>(Message.OnSelectHero, OnSelectHero);
			CEventCenter.RemoveListener<SubmitHeroRes>(Message.OnSubmitHero, OnSubmitHero);
			CEventCenter.RemoveListener(Message.OnStartLoadGame, OnStartLoadGame);
			CEventCenter.RemoveListener<UpdateSelectTimer>(Message.OnUpdateSelectTimer,OnUpdateSelectTimer);
		}


		public override void Destory()
		{
			base.Destory();
			this.seatId_MatcherRect.Clear();
		}		

		#endregion


		#region MessageHandler
		
		private void OnStartLoadGame()
		{
			PanelMgr.PushPanel(PanelName.LoadingPanel);
		}

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

			if (obj.seatId == GlobalVar.LocalSeatId)
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

			var config = ResourceMgr.GetAsset<HeroConfigAsset>(AssetConstUtil.GetHeroConfigKey(obj.hero_id));

			seatId_MatcherRect[obj.seatId].SelectHeroRes(config);

			if (obj.seatId == GlobalVar.LocalSeatId)
			{
				script.heroInfoRectUi.SetInfo(config);
			}
		}		

		#endregion
	}
}

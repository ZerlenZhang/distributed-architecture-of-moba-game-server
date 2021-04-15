using System;
using System.Collections;
using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Script;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Script;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PurificationPioneer.View
{
	public partial class LoadingPanel
	{
		private LoadingPanelScript script;
		private Coroutine loadSceneCorotine;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<LoadingPanelScript>();
			
			AddLoadingMatcherInfoUis();

			this.loadSceneCorotine = SceneMgr.LoadScene(
				SceneName.Battle, 
				progress => script.loadingProgressRect.SetProgress(progress / 100f)
				);
		}

		public override void Destory()
		{
			base.Destory();
			loadSceneCorotine = null;
		}

		private void AddLoadingMatcherInfoUis()
		{
			foreach (var kv in GlobalVar.SeatId_MatcherInfo)
			{
				var matcherInfo = kv.Value;
				var parent = matcherInfo.SeatId % 2 == 1
					? script.leftGroup
					: script.rightGroup;
				var loadingMatcherInfoUi = ResourceMgr.InstantiateGameObject(
					UiName.LoadMatcherUi, parent).GetComponent<LoadingMatcherInfoUi>();
				loadingMatcherInfoUi.InitValues(matcherInfo);
			}

			LayoutRebuilder.ForceRebuildLayoutImmediate(script.leftGroup);
			LayoutRebuilder.ForceRebuildLayoutImmediate(script.rightGroup);
		}
	}
}

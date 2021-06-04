using PurificationPioneer.Const;
using PurificationPioneer.Scriptable;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.View
{
	public partial class CharacterPanel
	{
		private CharacterPanelScript script;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<CharacterPanelScript>();
		}


		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener<HeroConfigAsset>(Message.ShowHeroInfo, OnShowHeroInfo);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			CEventCenter.RemoveListener<HeroConfigAsset>(Message.ShowHeroInfo, OnShowHeroInfo);
		}

		private void OnShowHeroInfo(HeroConfigAsset asset)
		{
			if (asset)
			{
				script.Init(asset);
			}
		}
	}
}

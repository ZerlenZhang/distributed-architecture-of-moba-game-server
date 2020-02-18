using Moba.Const;
using Moba.Global;
using ReadyGamerOne.Common;
using ReadyGamerOne.MemorySystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Moba.View
{
	public partial class HomePanel
	{
		private Text unick;
		private Image uface;
		partial void OnLoad()
		{
			//do any thing you want
			unick = view["Top/Uinfo/NickText"].GetComponent<Text>();
			uface = view["Top/Uinfo/HeadIcon"].GetComponent<Image>();

			Assert.IsTrue(unick && uface);
			
			add_button_listener("Top/Uinfo/HeadIcon",OnClickIcon);
		}


		public override void Enable()
		{
			base.Enable();
			//无论如何进来就先同步一下
			OnSyncNetInfo();
		}
		
		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener(Message.SyncNetInfo,OnSyncNetInfo);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();			
			CEventCenter.RemoveListener(Message.SyncNetInfo,OnSyncNetInfo);
		}

		private void OnSyncNetInfo()
		{
			unick.text = NetInfo.unick;
			uface.sprite = ResourceMgr.GetAsset<Sprite>("Avator_" + NetInfo.uface);
		}


		private void OnClickIcon()
		{
			var obj = ResourceMgr.InstantiateGameObject(UiName.UserInfoDlg,GlobalVar.G_Canvas.transform);
		}
	}
}

using Moba.Const;
using Moba.Data;
using Moba.Global;
using Moba.View.Dlgs;
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
		private Text coinText;
		private Text diamondText;

		private Text levelText;
		private Slider expSlider;
		private Text expText;

		private LoginBonuesUi _loginBonuesUi;
		partial void OnLoad()
		{
			//do any thing you want
			unick = view["Top/Uinfo/NickText"].GetComponent<Text>();
			uface = view["Top/Uinfo/HeadIcon"].GetComponent<Image>();
			coinText = view["Top/Center/CoinInfo/Text"].GetComponent<Text>();
			diamondText = view["Top/Center/DamdInfo/Text"].GetComponent<Text>();
			expSlider = view["Top/Uinfo/ExpSlider"].GetComponent<Slider>();
			levelText = view["Top/Uinfo/LevelText"].GetComponent<Text>();
			expText = view["Top/Uinfo/ExpText"].GetComponent<Text>();
			_loginBonuesUi = view["LoginBonues"].GetComponent<LoginBonuesUi>();
			
			add_button_listener("Top/Uinfo/HeadIcon",OnClickIcon);
		}


		public override void Enable()
		{
			base.Enable();
			//无论如何进来就先同步一下
			OnSyncAuthInfo();
			OnSyncSystemInfo();
			
			Debug.Log("有奖励吗？[NetInfo.gameInfo.bonues_status == 0]=="+(NetInfo.gameInfo.bonues_status == 0));
			
			
			if (NetInfo.gameInfo.bonues_status == 0)
			{
				//有奖励
				_loginBonuesUi.gameObject.SetActive(true);
				_loginBonuesUi.ShowLoginBonues(NetInfo.gameInfo.days);
			}
			else
			{
				_loginBonuesUi.gameObject.SetActive(false);
			}
		}
		
		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener(Message.SyncAuthInfo,OnSyncAuthInfo);
			CEventCenter.AddListener(Message.SyncUgameInfo,OnSyncSystemInfo);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();			
			CEventCenter.RemoveListener(Message.SyncAuthInfo,OnSyncAuthInfo);
			CEventCenter.RemoveListener(Message.SyncUgameInfo,OnSyncSystemInfo);
		} 

		private void OnSyncAuthInfo()
		{
			unick.text = NetInfo.unick;
			uface.sprite = ResourceMgr.GetAsset<Sprite>("Avator_" + NetInfo.uface);
		}
		private void OnSyncSystemInfo()
		{
			coinText.text = NetInfo.gameInfo.ucoin_1.ToString();
			diamondText.text = NetInfo.gameInfo.ucoin_2.ToString();

			int nowExp, nextLevelExp;
			var level = UlevelMgr.Instance.GetLevelInfo(
				NetInfo.gameInfo.uexp,
				out nowExp,
				out nextLevelExp);
			levelText.text = "LV\n" + level;
			expSlider.value = (float) nowExp / (float) nextLevelExp;
			expText.text = nowExp + " / " + nextLevelExp;
		}

		private void OnClickIcon()
		{
			var obj = ResourceMgr.InstantiateGameObject(UiName.UserInfoDlg,GlobalVar.G_Canvas.transform);
		}
	}
}

using Moba.Const;
using Moba.Data;
using Moba.Global;
using Moba.Protocol;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.View;
using UnityEngine;

namespace Moba.Script
{
	public partial class MobaMgr
	{
		public StringChooser startPanel = new StringChooser(typeof(PanelName));
		
		partial void OnSafeAwake()
		{
			//初始化等级数据
			UlevelMgr.Instance.Init();
			
			CEventCenter.AddListener(Message.LoginLogicServerSuccess,OnLoginLogicServerSuccess);
			CEventCenter.AddListener(Message.GetUgameInfoSuccess,OnGetUgaemInfoSuccess);
			//do any thing you want
			PanelMgr.PushPanel(startPanel.StringValue);
		}

		private void OnLoginLogicServerSuccess()
		{
			CEventCenter.RemoveListener(Message.LoginLogicServerSuccess,OnLoginLogicServerSuccess);
			Debug.Log("登陆逻辑服务器成功");
			PanelMgr.PushPanel(PanelName.HomePanel);
		}

		private void OnGetUgaemInfoSuccess()
		{
			CEventCenter.RemoveListener(Message.GetUgameInfoSuccess, OnGetUgaemInfoSuccess);
			Debug.Log("获取Moba信息成功，开始正在登陆逻辑服务器");
			LogicServiceProxy.Instance.LoginLogicServer();
		}
	}
}

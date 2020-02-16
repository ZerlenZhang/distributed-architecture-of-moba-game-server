using gprotocol;
using Moba.Const;
using Moba.Network;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Network;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEngine;

namespace Moba.Script
{
	public partial class MobaMgr
	{
		public StringChooser startPanel = new StringChooser(typeof(PanelName));
		partial void OnSafeAwake()
		{
			//do any thing you want
			PanelMgr.PushPanel(startPanel.StringValue);
		}

		public void Start()
		{
			NetworkMgr.Instance.AddCmdPackageListener(
				(int)ServiceType.Auth,
				OnAuthCmd);
		}

		private void OnAuthCmd(CmdPackageProtocol.CmdPackage pk)
		{
			
		}

		/// <summary>
		/// 游客登陆
		/// </summary>
		public void GuestLogin()
		{
			Debug.Log("游客登陆");

			var req = new GuestLoginReq
			{
				guest_key = RandomUtil.RandomStr(10)
			};

			NetworkMgr.Instance.SendProtobufCmd(
				(int) ServiceType.Auth,
				(int) LoginCmd.eGuestLoginReq,
				req);
		}
		
		
		
		
		
	}
}

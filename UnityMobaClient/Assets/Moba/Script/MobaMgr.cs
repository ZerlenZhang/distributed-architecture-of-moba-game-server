using Assets.Moba.Const;
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

		private void OnGuestLoginReturn(CmdPackageProtocol.CmdPackage pk)
		{
			var res = CmdPackageProtocol.ProtobufDeserialize<GuestLoginRes>(pk.body);
			if (null == res)
				return;
			if (res.status != Responce.Ok)
			{
				Debug.LogWarning("Guest Login status: " + res.status);
				return;
			}

			var info = res.uinfo;
			Debug.Log(info.unick+" "+info.uid);
		}	

		private void OnAuthCmd(CmdPackageProtocol.CmdPackage pk)
		{
			switch ((LoginCmd)pk.cmdType)
			{
				case LoginCmd.eGuestLoginRes:
					OnGuestLoginReturn(pk);
					break;
			}
		}

		/// <summary>
		/// 游客登陆
		/// </summary>
		public void GuestLogin()
		{
			Debug.Log("游客登陆");

			var req = new GuestLoginReq
			{
				guest_key = "2g12s12g2asrghtr2jyrei",
			};

			NetworkMgr.Instance.SendProtobufCmd(
				(int) ServiceType.Auth,
				(int) LoginCmd.eGuestLoginReq,
				req);
		}
		
		
		
		
		
	}
}

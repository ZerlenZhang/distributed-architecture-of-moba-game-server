using Moba.Const;
using Moba.Global;
using gprotocol;
using Moba.Network;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.Network;
using ReadyGamerOne.Utility;
using ReadyGamerOne.View;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Script
{
	public partial class MobaMgr
	{
		private EditProfileReq _editProfileReq = null;

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

		/// <summary>
		/// 游客登陆的返回
		/// </summary>
		/// <param name="pk"></param>
		private void OnGuestLoginReturn(CmdPackageProtocol.CmdPackage pk)
		{
			var res = CmdPackageProtocol.ProtobufDeserialize<GuestLoginRes>(pk.body);
			if (null == res)
			{
				return;
			}
			

			if (res.status != Responce.Ok)
			{
				Debug.LogWarning("Guest Login status: " + res.status);
				return;
			}
			
			//保存用户信息
			NetInfo.SaveInfo(res.uinfo,true);
			//同步网络信息
			CEventCenter.BroadMessage(Message.SyncNetInfo);
			//切换至主界面
			PanelMgr.PushPanel(PanelName.HomePanel);
		}	

		private void OnAuthCmd(CmdPackageProtocol.CmdPackage pk)
		{
//			print("recv cmd: "+pk.cmdType);
			switch ((LoginCmd)pk.cmdType)
			{
				case LoginCmd.eGuestLoginRes:
					OnGuestLoginReturn(pk);
					break;
				case LoginCmd.eEditProfileRes:
					OnEditProfileReturn(pk);
					break;
			}
		}

		/// <summary>
		/// 修改信息服务器回调
		/// </summary>
		/// <param name="pk"></param>
		private void OnEditProfileReturn(CmdPackageProtocol.CmdPackage pk)
		{
			var res = CmdPackageProtocol.ProtobufDeserialize<EditProfileRes>(pk.body);
			if (null == res)
			{
				Debug.LogError("解码失败");
				return;
			}

			if (res.status != Responce.Ok)
			{
				Debug.LogError("Error Status: " + res.status);
				return;
			}
			
			Assert.IsNotNull(_editProfileReq);
			NetInfo.SaveEditProfile(_editProfileReq.unick,_editProfileReq.uface,_editProfileReq.usex);
			_editProfileReq = null;
			//通知更新
			CEventCenter.BroadMessage(Message.SyncNetInfo);
		}

		/// <summary>
		/// 游客登陆
		/// </summary>
		public void GuestLogin()
		{
			Debug.Log("尝试游客登陆");

			var key = PlayerPrefs.GetString(PrefKey.GuestKey);

			if (string.IsNullOrEmpty(key))
			{
				key = RandomUtil.RandomStr(15);
				PlayerPrefs.SetString(PrefKey.GuestKey, key);
			}

			print(key);
			
			var req = new GuestLoginReq
			{
				guest_key = key,
			};

			NetworkMgr.Instance.SendProtobufCmd(
				(int) ServiceType.Auth,
				(int) LoginCmd.eGuestLoginReq,
				req);
		}

		public void EditProfile(string unick,int uface,int usex)
		{
			if (unick.Length <= 0)
				return;
			if (uface < 0 || uface > 7)
				return;
			if (usex != 0 && usex != 1)
				return;

			_editProfileReq = new EditProfileReq
			{
				usex = usex, uface = uface, unick = unick
			};

			NetworkMgr.Instance.SendProtobufCmd(
				(int)ServiceType.Auth,
				(int)LoginCmd.eEditProfileReq,
				_editProfileReq);
			
		}
	}
}

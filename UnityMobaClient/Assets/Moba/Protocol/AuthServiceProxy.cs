using gprotocol;
using Moba.Const;
using Moba.Global;
using Moba.Network;
using ReadyGamerOne.Common;
using ReadyGamerOne.Network;
using ReadyGamerOne.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Protocol
{
    public class AuthServiceProxy:Singleton<AuthServiceProxy>
    {
	    
	    private EditProfileReq _editProfileReq = null;
	    
        public AuthServiceProxy()
		{
			NetworkMgr.Instance.AddCmdPackageListener(
				(int)ServiceType.Auth,
				OnAuthCmd);
		}

        #region Native

        private void EnterGame()
        {
	        //获取游戏信息
	        SystemServiceProxy.Instance.LoadMobaInfo();
	        //PanelMgr.PushPanel(PanelName.HomePanel);
        }

        #endregion

		#region ServerCallBack

		private void OnAuthCmd(CmdPackageProtocol.CmdPackage pk)
		{
//			Debug.Log("recv cmd: "+pk.cmdType);
			switch ((LoginCmd)pk.cmdType)
			{
				case LoginCmd.eGuestLoginRes:
					OnGuestLoginReturn(pk);
					break;
				case LoginCmd.eEditProfileRes:
					OnEditProfileReturn(pk);
					break;
				case LoginCmd.eAccountUpgradeRes:
					OnAccountUpgradeReturn(pk);
					break;
				case LoginCmd.eUserLoginRes:
					OnUserLoginReturn(pk);
					break;
				case LoginCmd.eUserUnregisterRes:
					var res = CmdPackageProtocol.ProtobufDeserialize<UserUnregisterRes>(pk.body);
					if (res == null)
						return;
					if (res.status != Responce.Ok)
					{
						Debug.Log("Login status:" + res.status);
						break;
					}
					//注销成功
					CEventCenter.BroadMessage(Message.Unregister);
					break;
			}
		}

		/// <summary>
		/// 账号登陆返回
		/// </summary>
		/// <param name="pk"></param>
		private void OnUserLoginReturn(CmdPackageProtocol.CmdPackage pk)
		{			
			var res = CmdPackageProtocol.ProtobufDeserialize<UserLoginRes>(pk.body);
         	if (null == res) 
            { 
	            return; 
            }
            if (res.status != Responce.Ok)
            {
	            Debug.LogWarning("UserLogin status: " + res.status);
	            return;
            }
			
            //保存用户信息
            NetInfo.SaveInfo(res.uinfo,false);
            //同步网络信息
            CEventCenter.BroadMessage(Message.SyncAuthInfo);

            EnterGame();
		}

		/// <summary>
		/// 账号升级返回
		/// </summary>
		/// <param name="pk"></param>
		private void OnAccountUpgradeReturn(CmdPackageProtocol.CmdPackage pk)
		{
			var res = CmdPackageProtocol.ProtobufDeserialize<AccountUpgradeRes>(pk.body);
			if (null == res)
			{
				return;
			}

			Debug.Log("AccountUpgradeRes.Status: " + res.status);
			CEventCenter.BroadMessage(Message.UpgradeGuest,res.status);

			if (res.status == Responce.Ok)
			{
				NetInfo.SetIsGuest(false);
			}
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
				Debug.LogWarning("Guest UserLogin status: " + res.status);
				return;
			}
			
			//保存用户信息
			NetInfo.SaveInfo(res.uinfo,true);
			//同步网络信息
			CEventCenter.BroadMessage(Message.SyncAuthInfo);
			EnterGame();
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
			CEventCenter.BroadMessage(Message.SyncAuthInfo);
		}		

		#endregion
		
		#region public

		/// <summary>
		/// 正式注册
		/// </summary>
		/// <param name="uName"></param>
		/// <param name="upwdMd5"></param>
		public void UpgradeGuest(string uName, string upwdMd5)
		{
			Debug.Log("尝试注册");
			var req = new AccountUpgradeReq
			{
				uname = uName,
				upwd_md5 = upwdMd5,
			};
			NetworkMgr.Instance.TcpSendProtobufCmd(
				(int) ServiceType.Auth,
				(int) LoginCmd.eAccountUpgradeReq,
				req);
		}
		
		/// <summary>
		/// 游客登陆
		/// </summary>
		public void GuestLogin()
		{
			Debug.Log("尝试游客登陆");

			var key = ""; //PlayerPrefs.GetString(PrefKey.GuestKey);

			if (string.IsNullOrEmpty(key))
			{
				key = RandomUtil.RandomStr(15);
				PlayerPrefs.SetString(PrefKey.GuestKey, key);
			}

			Debug.Log(key);
			
			var req = new GuestLoginReq
			{
				guest_key = key,
			};

			NetworkMgr.Instance.TcpSendProtobufCmd(
				(int) ServiceType.Auth,
				(int) LoginCmd.eGuestLoginReq,
				req);
		}

		/// <summary>
		/// 用户登录
		/// </summary>
		/// <param name="uname"></param>
		/// <param name="upwd"></param>
		public void UserLogin(string uname, string upwd)
		{
			var md5 = SecurityUtil.Md5(upwd);
//			Debug.Log(uname + "  " + upwd);

			var req = new UserLoginReq
			{
				uname = uname,
				upwd_md5 = md5,
			};
			NetworkMgr.Instance.TcpSendProtobufCmd(
				(int) ServiceType.Auth,
				(int) LoginCmd.eUserLoginReq,
				req);
		}

		/// <summary>
		/// 修改资料
		/// </summary>
		/// <param name="unick"></param>
		/// <param name="uface"></param>
		/// <param name="usex"></param>
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

			NetworkMgr.Instance.TcpSendProtobufCmd(
				(int)ServiceType.Auth,
				(int)LoginCmd.eEditProfileReq,
				_editProfileReq);
			
		}


		/// <summary>
		/// 注销
		/// </summary>
		public void Unregister()
		{
			Debug.Log("注销");
			NetworkMgr.Instance.TcpSendProtobufCmd(
				(int)ServiceType.Auth,
				(int)LoginCmd.eUserUnregisterReq,
				null);
		}
		#endregion
    }
}
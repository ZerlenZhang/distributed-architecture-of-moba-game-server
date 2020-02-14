using gprotocol;
using Moba.Const;
using Moba.Network;
using ReadyGamerOne.MemorySystem;
using ReadyGamerOne.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Moba.View
{
	public partial class ChatPanel
	{
		private InputField _inputField;
		private ScrollRect scrollRect;
		private string talkContent;
		
		partial void OnLoad()
		{
			//do any thing you want
			add_button_listener("TitleBar/ExitBtn", OnExit);
			add_button_listener("TitleBar/LoginBtn", OnLogin);
			add_button_listener("TextBar/SendBtn", OnSend);

			_inputField = view["TextBar/InputField"].GetComponent<InputField>();
			scrollRect = view["ScrollView"].GetComponent<ScrollRect>();
			
		}

		protected override void OnAddListener()
		{
			base.OnAddListener();
			NetworkMgr.Instance.AddCmdPackageListener(2,this.OnServerReturn);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			NetworkMgr.Instance.RemoveCmdPackageListener(2,this.OnServerReturn);
		}

		#region Net_Callback

		private void OnServerReturn(CmdPackageProtocol.CmdPackage pk)
		{
			switch ((ChatCmd)pk.cmdType)
			{
				case ChatCmd.eOnUserExit:
					var onExit = CmdPackageProtocol.ProtobufDeserialize<OnUserExit>(pk.body);
					this.AddStatus(onExit.ip+":"+onExit.port+" 离开聊天室");
					break;
				case ChatCmd.eOnUserLogin:
					var onlogin = CmdPackageProtocol.ProtobufDeserialize<OnUserExit>(pk.body);
					this.AddStatus(onlogin.ip+":"+onlogin.port+"加入聊天室");
					break;
				case ChatCmd.eOnSendMsg:
					var onSend = CmdPackageProtocol.ProtobufDeserialize<OnSendMsg>(pk.body);
					this.AddTalkInfo(onSend.ip, onSend.port, onSend.content); 
					break;
				
				case ChatCmd.eExitRes:
					var exitRes = CmdPackageProtocol.ProtobufDeserialize<ExitRes>(pk.body);
					if (1 == exitRes.status)
					{
						this.AddStatus("顺利离开聊天室");
					}
					else
					{
						this.AddStatus("您早已不再聊天室");
					}

					break;
				case ChatCmd.eLoginRes:

					var res = CmdPackageProtocol.ProtobufDeserialize<LoginRes>(pk.body);
					if (1 == res.status)
					{
						this.AddStatus("加入成功");
					}
					else
					{
						this.AddStatus("加入失败");
					}
					
					break;
				case ChatCmd.eSendMsgRes:
					var sendReq = CmdPackageProtocol.ProtobufDeserialize<SendMsgRes>(pk.body);
					if (1 == sendReq.status)
					{
						this.AddTalkSelf("127.0.0.1",6080, this.talkContent);
					}
					else
					{
						this.AddStatus("发送失败");
					}			
					break;
			}
		}

		#endregion


		#region UI_Callback

		private void OnExit()
		{
			Debug.Log("on_exit");
			NetworkMgr.Instance.SendProtobufCmd(2,(int)ChatCmd.eExitReq,null);

		}

		private void OnLogin()
		{
			Debug.Log("on_login");
			NetworkMgr.Instance.SendProtobufCmd(2,(int)ChatCmd.eLoginReq,null);
		}

		private void OnSend()
		{
			Debug.Log("on_send");
			if (_inputField.text.Length == 0)
				return;
			var req = new SendMsgReq
			{
				content = _inputField.text
			};

			this.talkContent = _inputField.text;
			
			NetworkMgr.Instance.SendProtobufCmd(2,(int)ChatCmd.eSendMsgReq,req);
			
		}		

		#endregion

		#region Native

		private void AddStatus(string info)
		{
			var opt = ResourceMgr.InstantiateGameObject(UiName.StatusUi);
			opt.transform.SetParent(this.scrollRect.content,false);

			opt.transform.GetComponentInChildren<Text>().text = info;
			
			var contentSize = this.scrollRect.content.sizeDelta;
			contentSize.y += 100;
			this.scrollRect.content.sizeDelta = contentSize;

			var localPos = this.scrollRect.content.localPosition;
			localPos.y += 100;
			this.scrollRect.content.localPosition = localPos;
		}

		private void AddTalkInfo(string ip, int port, string content)
		{
			var opt = ResourceMgr.InstantiateGameObject(UiName.TalkUi);
			opt.transform.SetParent(this.scrollRect.content,false);

//			opt.transform.GetComponentInChildren<Text>().text = info;
			opt.transform.Find("Text").GetComponent<Text>().text = content;
			opt.transform.Find("IpPort").GetComponent<Text>().text = ip + ":" + port;

			var contentSize = this.scrollRect.content.sizeDelta;
			contentSize.y += 100;
			this.scrollRect.content.sizeDelta = contentSize;
			
			var localPos = this.scrollRect.content.localPosition;
			localPos.y += 100;
			this.scrollRect.content.localPosition = localPos;
		}
		
		private void AddTalkSelf(string ip, int port, string content)
		{
			var opt = ResourceMgr.InstantiateGameObject(UiName.SelfUi);
			opt.transform.SetParent(this.scrollRect.content,false);

//			opt.transform.GetComponentInChildren<Text>().text = info;
			opt.transform.Find("Text").GetComponent<Text>().text = content;
			opt.transform.Find("IpPort").GetComponent<Text>().text = ip + ":" + port;

			var contentSize = this.scrollRect.content.sizeDelta;
			contentSize.y += 100;
			this.scrollRect.content.sizeDelta = contentSize;
			
			var localPos = this.scrollRect.content.localPosition;
			localPos.y += 100;
			this.scrollRect.content.localPosition = localPos;
		}		

		#endregion


	}
}

using PurificationPioneer.Const;
using PurificationPioneer.Global;
using PurificationPioneer.Network.Proxy;
using ReadyGamerOne.Common;

namespace PurificationPioneer.View
{
	public partial class HomePanel
	{
		private HomePanelScript script;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<HomePanelScript>();
			
			//登陆逻辑服务器
			LogicProxy.Instance.Login();
			
			script.playBtn.onClick.AddListener(() =>
			{
				LogicProxy.Instance.StartMatch(
					GlobalVar.uname);
			});
		}

		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener(Message.OnStartMatch,OnStartMatch);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			CEventCenter.RemoveListener(Message.OnStartMatch,OnStartMatch);
		}

		private void OnStartMatch()
		{
			script.matchUi.StartMatch();
		}
	}
}

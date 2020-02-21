using Moba.Const;
using Moba.Data;
using Moba.Global;
using ReadyGamerOne.Common;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.View;

namespace Moba.Script
{
	public partial class MobaMgr
	{
		public StringChooser startPanel = new StringChooser(typeof(PanelName));
		
		partial void OnSafeAwake()
		{
			//初始化等级数据
			UlevelMgr.Instance.Init();
			
			CEventCenter.AddListener(Message.GetUgameInfoSuccess,OnGetUgaemInfoSuccess);
			//do any thing you want
			PanelMgr.PushPanel(startPanel.StringValue);
		}

		private void OnGetUgaemInfoSuccess()
		{
			CEventCenter.RemoveListener(Message.GetUgameInfoSuccess,OnGetUgaemInfoSuccess);
			PanelMgr.PushPanel(PanelName.HomePanel);
		}
	}
}

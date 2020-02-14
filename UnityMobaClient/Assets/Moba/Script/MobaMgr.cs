using Moba.Const;
using ReadyGamerOne.EditorExtension;
using ReadyGamerOne.View;

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
	}
}

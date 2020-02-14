using ReadyGamerOne.View;
using Moba.Const;
namespace Moba.View
{
	public partial class ChatPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.ChatPanel);
			OnLoad();
		}
	}
}

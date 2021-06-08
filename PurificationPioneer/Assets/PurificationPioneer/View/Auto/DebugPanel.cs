using ReadyGamerOne.View;
using PurificationPioneer.Const;
namespace PurificationPioneer.View
{
	public partial class DebugPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.DebugPanel);
			OnLoad();
		}
	}
}

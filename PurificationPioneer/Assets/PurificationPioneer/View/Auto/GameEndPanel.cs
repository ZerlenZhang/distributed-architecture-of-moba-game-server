using ReadyGamerOne.View;
using PurificationPioneer.Const;
namespace PurificationPioneer.View
{
	public partial class GameEndPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.GameEndPanel);
			OnLoad();
		}
	}
}

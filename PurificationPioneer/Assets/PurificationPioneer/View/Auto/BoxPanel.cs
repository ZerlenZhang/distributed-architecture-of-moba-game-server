using ReadyGamerOne.View;
using PurificationPioneer.Const;
namespace PurificationPioneer.View
{
	public partial class BoxPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.BoxPanel);
			OnLoad();
		}
	}
}

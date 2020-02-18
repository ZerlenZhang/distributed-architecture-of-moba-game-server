using ReadyGamerOne.View;
using Moba.Const;
namespace Moba.View
{
	public partial class HomePanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.HomePanel);
			OnLoad();
		}
	}
}

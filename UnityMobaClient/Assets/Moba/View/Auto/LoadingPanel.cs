using ReadyGamerOne.View;
using Moba.Const;
namespace Moba.View
{
	public partial class LoadingPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.LoadingPanel);
			OnLoad();
		}
	}
}

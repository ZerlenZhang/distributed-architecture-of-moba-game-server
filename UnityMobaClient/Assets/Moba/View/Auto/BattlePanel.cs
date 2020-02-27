using ReadyGamerOne.View;
using Moba.Const;
namespace Moba.View
{
	public partial class BattlePanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.BattlePanel);
			OnLoad();
		}
	}
}

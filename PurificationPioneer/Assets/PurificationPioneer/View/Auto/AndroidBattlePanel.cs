using ReadyGamerOne.View;
using PurificationPioneer.Const;
namespace PurificationPioneer.View
{
	public partial class AndroidBattlePanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.AndroidBattlePanel);
			OnLoad();
		}
	}
}

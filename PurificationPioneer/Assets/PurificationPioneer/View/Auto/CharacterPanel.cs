using ReadyGamerOne.View;
using PurificationPioneer.Const;
namespace PurificationPioneer.View
{
	public partial class CharacterPanel : AbstractPanel
	{
		partial void OnLoad();

		protected override void Load()
		{
			Create(PanelName.CharacterPanel);
			OnLoad();
		}
	}
}

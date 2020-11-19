namespace PurificationPioneer.View
{
	public partial class HomePanel
	{
		private HomePanelScript script;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<HomePanelScript>();
		}
	}
}

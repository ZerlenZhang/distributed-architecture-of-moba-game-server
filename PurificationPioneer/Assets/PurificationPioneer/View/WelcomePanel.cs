namespace PurificationPioneer.View
{
	public partial class WelcomePanel
	{
		private WelcomePanelScript script;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<WelcomePanelScript>();
		}
	}
}

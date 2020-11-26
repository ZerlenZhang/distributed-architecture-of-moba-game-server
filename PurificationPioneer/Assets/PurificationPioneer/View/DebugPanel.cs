namespace PurificationPioneer.View
{
	public partial class DebugPanel
	{
		private DebugPanelScript script;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<DebugPanelScript>();
			script.InitSettings();
		}
	}
}

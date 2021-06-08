namespace PurificationPioneer.View
{
	public partial class BoxPanel
	{
		private BoxPanelScript script;
		partial void OnLoad()
		{
			script = m_TransFrom.GetComponent<BoxPanelScript>();
			//do any thing you want
			
			script.Init();
		}
	}
}

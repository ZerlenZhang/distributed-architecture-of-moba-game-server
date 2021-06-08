using UnityEngine.Assertions;

namespace PurificationPioneer.View
{
	public partial class GameEndPanel
	{
		private GameEndPanelScript _script;
		partial void OnLoad()
		{
			//do any thing you want
			_script = m_TransFrom.GetComponent<GameEndPanelScript>();
			Assert.IsTrue(_script);
			_script.Init();
		}
	}
}

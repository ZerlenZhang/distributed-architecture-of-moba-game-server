using UnityEngine.Assertions;

namespace PurificationPioneer.View
{
	public partial class BattlePanel
	{
		private BattlePanelScript script;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<BattlePanelScript>();
			Assert.IsTrue(script);
			InitScript();
		}

		private void InitScript()
		{
			script.Init();
		}
	}
}

using UnityEngine;

namespace PurificationPioneer.View
{
	public partial class SimpleLoadingPanel
	{
		private SimpleLoadingPanelScript script;
		partial void OnLoad()
		{
			//do any thing you want
			script = m_TransFrom.GetComponent<SimpleLoadingPanelScript>();
		}

		public void SetProgress(float value)
		{
			script.m_Progress.value = Mathf.Clamp01(value);
		}
	}
}

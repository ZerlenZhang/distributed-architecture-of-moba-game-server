using System;
using PurificationPioneer.Const;
using ReadyGamerOne.Common;
using UnityEngine;

namespace PurificationPioneer.View
{
	public partial class AndroidBattlePanel
	{
		private AndroidBattlePanelScript script;
		partial void OnLoad()
		{
			script = m_TransFrom.GetComponent<AndroidBattlePanelScript>();
			//do any thing you want
		}

		protected override void OnAddListener()
		{
			base.OnAddListener();
			CEventCenter.AddListener<Action<Vector2>>(Message.AndroidMoveInput,OnCollectAndroidMoveInput);
		}

		protected override void OnRemoveListener()
		{
			base.OnRemoveListener();
			CEventCenter.RemoveListener<Action<Vector2>>(Message.AndroidMoveInput,OnCollectAndroidMoveInput);
		}

		private void OnCollectAndroidMoveInput(Action<Vector2> callBack)
		{
			callBack?.Invoke(script.joystick.TouchDir);
		}
	}
}

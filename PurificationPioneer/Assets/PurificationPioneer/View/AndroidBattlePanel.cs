using System;
using PurificationPioneer.Const;
using ReadyGamerOne.Common;
using UnityEngine;
using UnityEngine.Assertions;

namespace PurificationPioneer.View
{
	public partial class AndroidBattlePanel
	{
		private AndroidBattlePanelScript script;
		partial void OnLoad()
		{
			script = m_TransFrom.GetComponent<AndroidBattlePanelScript>();
			Assert.IsTrue(script);
			//do any thing you want
			InitScript();
		}

		private void InitScript()
		{
			script.Init();
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

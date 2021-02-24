 using System;
 using PurificationPioneer.Scriptable;
 using UnityEngine;

 namespace PurificationPioneer.Script
{
	public partial class  PurificationPioneerMgr
	{
		public event Action<GUIStyle> eventOnGameState;

		private GUIStyle _defaultGuiStyle;

		private GUIStyle DefaultGuiStyle
		{
			get
			{
				if (null == _defaultGuiStyle)
				{
					_defaultGuiStyle=new GUIStyle
					{
						fontSize = GameSettings.Instance.DefaultStateFontSize
					};
				}

				return _defaultGuiStyle;
			}
		}
		partial void OnSafeAwake()
		{
			//do any thing you want
			eventOnGameState += FrameSyncMgr.OnGui;
			eventOnGameState += PpPhysics.OnGui;
			
			PpPhysics.SetupPhysicsWorld();
		}
		
		private void OnGUI()
		{
			if(GameSettings.Instance.IfShowGizmos || Input.GetKey(GameSettings.Instance.GameStateKey))
				eventOnGameState?.Invoke(DefaultGuiStyle);
		}

		private void FixedUpdate()
		{
			PpPhysics.Simulate(Time.fixedDeltaTime,PpPhysicsSimulateOptions.NoEvent);
		}
	}
}

 using System;
 using PurificationPioneer.Scriptable;
 using ReadyGamerOne.Utility;
 using ReadyGamerOne.View;
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

			DialogSystem.Scripts.DialogSystem.onStartDialog += OnStartDialog;
			DialogSystem.Scripts.DialogSystem.onEndDialog += OnEndDialog;
			
		}
		
		private void OnGUI()
		{
			if(GameSettings.Instance.IfShowGizmos || Input.GetKey(GameSettings.Instance.GameStateKey))
				eventOnGameState?.Invoke(DefaultGuiStyle);
		}

		internal bool m_NeedResetMouseMode = false;
		protected override void Update()
		{
			base.Update();
			if (UnityAPI.IsLocked && Input.GetKeyDown(GameSettings.Instance.MouseMode))
			{
				m_NeedResetMouseMode = true;
				UnityAPI.FreeMouse();
			}else if (m_NeedResetMouseMode && Input.GetKeyUp(GameSettings.Instance.MouseMode))
			{
				m_NeedResetMouseMode = false;
				UnityAPI.LockMouse();
			}
		}

		protected override void OnAnySceneUnload()
		{
			base.OnAnySceneUnload();
			PanelMgr.Clear();
		}

		private void OnStartDialog()
		{
			UnityAPI.FreeMouse();
		}

		private void OnEndDialog()
		{
			UnityAPI.LockMouse();
		}
	}
}

using Moba.Const;
using Moba.Data;
using Moba.Protocol;
using ReadyGamerOne.Script;
using ReadyGamerOne.View;
using UnityEngine;

namespace Moba.Script
{

	public partial class MobaMgr
	{

		public bool udpTest = false;
		public bool c_test_load_scene = true;
		partial void OnSafeAwake()
		{
			//初始化等级数据
			UlevelMgr.Instance.Init();
		}

		private void Start()
		{
			if(udpTest)
				MainLoop.Instance.ExecuteLater(
					() => LogicServiceProxy.Instance.TestUdp(999), 
					3);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
			{
				PanelMgr.PushPanelWithMessage(PanelName.LoadingPanel,Message.LoadSceneAsync,SceneName.Battle);
			}
		}
	}
}

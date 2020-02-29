using System;
using System.Net.Sockets;
using Moba.Data;
using Moba.Protocol;
using ReadyGamerOne.Script;
using UnityEngine.Assertions;

namespace Moba.Script
{
	public partial class MobaMgr
	{

		public bool debug_UdpSocket = false;
		
		partial void OnSafeAwake()
		{
			//初始化等级数据
			UlevelMgr.Instance.Init();
		}

		private void Start()
		{
			MainLoop.Instance.ExecuteLater(
				() => LogicServiceProxy.Instance.TestUdp(999),  
				3);

			if (debug_UdpSocket)
			{
				var s = new Socket(
					AddressFamily.InterNetwork,
					SocketType.Stream,
					ProtocolType.Udp);
				Assert.IsNotNull(s);
			}
		}
		
		
		
	}
}

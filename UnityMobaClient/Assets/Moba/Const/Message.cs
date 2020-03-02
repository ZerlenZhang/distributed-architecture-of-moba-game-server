namespace Moba.Const
{
	/// <summary>
	/// 自定义的消息写到这里
	/// </summary>
	public class Message
	{
		/// <summary>
		/// 同步网络信息
		/// </summary>
		public static readonly string SyncAuthInfo = "SyncAuthInfo";

		/// <summary>
		/// 升级游客账号
		/// </summary>
		public static readonly string UpgradeGuest = "UpgradeGuest";


		/// <summary>
		/// 注销账号
		/// </summary>
		public static readonly string Unregister = "Unregister";
		
		/// <summary>
		/// 同步游戏数据
		/// </summary>
		public static readonly string SyncUgameInfo = "SyncUgameInfo";

		/// <summary>
		/// 成功获取游戏信息
		/// </summary>
		public static readonly string GetUgameInfoSuccess = "GetUgameInfoSuccess";


		/// <summary>
		/// 成功登陆逻辑服务器
		/// </summary>
		public static readonly string LoginLogicServerSuccess = "LoginLogicServerSuccess";


		/// <summary>
		/// 有其他玩家进入游戏
		/// <PlayerEnter>res</PlayerEnter>
		/// </summary>
		public static readonly string PlayerEnterRoom = "PlayerArriveRoom";

		/// <summary>
		/// 有其他玩家进入游戏
		/// </summary>
		public static readonly string PlayerExitRoom = "PlayerLeaveRoom";


		/// <summary>
		/// 玩家成功离开房间
		/// </summary>
		public static readonly string LeaveRoomSuccess = "LeaveRoomSuccess";


		/// <summary>
		/// 游戏开始
		/// </summary>
		public static readonly string GameStart = "GameStart";

		/// <summary>
		/// 异步加载场景
		/// <string>sceneName</string>
		/// </summary>
		public static readonly string LoadSceneAsync = "LoadSceneAsync";


		/// <summary>
		/// 逻辑帧事件
		/// <LogicFrame></LogicFrame>
		/// </summary>
		public static readonly string OnLogicFrame = "OnLogicFrame";
	}
}

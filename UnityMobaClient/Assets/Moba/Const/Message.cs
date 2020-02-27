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
		public const string SyncAuthInfo = "SyncAuthInfo";

		/// <summary>
		/// 升级游客账号
		/// </summary>
		public const string UpgradeGuest = "UpgradeGuest";


		/// <summary>
		/// 注销账号
		/// </summary>
		public const string Unregister = "Unregister";
		
		/// <summary>
		/// 同步游戏数据
		/// </summary>
		public const string SyncUgameInfo = "SyncUgameInfo";

		/// <summary>
		/// 成功获取游戏信息
		/// </summary>
		public const string GetUgameInfoSuccess = "GetUgameInfoSuccess";


		/// <summary>
		/// 成功登陆逻辑服务器
		/// </summary>
		public const string LoginLogicServerSuccess = "LoginLogicServerSuccess";


		/// <summary>
		/// 有其他玩家进入游戏
		/// <PlayerEnter>res</PlayerEnter>
		/// </summary>
		public const string PlayerEnterRoom = "PlayerArriveRoom";

		/// <summary>
		/// 有其他玩家进入游戏
		/// </summary>
		public const string PlayerExitRoom = "PlayerLeaveRoom";


		/// <summary>
		/// 玩家成功离开房间
		/// </summary>
		public const string LeaveRoomSuccess = "LeaveRoomSuccess";


		/// <summary>
		/// 游戏开始
		/// </summary>
		public const string GameStart = "GameStart";
	}
}

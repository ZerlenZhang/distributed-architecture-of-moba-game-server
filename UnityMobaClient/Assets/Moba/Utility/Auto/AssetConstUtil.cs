namespace Moba.Utility
{
	/// <summary>
	/// 这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug
	/// </summary>
	public class AssetConstUtil : ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>
	{
		private System.Collections.Generic.Dictionary<string,string> nameToPath 
			= new System.Collections.Generic.Dictionary<string,string>{
					{ @"BattlePanel" , @"ClassPanel\BattlePanel" },
					{ @"ChatPanel" , @"ClassPanel\ChatPanel" },
					{ @"HomePanel" , @"ClassPanel\HomePanel" },
					{ @"LoadingPanel" , @"ClassPanel\LoadingPanel" },
					{ @"LoginPanel" , @"ClassPanel\LoginPanel" },
					{ @"Avator_0" , @"ClassSprite\roundheader\Avator_0" },
					{ @"Avator_1" , @"ClassSprite\roundheader\Avator_1" },
					{ @"Avator_2" , @"ClassSprite\roundheader\Avator_2" },
					{ @"Avator_3" , @"ClassSprite\roundheader\Avator_3" },
					{ @"Avator_4" , @"ClassSprite\roundheader\Avator_4" },
					{ @"Avator_5" , @"ClassSprite\roundheader\Avator_5" },
					{ @"Avator_6" , @"ClassSprite\roundheader\Avator_6" },
					{ @"Avator_7" , @"ClassSprite\roundheader\Avator_7" },
					{ @"Avator_8" , @"ClassSprite\roundheader\Avator_8" },
					{ @"LoginBonues" , @"ClassUi\LoginBonues" },
					{ @"MatchDlgUi" , @"ClassUi\MatchDlgUi" },
					{ @"PlayerInfo" , @"ClassUi\PlayerInfo" },
					{ @"SelfUi" , @"ClassUi\SelfUi" },
					{ @"StatusUi" , @"ClassUi\StatusUi" },
					{ @"TalkUi" , @"ClassUi\TalkUi" },
					{ @"UserInfoDlg" , @"ClassUi\UserInfoDlg" },
					{ @"TimeLine" , @"TimeLine" },
				};
		public override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;
	}
}

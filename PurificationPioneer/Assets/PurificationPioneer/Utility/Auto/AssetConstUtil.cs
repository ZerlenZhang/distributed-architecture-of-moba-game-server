namespace PurificationPioneer.Utility
{
	/// <summary>
	/// 这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug
	/// </summary>
	public partial class AssetConstUtil : ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>
	{
		private System.Collections.Generic.Dictionary<string,string> nameToPath 
			= new System.Collections.Generic.Dictionary<string,string>{
					{ @"Character0" , @"ClassCharacter\Character0" },
					{ @"Character1" , @"ClassCharacter\Character1" },
					{ @"Character2" , @"ClassCharacter\Character2" },
					{ @"Character3" , @"ClassCharacter\Character3" },
					{ @"Character4" , @"ClassCharacter\Character4" },
					{ @"CharacterHeadCanvas" , @"ClassLocalAsset\CharacterHeadCanvas" },
					{ @"LocalCamera" , @"ClassLocalAsset\LocalCamera" },
					{ @"AndroidBattlePanel" , @"ClassPanel\AndroidBattlePanel" },
					{ @"BattlePanel" , @"ClassPanel\BattlePanel" },
					{ @"DebugPanel" , @"ClassPanel\DebugPanel" },
					{ @"HomePanel" , @"ClassPanel\HomePanel" },
					{ @"LoadingPanel" , @"ClassPanel\LoadingPanel" },
					{ @"MatchPanel" , @"ClassPanel\MatchPanel" },
					{ @"WelcomePanel" , @"ClassPanel\WelcomePanel" },
					{ @"Avatar0" , @"ClassSprite\UserIcons\Avatar0" },
					{ @"Avatar1" , @"ClassSprite\UserIcons\Avatar1" },
					{ @"Avatar2" , @"ClassSprite\UserIcons\Avatar2" },
					{ @"Avatar3" , @"ClassSprite\UserIcons\Avatar3" },
					{ @"Avatar4" , @"ClassSprite\UserIcons\Avatar4" },
					{ @"Avatar5" , @"ClassSprite\UserIcons\Avatar5" },
					{ @"Avatar6" , @"ClassSprite\UserIcons\Avatar6" },
					{ @"Avatar7" , @"ClassSprite\UserIcons\Avatar7" },
					{ @"HeroOption" , @"ClassUi\HeroOption" },
					{ @"LoadMatcherUi" , @"ClassUi\LoadMatcherUi" },
					{ @"MatcherRect" , @"ClassUi\MatcherRect" },
					{ @"Weapon0" , @"ClassWeapon\Weapon0" },
				};
		public override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;
	}
}

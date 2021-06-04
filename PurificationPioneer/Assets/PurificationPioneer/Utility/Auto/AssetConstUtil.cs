namespace PurificationPioneer.Utility
{
	/// <summary>
	/// 这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug
	/// </summary>
	public partial class AssetConstUtil : ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>
	{
		private System.Collections.Generic.Dictionary<string,string> nameToPath 
			= new System.Collections.Generic.Dictionary<string,string>{
					{ @"Advanture" , @"ClassAudio\bgm\Advanture" },
					{ @"Dialog" , @"ClassAudio\bgm\Dialog" },
					{ @"SadStory" , @"ClassAudio\bgm\SadStory" },
					{ @"WakeUp" , @"ClassAudio\bgm\WakeUp" },
					{ @"War" , @"ClassAudio\bgm\War" },
					{ @"Character0" , @"ClassCharacter\Character0" },
					{ @"Character1" , @"ClassCharacter\Character1" },
					{ @"Character2" , @"ClassCharacter\Character2" },
					{ @"Character3" , @"ClassCharacter\Character3" },
					{ @"Character4" , @"ClassCharacter\Character4" },
					{ @"CaptionChooseUi" , @"ClassDialog\CaptionChooseUi" },
					{ @"CaptionNarratorUi" , @"ClassDialog\CaptionNarratorUi" },
					{ @"CaptionWordUi" , @"ClassDialog\CaptionWordUi" },
					{ @"ChoiseBtn" , @"ClassDialog\ChoiseBtn" },
					{ @"CharacterHeadCanvas" , @"ClassLocalAsset\CharacterHeadCanvas" },
					{ @"LocalCamera" , @"ClassLocalAsset\LocalCamera" },
					{ @"ThirdPersonCamera Variant" , @"ClassLocalAsset\ThirdPersonCamera Variant" },
					{ @"AndroidBattlePanel" , @"ClassPanel\AndroidBattlePanel" },
					{ @"BattlePanel" , @"ClassPanel\BattlePanel" },
					{ @"BoxPanel" , @"ClassPanel\BoxPanel" },
					{ @"CharacterPanel" , @"ClassPanel\CharacterPanel" },
					{ @"DebugPanel" , @"ClassPanel\DebugPanel" },
					{ @"GameEndPanel" , @"ClassPanel\GameEndPanel" },
					{ @"HomePanel" , @"ClassPanel\HomePanel" },
					{ @"LoadingPanel" , @"ClassPanel\LoadingPanel" },
					{ @"MainCityPanel" , @"ClassPanel\MainCityPanel" },
					{ @"MatchPanel" , @"ClassPanel\MatchPanel" },
					{ @"SimpleLoadingPanel" , @"ClassPanel\SimpleLoadingPanel" },
					{ @"WakeUpPanel" , @"ClassPanel\WakeUpPanel" },
					{ @"WelcomePanel" , @"ClassPanel\WelcomePanel" },
					{ @"Avatar0" , @"ClassSprite\UserIcons\Avatar0" },
					{ @"Avatar1" , @"ClassSprite\UserIcons\Avatar1" },
					{ @"Avatar2" , @"ClassSprite\UserIcons\Avatar2" },
					{ @"Avatar3" , @"ClassSprite\UserIcons\Avatar3" },
					{ @"Avatar4" , @"ClassSprite\UserIcons\Avatar4" },
					{ @"Avatar5" , @"ClassSprite\UserIcons\Avatar5" },
					{ @"Avatar6" , @"ClassSprite\UserIcons\Avatar6" },
					{ @"Avatar7" , @"ClassSprite\UserIcons\Avatar7" },
					{ @"CharacterIntroBarUi" , @"ClassUi\CharacterIntroBarUi" },
					{ @"HeroOption" , @"ClassUi\HeroOption" },
					{ @"LoadMatcherUi" , @"ClassUi\LoadMatcherUi" },
					{ @"MatcherRect" , @"ClassUi\MatcherRect" },
					{ @"StoryUnitUi" , @"ClassUi\StoryUnitUi" },
					{ @"Weapon0" , @"ClassWeapon\Weapon0" },
					{ @"DOTweenSettings" , @"DOTweenSettings" },
				};
		public override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;
	}
}

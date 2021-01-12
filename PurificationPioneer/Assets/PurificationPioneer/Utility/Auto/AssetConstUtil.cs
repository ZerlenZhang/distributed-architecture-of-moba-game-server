namespace PurificationPioneer.Utility
{
	/// <summary>
	/// 这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug
	/// </summary>
	public partial class AssetConstUtil : ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>
	{
		private System.Collections.Generic.Dictionary<string,string> nameToPath 
			= new System.Collections.Generic.Dictionary<string,string>{
					{ @"Character3" , @"ClassCharacter\Character3" },
					{ @"BulletData_BulletData" , @"ClassFile\BulletData_BulletData" },
					{ @"DataConfig" , @"ClassFile\DataConfig" },
					{ @"LocalCamera" , @"ClassLocalAsset\LocalCamera" },
					{ @"AndroidBattlePanel" , @"ClassPanel\AndroidBattlePanel" },
					{ @"DebugPanel" , @"ClassPanel\DebugPanel" },
					{ @"HomePanel" , @"ClassPanel\HomePanel" },
					{ @"LoadingPanel" , @"ClassPanel\LoadingPanel" },
					{ @"MatchPanel" , @"ClassPanel\MatchPanel" },
					{ @"WelcomePanel" , @"ClassPanel\WelcomePanel" },
					{ @"HeroIcon0" , @"ClassSprite\ClassHeroIcon\HeroIcon0" },
					{ @"HeroIcon1" , @"ClassSprite\ClassHeroIcon\HeroIcon1" },
					{ @"HeroIcon2" , @"ClassSprite\ClassHeroIcon\HeroIcon2" },
					{ @"HeroIcon3" , @"ClassSprite\ClassHeroIcon\HeroIcon3" },
					{ @"Avatar0" , @"ClassSprite\ClassUserIcon\Avatar0" },
					{ @"Avatar1" , @"ClassSprite\ClassUserIcon\Avatar1" },
					{ @"Avatar2" , @"ClassSprite\ClassUserIcon\Avatar2" },
					{ @"Avatar3" , @"ClassSprite\ClassUserIcon\Avatar3" },
					{ @"Avatar4" , @"ClassSprite\ClassUserIcon\Avatar4" },
					{ @"Avatar5" , @"ClassSprite\ClassUserIcon\Avatar5" },
					{ @"Avatar6" , @"ClassSprite\ClassUserIcon\Avatar6" },
					{ @"Avatar7" , @"ClassSprite\ClassUserIcon\Avatar7" },
					{ @"HeroOption" , @"ClassUi\HeroOption" },
					{ @"LoadMatcherUi" , @"ClassUi\LoadMatcherUi" },
					{ @"MatcherRect" , @"ClassUi\MatcherRect" },
					{ @"Weapon0" , @"ClassWeapon\Weapon0" },
				};
		public override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;
	}
}

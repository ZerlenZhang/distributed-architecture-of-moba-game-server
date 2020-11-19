namespace PurificationPioneer.Utility
{
	/// <summary>
	/// 这个类提供了Resources下文件名和路径字典访问方式，同名资源可能引起bug
	/// </summary>
	public class AssetConstUtil : ReadyGamerOne.MemorySystem.AssetConstUtil<AssetConstUtil>
	{
		private System.Collections.Generic.Dictionary<string,string> nameToPath 
			= new System.Collections.Generic.Dictionary<string,string>{
					{ @"HomePanel" , @"ClassPanel\HomePanel" },
					{ @"WelcomePanel" , @"ClassPanel\WelcomePanel" },
				};
		public override System.Collections.Generic.Dictionary<string,string> NameToPath => nameToPath;
	}
}

using PurificationPioneer.Network.ProtoGen;

namespace PurificationPioneer.Global
{
	/// <summary>
	/// 这里写当前项目需要的全局变量，调用GlobalVar时，最好调用当前这个类
	/// </summary>
	public class GlobalVar : ReadyGamerOne.Global.GlobalVar
	{
		public static string uname;
		public static string unick;
		public static int ucoin;
		public static int uface;
		public static int ulevel;
		public static int urank;
		public static int udiamond;
		public static string usignature;

		public static void SaveInfo(UserAccountInfo uinfo)
		{
			uname = uinfo.uname;
			unick = uinfo.unick;
			ucoin = uinfo.ucoin;
			uface = uinfo.uface;
			ulevel = uinfo.ulevel;
			urank = uinfo.urank;
			udiamond = uinfo.udiamond;
			usignature = uinfo.usignature;
		}
	}
}

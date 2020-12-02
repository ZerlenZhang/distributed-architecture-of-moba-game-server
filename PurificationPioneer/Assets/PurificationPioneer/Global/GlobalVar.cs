using System.Collections.Generic;
using PurificationPioneer.Network.ProtoGen;
using UnityEngine;

namespace PurificationPioneer.Global
{
	/// <summary>
	/// 这里写当前项目需要的全局变量，调用GlobalVar时，最好调用当前这个类
	/// </summary>
	public class GlobalVar : ReadyGamerOne.Global.GlobalVar
	{
		#region UserInfo

		public static string Uname{ get; private set; }
		public static string Unick{ get; private set; }
		public static int Ucoin{ get; private set; }
		public static int Uface{ get; private set; }
		public static int Ulevel{ get; private set; }
		public static int Urank{ get; private set; }
		public static int Udiamond{ get; private set; }
		public static string Usignature{ get; private set; }
		public static int Uexp{ get; private set; }
		public static List<int> HeroIds{ get; private set; }
		public static void SaveInfo(UserAccountInfo uinfo)
		{
			Uname = uinfo.uname;
			Unick = uinfo.unick;
			Ucoin = uinfo.ucoin;
			Uface = uinfo.uface;
			Ulevel = uinfo.ulevel;
			Urank = uinfo.urank;
			Udiamond = uinfo.udiamond;
			Usignature = uinfo.usignature;
			Uexp = uinfo.uexp;
			if (null == uinfo.heros)
			{
				Debug.LogError($"uinfo.heros is null");
			}
			
			HeroIds=new List<int>();
			foreach (var idStr in uinfo.heros.Split(','))
			{
				if(string.IsNullOrEmpty(idStr))
					continue;
				HeroIds.Add(int.Parse(idStr));
			}
		}		

		#endregion
		
		#region MatchInfo

		public static int SelectHeroTime { get; private set; }
		public static int SeatId{ get; private set; }
		public static bool IsSubmit { get; private set; }
		public static void SetSubmit(bool value) => IsSubmit = value;
		public static List<MatchInfo> MatcherInfos{ get; private set; }
		public static void OnFinishMatchTick(FinishMatchTick finishMatchTick)
		{
			SelectHeroTime = finishMatchTick.heroSelectTime;
			MatcherInfos = finishMatchTick.matchers;
			foreach (var matcherInfo in MatcherInfos)
			{
				if (matcherInfo.unick == Unick)
				{
					SeatId = matcherInfo.seatId;
					break;
				}
			}
		}		

		#endregion
	}
}

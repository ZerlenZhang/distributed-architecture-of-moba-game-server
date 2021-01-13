using System.Collections.Generic;
using PurificationPioneer.Network.ProtoGen;
using PurificationPioneer.Scriptable;
using PurificationPioneer.Utility;
using ReadyGamerOne.MemorySystem;
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
		public static void SaveUserInfo(UserAccountInfo uinfo)
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
		
		#region MatcherInfo

		public class MatcherInfo
		{
			//userInfo
			public int SeatId { get; private set; } = -1;
			public string Unick { get; private set; }
			public int Urank { get; private set; } = 1;
			public int Uface { get; private set; } = -1;
			public int Ulevel { get; private set; } = 0;
			//matchInfo
			public int HeroId { get; private set; } = -1;
			public int WeaponId { get; private set; } = 0;
			public int SetHeroId(int heroId) => HeroId = heroId;
			public bool IsSubmit { get; private set; } = false;
			public bool SetSubmitted() => IsSubmit = true;
			
			public HeroConfigAsset HeroConfig { get; private set; }
			public void SetHeroConfig(HeroConfigAsset config) => HeroConfig = config;

			public MatcherInfo(MatchInfo matchInfo)
			{
				SeatId = matchInfo.seatId;
				Unick = matchInfo.unick;
				Urank = matchInfo.urank;
				Uface = matchInfo.uface;
				Ulevel = matchInfo.ulevel;
			}
		}
		
		public static int SelectHeroTime { get; private set; }
		public static int RoomType { get; private set; }
		public static void SetRoomType(int roomType) => RoomType = roomType;
		public static int RoomId { get; private set; }
		public static Dictionary<int, MatcherInfo> SeatId_MatcherInfo{ get; private set; }

		private static MatcherInfo self;
		public static int LocalSeatId => self.SeatId;
		public static bool IsLocalSubmit => self.IsSubmit;
		public static int LocalHeroId => self.HeroId;
		public static int LocalWeaponId => self.WeaponId;
		public static HeroConfigAsset LocalHeroConfig => self.HeroConfig;
		public static void SaveMatchers(FinishMatchTick finishMatchTick)
		{
			SelectHeroTime = finishMatchTick.heroSelectTime;
			RoomId = finishMatchTick.roomId;
			SeatId_MatcherInfo = new Dictionary<int, MatcherInfo>();
			
			foreach (var matchInfo in finishMatchTick.matchers)
			{
				var matcherInfo = new MatcherInfo(matchInfo);
				
				if (matchInfo.unick == Unick)
				{
					self = matcherInfo;
				}
				
				SeatId_MatcherInfo.Add(matchInfo.seatId, matcherInfo);
			}
		}

		public static void OnSelectHero(SelectHeroRes selectHeroRes)
		{
			if (!SeatId_MatcherInfo.ContainsKey(selectHeroRes.seatId))
			{
				Debug.LogError($"Unexcepted seatId: {selectHeroRes.seatId}");
				return;
			}

			SeatId_MatcherInfo[selectHeroRes.seatId].SetHeroId(selectHeroRes.hero_id);
		}

		public static void OnSubmitHero(SubmitHeroRes submitHeroRes)
		{
			if (!SeatId_MatcherInfo.ContainsKey(submitHeroRes.seatId))
			{
				Debug.LogError($"Unexcepted seatId: {submitHeroRes.seatId}");
				return;
			}

			var matcherInfo = SeatId_MatcherInfo[submitHeroRes.seatId];
			matcherInfo.SetSubmitted();
			matcherInfo.SetHeroConfig(
				ResourceMgr.GetAsset<HeroConfigAsset>(AssetConstUtil.GetHeroConfigKey(matcherInfo.HeroId)));

		}

		#endregion
		
		#region GameInfo

		public static int LogicFrameDeltaTime { get; private set; }
		public static int StartGameDelay { get; private set; }
		public static int GameTime { get; private set; }
		
		public static void SaveGameInfos(StartGameRes startGameRes)
		{
			LogicFrameDeltaTime = startGameRes.logicFrameDeltaTime;
			GameTime = startGameRes.gameTime;
			StartGameDelay = startGameRes.startGameDelay;
			//RandSeed
		}

		#endregion

		#region GameConfigs

		public const int MaxHitInfoCount = 3;

		#endregion
	}
}

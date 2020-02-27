using System.Collections.Generic;
using gprotocol;
using UnityEngine;
using UnityEngine.Assertions;

namespace Moba.Global
{
    public static class NetInfo
    {
        public static string unick { get; private set; }="Guest";
        public static int uface{ get; private set; }
        public static int usex{ get; private set; }
        public static int uvip{ get; private set; }
        public static bool isGuest { get; private set; } = true;

        //public static string guest_key { get; private set; } = null;

        public static void SetIsGuest(bool value)
        {
            isGuest = value;
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="info"></param>
        public static void SaveInfo(UserCenterInfo info,bool isGuest)//,string guestKey=null)
        {
            Assert.IsNotNull(info);
            unick = info.unick;
            uface = info.uface;
            usex = info.usex;
            uvip = info.uvip;
            NetInfo.isGuest = isGuest;
            //guest_key = guestKey;
        }

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="unick"></param>
        /// <param name="uface"></param>
        /// <param name="usex"></param>
        public static void SaveEditProfile(string unick,int uface,int usex)
        {
            Debug.Log("记录：nick: " + unick + " uface: " + uface + " usex: " + usex);
            NetInfo.unick = unick;
            NetInfo.uface = uface;
            NetInfo.usex = usex;
        }

        public static UserGameInfo gameInfo { get; private set; } = null;

        /// <summary>
        /// 保存游戏信息
        /// </summary>
        /// <param name="userGameInfo"></param>
        public static void SaveUgameInfo(UserGameInfo userGameInfo)
        {
            gameInfo = userGameInfo;
        }

        public static void RecvLoginBonues()
        {
            gameInfo.ucoin_1 += gameInfo.bonues;
            gameInfo.bonues_status = 1;
        }


        public static int zoneId { get; private set; } = -1;

        public static void SetZoneId(int id)
        {
            zoneId = id;
        }

        public static List<PlayerEnterRoom> otherPlayers = new List<PlayerEnterRoom>();

    }
}
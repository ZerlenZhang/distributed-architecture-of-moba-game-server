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
    }
}
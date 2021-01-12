using ReadyGamerOne.MemorySystem;
using UnityEngine;

namespace PurificationPioneer.Utility
{
    public partial class AssetConstUtil
    {
        private const string HeroIconKey = "HeroIcon";
        private const string UserIconKey = "Avatar";
        private const string HeroConfigKey = "Character";
        private const string WeaponConfigKey = "Weapon";

        public static string GetWeaponConfigKey(int weaponId)
        {
            return $"{WeaponConfigKey}{weaponId}";
        }

        public static string GetHeroConfigKey(int heroId)
        {
            return $"{HeroConfigKey}{heroId}";
        }
        
        public static Sprite GetHeroIcon(int heroId)
        {
            return ResourceMgr.GetAsset<Sprite>($"{HeroIconKey}{heroId}");
        }

        public static Sprite GetUserIcon(int uface)
        {
            return ResourceMgr.GetAsset<Sprite>($"{UserIconKey}{uface}");
        }
    }
}
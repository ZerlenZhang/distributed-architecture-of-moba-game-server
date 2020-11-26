using UnityEngine;

namespace PurificationPioneer.Global
{
    public static class GlobalPref
    {
        private const string AccountKey = "AccountKey";
        private const string PwdKey = "PwdKey";

        public static string Account
        {
            get => PlayerPrefs.GetString(AccountKey, "");
            set => PlayerPrefs.SetString(AccountKey, value);
        }

        public static string Pwd
        {
            get => PlayerPrefs.GetString(PwdKey, "");
            set => PlayerPrefs.SetString(PwdKey, value);
        }

        public static string GetAccount(string defaultValue = "")
        {
            return PlayerPrefs.GetString(AccountKey, defaultValue);
        }
    }
}
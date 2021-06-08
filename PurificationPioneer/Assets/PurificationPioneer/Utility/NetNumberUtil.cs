using System;

namespace PurificationPioneer.Utility
{
    public static class NetNumberUtil
    {
        private const int FLAG = 1000;
        public static float ToFloat(this int netNumber)
        {
            return netNumber / (float) FLAG;
        }

        public static int ToInt(this float localNumber)
        {
            return Convert.ToInt32(localNumber * FLAG);
        }
    }
}
namespace ReadyGamerOne.Utility
{
    public static class RandomUtil
    {
        public static string RandomStr(int len) {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            var r = new System.Random(System.BitConverter.ToInt32(b, 0));

            string str = null;
            str += "0123456789";
            str += "abcdefghijklmnopqrstuvwxyz";
            str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string s = null;

            for (var i = 0; i < len; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }
}
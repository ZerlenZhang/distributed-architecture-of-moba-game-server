using System.Text;

namespace ReadyGamerOne.Utility
{
    public static class BitUtil
    {
        public static string ToString(byte[] data, int length, int start=0)
        {
            var sb=new StringBuilder();
            for(var i=start;i<length;i++)
            {
                sb.Append(data[i].ToString());
            }

            return sb.ToString();
        }
    }
}
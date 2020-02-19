using System.Security.Cryptography;
using System.Text;

namespace ReadyGamerOne.Utility
{
    public class SecurityUtil
    {
        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            var md5Builder = new StringBuilder();
            var md5 = MD5.Create();
            //加密后的是一个字节类型数组，这里要注意编码UTF8/Unicode等的选择
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            //将字节类型数组转化为字符串，此字符串是常规字符串
            for (int i = 0; i < s.Length; i++)
            {
                //将得到的字符串使用十六进制类型格式，格式后的字符串是小写的字母
                md5Builder.Append(s[i].ToString("X2"));
            }

            return md5Builder.ToString();
        }
    }
}
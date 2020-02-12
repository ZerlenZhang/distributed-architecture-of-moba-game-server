using System;

namespace ReadyGamerOne.Utility
{
    public static class StringExtension
    {
        public static int FindFirstSubstring(this string sour, string substring)
        {
            var length = sour.Length;
            var times = length - substring.Length + 1;
            if (times == 0)
                throw new Exception("substring 比 sour 短，怎么找Index？");

            for (var i = 0; i < times; i++)
            {
                var temp = sour.Substring(i, length - i);
                if (temp.StartsWith(substring))
                    return i;
            }
            return -1;
        }

        public static string GetAfterSubstring(this string sour, string substring)
        {
            var index = sour.FindFirstSubstring(substring);
            index += substring.Length;
            return sour.Substring(index, sour.Length - index);
        }

        public static string GetBeforeSubstring(this string sour, string substring)
        {
            var index = sour.FindFirstSubstring(substring);
            return sour.Substring(0, index);
        }

        public static string GetAfterLastChar(this string sour, char c)
        {
            var index = sour.LastIndexOf(c);
            return sour.Substring(index+1,sour.Length-(1+index));
        }
    }
}
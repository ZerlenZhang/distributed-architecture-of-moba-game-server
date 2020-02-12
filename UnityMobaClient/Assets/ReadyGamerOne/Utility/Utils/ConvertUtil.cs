using System;
using UnityEngine;

namespace ReadyGamerOne.Utility
{
    public static class ConvertUtil
    {
        public static Vector3 String2Vector3(string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new Exception("字符串为空");
            var s = str.Substring(1, str.Length - 2);
            if(s.Length<=3)
                throw new Exception("所含float数据个数小于三");
            var arr = s.Split(',');
            return new Vector3(Convert.ToSingle(arr[0]),Convert.ToSingle(arr[1]),Convert.ToSingle(arr[2]));

        }
    }
}
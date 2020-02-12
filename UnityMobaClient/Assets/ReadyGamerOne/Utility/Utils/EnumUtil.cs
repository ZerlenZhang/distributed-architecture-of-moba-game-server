using System;
using System.Collections.Generic;

namespace ReadyGamerOne.Utility
{
    /// <summary>
    /// 枚举工具类
    /// 提供，枚举值，枚举名字，枚举Index之间的互相转化
    /// </summary>
    public static class EnumUtil
    {
        #region Private

        private class EnumInfo
        {
            public string EnumName;
            public List<int> values = new List<int>();
            public List<string> names = new List<string>();

            public EnumInfo(Type type)
            {
                this.EnumName = type.Name;
                foreach (var name in Enum.GetNames(type))
                {
                    names.Add(name);
                }

                foreach (var value in Enum.GetValues(type))
                {
                    values.Add((int)value);
                }
            }
        }

        private static Dictionary<Type, EnumInfo> _enumInfos = new Dictionary<Type, EnumInfo>();

        private static EnumInfo GetEnumInfo(Type type)
        {
            if (_enumInfos.ContainsKey(type))
                return _enumInfos[type];
            
            var info=new EnumInfo(type);
            _enumInfos.Add(type, info);
            return info;
        }        

        #endregion

        public static int GetEnumIndex<T>(T enumVar)
        {
            var index = 0;
            foreach (var VARIABLE in GetEnumInfo(typeof(T)).names)
            {
                if (VARIABLE == enumVar.ToString())
                    return index;
                index++;
            }
            throw new Exception("获取枚举Index异常");
        }
        public static int GetEnumValue<T>(int index)
        {
            return GetEnumInfo(typeof(T)).values[index];
        }

        public static List<string> GetNames<T>()
        {
            return GetEnumInfo(typeof(T)).names;
        }

        public static List<int> GetValues<T>()
        {
            return GetEnumInfo(typeof(T)).values;
        }

        public static string GetEnumName<T>(int index)
        {
            return GetEnumInfo(typeof(T)).names[index];
        }

        public static void Clear()
        {
            _enumInfos.Clear();
        }
    }
}
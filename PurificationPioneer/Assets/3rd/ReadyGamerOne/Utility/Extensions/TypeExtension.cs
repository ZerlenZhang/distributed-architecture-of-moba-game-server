using System;
using System.Reflection;

namespace ReadyGamerOne.Utility
{
    public static class TypeExtension
    {
        /// <summary>
        /// 获取Type的特性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type type, bool inherit = true)
            where T : System.Attribute
        {
            foreach (var attribute in type.GetCustomAttributes(inherit))
            {
                if (attribute is T)
                    return (T)attribute;
            }

            return null;
        }

        /// <summary>
        /// 根据名字获取父类中的方法
        /// </summary>
        /// <param name="self"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodFromBase(this Type self, string methodName)
        {
            var temp = self;
            while (null!=temp)
            {
                var method = temp.GetMethod(methodName);
                if (method != null)
                    return method;
                temp = temp.BaseType;
            }

            return null;
        }
    }
}
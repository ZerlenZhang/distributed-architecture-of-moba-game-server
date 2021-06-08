using System;

namespace ReadyGamerOne.Rougelike.Person
{
    /// <summary>
    /// 特性类，标记Person类使用哪个类数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UseCsvDataAttribute : System.Attribute
    {
        public Type dataType;

        public UseCsvDataAttribute(Type type)
        {
            dataType = type;
        } 
    }
}
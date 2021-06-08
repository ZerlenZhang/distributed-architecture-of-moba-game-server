using System;

namespace ReadyGamerOne.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptableSingletonInfoAttribute:Attribute
    {
        public string assetName;

        public ScriptableSingletonInfoAttribute(string assetName = null)
        {
            this.assetName = assetName;
        }
    }
}
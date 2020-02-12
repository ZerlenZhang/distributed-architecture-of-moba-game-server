using System.Collections.Generic;
using ReadyGamerOne.Common;

namespace ReadyGamerOne.Const
{
    public class ConstUtil<T>:
        Singleton<ConstUtil<T>>
    {
        protected virtual Dictionary<string, string> nameToPath => null;

        public string GetPath(string resName)
        {
            if (!nameToPath.ContainsKey(resName))
                throw new System.Exception("没有这个资源名：" + resName);
            return nameToPath[resName];
        }
    }
}
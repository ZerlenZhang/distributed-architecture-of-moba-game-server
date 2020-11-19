using System.IO;

namespace ReadyGamerOne.Utility
{
    public static class DirectoryInfoExtension
    {
        /// <summary>
        /// 判断某目录下是否有Unity有效文件
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool HasValuableFiles(this DirectoryInfo directory)
        {
            foreach (var fileInfo in directory.GetFiles())
            {
                if (!fileInfo.FullName.EndsWith(".meta"))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// 获取以Key为标识的特殊路径
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSpecialPath(this DirectoryInfo dir, string key)
        {
            var path = "";
            while (dir.Name.StartsWith(key))
            {
                path = "/" + dir.Name.GetAfterSubstring(key) +  path;
                dir = dir.Parent;
            }

            return path;
        }
    }
}
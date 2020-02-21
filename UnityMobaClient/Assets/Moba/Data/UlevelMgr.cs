using System.Linq;
using ReadyGamerOne.Common;

namespace Moba.Data
{
    public class UlevelMgr:Singleton<UlevelMgr>
    {
        private int[] levelMap;
        public void Init()
        {
            this.levelMap = new[]
            {
                0,
                100,
                200,
                300,
                400,
                500,
                600,
                700,
                800,
                900,
            };
        }

        public int GetLevelInfo(int uexp,out int nowExp,out int nextLevelExp)
        {
            nowExp = 0;
            nextLevelExp = 0;

            var level = 0;
            var lastExp = 0;//还剩多少经验
            while (level + 1 < this.levelMap.Length
                   && lastExp>=this.levelMap[level+1])
            {
                lastExp -= this.levelMap[level];
                level++;
            }

            nowExp = lastExp;
            if (level == this.levelMap.Length - 1)
            {
                nextLevelExp = this.levelMap.Last();
            }
            else
            {
                nextLevelExp = this.levelMap[level];
            }

            return level;
        }
    }
}
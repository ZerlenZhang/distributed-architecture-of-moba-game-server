namespace ReadyGamerOne.Utility
{
    public static class IntExtension
    {
        /// <summary>
        /// 获取Int数从右往左第Index位上的数字，只会返回 0 或 1
        /// </summary>
        /// <param name="self"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetNumAtBinary(this int self, int index)
        {
            return (((self & (1 << index)) >> index) == 1) ? 1 : 0;
        }
    }
}
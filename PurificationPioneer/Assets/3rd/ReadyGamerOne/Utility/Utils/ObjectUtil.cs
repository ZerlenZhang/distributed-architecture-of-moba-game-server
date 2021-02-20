namespace ReadyGamerOne.Utility
{
    public static class ObjectUtil
    {
        public static void SwapReference<T>(ref T a, ref T b)
            where T:class
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }
}
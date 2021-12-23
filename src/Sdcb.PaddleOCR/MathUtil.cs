namespace Sdcb.PaddleOCR
{
    internal static class MathUtil
    {
        public static int Clamp(int val, int min, int max)
        {
#if NET6_0_OR_GREATER
            return Math.Clamp(val, min, max);
#else
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }
            return val;
#endif
        }
    }
}

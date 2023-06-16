using System;

namespace Sdcb.PaddleDetection;

internal static class MathUtil
{
    internal static float Round(float x)
    {
#if NET6_0_OR_GREATER
        return MathF.Round(x);
#else
        return (float)Math.Round(x);
#endif
    }

    internal static float Ceiling(float x)
    {
#if NET6_0_OR_GREATER
        return MathF.Ceiling(x);
#else
        return (float)Math.Ceiling(x);
#endif
    }

    internal static float Exp(float x)
    {
#if NET6_0_OR_GREATER
        return MathF.Exp(x);
#else
        return (float)Math.Exp(x);
#endif
    }
}

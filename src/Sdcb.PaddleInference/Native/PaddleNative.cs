using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
[assembly: InternalsVisibleTo("Sdcb.PaddleInference.Tests")]

namespace Sdcb.PaddleInference.Native;

public partial class PaddleNative
{
    static PaddleNative()
    {
#if LINQPAD || NET6_0_OR_GREATER
        PaddleInferenceLibLoader.Init();
#elif NETSTANDARD2_0_OR_GREATER || NET45_OR_GREATER
		PaddleInferenceLibLoader.WindowsLoad();
#endif
    }

    /// <summary>
    /// Path of the Paddle Inference C library.
    /// </summary>
    public const string PaddleInferenceCLib =
#if NET45_OR_GREATER
            @"dll\x64\paddle_inference_c.dll";
#elif NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER || LINQPAD
            @"paddle_inference_c";
#endif
}

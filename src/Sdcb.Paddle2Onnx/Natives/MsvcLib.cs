using System;
using System.Runtime.InteropServices;

namespace Sdcb.Paddle2Onnx.Natives;

internal class MsvcLib
{
    [DllImport("api-ms-win-crt-heap-l1-1-0.dll", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr free(IntPtr ptr);
}

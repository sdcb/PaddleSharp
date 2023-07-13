using System;
using System.Text;

namespace Sdcb.Paddle2Onnx;

internal static class StringHelper
{
    public unsafe static string PtrToStringAnsi(this IntPtr ptr, int maxLength)
    {
        if (ptr == IntPtr.Zero)
            throw new ArgumentException("ptr is null");

        int length = 0;
        sbyte* psbyte = (sbyte*)ptr;
        while (length < maxLength && psbyte[length] != 0)
            length++;
        return new string(psbyte, 0, length, Encoding.UTF8);
    }
}

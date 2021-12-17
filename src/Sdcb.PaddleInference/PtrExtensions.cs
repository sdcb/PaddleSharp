using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.PaddleInference
{
    internal static class PtrExtensions
    {
#if NETSTANDARD2_1
        public static string AsUtf8String(this IntPtr ptr)
        {
            return Marshal.PtrToStringUTF8(ptr);
        }
#else
        public unsafe static string UTF8PtrToString(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;
            int length = 0;
            sbyte* psbyte = (sbyte*)ptr;
            while (psbyte[length] != 0)
                length++;
            return new string(psbyte, 0, length, Encoding.UTF8);
        }
#endif
    }
}

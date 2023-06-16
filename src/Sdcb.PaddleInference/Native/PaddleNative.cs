using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference.Native;

public partial class PaddleNative
{
    internal static void AddLibPathToEnvironment(string libPath)
    {
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
			const bool isWindows = true;
#endif
        string envId = isWindows ? "PATH" : "LD_LIBRARY_PATH";
        Environment.SetEnvironmentVariable(envId, Environment.GetEnvironmentVariable(envId) + Path.PathSeparator + libPath);
    }

    private unsafe struct PdStringArray
    {
#pragma warning disable CS0649
        public nint Size;
        public byte** Data;
#pragma warning restore CS0649

        public string[] ToArray()
        {
            var result = new string[Size];
            for (int i = 0; i < Size; ++i)
            {
                result[i] = ((IntPtr)Data[i]).UTF8PtrToString()!;
            }
            return result;
        }
    }

    public unsafe ref struct PdStringArrayWrapper
    {
        public IntPtr ptr;

        public unsafe string[] ToArray()
        {
            return ((PdStringArray*)ptr)->ToArray();
        }

        public void Dispose()
        {
            PD_OneDimArrayCstrDestroy(ptr);
            ptr = IntPtr.Zero;
        }
    }

    private unsafe struct PdIntArray
    {
        public nint Size;
        public int* Data;

        public int[] ToArray()
        {
            var result = new int[Size];
            for (int i = 0; i < Size; ++i)
            {
                result[i] = Data[i];
            }
            return result;
        }

        public unsafe void Dispose()
        {
            fixed (PdIntArray* ptr = &this)
            {
                PD_OneDimArrayInt32Destroy((IntPtr)ptr);
            }
        }
    }

    public ref struct PdIntArrayWrapper
    {
        public IntPtr ptr;

        public unsafe int[] ToArray()
        {
            return ((PdIntArray*)ptr)->ToArray();
        }

        public void Dispose()
        {
            PD_OneDimArrayInt32Destroy(ptr);
            ptr = IntPtr.Zero;
        }
    }

    public const string PaddleInferenceCLib =
        #if NET45_OR_GREATER
            @"dll\x64\paddle_inference_c.dll";
        #elif NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER || LINQPAD
            @"paddle_inference_c";
        #endif

    [StructLayout(LayoutKind.Sequential)]
    internal struct PdCStr
    {
        public uint Length;
        public IntPtr Data;

        public override string? ToString()
        {
            return Data.ANSIToString((int)Length - 1);
        }
    }
}

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

        public readonly string[] ToArray()
        {
            var result = new string[Size];
            for (int i = 0; i < Size; ++i)
            {
                result[i] = ((IntPtr)Data[i]).UTF8PtrToString()!;
            }
            return result;
        }
    }

    /// <summary>
    /// Wrapper for managing arrays of strings.
    /// </summary>
    public unsafe ref struct PdStringArrayWrapper
    {
        /// <summary>
        /// Pointer to the managed stack array.
        /// </summary>
        public IntPtr ptr;

        /// <summary>
        /// Converts the array to an array of strings.
        /// </summary>
        /// <returns>The array of strings.</returns>
        public readonly unsafe string[] ToArray()
        {
            return ((PdStringArray*)ptr)->ToArray();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the PdStringArrayWrapper, 
        /// and optionally releases the managed resources.
        /// </summary>
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

        public readonly int[] ToArray()
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

    /// <summary>
    /// Wrapper for managing arrays of integers.
    /// </summary>
    public ref struct PdIntArrayWrapper
    {
        /// <summary>
        /// Pointer to the managed stack array.
        /// </summary>
        public IntPtr ptr;

        /// <summary>
        /// Converts the array to an array of integers.
        /// </summary>
        /// <returns></returns>
        public readonly unsafe int[] ToArray()
        {
            return ((PdIntArray*)ptr)->ToArray();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the PdIntArrayWrapper, 
        /// and optionally releases the managed resources.
        /// </summary>
        public void Dispose()
        {
            PD_OneDimArrayInt32Destroy(ptr);
            ptr = IntPtr.Zero;
        }
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

    [StructLayout(LayoutKind.Sequential)]
    internal struct PdCStr
    {
        public uint Length;
        public IntPtr Data;

        public override readonly string? ToString()
        {
            return Data.ANSIToString((int)Length - 1);
        }
    }
}

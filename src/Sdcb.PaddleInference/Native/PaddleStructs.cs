using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArrayInt32
{
    public nint Size;
    public int* Data;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArraySize
{
    public nint Size;
    public nint* Data;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArrayInt64
{
    public nint Size;
    public long* Data;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArrayCstr
{
    public nint Size;
    public IntPtr* Data;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_Cstr
{
    public nint Size;
    public IntPtr Data;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_TwoDimArraySize
{
    public nint Size;
    public PD_OneDimArraySize** Data;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_IOInfo
{
    public PD_Cstr* Name;
    public PD_OneDimArrayInt64* Shape;
    public PaddleDataType DataType;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_IOInfos
{
    public nint Size;
    PD_IOInfo** IOInfo;
}

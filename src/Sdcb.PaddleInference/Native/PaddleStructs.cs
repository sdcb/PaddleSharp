using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference.Native;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArrayInt32
{
    public nint Size;
    public int* Data;

    public readonly int[] ToArray()
    {
        return new ReadOnlySpan<int>(Data, (int)Size).ToArray();
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArraySize
{
    public nint Size;
    public nint* Data;

    public readonly nint[] ToArray()
    {
        return new ReadOnlySpan<nint>(Data, (int)Size).ToArray();
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArrayInt64
{
    public nint Size;
    public long* Data;

    public readonly long[] ToArray()
    {
        return new ReadOnlySpan<long>(Data, (int)Size).ToArray();
    }

    public readonly int[] ToInt32Array()
    {
        return Array.ConvertAll(ToArray(), x => (int)x);
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_OneDimArrayCstr
{
    public nint Size;
    public IntPtr* Data;

    public readonly string[] ToArray()
    {
        string[] result = new string[Size];
        for (int i = 0; i < Size; i++)
        {
            result[i] = Data[i].ANSIToString()!;
        }
        return result;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_Cstr
{
    public nint Size;
    public IntPtr Data;

    public override readonly string ToString()
    {
        return Data.ANSIToString((int)Size - 1)!;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_TwoDimArraySize
{
    public nint Size;
    public PD_OneDimArraySize** Data;

    public readonly nint[][] ToArray()
    {
        nint[][] result = new nint[Size][];
        for (int i = 0; i < Size; i++)
        {
            result[i] = Data[i]->ToArray();
        }
        return result;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_IOInfo
{
    public PD_Cstr* Name;
    public PD_OneDimArrayInt64* Shape;
    public PaddleDataType DataType;

    public readonly PaddleIOInfo ToPaddleIOInfo()
    {
        return new PaddleIOInfo
        {
            Name = Name->ToString(),
            Shape = Shape->ToInt32Array(),
            DataType = DataType,
        };
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct PD_IOInfos
{
    public nint Size;
    public PD_IOInfo** IOInfo;

    public readonly PaddleIOInfo[] ToArray()
    {
        PaddleIOInfo[] result = new PaddleIOInfo[Size];
        for (int i = 0; i < Size; i++)
        {
            result[i] = IOInfo[i]->ToPaddleIOInfo();
        }
        return result;
    }
}

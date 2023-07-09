using System;
using System.Runtime.InteropServices;

namespace Sdcb.Paddle2Onnx;

[StructLayout(LayoutKind.Explicit, Size = 40012)]
internal struct PaddleReader
{
    [FieldOffset(0)]
    public byte _inputNames; // char[100][200]

    [FieldOffset(20000)]
    public byte _outputNames; // char[100][200]

    [FieldOffset(40000)]
    public int NumInputs;

    [FieldOffset(40004)]
    public int NumOutputs;

    [FieldOffset(40008)]
    public int HasNms;

    const int MaxStringLength = 200;

    public unsafe string[] InputNames
    {
        get
        {
            fixed (byte* pi = &_inputNames)
            {
                string[] result = new string[NumInputs];
                for (int i = 0; i < NumInputs; i++)
                {
                    byte* p = pi + i * MaxStringLength;
                    result[i] = ((IntPtr)p).PtrToStringAnsi(MaxStringLength);
                }
                return result;
            }
        }
    }

    public unsafe string[] OutputNames
    {
        get
        {
            fixed (byte* pi = &_outputNames)
            {
                string[] result = new string[NumOutputs];
                for (int i = 0; i < NumOutputs; i++)
                {
                    byte* p = pi + i * MaxStringLength;
                    result[i] = ((IntPtr)p).PtrToStringAnsi(MaxStringLength);
                }
                return result;
            }
        }
    }
}


[StructLayout(LayoutKind.Explicit, Size = 42208)]
public struct OnnxReader
{
    [FieldOffset(0)]
    public byte _inputNames;

    [FieldOffset(20000)]
    public byte _outputNames;

    [FieldOffset(40000)]
    public int _inputShapes;

    [FieldOffset(44000)]
    public int _outputShapes;

    [FieldOffset(48000)]
    public int _inputRanks;

    [FieldOffset(48400)]
    public int _outputRanks;

    [FieldOffset(48800)]
    public int NumInputs;

    [FieldOffset(48804)]
    public int NumOutputs;

    const int MaxStringLength = 200;
    const int MaxShapeRank = 10;

    public unsafe string[] InputNames
    {
        get
        {
            fixed (byte* pi = &_inputNames)
            {
                string[] result = new string[NumInputs];
                for (int i = 0; i < NumInputs; i++)
                {
                    byte* p = pi + i * MaxStringLength;
                    string s = Marshal.PtrToStringAnsi((IntPtr)p, MaxStringLength);
                    result[i] = s;
                }
                return result;
            }
        }
    }

    public unsafe string[] OutputNames
    {
        get
        {
            fixed (byte* pi = &_outputNames)
            {
                string[] result = new string[NumOutputs];
                for (int i = 0; i < NumOutputs; i++)
                {
                    byte* p = pi + i * MaxStringLength;
                    string s = Marshal.PtrToStringAnsi((IntPtr)p, MaxStringLength);
                    result[i] = s;
                }
                return result;
            }
        }
    }

    public unsafe int[][] InputShapes
    {
        get
        {
            int[] inputRanks = InputRanks;
            fixed (int* pi = &_inputShapes)
            {
                int[][] result = new int[NumInputs][];
                for (int i = 0; i < NumInputs; i++)
                {
                    result[i] = new ReadOnlySpan<int>(pi + i * MaxShapeRank, InputRanks[i]).ToArray();
                }
                return result;
            }
        }
    }

    public unsafe int[][] OutputShapes
    {
        get
        {
            int[] outputRanks = OutputRanks;
            fixed (int* pi = &_outputShapes)
            {
                int[][] result = new int[NumOutputs][];
                for (int i = 0; i < NumOutputs; i++)
                {
                    result[i] = new ReadOnlySpan<int>(pi + i * MaxShapeRank, OutputRanks[i]).ToArray();
                }
                return result;
            }
        }
    }

    public unsafe int[] InputRanks
    {
        get
        {
            fixed (int* pi = &_inputRanks)
            {
                return new ReadOnlySpan<int>(pi, NumInputs).ToArray();
            }
        }
    }

    public unsafe int[] OutputRanks
    {
        get
        {
            fixed (int* pi = &_outputRanks)
            {
                return new ReadOnlySpan<int>(pi, NumOutputs).ToArray();
            }
        }
    }
}
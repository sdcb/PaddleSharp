using System;
using System.Runtime.InteropServices;

namespace Sdcb.Paddle2Onnx.Natives;

[StructLayout(LayoutKind.Explicit, Size = 40012)]
internal readonly struct CPaddleReader
{
    [FieldOffset(0)]
    public readonly byte _inputNames; // char[100][200]

    [FieldOffset(20000)]
    public readonly byte _outputNames; // char[100][200]

    [FieldOffset(40000)]
    public readonly int NumInputs;

    [FieldOffset(40004)]
    public readonly int NumOutputs;

    [FieldOffset(40008)]
    public readonly int HasNms;

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

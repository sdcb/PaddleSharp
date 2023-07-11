using System;
using System.Runtime.InteropServices;

namespace Sdcb.Paddle2Onnx;

[StructLayout(LayoutKind.Explicit, Size = 112)]
internal unsafe struct ModelTensorInfo
{
    [FieldOffset(0)]
    private byte _name;

    [FieldOffset(100)]
    private int* _shape;

    [FieldOffset(108)]
    private int _rank;

    public string Name
    {
        get
        {
            fixed (byte* p = &_name)
            {
                return ((IntPtr)p).PtrToStringAnsi(maxLength: 100);
            }
        }
    }

    public readonly int[] Shape => new ReadOnlySpan<int>(_shape, _rank).ToArray();
}
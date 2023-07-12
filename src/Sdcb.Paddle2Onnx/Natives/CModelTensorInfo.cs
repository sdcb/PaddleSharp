using System;
using System.Runtime.InteropServices;

namespace Sdcb.Paddle2Onnx.Natives;

[StructLayout(LayoutKind.Explicit, Size = 112)]
internal readonly unsafe struct CModelTensorInfo
{
    [FieldOffset(0)]
    private readonly byte _name;

    [FieldOffset(100)]
    private readonly int* _shape;

    [FieldOffset(108)]
    private readonly int _rank;

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
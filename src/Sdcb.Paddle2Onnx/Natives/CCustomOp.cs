using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.Paddle2Onnx.Natives;

[StructLayout(LayoutKind.Explicit, Size = MaxLength * 2)]
internal struct CCustomOp
{
    [FieldOffset(0)]
    internal byte _opName;

    [FieldOffset(MaxLength)]
    internal byte _exportOpName;

    const int MaxLength = 100;

    public unsafe string OpName
    {
        get => ReadString(ref _opName);
        set => AssignString(ref _opName, value);
    }

    public unsafe string ExportOpName
    {
        get => ReadString(ref _exportOpName);
        set => AssignString(ref _exportOpName, value);
    }

    public void Assign(CustomOp op)
    {
        OpName = op.OpName;
        ExportOpName = op.ExportName;
    }

    private static unsafe string ReadString(ref byte field)
    {
        fixed (void* p = &field)
        {
            return ((IntPtr)p).PtrToStringAnsi(MaxLength);
        }
    }

    private static unsafe void AssignString(ref byte field, string value, [CallerMemberName] string? callerMember = null)
    {
        byte[] val = Encoding.UTF8.GetBytes(value);
        if (val.Length > MaxLength - 1)
        {
            throw new ArgumentException($"{callerMember} is too long, max length: {MaxLength - 1}.");
        }
        fixed (byte* p = &field)
        {
            Marshal.Copy(val, 0, (IntPtr)p, val.Length);
            p[val.Length] = 0;
        }
    }
}
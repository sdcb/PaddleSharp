using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.PaddleInference;

class PtrFromStringArray : IDisposable
{
    readonly IntPtr[] internalArray;
    readonly GCHandle[] handles;
    readonly GCHandle mainHandle;

    public PtrFromStringArray(string[] data)
    {
        handles = new GCHandle[data.Length];
        internalArray = new IntPtr[data.Length];

        for (int i = 0; i < data.Length; ++i)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(data[i] + '\0');
            handles[i] = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            internalArray[i] = handles[i].AddrOfPinnedObject();
        }

        mainHandle = GCHandle.Alloc(internalArray, GCHandleType.Pinned);
    }

    public IntPtr Ptr => mainHandle.AddrOfPinnedObject();

    public void Dispose()
    {
        foreach (GCHandle handle in handles) handle.Free();
        mainHandle.Free();
    }
}

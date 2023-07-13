using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference;

class PtrFromIntArray : IDisposable
{
    readonly IntPtr[] internalArray;
    readonly GCHandle[] handles;
    readonly GCHandle mainHandle;

    public PtrFromIntArray(int[][] data)
    {
        handles = new GCHandle[data.Length];
        internalArray = new IntPtr[data.Length];
        mainHandle = GCHandle.Alloc(internalArray, GCHandleType.Pinned);

        for (int i = 0; i < data.Length; ++i)
        {
            handles[i] = GCHandle.Alloc(data[i], GCHandleType.Pinned);
            internalArray[i] = handles[i].AddrOfPinnedObject();
        }
    }

    public IntPtr Ptr => mainHandle.AddrOfPinnedObject();

    public void Dispose()
    {
        foreach (GCHandle handle in handles) handle.Free();
        mainHandle.Free();
    }
}

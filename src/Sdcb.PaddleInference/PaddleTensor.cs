using Sdcb.PaddleInference.Native;
using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference;

/// <summary>
/// A tensor for Paddle Inference.
/// </summary>
public class PaddleTensor : IDisposable
{
    private IntPtr _ptr;

    /// <summary>
    /// Creates a new instance of <see cref="PaddleTensor"/> class.
    /// </summary>
    /// <param name="predictorPointer">Pointer to the associated predictor.</param>
    public PaddleTensor(IntPtr predictorPointer)
    {
        if (predictorPointer == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(predictorPointer));
        }
        _ptr = predictorPointer;
    }

    /// <summary>
    /// Gets the name of this tensor.
    /// </summary>
    public string Name
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_TensorGetName(_ptr).UTF8PtrToString()!;
        }
    }

    /// <summary>
    /// Gets or sets the shape of this tensor.
    /// </summary>
    public unsafe int[] Shape
    {
        get
        {
            ThrowIfDisposed();

            PD_OneDimArrayInt32* shape = null;
            try
            {
                shape = (PD_OneDimArrayInt32*)PaddleNative.PD_TensorGetShape(_ptr);
                return shape->ToArray();
            }
            finally
            {
                PaddleNative.PD_OneDimArrayInt32Destroy((IntPtr)shape);
            }
        }
        set
        {
            ThrowIfDisposed();

            fixed (int* ptr = value)
            {
                PaddleNative.PD_TensorReshape(_ptr, value.Length, (IntPtr)ptr);
            }
        }
    }

    /// <summary>
    /// Gets the data of this tensor.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the tensor.</typeparam>
    /// <returns>The data of this tensor.</returns>
    public unsafe T[] GetData<T>()
    {
        ThrowIfDisposed();

        TypeCode code = Type.GetTypeCode(typeof(T));
        Action<IntPtr, IntPtr> copyAction = code switch
        {
            TypeCode.Single => PaddleNative.PD_TensorCopyToCpuFloat,
            TypeCode.Int32 => PaddleNative.PD_TensorCopyToCpuInt32,
            TypeCode.Int64 => PaddleNative.PD_TensorCopyToCpuInt64,
            TypeCode.Byte => PaddleNative.PD_TensorCopyToCpuUint8,
            TypeCode.SByte => PaddleNative.PD_TensorCopyToCpuInt8,
            _ => throw new NotSupportedException($"GetData for {typeof(T).Name} is not supported.")
        };

        int[] shape = Shape;
        int size = 1;
        for (int i = 0; i < shape.Length; ++i)
        {
            size *= shape[i];
        }

        T[] result = new T[size];
        GCHandle handle = GCHandle.Alloc(result, GCHandleType.Pinned);
        copyAction(_ptr, handle.AddrOfPinnedObject());
        handle.Free();

        return result;
    }

    /// <summary>
    /// Sets the data of this tensor.
    /// </summary>
    /// <param name="data">The data to be set.</param>
    public unsafe void SetData(float[] data)
    {
        ThrowIfDisposed();

        fixed (void* ptr = data)
        {
            PaddleNative.PD_TensorCopyFromCpuFloat(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>
    /// Sets the data of this tensor.
    /// </summary>
    /// <param name="data">The data to be set.</param>
    public unsafe void SetData(int[] data)
    {
        ThrowIfDisposed();

        fixed (void* ptr = data)
        {
            PaddleNative.PD_TensorCopyFromCpuInt32(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>
    /// Sets the data of this tensor.
    /// </summary>
    /// <param name="data">The data to be set.</param>
    public unsafe void SetData(long[] data)
    {
        ThrowIfDisposed();

        fixed (void* ptr = data)
        {
            PaddleNative.PD_TensorCopyFromCpuInt64(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>
    /// Sets the data of this tensor.
    /// </summary>
    /// <param name="data">The data to be set.</param>
    public unsafe void SetData(byte[] data)
    {
        ThrowIfDisposed();

        fixed (void* ptr = data)
        {
            PaddleNative.PD_TensorCopyFromCpuUint8(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>
    /// Sets the data of this tensor.
    /// </summary>
    /// <param name="data">The data to be set.</param>
    public unsafe void SetData(sbyte[] data)
    {
        ThrowIfDisposed();

        fixed (void* ptr = data)
        {
            PaddleNative.PD_TensorCopyFromCpuInt8(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>
    /// Gets the data type of this tensor.
    /// </summary>
    public PaddleDataType DataType
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_TensorGetDataType(_ptr);
        }
    }

    void ThrowIfDisposed()
    {
        if (_ptr == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(PaddleTensor));
        }
    }

    /// <summary>
    /// Frees the unmanaged resources used by the <see cref="PaddleTensor"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // tell GC not to invoke the finalizer.
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="PaddleTensor"/> class.
    /// </summary>
    ~PaddleTensor()
    {
        Dispose(false);
    }

    /// <summary>
    /// Frees the unmanaged resources used by the <see cref="PaddleTensor"/> class.
    /// </summary>
    /// <param name="disposing">true if called from Dispose(); false if called from Finalize().</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_ptr != IntPtr.Zero)
        {
            PaddleNative.PD_TensorDestroy(_ptr);
            _ptr = IntPtr.Zero;
        }

        if (disposing)
        {
            // Release other managed resources
        }
    }
}

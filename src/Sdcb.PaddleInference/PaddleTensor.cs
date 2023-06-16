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
    public string Name => PaddleNative.PD_TensorGetName(_ptr).UTF8PtrToString()!;

    /// <summary>
    /// Gets or sets the shape of this tensor.
    /// </summary>
    public unsafe int[] Shape
    {
        get
        {
            using PaddleNative.PdIntArrayWrapper wrapper = new() { ptr = PaddleNative.PD_TensorGetShape(_ptr) };
            return wrapper.ToArray();
        }
        set
        {
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
        fixed (void* ptr = data)
        {
            PaddleNative.PD_TensorCopyFromCpuInt8(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>
    /// Gets the data type of this tensor.
    /// </summary>
    public DataTypes DataType => (DataTypes)PaddleNative.PD_TensorGetDataType(_ptr);

    /// <summary>
    /// Disposes any resources held by this tensor.
    /// </summary>
    public void Dispose()
    {
        if (_ptr != IntPtr.Zero)
        {
            PaddleNative.PD_TensorDestroy(_ptr);
            _ptr = IntPtr.Zero;
        }
    }
}

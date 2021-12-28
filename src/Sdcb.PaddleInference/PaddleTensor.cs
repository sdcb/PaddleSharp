using Sdcb.PaddleInference.Native;
using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference
{
    public class PaddleTensor : IDisposable
	{
		private IntPtr _ptr;

		public PaddleTensor(IntPtr predictorPointer)
		{
			if (predictorPointer == IntPtr.Zero)
			{
				throw new ArgumentNullException(nameof(predictorPointer));
			}
			_ptr = predictorPointer;
		}

		public string Name => PaddleNative.PD_TensorGetName(_ptr).UTF8PtrToString()!;
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

		public unsafe void SetData(float[] data)
		{
			fixed (void* ptr = data)
			{
				PaddleNative.PD_TensorCopyFromCpuFloat(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(int[] data)
		{
			fixed (void* ptr = data)
			{
				PaddleNative.PD_TensorCopyFromCpuInt32(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(long[] data)
		{
			fixed (void* ptr = data)
			{
				PaddleNative.PD_TensorCopyFromCpuInt64(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(byte[] data)
		{
			fixed (void* ptr = data)
			{
				PaddleNative.PD_TensorCopyFromCpuUint8(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(sbyte[] data)
		{
			fixed (void* ptr = data)
			{
				PaddleNative.PD_TensorCopyFromCpuInt8(_ptr, (IntPtr)ptr);
			}
		}

		public DataTypes DataType => (DataTypes)PaddleNative.PD_TensorGetDataType(_ptr);

		public void Dispose()
		{
			if (_ptr != IntPtr.Zero)
			{
				PaddleNative.PD_TensorDestroy(_ptr);
				_ptr = IntPtr.Zero;
			}
		}
	}
}

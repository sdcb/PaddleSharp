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

		public string Name => Marshal.PtrToStringUTF8(PdInvoke.PD_TensorGetName(_ptr));
		public unsafe int[] Shape
		{
			get
			{
				using PdInvoke.PdIntArrayWrapper wrapper = new() { ptr = PdInvoke.PD_TensorGetShape(_ptr) };
				return wrapper.ToArray();
			}
			set
			{
				fixed (int* ptr = value)
				{
					PdInvoke.PD_TensorReshape(_ptr, value.Length, (IntPtr)ptr);
				}
			}
		}

		public unsafe T[] GetData<T>()
		{
			TypeCode code = Type.GetTypeCode(typeof(T));
			Action<IntPtr, IntPtr> copyAction = code switch
			{
				TypeCode.Single => PdInvoke.PD_TensorCopyToCpuFloat,
				TypeCode.Int32 => PdInvoke.PD_TensorCopyToCpuInt32,
				TypeCode.Int64 => PdInvoke.PD_TensorCopyToCpuInt64,
				TypeCode.Byte => PdInvoke.PD_TensorCopyToCpuUint8,
				TypeCode.SByte => PdInvoke.PD_TensorCopyToCpuInt8,
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
				PdInvoke.PD_TensorCopyFromCpuFloat(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(int[] data)
		{
			fixed (void* ptr = data)
			{
				PdInvoke.PD_TensorCopyFromCpuInt32(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(long[] data)
		{
			fixed (void* ptr = data)
			{
				PdInvoke.PD_TensorCopyFromCpuInt64(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(byte[] data)
		{
			fixed (void* ptr = data)
			{
				PdInvoke.PD_TensorCopyFromCpuUint8(_ptr, (IntPtr)ptr);
			}
		}

		public unsafe void SetData(sbyte[] data)
		{
			fixed (void* ptr = data)
			{
				PdInvoke.PD_TensorCopyFromCpuInt8(_ptr, (IntPtr)ptr);
			}
		}

		public DataTypes DataType => (DataTypes)PdInvoke.PD_TensorGetDataType(_ptr);

		public void Dispose()
		{
			if (_ptr != IntPtr.Zero)
			{
				PdInvoke.PD_TensorDestroy(_ptr);
				_ptr = IntPtr.Zero;
			}
		}
	}
}

using System;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference.Native
{
	public class PaddleNative
	{
        private unsafe struct PdStringArray
        {
#pragma warning disable CS0649
			public nint Size;
            public byte** Data;
#pragma warning restore CS0649

			public string[] ToArray()
            {
                var result = new string[Size];
                for (int i = 0; i < Size; ++i)
                {
                    result[i] = ((IntPtr)Data[i]).UTF8PtrToString();
                }
                return result;
            }
        }

        public unsafe ref struct PdStringArrayWrapper
        {
            public IntPtr ptr;

            public unsafe string[] ToArray()
            {
                return ((PdStringArray*)ptr)->ToArray();
            }

            public void Dispose()
            {
                PD_OneDimArrayCstrDestroy(ptr);
                ptr = IntPtr.Zero;
            }
        }

        private unsafe struct PdIntArray
		{
			public nint Size;
			public int* Data;

			public int[] ToArray()
			{
				var result = new int[Size];
				for (int i = 0; i < Size; ++i)
				{
					result[i] = Data[i];
				}
				return result;
			}

			public unsafe void Dispose()
			{
				fixed (PdIntArray* ptr = &this)
				{
					PD_OneDimArrayInt32Destroy((IntPtr)ptr);
				}
			}
		}

		public unsafe ref struct PdIntArrayWrapper
		{
			public IntPtr ptr;

			public unsafe int[] ToArray()
			{
				return ((PdIntArray*)ptr)->ToArray();
			}

			public void Dispose()
			{
				PD_OneDimArrayInt32Destroy(ptr);
				ptr = IntPtr.Zero;
			}
		}

		public const string PaddleInferenceCLib = 
#if NETSTANDARD 
			@"paddle_inference_c.dll";
#else
			@"dll\x64\paddle_inference_c.dll";
#endif


		[DllImport(PaddleInferenceCLib)]
		public static extern IntPtr PD_GetVersion();

		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_ConfigCreate();
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_ConfigDestroy(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern byte PD_ConfigGlogInfoDisabled();
		[DllImport(PaddleInferenceCLib)] public static extern void PD_ConfigDisableGlogInfo(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern byte PD_ConfigModelFromMemory(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_ConfigGetModelDir(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_ConfigSetModelDir(IntPtr config, string modelDir);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_ConfigSetModel(IntPtr config, string modelPath, string paramsPath);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_ConfigGetProgFile(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_ConfigGetParamsFile(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_ConfigSetModelBuffer(IntPtr config, IntPtr programBuffer, nint programBufferSize, IntPtr paramsBuffer, nint paramsBufferSize);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_ConfigSetCpuMathLibraryNumThreads(IntPtr config, int threadCount);
		[DllImport(PaddleInferenceCLib)] public static extern int PD_ConfigGetCpuMathLibraryNumThreads(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern byte PD_ConfigMkldnnEnabled(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_ConfigEnableMKLDNN(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_ConfigSetMkldnnCacheCapacity(IntPtr config, int capacity);


		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_PredictorCreate(IntPtr config);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_PredictorClone(IntPtr predictor);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_PredictorDestroy(IntPtr predictor);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_PredictorGetInputNames(IntPtr predictor);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_PredictorGetOutputNames(IntPtr predictor);
		[DllImport(PaddleInferenceCLib)] public static extern nint PD_PredictorGetInputNum(IntPtr predictor);
		[DllImport(PaddleInferenceCLib)] public static extern nint PD_PredictorGetOutputNum(IntPtr predictor);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_PredictorGetInputHandle(IntPtr predictor, string name);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_PredictorGetOutputHandle(IntPtr predictor, string name);
		[DllImport(PaddleInferenceCLib)] public static extern byte PD_PredictorRun(IntPtr predictor);


		[DllImport(PaddleInferenceCLib)] public static extern void PD_OneDimArrayInt32Destroy(IntPtr array);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_OneDimArrayCstrDestroy(IntPtr array);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_OneDimArraySizeDestroy(IntPtr array);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TwoDimArraySizeDestroy(IntPtr array);


		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorDestroy(IntPtr tensor);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorReshape(IntPtr tensor, nint size, IntPtr shape);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyFromCpuFloat(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyFromCpuInt64(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyFromCpuInt32(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyFromCpuUint8(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyFromCpuInt8(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyToCpuFloat(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyToCpuInt64(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyToCpuInt32(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyToCpuUint8(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern void PD_TensorCopyToCpuInt8(IntPtr tensor, IntPtr data);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_TensorGetShape(IntPtr tensor);
		[DllImport(PaddleInferenceCLib)] public static extern IntPtr PD_TensorGetName(IntPtr tensor);
		[DllImport(PaddleInferenceCLib)] public static extern int PD_TensorGetDataType(IntPtr tensor);
	}
}

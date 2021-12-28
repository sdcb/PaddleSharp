using Sdcb.PaddleInference.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sdcb.PaddleInference
{
    public class PaddleConfig : IDisposable
	{
		private IntPtr _ptr;

		public PaddleConfig()
		{
			_ptr = PaddleNative.PD_ConfigCreate();
		}

		public PaddleConfig(IntPtr configPointer)
		{
			_ptr = configPointer;

			if (!EnableGLogByDefault)
			{
				GLogEnabled = EnableGLogByDefault;
			}
		}

		public static PaddleConfig FromModelDir(string modelDir)
		{
			PaddleConfig c = CreateDefault();
			c.SetModel(
				Path.Combine(modelDir, "inference.pdmodel"),
				Path.Combine(modelDir, "inference.pdiparams"));
			return c;
		}

		public static PaddleConfig FromModelFiles(string programPath, string paramsPath)
		{
			PaddleConfig c = CreateDefault();
			c.SetModel(
				programPath,
				paramsPath);
			return c;
		}

		public static PaddleConfig FromMemoryModel(byte[] programBuffer, byte[] paramsBuffer)
		{
			PaddleConfig c = CreateDefault();
			c.SetMemoryModel(programBuffer, paramsBuffer);
			return c;
		}

		private static PaddleConfig CreateDefault()
		{
			var c = new PaddleConfig();

			if (!EnableGLogByDefault)
			{
				c.GLogEnabled = EnableGLogByDefault;
			}
			if (EnableMkldnnByDefault)
			{
				c.MkldnnEnabled = true;
				c.MkldnnCacheCapacity = MkldnnDefaultCacheCapacity;
			}
			if (CpuMathDefaultThreadCount != 0)
			{
				c.CpuMathThreadCount = CpuMathDefaultThreadCount;
			}
			if (MemoryOptimizedByDefault)
			{
				c.MemoryOptimized = MemoryOptimizedByDefault;
			}
			if (ProfileEnabledByDefault)
            {
				c.ProfileEnabled = ProfileEnabledByDefault;
            }
			if (UseGpuByDefault)
            {
				c.EnableUseGpu(DefaultInitialGpuMemoryMb, DefaultGpuDeviceId);
				if (EnableGpuMultiStreamByDefault)
                {
					c.EnableGpuMultiStream = EnableGpuMultiStreamByDefault;
                }
            }

			return c;
		}

		static PaddleConfig()
		{
			if (IntPtr.Size == 4)
            {
				throw new PlatformNotSupportedException("Paddle Inference does not support 32bit platform.");
            }

#if NET6_0_OR_GREATER
			SearchPathLoad();
#elif NETSTANDARD2_0_OR_GREATER || NET461_OR_GREATER
			AutoLoad();
#endif
		}

		private static void AutoLoad()
        {
			// Linux would not supported in this case.
			_ = Version;
			string mkldnnPath = Path.GetDirectoryName(Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
				.Single(x => Path.GetFileNameWithoutExtension(x.ModuleName) == "paddle_inference_c")
				.FileName)!;
			AddLibPathToEnvironment(mkldnnPath);
		}

#if NET6_0_OR_GREATER
		private static void SearchPathLoad()
        {
			string? dirs = (string?)AppContext.GetData("NATIVE_DLL_SEARCH_DIRECTORIES");
			if (dirs != null)
			{
				bool windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
				bool linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
				if (windows)
				{
					string libName = windows ?
						$"{PaddleNative.PaddleInferenceCLib}.dll" :
						$"lib{PaddleNative.PaddleInferenceCLib}.so";

					string? libPath = dirs.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries)
						.FirstOrDefault(dir => File.Exists(Path.Combine(dir, libName)));
					if (libPath != null)
					{
						AddLibPathToEnvironment(libPath);
					}
					else
					{
						Console.WriteLine($"Warn: {libName} not found from {dirs}, fallback to use auto load.");
						AutoLoad();
					}
				}
				else if (!linux)
				{
					Console.WriteLine("Warn: OSPlatform is not windows or linux, platform might not supported.");
				}
			}
		}
#endif

		private static void AddLibPathToEnvironment(string libPath)
        {
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
			bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
			const bool isWindows = true;
#endif
			string envId = isWindows ? "PATH" : "LD_LIBRARY_PATH";
			Environment.SetEnvironmentVariable(envId, Environment.GetEnvironmentVariable(envId) + Path.PathSeparator + libPath);
		}

		public static string Version => PaddleNative.PD_GetVersion().UTF8PtrToString()!;

		public static bool EnableGLogByDefault = false;
		public static bool EnableMkldnnByDefault = true;
		public static int MkldnnDefaultCacheCapacity = 10;
		public static int CpuMathDefaultThreadCount = 0;
		public static bool MemoryOptimizedByDefault = true;
		public static bool ProfileEnabledByDefault = false;
		public static bool UseGpuByDefault = false;
		public static int DefaultInitialGpuMemoryMb = 500;
		public static int DefaultGpuDeviceId = 0;
		public static bool EnableGpuMultiStreamByDefault = true;


		public bool GLogEnabled
		{
			get => PaddleNative.PD_ConfigGlogInfoDisabled() == 0;
			set
			{
				if (!value)
				{
					PaddleNative.PD_ConfigDisableGlogInfo(_ptr);
				}
				else if (!GLogEnabled)
				{
					Console.WriteLine($"Warn: Glog cannot re-enable after disabled.");
				}
			}
		}

		public bool MemoryOptimized
		{
			get => PaddleNative.PD_ConfigMemoryOptimEnabled(_ptr) != 0;
			set
			{
				if (value)
				{
					PaddleNative.PD_ConfigEnableMemoryOptim(_ptr);
				}
				else if (PaddleNative.PD_ConfigMemoryOptimEnabled(_ptr) != 0)
				{
					Console.WriteLine($"Warn: Memory optimized cannot disabled after enabled.");
				}
			}
		}

		public bool IsMemoryModel => PaddleNative.PD_ConfigModelFromMemory(_ptr) != 0;
		public bool MkldnnEnabled
		{
			get => PaddleNative.PD_ConfigMkldnnEnabled(_ptr) != 0;
			set
			{
				if (value)
				{
					PaddleNative.PD_ConfigEnableMKLDNN(_ptr);
				}
				else if (MkldnnEnabled)
				{
					Console.WriteLine($"Warn: Mkldnn cannot disabled after enabled.");
				}
			}
		}

		private int _MkldnnCacheCapacity = 0;
		public int MkldnnCacheCapacity
		{
			get => _MkldnnCacheCapacity;
			set
			{
				_MkldnnCacheCapacity = value;
				PaddleNative.PD_ConfigSetMkldnnCacheCapacity(_ptr, value);
			}
		}

		public bool ProfileEnabled
		{
			get => PaddleNative.PD_ConfigProfileEnabled(_ptr) != 0;
			set
			{
				if (value)
				{
					PaddleNative.PD_ConfigEnableProfile(_ptr);
				}
				else if (PaddleNative.PD_ConfigProfileEnabled(_ptr) != 0)
				{
					Console.WriteLine($"Warn: Profile cannot disabled after enabled.");
				}
			}
		}

		//public string ModelDir
		//{
		//	get => Marshal.PtrToStringUTF8(PdInvoke.PD_ConfigGetModelDir(_ptr));
		//	set => PdInvoke.PD_ConfigSetModelDir(_ptr, value);
		//}

		public bool UseGpu
		{
			get => PaddleNative.PD_ConfigUseGpu(_ptr) != 0;
			set
			{
				if (!value)
				{
					PaddleNative.PD_ConfigDisableGpu(_ptr);
				}
				else
				{
					Console.WriteLine($"Warn: Use EnableUseGpu to use GPU.");
				}
			}
		}

		public int GpuDeviceId => PaddleNative.PD_ConfigGpuDeviceId(_ptr);

		public int InitialGpuMemorySizeMB => PaddleNative.PD_ConfigMemoryPoolInitSizeMb(_ptr);

		public float FractionOfGpuMemoryForPool => PaddleNative.PD_ConfigFractionOfGpuMemoryForPool(_ptr);

		public bool EnableGpuMultiStream
		{
			get => PaddleNative.PD_ConfigThreadLocalStreamEnabled(_ptr) != 0;
			set
			{
				if (value)
				{
					PaddleNative.PD_ConfigEnableGpuMultiStream(_ptr);
				}
				else
				{
					Console.WriteLine($"Warn: GpuMultiStream cannot disabled after enabled.");
				}
			}
		}

		public void EnableUseGpu(int initialMemoryMB, int deviceId)
		{
			PaddleNative.PD_ConfigEnableUseGpu(_ptr, (ulong)initialMemoryMB, deviceId);
		}

		public void SetModel(string programPath, string paramsPath)
		{
			if (programPath == null) throw new ArgumentNullException(nameof(programPath));
			if (paramsPath == null) throw new ArgumentNullException(nameof(paramsPath));
			if (!File.Exists(programPath)) throw new FileNotFoundException("programPath not found", programPath);
			if (!File.Exists(paramsPath)) throw new FileNotFoundException("paramsPath not found", paramsPath);
			PaddleNative.PD_ConfigSetModel(_ptr, programPath, paramsPath);
		}

		public string? ProgramPath => PaddleNative.PD_ConfigGetProgFile(_ptr).UTF8PtrToString();
		public string? ParamsPath => PaddleNative.PD_ConfigGetParamsFile(_ptr).UTF8PtrToString();

		public unsafe void SetMemoryModel(byte[] programBuffer, byte[] paramsBuffer)
		{
			fixed (byte* pprogram = programBuffer)
			fixed (byte* pparams = paramsBuffer)
			{
				PaddleNative.PD_ConfigSetModelBuffer(_ptr,
					(IntPtr)pprogram, programBuffer.Length,
					(IntPtr)pparams, paramsBuffer.Length);
			}
		}

		public int CpuMathThreadCount
		{
			get => PaddleNative.PD_ConfigGetCpuMathLibraryNumThreads(_ptr);
			set => PaddleNative.PD_ConfigSetCpuMathLibraryNumThreads(_ptr, value);
		}

		public PaddlePredictor CreatePredictor()
		{
			try
			{
				return new PaddlePredictor(PaddleNative.PD_PredictorCreate(_ptr));
			}
			finally
			{
				_ptr = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			if (_ptr != IntPtr.Zero)
			{
				PaddleNative.PD_ConfigDestroy(_ptr);
				_ptr = IntPtr.Zero;
			}
		}
	}
}

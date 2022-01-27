using Sdcb.PaddleInference.Native;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.PaddleInference
{
    public class PaddleConfig : IDisposable
    {
        private IntPtr _ptr;

        public PaddleConfig()
        {
            _ptr = PaddleNative.PD_ConfigCreate();
        }

        public static readonly PaddleConfigDefaults Defaults = new();

        public PaddleConfig(IntPtr configPointer)
        {
            _ptr = configPointer;

            if (!Defaults.EnableGLog)
            {
                GLogEnabled = Defaults.EnableGLog;
            }
        }

        public static PaddleConfig FromModelDir(string modelDir)
        {
            if (!Directory.Exists(modelDir))
            {
                throw new DirectoryNotFoundException(modelDir);
            }

            string[] allowedPrefixes = new[]
            {
                "inference", 
                "model", 
            };

            foreach (string prefix in allowedPrefixes)
            {
                string pdmodel = Path.Combine(modelDir, prefix + ".pdmodel");
                string pdiparams = Path.Combine(modelDir, prefix + ".pdiparams");

                if (File.Exists(pdmodel) && File.Exists(pdiparams))
                {
                    PaddleConfig c = CreateDefault();
                    c.SetModel(pdmodel, pdiparams);
                    return c;
                }
            }

            throw new FileNotFoundException($"Model file not find in model dir: {modelDir}");
        }

        public static PaddleConfig FromModelFiles(string programPath, string paramsPath)
        {
            PaddleConfig c = CreateDefault();
            c.SetModel(programPath, paramsPath);
            return c;
        }

        public static PaddleConfig FromMemoryModel(byte[] programBuffer, byte[] paramsBuffer)
        {
            PaddleConfig c = CreateDefault();
            c.SetMemoryModel(programBuffer, paramsBuffer);
            return c;
        }

        public static PaddleConfig CreateDefault()
        {
            var c = new PaddleConfig();

            if (!Defaults.EnableGLog)
            {
                c.GLogEnabled = Defaults.EnableGLog;
            }
            if (Defaults.CpuMathThreadCount != 0)
            {
                c.CpuMathThreadCount = Defaults.CpuMathThreadCount;
            }
            if (Defaults.MemoryOptimized)
            {
                c.MemoryOptimized = Defaults.MemoryOptimized;
            }
            if (Defaults.ProfileEnabled)
            {
                c.ProfileEnabled = Defaults.ProfileEnabled;
            }
            if (Defaults.UseGpu)
            {
                c.EnableUseGpu(Defaults.InitialGpuMemoryMb, Defaults.GpuDeviceId);
                if (Defaults.EnableGpuMultiStream)
                {
                    c.EnableGpuMultiStream = Defaults.EnableGpuMultiStream;
                }
            }
            else if (Defaults.UseMkldnn)
            {
                c.MkldnnEnabled = true;
                c.MkldnnCacheCapacity = Defaults.MkldnnCacheCapacity;
            }

            return c;
        }

        public static readonly Encoding PaddleEncoding;

        static PaddleConfig()
        {
            if (IntPtr.Size == 4)
            {
                throw new PlatformNotSupportedException("Paddle Inference does not support 32bit platform.");
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            PaddleEncoding = Environment.OSVersion.Platform == PlatformID.Win32NT ? Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage) : Encoding.UTF8;

#if LINQPAD
			SearchPathLoad();
#elif NET6_0_OR_GREATER
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
            PaddleNative.AddLibPathToEnvironment(mkldnnPath);
        }

#if NET6_0_OR_GREATER || LINQPAD
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
                        PaddleNative.AddLibPathToEnvironment(libPath);
                    }
                    else
                    {
#if !LINQPAD
                        Console.WriteLine($"Warn: {libName} not found from {dirs}, fallback to use auto load.");
#endif
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

        public static string Version => PaddleNative.PD_GetVersion().UTF8PtrToString()!;


        public bool GLogEnabled
        {
            get => PaddleNative.PD_ConfigGlogInfoDisabled(_ptr) == 0;
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
            set => PaddleNative.PD_ConfigEnableMemoryOptim(_ptr, (sbyte)(value ? 1 : 0));
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

        public unsafe void SetModel(string programPath, string paramsPath)
        {
            if (programPath == null) throw new ArgumentNullException(nameof(programPath));
            if (paramsPath == null) throw new ArgumentNullException(nameof(paramsPath));
            if (!File.Exists(programPath)) throw new FileNotFoundException("programPath not found", programPath);
            if (!File.Exists(paramsPath)) throw new FileNotFoundException("paramsPath not found", paramsPath);
            byte[] programBytes = PaddleEncoding.GetBytes(programPath);
            byte[] paramsBytes = PaddleEncoding.GetBytes(paramsPath);
            fixed (byte* programPtr = programBytes)
            fixed (byte* paramsPtr = paramsBytes)
            {
                PaddleNative.PD_ConfigSetModel(_ptr, (IntPtr)programPtr, (IntPtr)paramsPtr);
            }
        }

        public string? ProgramPath => PaddleNative.PD_ConfigGetProgFile(_ptr).ANSIToString();
        public string? ParamsPath => PaddleNative.PD_ConfigGetParamsFile(_ptr).ANSIToString();

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

    public class PaddleConfigDefaults
    {
        public bool EnableGLog { get; set; } = false;

        public bool UseMkldnn { get; set; } = true;
        public int MkldnnCacheCapacity { get; set; } = 10;

        public int CpuMathThreadCount { get; set; } = 0;

        public bool MemoryOptimized { get; set; } = true;

        public bool ProfileEnabled { get; set; } = false;

        public bool UseGpu { get; set; } = false;
        public int InitialGpuMemoryMb { get; set; } = 500;
        public int GpuDeviceId { get; set; } = 0;
        public bool EnableGpuMultiStream { get; set; } = true;
    }
}

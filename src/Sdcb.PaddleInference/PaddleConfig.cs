using Sdcb.PaddleInference.Native;
using Sdcb.PaddleInference.TensorRt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.PaddleInference;

/// <summary>
/// Provides a configuration for Paddle Inference.
/// </summary>
public sealed class PaddleConfig : IDisposable
{
    private IntPtr _ptr;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleConfig"/> class.
    /// </summary>
    public PaddleConfig()
    {
        _ptr = PaddleNative.PD_ConfigCreate();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleConfig"/> class.
    /// </summary>
    /// <param name="configPointer">A pointer to a paddle config object</param>
    public PaddleConfig(IntPtr configPointer)
    {
        _ptr = configPointer;
    }

    /// <summary>
    /// Returns a PaddleConfig instance from the given model directory.
    /// </summary>
    /// <param name="modelDir">The path to the directory containing the model files.</param>
    /// <returns>A new instance of PaddleConfig.</returns>
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
                PaddleConfig c = new();
                c.SetModel(pdmodel, pdiparams);
                return c;
            }
        }

        throw new FileNotFoundException($"Model file not find in model dir: {modelDir}");
    }

    /// <summary>
    /// Returns a PaddleConfig instance from paddle model files.
    /// </summary>
    /// <param name="programPath">Path of the PaddlePaddle model program file.</param>
    /// <param name="paramsPath">Path of the PaddlePaddle model parameter file.</param>
    /// <returns>A PaddleConfig instance.</returns>
    public static PaddleConfig FromModelFiles(string programPath, string paramsPath)
    {
        PaddleConfig c = new();
        c.SetModel(programPath, paramsPath);
        return c;
    }

    /// <summary>
    /// Creates the <see cref="PaddleConfig"/> object from in-memory program data.
    /// </summary>
    /// <param name="programBuffer">In-memory program data.</param>
    /// <param name="paramsBuffer">In-memory parameter data.</param>
    /// <returns>The <see cref="PaddleConfig"/> object.</returns>
    public static PaddleConfig FromMemoryModel(byte[] programBuffer, byte[] paramsBuffer)
    {
        PaddleConfig c = new();
        c.SetMemoryModel(programBuffer, paramsBuffer);
        return c;
    }

    /// <summary>
    /// Gets the encoding used by Paddle Inference.
    /// </summary>
    public static readonly Encoding PaddleEncoding;

    static PaddleConfig()
    {
        if (IntPtr.Size == 4)
        {
            throw new PlatformNotSupportedException("Paddle Inference does not support 32bit platform.");
        }

#if NETSTANDARD || NET6_0_OR_GREATER
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif
        PaddleEncoding = Environment.OSVersion.Platform == PlatformID.Win32NT ? Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage) : Encoding.UTF8;
    }

    /// <summary>Get version info.</summary>
    public static string Version => PaddleNative.PD_GetVersion().UTF8PtrToString()!;

    /// <summary>
    /// Returns a pointer to the underlying unmanaged object handle.
    /// </summary>
    /// <returns>An unmanaged object handle.</returns>
    public IntPtr UnsafeGetHandle() => _ptr;

    /// <summary>A boolean state telling whether logs in Paddle inference are enabled.</summary>
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

    /// <summary>A boolean state telling whether the memory optimization is activated.</summary>
    public bool MemoryOptimized
    {
        get => PaddleNative.PD_ConfigMemoryOptimEnabled(_ptr) != 0;
        set => PaddleNative.PD_ConfigEnableMemoryOptim(_ptr, (sbyte)(value ? 1 : 0));
    }

    /// <summary>A boolean state telling whether the model is set from the CPU memory.</summary>
    public bool IsMemoryModel => PaddleNative.PD_ConfigModelFromMemory(_ptr) != 0;

    /// <summary>A boolean state telling whether to use the MKLDNN.</summary>
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

    /// <summary>Set the cache capacity of different input shapes for MKLDNN. Default value 0 means not caching any shape.</summary>
    public int MkldnnCacheCapacity
    {
        get => _MkldnnCacheCapacity;
        set
        {
            _MkldnnCacheCapacity = value;
            PaddleNative.PD_ConfigSetMkldnnCacheCapacity(_ptr, value);
        }
    }

    /// <summary>Turn on profiling report. If not turned on, no profiling report will be generated.</summary>
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

    /// <summary>A boolean state telling whether the GPU is turned on.</summary>
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

    /// <summary>Get the GPU device id.</summary>
    public int GpuDeviceId => PaddleNative.PD_ConfigGpuDeviceId(_ptr);

    /// <summary>Get the initial size in MB of the GPU memory pool.</summary>
    public int InitialGpuMemorySizeMB => PaddleNative.PD_ConfigMemoryPoolInitSizeMb(_ptr);

    /// <summary>Get the proportion of the initial memory pool size compared to the device.</summary>
    public float FractionOfGpuMemoryForPool => PaddleNative.PD_ConfigFractionOfGpuMemoryForPool(_ptr);

    /// <param name="workspaceSize">The memory size(in byte) used for TensorRT workspace.</param>
    /// <param name="maxBatchSize">The maximum batch size of this prediction task, better set as small as possible for less performance loss.</param>
    /// <param name="minSubgraphSize">Paddle-TRT is running under subgraph, Paddle-TRT will only been enabled when subgraph node count > min_subgraph_size to avoid performance loss</param>
    /// <param name="precision">The precision used in TensorRT.</param>
    /// <param name="useStatic">Serialize optimization information to disk for reusing.</param>
    /// <param name="useCalibMode">Use TRT int8 calibration(post training quantization)</param>
    public void EnableTensorRtEngine(
        long workspaceSize = 1 << 20,
        int maxBatchSize = 1,
        int minSubgraphSize = 20,
        PaddlePrecision precision = PaddlePrecision.Float32,
        bool useStatic = true,
        bool useCalibMode = false)
        => PaddleNative.PD_ConfigEnableTensorRtEngine(_ptr, workspaceSize, maxBatchSize, minSubgraphSize, (int)precision, (sbyte)(useStatic ? 1 : 0), (sbyte)(useCalibMode ? 1 : 0));

    /// <summary>A boolean state telling whether the TensorRT engine is used.</summary>
    public bool TensorRtEngineEnabled => PaddleNative.PD_ConfigTensorRtEngineEnabled(_ptr) != 0;

    /// <summary>A boolean state telling whether the trt dynamic_shape is used.</summary>
    public bool TensorRtDynamicShapeEnabled => PaddleNative.PD_ConfigTensorRtDynamicShapeEnabled(_ptr) != 0;

    /// <summary>A boolean state telling whether to use the TensorRT DLA.</summary>
    public bool TensorRtDlaEnabled => PaddleNative.PD_ConfigTensorRtDlaEnabled(_ptr) != 0;

    /// <summary>Collect shape info of all tensors in compute graph.</summary>
    public unsafe void CollectShapeRangeInfo(string rangeShapeInfoPath)
    {
        fixed (byte* cacheDirPtr = PaddleEncoding.GetBytes(rangeShapeInfoPath))
        {
            PaddleNative.PD_ConfigCollectShapeRangeInfo(_ptr, (IntPtr)cacheDirPtr);
        }
    }

    /// <summary>A boolean state telling whether to collect shape info.</summary>
    public bool ShapeRangeInfoCollected => PaddleNative.PD_ConfigShapeRangeInfoCollected(_ptr) != 0;

    /// <summary>Enable tuned tensorrt dynamic shape.</summary>
    public unsafe void EnableTunedTensorRtDynamicShape(string rangeShapeInfoPath, bool allowBuildAtRuntime = true)
    {
        fixed (byte* cacheDirPtr = PaddleEncoding.GetBytes(rangeShapeInfoPath))
        {
            PaddleNative.PD_ConfigEnableTunedTensorRtDynamicShape(_ptr, (IntPtr)cacheDirPtr, (sbyte)(allowBuildAtRuntime ? 1 : 0));
        }
    }

    /// <summary>A boolean state telling whether to use tuned tensorrt dynamic shape.</summary>
    public bool TunedTensorRtDynamicShape => PaddleNative.PD_ConfigTunedTensorRtDynamicShape(_ptr) != 0;

    /// <summary>Set the path of optimization cache directory.</summary>
    public unsafe void SetOptimCacheDir(string path)
    {
        fixed (byte* cacheDirPtr = PaddleEncoding.GetBytes(path))
        {
            PaddleNative.PD_ConfigSetOptimCacheDir(_ptr, (IntPtr)cacheDirPtr);
        }
    }

    /// <summary>Set min, max, opt shape for TensorRT Dynamic shape mode.</summary>
    public unsafe void SetTrtDynamicShapeInfo(Dictionary<string, TensorRtDynamicShapeGroup> shapeInfo)
    {
        using PtrFromStringArray shapeNames = new(shapeInfo.Keys.ToArray());

        long[] shape = new long[shapeInfo.Count];
        for (int i = 0; i < shape.Length; ++i)
        {
            shape[i] = 4;
        }
        using PtrFromIntArray minShapePtr = new(shapeInfo.Select(x => x.Value.Min.ToArray()).ToArray());
        using PtrFromIntArray maxShapePtr = new(shapeInfo.Select(x => x.Value.Max.ToArray()).ToArray());
        using PtrFromIntArray optShapePtr = new(shapeInfo.Select(x => x.Value.Optimal.ToArray()).ToArray());

        fixed (long* shapePtr = shape)
        {
            PaddleNative.PD_ConfigSetTrtDynamicShapeInfo(_ptr, 10, shapeNames.Ptr, (IntPtr)shapePtr,
                minShapePtr.Ptr,
                maxShapePtr.Ptr,
                optShapePtr.Ptr, 0);
        }
    }

    /// <summary>Get information of config.</summary>
    public unsafe string? Summary
    {
        get
        {
            IntPtr summaryPtr = default;
            try
            {
                summaryPtr = PaddleNative.PD_ConfigSummary(_ptr);
                return ((PaddleNative.PdCStr*)summaryPtr)->ToString();
            }
            finally
            {
                if (summaryPtr != IntPtr.Zero)
                {
                    PaddleNative.PD_CstrDestroy(summaryPtr);
                }
            }
        }
    }

    /// <summary>A boolean state telling whether the thread local CUDA stream is enabled.</summary>
    public bool EnableGpuMultiStream
    {
        get => PaddleNative.PD_ConfigThreadLocalStreamEnabled(_ptr) != 0;
        set
        {
            if (value)
            {
                PaddleNative.PD_ConfigEnableGpuMultiStream(_ptr);
            }
            else if (EnableGpuMultiStream)
            {
                Console.WriteLine($"Warn: GpuMultiStream cannot disabled after enabled.");
            }
        }
    }

    /// <summary>A boolean state telling whether the Config is valid.</summary>
    public bool Valid => PaddleNative.PD_ConfigIsValid(_ptr) != 0;

    /// <summary>Turn on GPU.</summary>
    public void EnableUseGpu(int initialMemoryMB, int deviceId)
    {
        PaddleNative.PD_ConfigEnableUseGpu(_ptr, (ulong)initialMemoryMB, deviceId);
    }

    /// <summary>Set the combined model with two specific pathes for program and parameters.</summary>
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

    /// <summary>Get the program file path.</summary>
    public string? ProgramPath => PaddleNative.PD_ConfigGetProgFile(_ptr).ANSIToString();

    /// <summary>Get the params file path.</summary>
    public string? ParamsPath => PaddleNative.PD_ConfigGetParamsFile(_ptr).ANSIToString();

    /// <summary>Specify the memory buffer of program and parameter. Used when model and params are loaded directly from memory.</summary>
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

    /// <summary>An int state telling how many threads are used in the CPU math library.</summary>
    public int CpuMathThreadCount
    {
        get => PaddleNative.PD_ConfigGetCpuMathLibraryNumThreads(_ptr);
        set => PaddleNative.PD_ConfigSetCpuMathLibraryNumThreads(_ptr, value);
    }

    /// <summary>Create a new Predictor</summary>
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

    /// <summary>Delete all passes that has a certain type 'pass'.</summary>
    public unsafe void DeletePass(string passName)
    {
        byte[] passNameBytes = PaddleEncoding.GetBytes(passName);
        fixed (byte* ptr = passNameBytes)
        {
            PaddleNative.PD_ConfigDeletePass(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>Destroy the paddle config</summary>
    public void Dispose()
    {
        if (_ptr != IntPtr.Zero)
        {
            PaddleNative.PD_ConfigDestroy(_ptr);
            _ptr = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Apply the configuration to the PaddleConfig.
    /// </summary>
    /// <param name="configure">The action to configure the PaddleConfig.</param>
    /// <returns>The modified PaddleConfig.</returns>
    public PaddleConfig Apply(Action<PaddleConfig> configure)
    {
        configure(this);
        return this;
    }
}

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

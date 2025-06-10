using Sdcb.PaddleInference.Native;
using Sdcb.PaddleInference.TensorRt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sdcb.PaddleInference;

/// <summary>
/// Provides a configuration for Paddle Inference.
/// </summary>
public class PaddleConfig : IDisposable
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

        Dictionary<string, string> allowedCombination = new()
        {
            ["inference.json"]    = "inference.pdiparams",
            ["model.json"]        = "model.pdiparams",
            ["inference.pdmodel"] = "inference.pdiparams",
            ["model.pdmodel"]     = "model.pdiparams",
        };

        foreach (KeyValuePair<string, string> kv in allowedCombination)
        {
            string pdmodel = Path.Combine(modelDir, kv.Key);
            string pdiparams = Path.Combine(modelDir, kv.Value);

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

    internal static Version GetVersion()
    {
        string version = Version.Split('\n')[0].Split(':')[1];
        string[] parts = version.Split('.');
        if (parts.Length != 3)
        {
            return new Version(2, 5, 0);
        }

        return new Version(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }

    /// <summary>
    /// Returns a pointer to the underlying unmanaged object handle.
    /// </summary>
    /// <returns>An unmanaged object handle.</returns>
    public IntPtr UnsafeGetHandle() => _ptr;

    /// <summary>A boolean state telling whether logs in Paddle inference are enabled.</summary>
    public bool GLogEnabled
    {
        get
        {
            ThrowIfDisposed();

            return PaddleNative.PD_ConfigGlogInfoDisabled(_ptr) == 0;
        }
        set
        {
            ThrowIfDisposed();

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
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigMemoryOptimEnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();
            PaddleNative.PD_ConfigEnableMemoryOptim(_ptr, (sbyte)(value ? 1 : 0));
        }
    }

    /// <summary>A boolean state telling whether the model is set from the CPU memory.</summary>
    public bool IsMemoryModel
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigModelFromMemory(_ptr) != 0;
        }
    }

    /// <summary>A boolean state telling whether to use the MKLDNN.</summary>
    public bool MkldnnEnabled
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigMkldnnEnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();

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

    /// <summary>A boolean state telling whether to use CUDNN.</summary>
    public bool CudnnEnabled
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigCudnnEnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();

            if (value)
            {
                PaddleNative.PD_ConfigEnableCudnn(_ptr);
            }
            else if (CudnnEnabled)
            {
                Console.WriteLine($"Warn: Cudnn cannot disabled after enabled.");
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
            ThrowIfDisposed();
            PaddleNative.PD_ConfigSetMkldnnCacheCapacity(_ptr, value);
        }
    }

    /// <summary>
    /// Gets or sets a bool indicating whether ONNX Runtime is enabled.
    /// </summary>
    /// <remarks>
    /// If true, ONNX Runtime is enabled, otherwise disabled.
    /// </remarks>
    public bool OnnxEnabled
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigONNXRuntimeEnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();

            if (value)
            {
                PaddleNative.PD_ConfigEnableONNXRuntime(_ptr);
            }
            else
            {
                PaddleNative.PD_ConfigDisableONNXRuntime(_ptr);
            }
        }
    }

    /// <summary>
    /// Enable ONNX Runtime optimization.
    /// </summary>
    public void EnableOnnxOptimization()
    {
        ThrowIfDisposed();
        PaddleNative.PD_ConfigEnableORTOptimization(_ptr);
    }

    /// <summary>Turn on profiling report. If not turned on, no profiling report will be generated.</summary>
    public bool ProfileEnabled
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigProfileEnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();
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
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigUseGpu(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();
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
    public int GpuDeviceId
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigGpuDeviceId(_ptr);
        }
    }

    /// <summary>Get the initial size in MB of the GPU memory pool.</summary>
    public int InitialGpuMemorySizeMB
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigMemoryPoolInitSizeMb(_ptr);
        }
    }

    /// <summary>Get the proportion of the initial memory pool size compared to the device.</summary>
    public float FractionOfGpuMemoryForPool
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigFractionOfGpuMemoryForPool(_ptr);
        }
    }

    /// <param name="workspaceSize">The memory size(in byte) used for TensorRT workspace.</param>
    /// <param name="maxBatchSize">The maximum batch size of this prediction task, better set as small as possible for less performance loss.</param>
    /// <param name="minSubgraphSize">Paddle-TRT is running under subgraph, Paddle-TRT will only been enabled when subgraph node count > min_subgraph_size to avoid performance loss</param>
    /// <param name="precision">The precision used in TensorRT.</param>
    /// <param name="useStatic">Serialize optimization information to disk for reusing.</param>
    /// <param name="useCalibMode">Use TRT int8 calibration(post training quantization)</param>
    public void EnableTensorRtEngine(
        int workspaceSize = 1 << 20,
        int maxBatchSize = 1,
        int minSubgraphSize = 20,
        PaddlePrecision precision = PaddlePrecision.Float32,
        bool useStatic = true,
        bool useCalibMode = false)
    {
        ThrowIfDisposed();
        PaddleNative.PD_ConfigEnableTensorRtEngine(_ptr, workspaceSize, maxBatchSize, minSubgraphSize, precision, (sbyte)(useStatic ? 1 : 0), (sbyte)(useCalibMode ? 1 : 0));
    }

    /// <summary>A boolean state telling whether the TensorRT engine is used.</summary>
    public bool TensorRtEngineEnabled
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigTensorRtEngineEnabled(_ptr) != 0;
        }
    }

    /// <summary>A boolean state telling whether the trt dynamic_shape is used.</summary>
    public bool TensorRtDynamicShapeEnabled
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigTensorRtDynamicShapeEnabled(_ptr) != 0;
        }
    }

    /// <summary>A boolean state telling whether to use the TensorRT DLA.</summary>
    public bool TensorRtDlaEnabled
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigTensorRtDlaEnabled(_ptr) != 0;
        }
    }

    /// <summary>Collect shape info of all tensors in compute graph.</summary>
    public unsafe void CollectShapeRangeInfo(string rangeShapeInfoPath)
    {
        ThrowIfDisposed();
        fixed (byte* cacheDirPtr = PaddleEncoding.GetBytes(rangeShapeInfoPath))
        {
            PaddleNative.PD_ConfigCollectShapeRangeInfo(_ptr, (IntPtr)cacheDirPtr);
        }
    }

    /// <summary>A boolean state telling whether to collect shape info.</summary>
    public bool ShapeRangeInfoCollected
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigShapeRangeInfoCollected(_ptr) != 0;
        }
    }

    /// <summary>Enable tuned tensorrt dynamic shape.</summary>
    public unsafe void EnableTunedTensorRtDynamicShape(string rangeShapeInfoPath, bool allowBuildAtRuntime = true)
    {
        ThrowIfDisposed();
        fixed (byte* cacheDirPtr = PaddleEncoding.GetBytes(rangeShapeInfoPath))
        {
            PaddleNative.PD_ConfigEnableTunedTensorRtDynamicShape(_ptr, (IntPtr)cacheDirPtr, (sbyte)(allowBuildAtRuntime ? 1 : 0));
        }
    }

    /// <summary>A boolean state telling whether to use tuned tensorrt dynamic shape.</summary>
    public bool TunedTensorRtDynamicShape
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigTunedTensorRtDynamicShape(_ptr) != 0;
        }
    }

    /// <summary>Set the path of optimization cache directory.</summary>
    public unsafe void SetOptimCacheDir(string path)
    {
        ThrowIfDisposed();
        fixed (byte* cacheDirPtr = PaddleEncoding.GetBytes(path))
        {
            PaddleNative.PD_ConfigSetOptimCacheDir(_ptr, (IntPtr)cacheDirPtr);
        }
    }

    /// <summary>Set min, max, opt shape for TensorRT Dynamic shape mode.</summary>
    public unsafe void SetTrtDynamicShapeInfo(Dictionary<string, TensorRtDynamicShapeGroup> shapeInfo)
    {
        ThrowIfDisposed();
        using PtrFromStringArray shapeNames = new([.. shapeInfo.Keys]);

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
            ThrowIfDisposed();
            PD_Cstr* summaryPtr = null;
            try
            {
                summaryPtr = (PD_Cstr*)PaddleNative.PD_ConfigSummary(_ptr);
                return summaryPtr->ToString();
            }
            finally
            {
                if (summaryPtr != null)
                {
                    PaddleNative.PD_CstrDestroy((IntPtr)summaryPtr);
                }
            }
        }
    }

    /// <summary>A boolean state telling whether the thread local CUDA stream is enabled.</summary>
    public bool EnableGpuMultiStream
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigThreadLocalStreamEnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();
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
    public bool Valid
    {
        get
        {
            if (_ptr == IntPtr.Zero)
            {
                return false;
            }
            return PaddleNative.PD_ConfigIsValid(_ptr) != 0;
        }
    }

    /// <summary>Turn on GPU.</summary>
    public void EnableUseGpu(int initialMemoryMB, int deviceId, PaddlePrecision precision = PaddlePrecision.Float32)
    {
        ThrowIfDisposed();
        if (GetVersion() >= new Version(2, 5, 0))
        {
            // 2.5.0+ support precision
            PaddleNative.PD_ConfigEnableUseGpu(_ptr, (ulong)initialMemoryMB, deviceId, precision);
        }
        else
        {
            PaddleNative.PD_ConfigEnableUseGpu(_ptr, (ulong)initialMemoryMB, deviceId);
        }
    }

    /// <summary>Set the combined model with two specific pathes for program and parameters.</summary>
    public unsafe void SetModel(string programPath, string paramsPath)
    {
        ThrowIfDisposed();
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
    public string? ProgramPath
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigGetProgFile(_ptr).ANSIToString();
        }
    }

    /// <summary>Get the params file path.</summary>
    public string? ParamsPat
    {
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigGetParamsFile(_ptr).ANSIToString();
        }
    }

    /// <summary>Specify the memory buffer of program and parameter. Used when model and params are loaded directly from memory.</summary>
    public unsafe void SetMemoryModel(byte[] programBuffer, byte[] paramsBuffer)
    {
        ThrowIfDisposed();
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
        get
        {
            ThrowIfDisposed();
            return PaddleNative.PD_ConfigGetCpuMathLibraryNumThreads(_ptr);
        }
        set
        {
            ThrowIfDisposed();
            PaddleNative.PD_ConfigSetCpuMathLibraryNumThreads(_ptr, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the new executor is enabled.
    /// </summary>
    public bool NewExecutorEnabled
    {
        get
        {
            ThrowIfDisposed();
            if (GetVersion() < new Version(3, 0, 0))
            {
                return false;
            }
            return PaddleNative.PD_ConfigNewExecutorEnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();
            if (GetVersion() < new Version(3, 0, 0))
            {
                Console.WriteLine("New Executor is not supported in Paddle Inference version < 3.0.0");
                return;
            }
            PaddleNative.PD_ConfigEnableNewExecutor(_ptr, (sbyte)(value ? 1 : 0));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the new IR (Intermediate Representation) feature is enabled.
    /// </summary>
    public bool NewIREnabled
    {
        get
        {
            ThrowIfDisposed();
            if (GetVersion() < new Version(3, 0, 0))
            {
                return false;
            }

            return PaddleNative.PD_ConfigNewIREnabled(_ptr) != 0;
        }
        set
        {
            ThrowIfDisposed();
            if (GetVersion() < new Version(3, 0, 0))
            {
                Console.WriteLine("New IR is not supported in Paddle Inference version < 3.0.0");
                return;
            }

            PaddleNative.PD_ConfigEnableNewIR(_ptr, (sbyte)(value ? 1 : 0));
        }
    }

    /// <summary>
    /// Sets a value indicating whether the optimized model should be used.
    /// </summary>
    public bool UseOptimizedModel
    {
        set
        {
            ThrowIfDisposed();
            PaddleNative.PD_ConfigUseOptimizedModel(_ptr, (sbyte)(value ? 1 : 0));
        }
    }

    /// <summary>Create a new Predictor, and then dispose the config.</summary>
    public PaddlePredictor CreatePredictor()
    {
        ThrowIfDisposed();

        try
        {
            return new PaddlePredictor(PaddleNative.PD_PredictorCreate(_ptr));
        }
        finally
        {
            if (GetVersion() >= new Version(2, 5, 0))
            {
                // For compatibility with version < 2.5.0, we need to delete the config in C#.
                Dispose();
            }
            else
            {
                // Version < 2.5.0 will delete the config in C++ when creating a predictor.
                _ptr = IntPtr.Zero;
            }
        }
    }

    /// <summary>
    /// Deletes a pass with the specified name.
    /// </summary>
    /// <param name="passName">The name of the pass to be deleted.</param>
    /// <remarks>
    /// This method deletes a pass from the configuration using the provided name.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="passName"/> is null.</exception>
    /// <seealso cref="PaddleNative.PD_ConfigDeletePass(IntPtr, IntPtr)"/>
    public unsafe void DeletePass(string passName)
    {
        ThrowIfDisposed();

        byte[] passNameBytes = PaddleEncoding.GetBytes(passName);
        fixed (byte* ptr = passNameBytes)
        {
            PaddleNative.PD_ConfigDeletePass(_ptr, (IntPtr)ptr);
        }
    }

    /// <summary>Get information of passes.</summary>
    public unsafe string[] Passes
    {
        get
        {
            ThrowIfDisposed();

            PD_OneDimArrayCstr* passInfo = (PD_OneDimArrayCstr*)PaddleNative.PD_ConfigAllPasses(_ptr);
            try
            {
                return passInfo->ToArray();
            }
            finally
            {
                PaddleNative.PD_OneDimArrayCstrDestroy((IntPtr)passInfo);
            }
        }
    }

    /// <summary>
    /// Apply the configuration to the PaddleConfig.
    /// </summary>
    /// <param name="configure">The action to configure the PaddleConfig.</param>
    /// <returns>The modified PaddleConfig.</returns>
    public PaddleConfig Apply(Action<PaddleConfig> configure)
    {
        ThrowIfDisposed();

        configure(this);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ThrowIfDisposed()
    {
        if (_ptr == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(PaddleConfig));
        }
    }

    /// <summary>
    /// Frees the unmanaged resources used by the <see cref="PaddleConfig"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // tell GC not to invoke the finalizer.
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="PaddleConfig"/> class.
    /// </summary>
    ~PaddleConfig()
    {
        Dispose(false);
    }

    /// <summary>
    /// Frees the unmanaged resources used by the <see cref="PaddleConfig"/> class.
    /// </summary>
    /// <param name="disposing">true if called from Dispose(); false if called from Finalize().</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_ptr != IntPtr.Zero)
        {
            PaddleNative.PD_ConfigDestroy(_ptr);
            _ptr = IntPtr.Zero;
        }

        if (disposing)
        {
            // Release other managed resources
        }
    }
}

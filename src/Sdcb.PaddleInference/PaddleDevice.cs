using Sdcb.PaddleInference.TensorRt;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdcb.PaddleInference;

/// <summary>
/// Static class that provides actions for creating device-specific configurations for Paddle.
/// </summary>
public static class PaddleDevice
{
    /// <summary>
    /// Creates an action that sets up a Paddle configuration for using MKL-DNN on the CPU.
    /// </summary>
    /// <param name="cacheCapacity">The capacity(in MB) of the MKL-DNN primitive cache. Default value is 1.</param>
    /// <param name="cpuMathThreadCount">The number of CPU math threads to use. Default value is 0.</param>
    /// <param name="memoryOptimized">Whether to use memory optimized mode. Default value is true.</param>
    /// <param name="glogEnabled">Whether to enable GLog. Default value is false.</param>
    /// <returns>An Action that accepts a PaddleConfig as parameter.</returns>
    public static Action<PaddleConfig> Mkldnn(int cacheCapacity = 1, int cpuMathThreadCount = 0, bool memoryOptimized = true, bool glogEnabled = false)
    {
        return cfg =>
        {
            cfg.MkldnnEnabled = true;
            cfg.MkldnnCacheCapacity = cacheCapacity;
            cfg.CpuMathThreadCount = cpuMathThreadCount;
            CommonAction(cfg, memoryOptimized, glogEnabled);
        };
    }

    /// <summary>
    /// Creates an action that sets up a Paddle configuration for using the GPU with CUDA.
    /// </summary>
    /// <param name="initialMemoryMB">The initial memory size in megabytes. Default value is 500.</param>
    /// <param name="deviceId">The GPU device ID to use. Default value is 0.</param>
    /// <param name="multiStream">Whether to use multi-stream for concurrency. Default value is false.</param>
    /// <param name="memoryOptimized">Whether to use memory optimized mode. Default value is true.</param>
    /// <param name="glogEnabled">Whether to enable GLog. Default value is false.</param>
    /// <returns>An Action that accepts a PaddleConfig as parameter.</returns>
    public static Action<PaddleConfig> Gpu(int initialMemoryMB = 500, int deviceId = 0, bool multiStream = false, bool memoryOptimized = true, bool glogEnabled = false)
    {
        return cfg =>
        {
            cfg.EnableUseGpu(initialMemoryMB, deviceId);
            cfg.EnableGpuMultiStream = multiStream;
            CommonAction(cfg, memoryOptimized, glogEnabled);
        };
    }

    /// <summary>
    /// Creates an action that sets up a Paddle configuration for using the TensorRT engine.
    /// </summary>
    /// <param name="rangeShapeInfoKey">The key name of the range-shape information. This should match the name of the range-shape information dumped by PaddleInference. Required parameter.</param>
    /// <param name="cacheDir">The path to the TensorRT cache directory. If null, the default cache directory is used.</param>
    /// <param name="workspaceSize">The workspace size in bytes for TensorRT engine. Default value is 1MB.</param>
    /// <param name="maxBatchSize">The maximum batch size for TensorRT engine. Default value is 1.</param>
    /// <param name="minSubgraphSize">The minimum sub-graph size for tensorRT engine. Default value is 20.</param>
    /// <param name="precision">The Precision mode for TensorRT engine. Default value is Float32.</param>
    /// <param name="useStatic">Whether to use static engine optimization or dynamic engine optimization. Default value is true.</param>
    /// <param name="useCalibMode">Whether to use TensorRT optimization with calibration mode. Default value is false.</param>
    /// <returns>An Action that accepts a PaddleConfig as parameter.</returns>
    public static Action<PaddleConfig> TensorRt(string rangeShapeInfoKey, string? cacheDir = null,
        long workspaceSize = 1 << 20,
        int maxBatchSize = 1,
        int minSubgraphSize = 20,
        PaddlePrecision precision = PaddlePrecision.Float32,
        bool useStatic = true,
        bool useCalibMode = false)
    {
        return cfg =>
        {
            cacheDir ??= TensorRtDefaults.DefaultCacheFolder;
            Directory.CreateDirectory(cacheDir);
            string subGraphFileName = Path.Combine(cacheDir, rangeShapeInfoKey);
            if (!File.Exists(subGraphFileName))
            {
                cfg.CollectShapeRangeInfo(subGraphFileName);
            }
            else
            {
                cfg.EnableTunedTensorRtDynamicShape(subGraphFileName, allowBuildAtRuntime: true);
            }
            cfg.SetOptimCacheDir(cacheDir);
            cfg.EnableTensorRtEngine(workspaceSize, maxBatchSize, minSubgraphSize, precision, useStatic, useCalibMode);
        };
    }

    /// <summary>
    /// Creates an action that sets up a Paddle configuration for using the TensorRT engine with dynamic shapes.
    /// </summary>
    /// <param name="shapeInfo">The TensorRT dynamic shape information to set. Required parameter.</param>
    /// <param name="cacheDir">The path to the TensorRT cache directory.</param>
    /// <param name="workspaceSize">The workspace size in bytes for TensorRT engine. Default value is 1MB.</param>
    /// <param name="maxBatchSize">The maximum batch size for TensorRT engine. Default value is 1.</param>
    /// <param name="minSubgraphSize">The minimum sub-graph size for tensorRT engine. Default value is 20.</param>
    /// <param name="precision">The Precision mode for TensorRT engine. Default value is Float32.</param>
    /// <param name="useStatic">Whether to use static engine optimization or dynamic engine optimization. Default value is true.</param>
    /// <param name="useCalibMode">Whether to use TensorRT optimization with calibration mode. Default value is false.</param>
    /// <returns>An Action that accepts a PaddleConfig as parameter.</returns>
    public static Action<PaddleConfig> TensorRt(Dictionary<string, TensorRtDynamicShapeGroup> shapeInfo, string? cacheDir = null,
        long workspaceSize = 1 << 20,
        int maxBatchSize = 1,
        int minSubgraphSize = 20,
        PaddlePrecision precision = PaddlePrecision.Float32,
        bool useStatic = true,
        bool useCalibMode = false)
    {
        return cfg =>
        {
            cacheDir ??= TensorRtDefaults.DefaultCacheFolder;
            Directory.CreateDirectory(cacheDir);
            cfg.SetOptimCacheDir(cacheDir);
            cfg.SetTrtDynamicShapeInfo(shapeInfo);
            cfg.EnableTensorRtEngine(workspaceSize, maxBatchSize, minSubgraphSize, precision, useStatic, useCalibMode);
        };
    }

    /// <summary>
    /// Creates an action that sets up a Paddle configuration for using OpenBLAS on the CPU.
    /// </summary>
    /// <param name="cpuMathThreadCount">The number of CPU math threads to use. Default value is 0.</param>
    /// <param name="memoryOptimized">Whether to use memory optimized mode. Default value is true.</param>
    /// <param name="glogEnabled">Whether to enable GLog. Default value is false.</param>
    /// <returns>An Action that accepts a PaddleConfig as parameter.</returns>
    public static Action<PaddleConfig> Openblas(int cpuMathThreadCount = 0, bool memoryOptimized = true, bool glogEnabled = false)
    {
        return cfg =>
        {
            cfg.CpuMathThreadCount = cpuMathThreadCount;
            CommonAction(cfg, memoryOptimized, glogEnabled);
        };
    }

    private static void CommonAction(PaddleConfig cfg, bool memoryOptimized, bool glogEnabled)
    {
        cfg.MemoryOptimized = memoryOptimized;
        cfg.GLogEnabled = glogEnabled;
    }
}

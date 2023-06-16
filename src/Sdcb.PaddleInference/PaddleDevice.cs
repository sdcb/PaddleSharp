using Sdcb.PaddleInference.TensorRt;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sdcb.PaddleInference;

public static class PaddleDevice
{
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

    public static Action<PaddleConfig> Gpu(int initialMemoryMB = 500, int deviceId = 0, bool multiStream = false, bool memoryOptimized = true, bool glogEnabled = false)
    {
        return cfg =>
        {
            cfg.EnableUseGpu(initialMemoryMB, deviceId);
            cfg.EnableGpuMultiStream = multiStream;
            CommonAction(cfg, memoryOptimized, glogEnabled);
        };
    }

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

public static class PaddleConfigureExtensions
{
    public static Action<PaddleConfig> And(this Action<PaddleConfig> action1, Action<PaddleConfig> action2)
    {
        return cfg =>
        {
            action1(cfg);
            action2(cfg);
        };
    }
}

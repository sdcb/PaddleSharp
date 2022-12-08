using System;

namespace Sdcb.PaddleInference
{
    public static class PaddleConfigure
    {
        public static Action<PaddleConfig> Mkldnn(int cacheCapacity = 10, int cpuMathThreadCount = 0, bool memoryOptimized = true, bool glogEnabled = false)
        {
            return cfg =>
            {
                cfg.MkldnnEnabled = true;
                cfg.MkldnnCacheCapacity = cacheCapacity;
                cfg.CpuMathThreadCount = cpuMathThreadCount;
                CommonAction(cfg, memoryOptimized, glogEnabled);
            };
        }

        public static Action<PaddleConfig> Gpu(int initialMemoryMB = 200, int deviceId = 0, bool multiStream = false, bool memoryOptimized = true, bool glogEnabled = false)
        {
            return cfg =>
            {
                cfg.EnableUseGpu(initialMemoryMB, deviceId);
                cfg.EnableGpuMultiStream = multiStream;
                CommonAction(cfg, memoryOptimized, glogEnabled);
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
}

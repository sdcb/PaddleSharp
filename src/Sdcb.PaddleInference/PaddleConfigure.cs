namespace Sdcb.PaddleInference
{
    public static class PaddleConfigure
    {
        public static void EnableMkldnn(this PaddleConfig c)
        {
            c.MkldnnEnabled = true;
            c.MkldnnCacheCapacity = 10;
            c.MemoryOptimized = true;
        }

        public static void EnableGpu(this PaddleConfig c, int initialMemoryMB = 500, int deviceId = 0)
        {
            c.EnableUseGpu(initialMemoryMB, deviceId);
            c.EnableGpuMultiStream = true;
            c.MemoryOptimized = true;
        }
    }
}

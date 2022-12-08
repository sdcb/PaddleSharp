using System;
using System.IO;

namespace Sdcb.PaddleInference.TensorRt
{
    public static class TensorRtDefaults
    {
        public static readonly string DefaultCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sdcb.PaddleInference", "TensorRtCache");
    }
}

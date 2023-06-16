using System;
using System.IO;

namespace Sdcb.PaddleInference.TensorRt;

/// <summary>
/// Default paths and settings for TensorRT integration with Paddle.
/// </summary>
public static class TensorRtDefaults
{
    /// <summary>
    /// The default cache folder for TensorRT files.
    /// </summary>
    public static readonly string DefaultCacheFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sdcb.PaddleInference", "TensorRtCache");
}

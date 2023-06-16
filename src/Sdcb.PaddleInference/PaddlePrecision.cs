namespace Sdcb.PaddleInference;

/// <summary>
/// Represents the supported precisions in Paddle Inference.
/// </summary>
public enum PaddlePrecision
{
    /// <summary>
    /// Single-precision floating point format.
    /// </summary>
    Float32 = 0,
    /// <summary>
    /// 8-bit integer format.
    /// </summary>
    Int8 = 1,
    /// <summary>
    /// Half-precision floating point format.
    /// </summary>
    Float16 = 2,
}

namespace Sdcb.PaddleInference;

/// <summary>
/// Represents the place for a paddle memory(C API type: PD_PlaceType).
/// </summary>
public enum PaddleMemoryPlace
{
    /// <summary>
    /// The memory place is unknown.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// The memory place is a central processing unit (CPU).
    /// </summary>
    Cpu,

    /// <summary>
    /// The memory place is a graphics processing unit (GPU).
    /// </summary>
    Gpu,

    /// <summary>
    /// The memory place is a eXtensible processing unit (XPU).
    /// </summary>
    Xpu,

    /// <summary>
    /// The memory place is a custom-defined place.
    /// </summary>
    Custom
}

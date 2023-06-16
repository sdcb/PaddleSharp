namespace Sdcb.PaddleInference;

/// <summary>
/// An enumeration representing the different data types
/// that can be used in Paddle Inference.
/// </summary>
public enum DataTypes
{
    /// <summary>
    /// The data type is unknown.
    /// </summary>
    Unknown = -1,
    /// <summary>
    /// 32-bit floating point number data type.
    /// </summary>
    Float32,
    /// <summary>
    /// 32-bit integer data type.
    /// </summary>
    Int32,
    /// <summary>
    /// 64-bit integer data type.
    /// </summary>
    Int64,
    /// <summary>
    /// Unsigned 8-bit integer data type.
    /// </summary>
    UInt8,
    /// <summary>
    /// Signed 8-bit integer data type.
    /// </summary>
    Int8,
}

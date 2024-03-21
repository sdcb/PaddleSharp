namespace Sdcb.PaddleInference;

/// <summary>
/// Represents the input/output information for the PaddlePaddle inference framework.
/// </summary>
public record PaddleIOInfo
{
    /// <summary>
    /// Gets or init the name of the input/output tensor.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or init the shape of the input/output tensor.
    /// </summary>
    public required int[] Shape { get; init; }

    /// <summary>
    /// Gets or init the data type of the input/output tensor.
    /// </summary>
    public required PaddleDataType DataType { get; init; }
}

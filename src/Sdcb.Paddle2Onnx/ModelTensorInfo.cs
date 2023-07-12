namespace Sdcb.Paddle2Onnx;

/// <summary>
/// Represents a model tensor with its name, shape, and rank.
/// </summary>
public record ModelTensorInfo(string Name, int[] Shape, int Rank);

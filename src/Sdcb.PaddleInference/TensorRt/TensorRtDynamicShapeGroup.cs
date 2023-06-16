namespace Sdcb.PaddleInference.TensorRt;

/// <summary>
/// Represents a group of tensor shapes with dynamic dimensions for inference in TensorRT.
/// </summary>
/// <remarks>
/// This record contains the minimum, maximum, and optimal shapes for a group of tensors.
/// </remarks>
public record TensorRtDynamicShapeGroup(
    int[] Min,
    int[] Max,
    int[] Optimal);

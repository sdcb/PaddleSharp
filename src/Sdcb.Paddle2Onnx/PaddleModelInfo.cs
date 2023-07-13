namespace Sdcb.Paddle2Onnx;

/// <summary>
/// Represents a Paddle model information.
/// </summary>
public record PaddleModelInfo(string[] InputNames, string[] OutputNames);

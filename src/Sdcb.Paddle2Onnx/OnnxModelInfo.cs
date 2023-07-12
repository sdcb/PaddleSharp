namespace Sdcb.Paddle2Onnx;

/// <summary>
/// Represents information about an ONNX model, including its input and output tensors.
/// </summary>
public record OnnxModelInfo(ModelTensorInfo[] Inputs, ModelTensorInfo[] Outputs);

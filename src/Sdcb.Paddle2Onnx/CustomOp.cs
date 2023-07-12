namespace Sdcb.Paddle2Onnx;

/// <summary>
/// Represents a custom operation to be exported to ONNX.
/// </summary>
public record CustomOp(string OpName, string ExportName);

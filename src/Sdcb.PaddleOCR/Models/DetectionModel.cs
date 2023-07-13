using Sdcb.PaddleInference;
using Sdcb.PaddleInference.TensorRt;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;

namespace Sdcb.PaddleOCR.Models;

/// <summary>
/// Represents a detection model.
/// </summary>
public abstract class DetectionModel : OcrBaseModel
{
    /// <summary>
    /// Constructor for initializing an instance of the <see cref="DetectionModel"/> class.
    /// </summary>
    /// <param name="version">The version of detection model.</param>
    public DetectionModel(ModelVersion version) : base(version)
    {
    }

    /// <inheritdoc/>
    public override Action<PaddleConfig> DefaultDevice => Version switch
    {
        ModelVersion.V4 => PaddleDevice.Onnx(),
        _ => PaddleDevice.Mkldnn(),
    };

    /// <summary>
    /// Returns an instance of the DetectionModel class from the directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path where model files are located.</param>
    /// <param name="version">The version of detection model.</param>
    /// <returns>An instance of the DetectionModel class.</returns>
    public static DetectionModel FromDirectory(string directoryPath, ModelVersion version) => new FileDetectionModel(directoryPath, version);

    /// <summary>
    /// Configures PaddleDevice to use TensorRt dynamic shape group and returns an action that takes PaddleConfig as input.
    /// </summary>
    /// <param name="maxEdge">The maximum edge of input image size.</param>
    /// <param name="cacheDir">The directory to store the transformed model files.</param>
    /// <returns>An action that takes PaddleConfig as input.</returns>
    public static Action<PaddleConfig> V3TensorRt(int maxEdge = 1536, string? cacheDir = null)
    {
        return PaddleDevice.TensorRt(new Dictionary<string, TensorRtDynamicShapeGroup>
        {
            // tensor name                  min shape                     max shape                       optimal shape
            ["x"] = new(new[] { 1, 3, 50, 50 }, new[] { 1, 3, maxEdge, maxEdge }, new[] { 1, 3, 640, 640 }),
            ["conv2d_92.tmp_0"] = new(new[] { 1, 120, 20, 20 }, new[] { 1, 120, 400, 400 }, new[] { 1, 120, 160, 160 }),
            ["conv2d_91.tmp_0"] = new(new[] { 1, 24, 10, 10 }, new[] { 1, 24, 200, 200 }, new[] { 1, 24, 80, 80 }),
            ["conv2d_59.tmp_0"] = new(new[] { 1, 96, 20, 20 }, new[] { 1, 96, 400, 400 }, new[] { 1, 96, 160, 160 }),
            ["nearest_interp_v2_1.tmp_0"] = new(new[] { 1, 256, 10, 10 }, new[] { 1, 256, 200, 200 }, new[] { 1, 256, 80, 80 }),
            ["nearest_interp_v2_2.tmp_0"] = new(new[] { 1, 256, 20, 20 }, new[] { 1, 256, 400, 400 }, new[] { 1, 256, 160, 160 }),
            ["conv2d_124.tmp_0"] = new(new[] { 1, 256, 20, 20 }, new[] { 1, 256, 400, 400 }, new[] { 1, 256, 160, 160 }),
            ["nearest_interp_v2_3.tmp_0"] = new(new[] { 1, 64, 20, 20 }, new[] { 1, 64, 400, 400 }, new[] { 1, 64, 160, 160 }),
            ["nearest_interp_v2_4.tmp_0"] = new(new[] { 1, 64, 20, 20 }, new[] { 1, 64, 400, 400 }, new[] { 1, 64, 160, 160 }),
            ["nearest_interp_v2_5.tmp_0"] = new(new[] { 1, 64, 20, 20 }, new[] { 1, 64, 400, 400 }, new[] { 1, 64, 160, 160 }),
            ["elementwise_add_7"] = new(new[] { 1, 56, 2, 2 }, new[] { 1, 56, 400, 400 }, new[] { 1, 56, 40, 40 }),
            ["nearest_interp_v2_0.tmp_0"] = new(new[] { 1, 256, 2, 2 }, new[] { 1, 256, 400, 400 }, new[] { 1, 256, 40, 40 }),
        }, cacheDir);
    }
}

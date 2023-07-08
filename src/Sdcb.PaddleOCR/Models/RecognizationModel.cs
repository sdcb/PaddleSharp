using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;

namespace Sdcb.PaddleOCR.Models;

/// <summary>
/// The abstract base class for recognition model.
/// </summary>
public abstract class RecognizationModel
{
    /// <summary>
    /// Constructor for initializing an instance of the <see cref="RecognizationModel"/> class.
    /// </summary>
    /// <param name="version">The version of recognition model.</param>
    public RecognizationModel(ModelVersion version)
    {
        Version = version;
    }

    /// <summary>
    /// Gets the version of the OCR model.
    /// </summary>
    public ModelVersion Version { get; }

    /// <summary>
    /// Get the label by specifying the index.
    /// </summary>
    /// <param name="i">The index of the label.</param>
    /// <returns>The label with the specified index.</returns>
    public abstract string GetLabelByIndex(int i);

    /// <summary>
    /// Gets a label by its index.
    /// </summary>
    /// <param name="i">The index of the label.</param>
    /// <param name="labels">The labels to search for the index.</param>
    /// <returns>The label at the specified index.</returns>
    public static string GetLabelByIndex(int i, IReadOnlyList<string> labels)
    {
        return i switch
        {
            var x when x > 0 && x <= labels.Count => labels[x - 1],
            var x when x == labels.Count + 1 => " ",
            _ => throw new Exception($"Unable to GetLabelByIndex: index {i} out of range {labels.Count}, OCR model or labels not matched?"),
        };
    }

    /// <summary>
    /// Create the paddle config for recognition model.
    /// </summary>
    /// <returns>The paddle config for recognition model.</returns>
    public abstract PaddleConfig CreateConfig();

    /// <summary>
    /// Get the OcrShape of recognition model.
    /// </summary>
    public virtual OcrShape Shape => Version switch
    {
        ModelVersion.V2 => new(3, 320, 32),
        ModelVersion.V3 => new(3, 320, 48),
        ModelVersion.V4 => new(3, 320, 48),
        _ => throw new ArgumentOutOfRangeException($"Unknown OCR model version: {Version}."),
    };

    /// <summary>
    /// Gets the default device for the classification model.
    /// </summary>
    public virtual Action<PaddleConfig> DefaultDevice => Version switch
    {
        ModelVersion.V2 => PaddleDevice.Mkldnn(),
        _ => PaddleDevice.Onnx(),
    };

    /// <summary>
    /// Deletes a pass from the PaddleConfig if the OCR model version is V3.
    /// </summary>
    /// <param name="config">The PaddleConfig to modify.</param>
    protected void ConfigPostProcess(PaddleConfig config)
    {
        if (Version == ModelVersion.V3 || Version == ModelVersion.V4)
        {
            config.DeletePass("matmul_transpose_reshape_fuse_pass");
        }
    }

    /// <summary>
    /// Create the RecognizationModel object with the specified directory path, label path and model version.
    /// </summary>
    /// <param name="directoryPath">The directory path of recognition model.</param>
    /// <param name="labelPath">The label path of recognition model.</param>
    /// <param name="version">The version of recognition model.</param>
    /// <returns>The RecognizationModel object created with the specified directory path, label path and model version.</returns>
    public static RecognizationModel FromDirectory(string directoryPath, string labelPath, ModelVersion version) => new FileRecognizationModel(directoryPath, labelPath, version);
}

using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;

namespace Sdcb.PaddleOCR.Models.Details;

/// <summary>
/// Base class for OCR model with version. 
/// </summary>
public abstract class VersionedRecognizationModel : RecognizationModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VersionedRecognizationModel"/> class.
    /// </summary>
    /// <param name="version">Version of the OCR model.</param>
    public VersionedRecognizationModel(ModelVersion version)
    {
        Version = version;
    }

    /// <summary>
    /// Gets the version of the OCR model.
    /// </summary>
    public ModelVersion Version { get; }

    /// <inheritdoc/>
    public override OcrShape Shape => Version switch
    {
        ModelVersion.V2 => new(3, 320, 32),
        ModelVersion.V3 => new(3, 320, 48),
        ModelVersion.V4 => new(3, 320, 48),
        _ => throw new ArgumentOutOfRangeException($"Unknown OCR model version: {Version}."),
    };

    /// <summary>
    /// Gets a label by its index.
    /// </summary>
    /// <param name="i">The index of the label.</param>
    /// <param name="labels">The labels to search for the index.</param>
    /// <returns>The label at the specified index.</returns>
    protected static string GetLabelByIndex(int i, IReadOnlyList<string> labels)
    {
        return i switch
        {
            var x when x > 0 && x <= labels.Count => labels[x - 1],
            var x when x == labels.Count + 1 => " ",
            _ => throw new Exception($"Unable to GetLabelByIndex: index {i} out of range {labels.Count}, OCR model or labels not matched?"),
        };
    }

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
}

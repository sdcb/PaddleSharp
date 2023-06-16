using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;

namespace Sdcb.PaddleOCR.Models;

/// <summary>
/// The abstract base class for recognition model.
/// </summary>
public abstract class RecognizationModel
{
    /// <summary>
    /// Get the label by specifying the index.
    /// </summary>
    /// <param name="i">The index of the label.</param>
    /// <returns>The label with the specified index.</returns>
    public abstract string GetLabelByIndex(int i);

    /// <summary>
    /// Create the paddle config for recognition model.
    /// </summary>
    /// <returns>The paddle config for recognition model.</returns>
    public abstract PaddleConfig CreateConfig();

    /// <summary>
    /// Get the OcrShape of recognition model.
    /// </summary>
    public abstract OcrShape Shape { get; }

    /// <summary>
    /// Create the RecognizationModel object with the specified directory path, label path and model version.
    /// </summary>
    /// <param name="directoryPath">The directory path of recognition model.</param>
    /// <param name="labelPath">The label path of recognition model.</param>
    /// <param name="version">The version of recognition model.</param>
    /// <returns>The RecognizationModel object created with the specified directory path, label path and model version.</returns>
    public static RecognizationModel FromDirectory(string directoryPath, string labelPath, ModelVersion version) => new FileRecognizationModel(directoryPath, labelPath, version);
}

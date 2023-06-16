using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;

namespace Sdcb.PaddleOCR.Models;

/// <summary>
/// A base class for classification models used in PaddleOCR. Provides methods to create 
/// a PaddleConfig object and OcrShape object, as well as a static method to create a
/// ClassificationModel object from a directory path. 
/// </summary>
public abstract class ClassificationModel
{
    /// <summary>
    /// Gets the OcrShape of this classification model.
    /// </summary>
    public abstract OcrShape Shape { get; }

    /// <summary>
    /// Creates a PaddleConfig object for this classification model.
    /// </summary>
    /// <returns>a new PaddleConfig object</returns>
    public abstract PaddleConfig CreateConfig();

    /// <summary>
    /// The default OcrShape used in the classification model.
    /// </summary>
    public static OcrShape DefaultShape = new(3, 192, 48);

    /// <summary>
    /// Creates a ClassificationModel object from the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The path to the directory containing the model files</param>
    /// <returns>a new ClassificationModel object</returns>
    public static ClassificationModel FromDirectory(string directoryPath) => new FileClassificationModel(directoryPath);
}

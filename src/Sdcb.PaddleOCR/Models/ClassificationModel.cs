using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;
using System;

namespace Sdcb.PaddleOCR.Models;

/// <summary>
/// A base class for classification models used in PaddleOCR. Provides methods to create 
/// a PaddleConfig object and OcrShape object, as well as a static method to create a
/// ClassificationModel object from a directory path. 
/// </summary>
public abstract class ClassificationModel : OcrBaseModel
{
    /// <summary>
    /// Gets the OcrShape of this classification model.
    /// </summary>
    public abstract OcrShape Shape { get; }

    /// <summary>
    /// The default OcrShape used in the classification model.
    /// </summary>
    public static OcrShape DefaultShape = new(3, 192, 48);

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassificationModel"/> class with a given model version.
    /// </summary>
    /// <param name="version">The version of the classification model.</param>
    protected ClassificationModel(ModelVersion version) : base(version)
    {
    }

    /// <summary>
    /// Gets the default device for the classification model.
    /// </summary>
    public override Action<PaddleConfig> DefaultDevice => PaddleDevice.Mkldnn();

    /// <summary>
    /// Creates a ClassificationModel object from the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The path to the directory containing the model files</param>
    /// <param name="version">The version of the classification model.</param>
    /// <returns>a new ClassificationModel object</returns>
    public static ClassificationModel FromDirectory(string directoryPath, ModelVersion version = ModelVersion.V2) => new FileClassificationModel(directoryPath, version);
}

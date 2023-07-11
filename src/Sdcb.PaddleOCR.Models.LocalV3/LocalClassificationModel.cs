using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.LocalV3.Details;

namespace Sdcb.PaddleOCR.Models.LocalV3;

/// <summary>
/// Represents a local classification model.
/// </summary>
public class LocalClassificationModel : ClassificationModel
{
    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the shape of the model.
    /// </summary>
    public override OcrShape Shape => DefaultShape;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalClassificationModel"/> class.
    /// </summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="version">The version of the model.</param>
    public LocalClassificationModel(string name, ModelVersion version) : base(version)
    {
        Name = name;
    }

    /// <summary>
    /// Creates a PaddleConfig instance for this model.
    /// </summary>
    /// <returns>A PaddleConfig instance.</returns>
    public override PaddleConfig CreateConfig() => Utils.LoadLocalModel(Name);

    /// <summary>
    /// Gets the original model for text angle classification (Size: 1.38M).
    /// </summary>
    public static LocalClassificationModel ChineseMobileV2 => new("ch_ppocr_mobile_v2.0_cls", ModelVersion.V2);

    /// <summary>
    /// Gets all available classification models.
    /// </summary>
    public static LocalClassificationModel[] All => new[]
    {
        ChineseMobileV2,
    };
}

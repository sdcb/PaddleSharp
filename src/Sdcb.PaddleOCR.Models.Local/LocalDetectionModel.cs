using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Local.Details;

namespace Sdcb.PaddleOCR.Models.Local;

/// <summary>
/// This class represents a local detection model used by PaddleOCR to detect text from an image.
/// </summary>
public class LocalDetectionModel : DetectionModel
{
    /// <summary>
    /// Gets the name of this model.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalDetectionModel"/> class with the specified name and version.
    /// </summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="version">The version of the model.</param>
    public LocalDetectionModel(string name, ModelVersion version) : base(version)
    {
        Name = name;
    }

    /// <inheritdoc/>
    public override PaddleConfig CreateConfig() => Utils.LoadLocalModel(Name);

    /// <summary>
    /// Original lightweight model, supporting Chinese, English, multilingual text detection(Size: 3.8M)
    /// </summary>
    public static LocalDetectionModel ChineseV3 => new("ch_PP-OCRv3_det", ModelVersion.V3);

    /// <summary>
    /// Original lightweight detection model, supporting English(Size: 3.8M)
    /// </summary>
    public static LocalDetectionModel EnglishV3 => new("en_PP-OCRv3_det", ModelVersion.V3);

    /// <summary>
    /// Original lightweight detection model, supporting multiple languages(Size: 3.8M)
    /// </summary>
    public static LocalDetectionModel MultiLanguageV3 => new("ml_PP-OCRv3_det", ModelVersion.V3);

    /// <summary>
    /// Gets an array of all the available <see cref="LocalDetectionModel"/> objects.
    /// </summary>
    public static LocalDetectionModel[] All => new[]
    {
        ChineseV3,
        EnglishV3,
        MultiLanguageV3,
    };
}

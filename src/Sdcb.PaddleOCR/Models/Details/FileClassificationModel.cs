using Sdcb.PaddleInference;

namespace Sdcb.PaddleOCR.Models.Details;

/// <summary>
/// File classification model.
/// </summary>
public class FileClassificationModel : ClassificationModel
{
    /// <summary>
    /// Gets or sets the directory path for the model.
    /// </summary>
    public string DirectoryPath { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileClassificationModel"/> class.
    /// </summary>
    /// <param name="directoryPath">The directory path for the model.</param>
    public FileClassificationModel(string directoryPath)
    {
        DirectoryPath = directoryPath;
    }

    /// <inheritdoc/>
    public override OcrShape Shape => DefaultShape;

    /// <inheritdoc/>
    public override PaddleConfig CreateConfig() => PaddleConfig.FromModelDir(DirectoryPath);
}

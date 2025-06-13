namespace Sdcb.PaddleOCR.Models;

/// <summary>
/// Represents a full OCR model composed of a detection, classification and recognition model.
/// </summary>
public class FullOcrModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FullOcrModel"/> class.
    /// </summary>
    /// <param name="detectionModel">A detection model.</param>
    /// <param name="classificationModel">A classification model.</param>
    /// <param name="recognizationModel">A recognition model.</param>
    public FullOcrModel(DetectionModel detectionModel, ClassificationModel? classificationModel, RecognizationModel recognizationModel)
    {
        DetectionModel = detectionModel;
        ClassificationModel = classificationModel;
        RecognizationModel = recognizationModel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FullOcrModel"/> class.
    /// </summary>
    /// <param name="detectionModel">A detection model.</param>
    /// <param name="recognizationModel">A recognition model.</param>
    public FullOcrModel(DetectionModel detectionModel, RecognizationModel recognizationModel)
    {
        DetectionModel = detectionModel;
        RecognizationModel = recognizationModel;
    }

    /// <summary>
    /// Gets or sets the detection model.
    /// </summary>
    public DetectionModel DetectionModel { get; init; }

    /// <summary>
    /// Gets or sets the classification model.
    /// </summary>
    public ClassificationModel? ClassificationModel { get; init; }

    /// <summary>
    /// Gets or sets the recognition model.
    /// </summary>
    public RecognizationModel RecognizationModel { get; init; }
}

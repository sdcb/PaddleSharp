using System;
using System.IO;

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

    /// <summary>
    /// Creates an instance of the <see cref="FullOcrModel"/> class from a directory that contains OCR model files.
    /// </summary>
    /// <param name="modelFolderPath">The path to the model directory.</param>
    /// <param name="labelFilePath">The path to the label file.</param>
    /// <param name="version">The OCR model version.</param>
    /// <returns>A new instance of <see cref="FullOcrModel"/>.</returns>
    [Obsolete("This method is deprecated, use 'FromDirectory(string detectionModelDir, string classificationModelDir, string recognitionModelDir, string labelFilePath, ModelVersion version)' instead.")]
    public static FullOcrModel FromDirectory(string modelFolderPath, string labelFilePath, ModelVersion version)
    {
        return new FullOcrModel(
            DetectionModel.FromDirectory(Path.Combine(modelFolderPath, "det"), version),
            ClassificationModel.FromDirectory(Path.Combine(modelFolderPath, "cls")),
            RecognizationModel.FromDirectory(Path.Combine(modelFolderPath, "rec"), labelFilePath, version));
    }

    /// <summary>
    /// Creates an instance of the <see cref="FullOcrModel"/> class from directories that contain OCR model files.
    /// </summary>
    /// <param name="detectionModelDir">The path to the detection model directory.</param>
    /// <param name="classificationModelDir">The path to the classification model directory.</param>
    /// <param name="recognitionModelDir">The path to the recognition model directory.</param>
    /// <param name="labelFilePath">The path to the label file.</param>
    /// <param name="version">The OCR model version.</param>
    /// <returns>A new instance of <see cref="FullOcrModel"/>.</returns>
    [Obsolete("This method is deprecated, use 'FromDirectory(string detectionModelPath, string classificationModelPath, string recognitionModelPath, string labelFilePath, ModelVersion version)' instead.")]
    public static FullOcrModel FromDirectory(string detectionModelDir, string classificationModelDir, string recognitionModelDir, string labelFilePath, ModelVersion version)
    {
        return new FullOcrModel(
            DetectionModel.FromDirectory(detectionModelDir, version),
            ClassificationModel.FromDirectory(classificationModelDir),
            RecognizationModel.FromDirectory(recognitionModelDir, labelFilePath, version));
    }
}

using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models;
using System;
using System.Linq;

namespace Sdcb.PaddleOCR;

/// <summary>
/// Represents an OCR engine that uses PaddlePaddle models for object detection, classification, and recognition.
/// </summary>
public class PaddleOcrAll : IDisposable
{
    /// <summary>
    /// Gets the object detector used by this OCR engine.
    /// </summary>
    public PaddleOcrDetector Detector { get; }

    /// <summary>
    /// Gets the object classifier used by this OCR engine, or null if no classifier is used.
    /// </summary>
    public PaddleOcrClassifier? Classifier { get; }

    /// <summary>
    /// Gets the text recognizer used by this OCR engine.
    /// </summary>
    public PaddleOcrRecognizer Recognizer { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable 180-degree classification.
    /// </summary>
    public bool Enable180Classification { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to allow rotation detection.
    /// </summary>
    public bool AllowRotateDetection { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified PaddlePaddle models and device configuration.
    /// </summary>
    /// <param name="model">The full OCR model containing detection, classification, and recognition models.</param>
    /// <param name="device">The device configuration for running det, cls and rec models.</param>
    public PaddleOcrAll(FullOcrModel model, Action<PaddleConfig> device)
    {
        Detector = new PaddleOcrDetector(model.DetectionModel, device);
        if (model.ClassificationModel != null)
        {
            Classifier = new PaddleOcrClassifier(model.ClassificationModel, device);
        }
        Recognizer = new PaddleOcrRecognizer(model.RecognizationModel, device);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified PaddlePaddle models and device configurations for each model.
    /// </summary>
    /// <param name="model">The full OCR model containing detection, classification, and recognition models.</param>
    /// <param name="detectorDevice">The device configuration for running the detection model, default: Onnx.</param>
    /// <param name="classifierDevice">The device configuration for running the classification model, default: Mkldnn.</param>
    /// <param name="recognizerDevice">The device configuration for running the recognition model, default: Onnx.</param>
    public PaddleOcrAll(FullOcrModel model,
        Action<PaddleConfig>? detectorDevice = null,
        Action<PaddleConfig>? classifierDevice = null,
        Action<PaddleConfig>? recognizerDevice = null)
    {
        Detector = new PaddleOcrDetector(model.DetectionModel, detectorDevice ?? model.DetectionModel.DefaultDevice);
        if (model.ClassificationModel != null)
        {
            Classifier = new PaddleOcrClassifier(model.ClassificationModel, classifierDevice ?? model.ClassificationModel.DefaultDevice);
        }
        Recognizer = new PaddleOcrRecognizer(model.RecognizationModel, recognizerDevice ?? model.RecognizationModel.DefaultDevice);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified model path, label file path, model version and configure action.
    /// </summary>
    /// <param name="modelPath">Path to the model.</param>
    /// <param name="labelFilePath">Path to the label file.</param>
    /// <param name="version">Version of the model.</param>
    /// <param name="configure">Configure action for PaddleConfig.</param>
    [Obsolete("use PaddleOcrAll(PaddleOcrDetector detector, PaddleOcrClassifier? classifier, PaddleOcrRecognizer recognizer)")]
    public PaddleOcrAll(string modelPath, string labelFilePath, ModelVersion version, Action<PaddleConfig> configure)
        : this(FullOcrModel.FromDirectory(modelPath, labelFilePath, version), configure)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the detection, classification, recognition model directories, label file path, model version and the configure action.
    /// </summary>
    /// <param name="detectionModelDir">Path to the detection model directory.</param>
    /// <param name="classificationModelDir">Path to the classification model directory.</param>
    /// <param name="recognitionModelDir">Path to the recognition model directory.</param>
    /// <param name="labelFilePath">Path to the label file.</param>
    /// <param name="version">Version of the model.</param>
    /// <param name="configure">Configure action for PaddleConfig.</param>
    [Obsolete("use PaddleOcrAll(PaddleOcrDetector detector, PaddleOcrClassifier? classifier, PaddleOcrRecognizer recognizer)")]
    public PaddleOcrAll(string detectionModelDir, string classificationModelDir, string recognitionModelDir, string labelFilePath, ModelVersion version, Action<PaddleConfig> configure)
        : this(FullOcrModel.FromDirectory(detectionModelDir, classificationModelDir, recognitionModelDir, labelFilePath, version), configure)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified PaddlePaddle models and device configuration.
    /// </summary>
    /// <param name="detector">The object detector to use.</param>
    /// <param name="classifier">The object classifier to use, or null if no classifier is used.</param>
    /// <param name="recognizer">The text recognizer to use.</param>
    public PaddleOcrAll(PaddleOcrDetector detector, PaddleOcrClassifier? classifier, PaddleOcrRecognizer recognizer)
    {
        Detector = detector;
        Classifier = classifier;
        Recognizer = recognizer;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PaddleOcrAll"/> class that is a deep copy of the current instance.
    /// </summary>
    /// <returns>A new instance of the <see cref="PaddleOcrAll"/> class that is a deep copy of the current instance.</returns>
    public PaddleOcrAll Clone()
    {
        return new PaddleOcrAll(Detector.Clone(), Classifier?.Clone(), Recognizer.Clone())
        {
            AllowRotateDetection = AllowRotateDetection,
            Enable180Classification = Enable180Classification,
        };
    }

    /// <summary>
    /// Gets the cropped region of the source image specified by the given rectangle, clamping the rectangle coordinates to the image bounds.
    /// </summary>
    /// <param name="rect">The rectangle to crop.</param>
    /// <param name="size">The size of the source image.</param>
    /// <returns>The cropped rectangle.</returns>
    private static Rect GetCropedRect(Rect rect, Size size)
    {
        return Rect.FromLTRB(
            MathUtil.Clamp(rect.Left, 0, size.Width),
            MathUtil.Clamp(rect.Top, 0, size.Height),
            MathUtil.Clamp(rect.Right, 0, size.Width),
            MathUtil.Clamp(rect.Bottom, 0, size.Height));
    }

    /// <summary>
    /// Runs the OCR engine on the specified source image.
    /// </summary>
    /// <param name="src">The source image to run OCR on.</param>
    /// <param name="recognizeBatchSize">The batch size for recognition.</param>
    /// <returns>The OCR result.</returns>
    /// <exception cref="Exception">Thrown if 180-degree classification is enabled but no classifier is set.</exception>
    public PaddleOcrResult Run(Mat src, int recognizeBatchSize = 0)
    {
        if (Enable180Classification && Classifier == null)
        {
            throw new Exception($"Unable to do 180 degree Classification when classifier model is not set.");
        }

        RotatedRect[] rects = Detector.Run(src);

        Mat[] mats =
            rects.Select(rect =>
            {
                Mat roi = AllowRotateDetection ? GetRotateCropImage(src, rect) : src[GetCropedRect(rect.BoundingRect(), src.Size())];
                return Enable180Classification ? Classifier!.Run(roi) : roi;
            })
            .ToArray();
        try
        {
            return new PaddleOcrResult(Recognizer.Run(mats, recognizeBatchSize)
                .Select((result, i) => new PaddleOcrResultRegion(rects[i], result.Text, result.Score))
                .ToArray());
        }
        finally
        {
            foreach (Mat mat in mats)
            {
                mat.Dispose();
            }
        }
    }

    /// <summary>
    /// Gets the cropped and rotated image specified by the given rectangle from the source image.
    /// </summary>
    /// <param name="src">The source image to crop and rotate.</param>
    /// <param name="rect">The rotated rectangle specifying the region to crop and rotate.</param>
    /// <returns>The cropped and rotated image.</returns>
    public static Mat GetRotateCropImage(Mat src, RotatedRect rect)
    {
        bool wider = rect.Size.Width > rect.Size.Height;
        float angle = rect.Angle;
        Size srcSize = src.Size();
        Rect boundingRect = rect.BoundingRect();

        int expTop = Math.Max(0, 0 - boundingRect.Top);
        int expBottom = Math.Max(0, boundingRect.Bottom - srcSize.Height);
        int expLeft = Math.Max(0, 0 - boundingRect.Left);
        int expRight = Math.Max(0, boundingRect.Right - srcSize.Width);

        Rect rectToExp = boundingRect + new Point(expTop, expLeft);
        Rect roiRect = Rect.FromLTRB(
            boundingRect.Left + expLeft,
            boundingRect.Top + expTop,
            boundingRect.Right - expRight,
            boundingRect.Bottom - expBottom);
        using Mat boundingMat = src[roiRect];
        using Mat expanded = boundingMat.CopyMakeBorder(expTop, expBottom, expLeft, expRight, BorderTypes.Replicate);
        Point2f[] rp = rect.Points()
            .Select(v => new Point2f(v.X - rectToExp.X, v.Y - rectToExp.Y))
            .ToArray();

        Point2f[] srcPoints = (wider, angle) switch
        {
            (true, >= 0 and < 45) => new[] { rp[1], rp[2], rp[3], rp[0] },
            _ => new[] { rp[0], rp[3], rp[2], rp[1] }
        };

        var ptsDst0 = new Point2f(0, 0);
        var ptsDst1 = new Point2f(rect.Size.Width, 0);
        var ptsDst2 = new Point2f(rect.Size.Width, rect.Size.Height);
        var ptsDst3 = new Point2f(0, rect.Size.Height);

        using Mat matrix = Cv2.GetPerspectiveTransform(srcPoints, new[] { ptsDst0, ptsDst1, ptsDst2, ptsDst3 });

        Mat dest = expanded.WarpPerspective(matrix, new Size(rect.Size.Width, rect.Size.Height), InterpolationFlags.Nearest, BorderTypes.Replicate);

        if (!wider)
        {
            Cv2.Transpose(dest, dest);
        }
        else if (angle > 45)
        {
            Cv2.Flip(dest, dest, FlipMode.X);
        }
        return dest;
    }

    /// <summary>
    /// Releases the resources used by this OCR engine.
    /// </summary>
    public void Dispose()
    {
        Detector.Dispose();
        Classifier?.Dispose();
        Recognizer.Dispose();
    }
}

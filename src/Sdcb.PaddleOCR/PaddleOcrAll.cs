using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models;
using System;
using System.Linq;
using System.Runtime.InteropServices;

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
    /// <param name="detectorDevice">The device configuration for running the detection model, default: Mkldnn.</param>
    /// <param name="classifierDevice">The device configuration for running the classification model, default: Mkldnn.</param>
    /// <param name="recognizerDevice">The device configuration for running the recognition model, default: Mkldnn.</param>
    public PaddleOcrAll(FullOcrModel model,
        Action<PaddleConfig>? detectorDevice = null,
        Action<PaddleConfig>? classifierDevice = null,
        Action<PaddleConfig>? recognizerDevice = null)
    {
        Detector = new PaddleOcrDetector(model.DetectionModel, detectorDevice ?? model.DetectionModel.DefaultDevice);
        if (model.ClassificationModel != null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && RuntimeInformation.OSArchitecture == Architecture.Arm64)
            {
                Console.WriteLine("Skipping classifier model on macOS arm64 due to known issues: https://github.com/PaddlePaddle/Paddle/issues/72413");
            }
            else
            {
                Classifier = new PaddleOcrClassifier(model.ClassificationModel, classifierDevice ?? model.ClassificationModel.DefaultDevice);
            }
        }
        Recognizer = new PaddleOcrRecognizer(model.RecognizationModel, recognizerDevice ?? model.RecognizationModel.DefaultDevice);
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
                .Select((result, i) => new PaddleOcrResultRegion(rects[i], result.Text, result.Score, result.Chars))
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

        // Convention-independent corner ordering. RotatedRect.Angle / Points() ordering changed
        // across OpenCV versions (4.13 differs from 4.11), which broke the previous hard-coded
        // angle-based corner selection. OrderPointsClockwise orders the four corners geometrically
        // and is robust even near ±45° (a min/max of x±y can select duplicate points there and
        // produce a degenerate perspective transform).
        Point2f[] srcPoints = OrderPointsClockwise(rp); // tl, tr, br, bl

        int cw = Math.Max(1, (int)Math.Round(Math.Max(
            GetDistance(srcPoints[0], srcPoints[1]),
            GetDistance(srcPoints[2], srcPoints[3]))));
        int ch = Math.Max(1, (int)Math.Round(Math.Max(
            GetDistance(srcPoints[0], srcPoints[3]),
            GetDistance(srcPoints[1], srcPoints[2]))));

        Point2f[] dstPoints =
        {
            new Point2f(0, 0),
            new Point2f(cw, 0),
            new Point2f(cw, ch),
            new Point2f(0, ch),
        };

        using Mat matrix = Cv2.GetPerspectiveTransform(srcPoints, dstPoints);
        Mat dest = expanded.WarpPerspective(matrix, new Size(cw, ch), InterpolationFlags.Nearest, BorderTypes.Replicate);

        // Vertical text line → rotate 90° (CCW) so the recognizer sees a horizontal strip.
        if (ch >= cw * 1.5)
        {
            Cv2.Transpose(dest, dest);
            Cv2.Flip(dest, dest, FlipMode.X);
        }
        return dest;
    }

    /// <summary>
    /// Orders four polygon corners as top-left, top-right, bottom-right, bottom-left, independently
    /// of any OpenCV RotatedRect angle/point convention. Sorts by X, splits the two left-most and
    /// two right-most points (always disjoint), then disambiguates by Y on the left and by distance
    /// to the top-left corner on the right. Robust near ±45° where x±y heuristics pick duplicates.
    /// </summary>
    private static Point2f[] OrderPointsClockwise(Point2f[] points)
    {
        Point2f[] xSorted = points.OrderBy(p => p.X).ToArray();
        Point2f[] leftMost = xSorted.Take(2).OrderBy(p => p.Y).ToArray();
        Point2f[] rightMost = xSorted.Skip(2).ToArray();

        Point2f tl = leftMost[0];
        Point2f bl = leftMost[1];
        Point2f br = rightMost.OrderByDescending(p => GetDistance(tl, p)).First();
        Point2f tr = rightMost.Single(p => p != br);
        return new[] { tl, tr, br, bl };
    }

    private static float GetDistance(Point2f a, Point2f b)
        => (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

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

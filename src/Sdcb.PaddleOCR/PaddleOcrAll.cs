using OpenCvSharp;
using Sdcb.PaddleOCR.Models;
using System;
using System.Linq;

namespace Sdcb.PaddleOCR
{
    public class PaddleOcrAll : IDisposable
    {
        public PaddleOcrDetector Detector { get; }
        public PaddleOcrClassifier? Classifier { get; }
        public PaddleOcrRecognizer Recognizer { get; }

        public bool Enable180Classification { get; set; } = false;
        public bool AllowRotateDetection { get; set; } = true;

        public PaddleOcrAll(FullOcrModel model)
        {
            Detector = new PaddleOcrDetector(model.DetectionModel);
            if (model.ClassificationModel != null)
            {
                Classifier = new PaddleOcrClassifier(model.ClassificationModel);
            }
            Recognizer = new PaddleOcrRecognizer(model.RecognizationModel);
        }

        [Obsolete("use PaddleOcrAll(PaddleOcrDetector detector, PaddleOcrClassifier? classifier, PaddleOcrRecognizer recognizer)")]
        public PaddleOcrAll(string modelPath, string labelFilePath, ModelVersion version)
            : this(FullOcrModel.FromDirectory(modelPath, labelFilePath, version))
        {
        }

        [Obsolete("use PaddleOcrAll(PaddleOcrDetector detector, PaddleOcrClassifier? classifier, PaddleOcrRecognizer recognizer)")]
        public PaddleOcrAll(string detectionModelDir, string classificationModelDir, string recognitionModelDir, string labelFilePath, ModelVersion version)
            : this(FullOcrModel.FromDirectory(detectionModelDir, classificationModelDir, recognitionModelDir, labelFilePath, version))
        {
        }

        public PaddleOcrAll(PaddleOcrDetector detector, PaddleOcrClassifier? classifier, PaddleOcrRecognizer recognizer)
        {
            Detector = detector;
            Classifier = classifier;
            Recognizer = recognizer;
        }

        public PaddleOcrAll Clone()
        {
            return new PaddleOcrAll(Detector.Clone(), Classifier?.Clone(), Recognizer.Clone())
            {
                AllowRotateDetection = AllowRotateDetection,
                Enable180Classification = Enable180Classification,
            };
        }

        private static Rect GetCropedRect(Rect rect, Size size)
        {
            return Rect.FromLTRB(
                MathUtil.Clamp(rect.Left, 0, size.Width),
                MathUtil.Clamp(rect.Top, 0, size.Height),
                MathUtil.Clamp(rect.Right, 0, size.Width),
                MathUtil.Clamp(rect.Bottom, 0, size.Height));
        }

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
                    using Mat roi = AllowRotateDetection ? GetRotateCropImage(src, rect) : src[GetCropedRect(rect.BoundingRect(), src.Size())];
                    return Enable180Classification ? Classifier!.Run(roi) : roi.Clone();
                })
                .ToArray();

            return new PaddleOcrResult(Recognizer.Run(mats, recognizeBatchSize)
                .Select((result, i) => new PaddleOcrResultRegion(rects[i], result.Text, result.Score))
                .ToArray());
        }

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

        public void Dispose()
        {
            Detector.Dispose();
            Classifier?.Dispose();
            Recognizer.Dispose();
        }
    }
}

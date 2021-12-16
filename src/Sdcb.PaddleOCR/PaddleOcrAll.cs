using OpenCvSharp;
using System;
using System.IO;
using System.Linq;

namespace Sdcb.PaddleOCR
{
    public class PaddleOcrAll : IDisposable
	{
		public PaddleOcrDetector Detector { get; }
		public PaddleOcrRecognizer Recognizer { get; }

		public PaddleOcrAll(string modelPath, string labelFilePath)
		{
			Detector = new(Path.Combine(modelPath, "det"));
			Recognizer = new(Path.Combine(modelPath, "rec"), labelFilePath);
		}

		public PaddleOcrResult Run(Mat src)
		{
			Rect[] rects = Detector.Run(src);
			return new PaddleOcrResult(rects
				.Select(rect =>
				{
					PaddleOcrRecognizerResult result = Recognizer.Run(src[rect]);
					PaddleOcrResultRegion region = new(rect, result.Text, result.Score);
					return region;
				})
				.ToArray());
		}

		public PaddleOcrResult ConcurrentRun(Mat src)
		{
			Rect[] rects = Detector.ConcurrentRun(src);
			return new PaddleOcrResult(rects
				.AsParallel()
				.Select(rect =>
				{
					PaddleOcrRecognizerResult result = Recognizer.ConcurrentRun(src[rect]);
					PaddleOcrResultRegion region = new(rect, result.Text, result.Score);
					return region;
				})
				.ToArray());
		}

		public void Dispose()
		{
			Detector.Dispose();
			Recognizer.Dispose();
		}
	}
}

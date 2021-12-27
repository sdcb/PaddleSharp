using OpenCvSharp;
using Sdcb.PaddleInference;
using System;
using System.IO;
using System.Linq;

namespace Sdcb.PaddleOCR
{
    public class PaddleOcrDetector : IDisposable
	{
		PaddleConfig _c;
		PaddlePredictor _p;

		public int? MaxSize { get; set; } = 2048;
		public int? DilatedSize { get; set; } = 2;
		public float? BoxScoreThreahold { get; set; } = 0.7f;
		public float? BoxThreshold { get; set; } = 0.3f;
		public int MinSize { get; set; } = 3;
		public float UnclipRatio { get; set; } = 2.0f;

		public PaddleOcrDetector(string modelDir)
		{
			_c = new PaddleConfig();
			_c.SetModel(
				Path.Combine(modelDir, "inference.pdmodel"),
				Path.Combine(modelDir, "inference.pdiparams"));
			_p = _c.CreatePredictor();
		}

		public PaddleOcrDetector(PaddleConfig config)
		{
			_c = config;
			_p = _c.CreatePredictor();
		}

		public void Dispose()
		{
			_p.Dispose();
			_c.Dispose();
		}

		public static Mat Visualize(Mat src, RotatedRect[] rects, Scalar color, int thickness)
		{
			Mat clone = src.Clone();
			clone.DrawContours(rects.Select(x => x.Points().Select(x => (Point)x)), -1, color, thickness);
			return clone;
		}

		public RotatedRect[] Run(Mat src)
		{
			if (src.Empty())
			{
				throw new ArgumentException("src size should not be 0, wrong input picture provided?");
			}

			if (src.Channels() != 3)
			{
				throw new NotSupportedException($"{nameof(src)} channel must be 3, provided {src.Channels()}.");
			}

			using Mat resized = MatResize(src, MaxSize);
			using Mat padded = MatPadding32(resized);
			using Mat normalized = Normalize(padded);
			Size resizedSize = resized.Size();

			using (PaddleTensor input = _p.GetInputTensor(_p.InputNames[0]))
			{
				input.Shape = new[] { 1, 3, normalized.Rows, normalized.Cols };
				float[] data = ExtractMat(normalized);
				input.SetData(data);
			}
			if (!_p.Run())
			{
				throw new Exception("PaddlePredictor(Detector) run failed.");
			}

			using (PaddleTensor output = _p.GetOutputTensor(_p.OutputNames[0]))
			{
				float[] data = output.GetData<float>();
				int[] shape = output.Shape;

				using Mat pred = new Mat(shape[2], shape[3], MatType.CV_32FC1, data);
				using Mat cbuf = new();
				{
					using Mat roi = pred[0, resizedSize.Height, 0, resizedSize.Width];
					roi.ConvertTo(cbuf, MatType.CV_8UC1, 255);
				}
				using Mat dilated = new();
				{
					using Mat binary = BoxThreshold != null ?
						cbuf.Threshold((int)(BoxThreshold * 255), 255, ThresholdTypes.Binary) :
						cbuf;

					if (DilatedSize != null)
					{
						using Mat ones = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(DilatedSize.Value, DilatedSize.Value));
						Cv2.Dilate(binary, dilated, ones);
					}
					else
					{
						Cv2.CopyTo(binary, dilated);
					}
				}

				Point[][] contours = dilated.FindContoursAsArray(RetrievalModes.List, ContourApproximationModes.ApproxSimple);
				Size size = src.Size();
				double scaleRate = 1.0 * src.Width / resizedSize.Width;

				RotatedRect[] rects = contours
					.Where(x => BoxScoreThreahold == null || GetScore(x, pred) > BoxScoreThreahold)
					.Select(x => Cv2.MinAreaRect(x))
					.Where(x => x.Size.Width > MinSize && x.Size.Height > MinSize)
					.Select(rect =>
					{
						float minEdge = Math.Min(rect.Size.Width, rect.Size.Height);
						Size2f newSize = new Size2f(
							(rect.Size.Width + UnclipRatio * minEdge) * scaleRate,
							(rect.Size.Height + UnclipRatio * minEdge) * scaleRate);
						RotatedRect largerRect = new RotatedRect(rect.Center * scaleRate, newSize, rect.Angle);
						return largerRect;
					})
					.ToArray();
				//{
				//	using Mat demo = dilated.CvtColor(ColorConversionCodes.GRAY2RGB);
				//	demo.DrawContours(contours, -1, Scalar.Red);
				//	Image(demo).Dump();
				//}
				return rects;
			}
		}

		private float GetScore(Point[] contour, Mat pred)
		{
			int width = pred.Width;
			int height = pred.Height;
			int[] boxX = contour.Select(v => v.X).ToArray();
			int[] boxY = contour.Select(v => v.Y).ToArray();

			int xmin = MathUtil.Clamp(boxX.Min(), 0, width - 1);
			int xmax = MathUtil.Clamp(boxX.Max(), 0, width - 1);
			int ymin = MathUtil.Clamp(boxY.Min(), 0, height - 1);
			int ymax = MathUtil.Clamp(boxY.Max(), 0, height - 1);

			Point[] rootPoints = contour
				.Select(v => new Point(v.X - xmin, v.Y - ymin))
				.ToArray();
			using Mat mask = new Mat(ymax - ymin + 1, xmax - xmin + 1, MatType.CV_8UC1, Scalar.Black);
			mask.FillPoly(new[] { rootPoints }, new Scalar(1));

			using Mat croppedMat = pred[ymin, ymax + 1, xmin, xmax + 1];
			float score = (float)croppedMat.Mean(mask).Val0;

			// Debug
			//{
			//	using Mat cu = new Mat();
			//	croppedMat.ConvertTo(cu, MatType.CV_8UC1, 255);
			//	Util.HorizontalRun(true, Image(cu), Image(mask), score).Dump();
			//}

			return score;
		}

		private static Mat MatResize(Mat src, int? maxSize)
        {
			if (maxSize == null) return src.Clone();

			Size size = src.Size();
			int longEdge = Math.Max(size.Width, size.Height);
			double scaleRate = 1.0 * maxSize.Value / longEdge;

			return scaleRate < 1.0 ?
				src.Resize(Size.Zero, scaleRate, scaleRate) :
				src.Clone();
		}

        private unsafe static float[] ExtractMat(Mat src)
		{
			int rows = src.Rows;
			int cols = src.Cols;
			float[] result = new float[rows * cols * 3];
			fixed (float* data = result)
			{
				for (int i = 0; i < src.Channels(); ++i)
				{
					using Mat dest = new Mat(rows, cols, MatType.CV_32FC1, (IntPtr)(data + i * rows * cols));
					Cv2.ExtractChannel(src, dest, i);
				}
			}
			return result;
		}

		private static Mat MatPadding32(Mat src)
		{
			Size size = src.Size();
			Size newSize = new(
				32 * Math.Ceiling(1.0 * size.Width / 32),
				32 * Math.Ceiling(1.0 * size.Height / 32));
			return src.CopyMakeBorder(0, newSize.Height - size.Height, 0, newSize.Width - size.Width, BorderTypes.Constant, Scalar.Black);
		}

		private static Mat Normalize(Mat src)
		{
			using Mat normalized = new();
			src.ConvertTo(normalized, MatType.CV_32FC3, 1.0 / 255);
			Mat[] bgr = normalized.Split();
			float[] scales = new[] { 1 / 0.229f, 1 / 0.224f, 1 / 0.225f };
			float[] means = new[] { 0.485f, 0.456f, 0.406f };
			for (int i = 0; i < bgr.Length; ++i)
			{
				bgr[i].ConvertTo(bgr[i], MatType.CV_32FC1, 1.0 * scales[i], (0.0 - means[i]) * scales[i]);
			}

			Mat dest = new();
			Cv2.Merge(bgr, dest);

			foreach (Mat channel in bgr)
			{
				channel.Dispose();
			}

			return dest;
		}
	}
}

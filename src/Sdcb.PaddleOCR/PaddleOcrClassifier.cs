using OpenCvSharp;
using Sdcb.PaddleInference;
using System;

namespace Sdcb.PaddleOCR
{

    public class PaddleOcrClassifier : IDisposable
	{
		private readonly PaddlePredictor _p;

		public double RotateThreshold { get; set; } = 0.75;

		public PaddleOcrClassifier(PaddleConfig config) : this(config.CreatePredictor())
		{
		}

		public PaddleOcrClassifier(PaddlePredictor predictor)
		{
			_p = predictor;
		}

		public PaddleOcrClassifier(string modelDir) : this(PaddleConfig.FromModelDir(modelDir))
		{
		}

		public PaddleOcrClassifier Clone()
		{
			return new PaddleOcrClassifier(_p.Clone());
		}

		public void Dispose()
		{
			_p.Dispose();
		}

		public Mat Run(Mat src)
		{
			if (src.Empty())
			{
				throw new ArgumentException("src size should not be 0, wrong input picture provided?");
			}

			if (src.Channels() != 3)
			{
				throw new NotSupportedException($"{nameof(src)} channel must be 3, provided {src.Channels()}.");
			}

			using Mat resized = ResizePadding(src);
			using Mat normalized = Normalize(resized);

			using (PaddleTensor input = _p.GetInputTensor(_p.InputNames[0]))
			{
				input.Shape = new[] { 1, 3, normalized.Rows, normalized.Cols };
				float[] data = PaddleOcrDetector.ExtractMat(normalized);
				input.SetData(data);
			}
			if (!_p.Run())
			{
				throw new Exception("PaddlePredictor(Classifier) run failed.");
			}

			using (PaddleTensor output = _p.GetOutputTensor(_p.OutputNames[0]))
			{
				float[] softmax = output.GetData<float>();
				float score = 0;
				int label = 0;
				for (int i = 0; i < softmax.Length; ++i)
				{
					if (softmax[i] > score)
					{
						score = softmax[i];
						label = i;
					}
				}

				if (label % 2 == 1 && score > RotateThreshold)
				{
					Mat dest = new();
					Cv2.Rotate(src, dest, RotateFlags.Rotate180);
					return dest;
				}

				return src;
			}
		}

		private static (int channel, int height, int width) shape = (3, 48, 192);

		private static Mat ResizePadding(Mat src)
		{
			Size srcSize = src.Size();
			using Mat roi = srcSize.Width / srcSize.Height > shape.width / shape.height ?
				src[0, srcSize.Height, 0, (int)Math.Floor(1.0 * srcSize.Height * shape.width / shape.height)] :
				src.Clone();
			double scaleRate = 1.0 * shape.height / srcSize.Height;
			Mat resized = roi.Resize(new Size(Math.Floor(roi.Width * scaleRate), shape.height));
			if (resized.Width < shape.width)
			{
				Cv2.CopyMakeBorder(resized, resized, 0, 0, 0, shape.width - resized.Width, BorderTypes.Constant, Scalar.Black);
			}
			return resized;
		}

		private static Mat Normalize(Mat src)
		{
			using Mat normalized = new();
			src.ConvertTo(normalized, MatType.CV_32FC3, 1.0 / 255);
			Mat[] bgr = normalized.Split();
			float[] scales = new[] { 2.0f, 2.0f, 2.0f };
			float[] means = new[] { 0.5f, 0.5f, 0.5f };
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

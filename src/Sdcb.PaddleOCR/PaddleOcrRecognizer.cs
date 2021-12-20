using OpenCvSharp;
using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Sdcb.PaddleOCR
{

    public class PaddleOcrRecognizer : IDisposable
	{
		private readonly PaddleConfig _c;
		private readonly PaddlePredictor _p;
		private readonly IReadOnlyList<string> _labels;

		public PaddleOcrRecognizer(string modelDir, string labelFilePath)
		{
			_c = new PaddleConfig
			{
				CpuMathThreadCount = 0,
				MkldnnEnabled = true,
				MkldnnCacheCapacity = 10,
			};
			_c.SetModel(
				Path.Combine(modelDir, "inference.pdmodel"),
				Path.Combine(modelDir, "inference.pdiparams"));
			_p = _c.CreatePredictor();

			List<string> labels = File.ReadAllLines(labelFilePath).ToList();
			labels.Insert(0, "!!");
			labels.Add(" ");
			_labels = labels;
		}

		public PaddleOcrRecognizer(PaddleConfig config, string labelFilePath)
		{
			_c = config;
			_p = _c.CreatePredictor();

			List<string> labels = File.ReadAllLines(labelFilePath).ToList();
			labels.Insert(0, "!!");
			labels.Add(" ");
			_labels = labels;
		}

		public void Dispose()
		{
			_p.Dispose();
			_c.Dispose();
		}

		public static PaddleOcrRecognizerResult StaticRun(PaddlePredictor predictor, Mat src, IReadOnlyList<string> labels)
		{
			using Mat resized = ResizePadding(src);
			using Mat normalized = Normalize(resized);

			using (PaddleTensor input = predictor.GetInputTensor(predictor.InputNames[0]))
			{
				input.Shape = new[] { 1, 3, normalized.Rows, normalized.Cols };
				float[] data = ExtractMat(normalized);
				input.SetData(data);
			}
			if (!predictor.Run())
            {
				throw new Exception($"PaddlePredictor(Recognizer) run failed.");
            }

			using (PaddleTensor output = predictor.GetOutputTensor(predictor.OutputNames[0]))
			{
				float[] data = output.GetData<float>();
				int[] shape = output.Shape;

				var sb = new StringBuilder();
				int lastIndex = 0;
				float score = 0;

				for (int n = 0; n < shape[1]; ++n)
				{
					float[] matArray = new float[shape[2]];
					Array.Copy(data, n * shape[2], matArray, 0, matArray.Length);
					using Mat mat = new Mat(1, shape[2], MatType.CV_32FC1, matArray);
					int[] maxIdx = new int[2];
					mat.MinMaxIdx(out double _, out double maxVal, new int[0], maxIdx);

					if (maxIdx[1] > 0 && (!(n > 0 && maxIdx[1] == lastIndex)))
					{
						score += (float)maxVal;
						sb.Append(labels[maxIdx[1]]);
					}
					lastIndex = maxIdx[1];
				}
				return new(sb.ToString(), score / sb.Length);
			}
		}

		public PaddleOcrRecognizerResult Run(Mat src)
		{
			return StaticRun(_p, src, _labels);
		}

		public PaddleOcrRecognizerResult ConcurrentRun(Mat src)
		{
			PaddlePredictor p;
			lock (_p)
			{
				p = _p.Clone();
			}

			using (p)
			{
				return StaticRun(p, src, _labels);
			}
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

		private static Mat ResizePadding(Mat src)
		{
			Size size = src.Size();
			float whRatio = 1.0f * size.Width / size.Height;
			int height = 32;
			int width = (int)Math.Ceiling(height * whRatio);

			return src.Resize(new Size(width, height));
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

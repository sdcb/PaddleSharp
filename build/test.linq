<Query Kind="Program">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleInference</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleInference.runtime.win64.mkl</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR</NuGetReference>
  <Namespace>OpenCvSharp</Namespace>
  <Namespace>Sdcb.PaddleInference</Namespace>
  <Namespace>Sdcb.PaddleOCR</Namespace>
</Query>

void Main()
{
	using PaddleOcrAll all = new (@"C:\_\3rd\paddle\models\ppocr-v2", @"C:\_\3rd\paddle\models\keys-cn.txt");
	using Mat src = Cv2.ImRead(@"C:\Users\ZhouJie\Pictures\xdr5480.jpg");
	all.Run(src).Dump();
	
	//PaddleConfig.Version.Dump();
	//
	//using PaddleOcrDetector detector = new(@"C:\_\3rd\paddle\models\ppocr-v2\det");
	//using Mat src = Cv2.ImRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "xdr5480.jpg"));
	//Rect[] rects = detector.Run(src);
	//rects.Dump();
}


public class PaddleOcrDetector2 : IDisposable
{
	PaddleConfig _c;
	PaddlePredictor _p;

	public PaddleOcrDetector2(string modelDir)
	{
		_c = new PaddleConfig
		{
			GLogEnabled = false,
			CpuMathThreadCount = 0,
			MkldnnEnabled = true,
			MkldnnCacheCapacity = 10,
		};
		_c.SetModel(
			Path.Combine(modelDir, "inference.pdmodel"),
			Path.Combine(modelDir, "inference.pdiparams"));
		_p = _c.CreatePredictor();
	}

	public void Dispose()
	{
		_p.Dispose();
		_c.Dispose();
	}

	public static Mat Visualize(Mat src, Rect[] rects, Scalar color, int thickness)
	{
		Mat clone = src.Clone();
		foreach (Rect rect in rects)
		{
			clone.Rectangle(rect, color, thickness);
		}
		return clone;
	}

	public Rect[] Run(Mat src)
	{
		return StaticRun(_p, src);
	}

	public Rect[] ConcurrentRun(Mat src)
	{
		PaddlePredictor predictor;
		lock (_p)
		{
			predictor = _p.Clone();
		}
		using (predictor)
		{
			return StaticRun(predictor, src);
		}
	}

	public static Rect[] StaticRun(PaddlePredictor predictor, Mat src)
	{
		using Mat resized = MatPadding32(src);
		using Mat normalized = Normalize(resized);

		using (PaddleTensor input = predictor.GetInputTensor(predictor.InputNames[0].Dump()))
		{
			input.Shape = new[] { 1, 3, normalized.Rows, normalized.Cols };
			float[] data = ExtractMat(normalized);
			input.SetData(data);
		}
		Debug.Assert(predictor.Run());

		using (PaddleTensor output = predictor.GetOutputTensor(predictor.OutputNames[0].Dump()))
		{
			float[] data = output.GetData<float>();
			int[] shape = output.Shape;

			using Mat dilated = new();
			{
				Mat pred = new Mat(shape[2], shape[3], MatType.CV_32FC1, data);

				using Mat cbuf = new();
				using (pred)
				{
					using Mat roi = pred[0, src.Rows, 0, src.Cols];
					roi.ConvertTo(cbuf, MatType.CV_8UC1, 255);
				}

				using Mat ones = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2, 2));
				Cv2.Dilate(cbuf, dilated, ones);
			}

			Point[][] contours = dilated.FindContoursAsArray(RetrievalModes.List, ContourApproximationModes.ApproxSimple);
			Size size = src.Size();
			Rect[] rects = contours
				.Select(x => Cv2.BoundingRect(x))
				.Where(x => x.Width > 5 && x.Height > 5)
				.Select(rect =>
				{
					int x = MathUtil.Clamp(rect.Left - rect.Height, 0, size.Width);
					int y = MathUtil.Clamp(rect.Top - rect.Height, 0, size.Height);
					int width = rect.Width + 2 * rect.Height;
					int height = rect.Height * 3;
					return new Rect(x, y,
						MathUtil.Clamp(width, 0, size.Width - x),
						MathUtil.Clamp(height, 0, size.Height - y));
				})
				.ToArray();
			return rects;
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

internal static class MathUtil
{
	public static int Clamp(int val, int min, int max)
	{
#if NET6_0_OR_GREATER
            return Math.Clamp(val, min, max);
#else
		if (val < min)
		{
			return min;
		}
		else if (val > max)
		{
			return max;
		}
		return val;
#endif
	}
}
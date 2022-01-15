using OpenCvSharp;
using Sdcb.PaddleDetection.Preprocesses;
using Sdcb.PaddleInference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection
{
    public class PaddleDetector : IDisposable
	{
		private readonly PaddlePredictor _p;
		private readonly Preprocessor _preprocessor;
		public DetectionModelConfig Config { get; }

		public PaddleDetector(PaddleConfig config, string configYmlPath) : this(config.CreatePredictor(), configYmlPath)
		{
		}

		public PaddleDetector(PaddlePredictor predictor, string configYmlPath)
		{
			_p = predictor;
			var yaml = new YamlStream();
			using FileStream ymlFile = File.OpenRead(configYmlPath);
			yaml.Load(new StreamReader(ymlFile));
			YamlDocument doc = yaml.Documents[0];
			Config = DetectionModelConfig.Parse((YamlMappingNode)doc.RootNode);
			_preprocessor = new Preprocessor((YamlSequenceNode)doc.RootNode["Preprocess"]);
		}

		public PaddleDetector(string modelDir, string configYmlPath) : this(PaddleConfig.FromModelFiles(
			Path.Combine(modelDir, "model.pdmodel"),
			Path.Combine(modelDir, "model.pdiparams")), configYmlPath)
		{
		}

		public void Dispose()
		{
			_p.Dispose();
		}

		public DetectionResult[] Run(Mat src)
		{
			if (src.Empty())
			{
				throw new ArgumentException("src size should not be 0, wrong input picture provided?");
			}

			ImageProcessContext ctx = new();
			_preprocessor.Run(src, ctx);
			foreach (string inputName in _p.InputNames)
			{
				using (PaddleTensor input = _p.GetInputTensor(inputName))
				{
					if (inputName == "image")
					{
						input.Shape = new int[] { 1, 3, (int)ctx.NetShape.Height, (int)ctx.NetShape.Width };
						input.SetData(ctx.Data);
					}
					else if (inputName == "im_shape")
					{
						input.Shape = new int[] { 1, 2 };
						input.SetData(new float[] { ctx.Shape.Height, ctx.Shape.Width });
					}
					else if (inputName == "scale_factor")
					{
						input.Shape = new int[] { 1, 2 };
						input.SetData(new[] { ctx.ScaleFactor.Y, ctx.ScaleFactor.X });
					}
				}
			}

			if (!_p.Run())
			{
				throw new Exception("PaddlePredictor(Detector) run failed.");
			}

			TensorOutput[] outputs = _p.OutputNames
				.Select(x =>
				{
					using PaddleTensor tensor = _p.GetOutputTensor(x);
					int[] shape = tensor.Shape;
					return new TensorOutput
					{
						Shape = shape,
						DataType = tensor.DataType,
						Name = x,
						Data = tensor.DataType switch
						{
							DataTypes.Int32 => tensor.GetData<int>(),
							DataTypes.Float32 => tensor.GetData<float>(),
							_ => throw new NotSupportedException($"Unexpected datatype: {tensor.DataType}.")
						},
					};
				})
				.ToArray();

			if (Config.Arch == "PicoDet")
			{
				if (outputs.Length != Config.FpnStride.Length * 2)
				{
					Console.WriteLine(
						$"Warn: output_count({outputs.Length}) != fpn_stride_count({Config.FpnStride.Length}), model might be wrong.");
				}
				int regCount = outputs[Config.FpnStride.Length].Shape[2] / 4;

				int labelCount = outputs[0].Shape[2];
				if (labelCount != Config.LabelList.Length)
				{
					Console.WriteLine(
						$"Warn: output_label_count({labelCount}) != model_label_count({Config.LabelList.Length}), model might be wrong.");
				}

				return PicoDetectionPostProcess(
					outputs.Select(x => (float[])x.Data).ToArray(),
					Config.FpnStride,
					ctx.Shape,
					ctx.ScaleFactor,
					Config.NmsInfo.ScoreThreshold,
					Config.NmsInfo.NmsThreshold,
					Config.LabelList,
					regCount);
			}
			else
			{
				bool isRbox = outputs[0].Shape[outputs.Length - 1] % 10 == 0;
				int count = ((int[])outputs[1].Data)[0];
				Size r = Config.Arch == "Face" ? src.Size() : new Size(1, 1);

				var result = new DetectionResult[count];
				for (int j = 0; j < count; ++j)
				{
					float[] outputData = (float[])outputs[0].Data;
					if (isRbox)
					{
						int classId = (int)MathUtil.Round(outputData[0 + j * 10]);
						float score = outputData[1 + j * 10];
						int x1 = (int)(outputData[2 + j * 10] * r.Width);
						int y1 = (int)(outputData[3 + j * 10] * r.Height);
						int x2 = (int)(outputData[4 + j * 10] * r.Width);
						int y2 = (int)(outputData[5 + j * 10] * r.Height);
						int x3 = (int)(outputData[6 + j * 10] * r.Width);
						int y3 = (int)(outputData[7 + j * 10] * r.Height);
						int x4 = (int)(outputData[8 + j * 10] * r.Width);
						int y4 = (int)(outputData[9 + j * 10] * r.Height);
						result[j] = new DetectionResult(new[] { x1, y1, x2, y2, x3, y3, x4, y4 }, classId, Config.LabelList[classId], score);
					}
					else
					{
						int classId = (int)MathUtil.Round(outputData[0 + j * 6]);
						float score = outputData[1 + j * 6];
						int xmin = (int)(outputData[2 + j * 6] * r.Width);
						int ymin = (int)(outputData[3 + j * 6] * r.Height);
						int xmax = (int)(outputData[4 + j * 6] * r.Width);
						int ymax = (int)(outputData[5 + j * 6] * r.Height);
						result[j] = new DetectionResult(new[] { xmin, ymin, xmax, ymax }, classId, Config.LabelList[classId], score);
					}
				}
				return result;
			}
		}

		private static List<DetectionResult> Nms(List<DetectionResult> srcBoxs, float nmsThreshold)
		{
			List<DetectionResult> boxs = srcBoxs
				.OrderByDescending(x => x.Confidence)
				.ToList();
			List<int> areas = boxs.Select(x => (int)((x.Rect.Width + 1) * (x.Rect.Height + 1))).ToList();

			for (int i = 0; i < boxs.Count; ++i)
			{
				for (int j = i + 1; j < boxs.Count;)
				{
					float xx1 = Math.Max(boxs[i].Rect.Left, boxs[j].Rect.Left);
					float yy1 = Math.Max(boxs[i].Rect.Top, boxs[j].Rect.Top);
					float xx2 = Math.Min(boxs[i].Rect.Right, boxs[j].Rect.Right);
					float yy2 = Math.Min(boxs[i].Rect.Bottom, boxs[j].Rect.Bottom);
					float w = Math.Max(0, xx2 - xx1 + 1);
					float h = Math.Max(0, yy2 - yy1 + 1);
					float area = w * h;
					float overlapRatio = area / (areas[i] + areas[j] - area);
					if (overlapRatio >= nmsThreshold)
					{
						boxs.RemoveAt(j);
						areas.RemoveAt(j);
					}
					else
					{
						++j;
					}
				}
			}

			return boxs;
		}

		private static DetectionResult[] PicoDetectionPostProcess(
			float[][] outputs,
			int[] fpnStride,
			Size inputSize,
			Point2f scaleFactor,
			float scoreThreshold,
			float nmsThreshold,
			IReadOnlyList<string> labels,
			int regCount)
		{
			List<DetectionResult>[] bboxResults = Enumerable
				.Range(0, labels.Count)
				.Select(x => new List<DetectionResult>())
				.ToArray();

			for (int i = 0; i < fpnStride.Length; ++i)
			{
				Size featureSize = new(
					(int)MathUtil.Ceiling(1.0f * inputSize.Width / fpnStride[i]),
					(int)MathUtil.Ceiling(1.0f * inputSize.Height / fpnStride[i]));

				for (int f = 0; f < featureSize.Width * featureSize.Height; ++f)
				{
					int row = f / featureSize.Width;
					int col = f % featureSize.Width;
					Span<float> scores = outputs[i].AsSpan(f * labels.Count, labels.Count);

					float score = 0;
					int currentLabel = 0;
					string currentLabelText = null;
					for (int label = 0; label < labels.Count; ++label)
					{
						if (scores[label] > score)
						{
							score = scores[label];
							currentLabel = label;
							currentLabelText = labels[currentLabel];
						}
					}

					if (score > scoreThreshold)
					{
						Span<float> bboxPred = outputs[i + fpnStride.Length].AsSpan(f * 4 * regCount, regCount * 4);
						Rect rect = Prediction2Box(bboxPred, col, row, fpnStride[i], inputSize, regCount);
						bboxResults[currentLabel].Add(new DetectionResult(rect, currentLabel, currentLabelText, score));
					}
				}
			}

			return bboxResults
				.Select(x => Nms(x, nmsThreshold))
				.SelectMany(x => x)
				.Select(x => x.WithRect(new Rect(
					(int)(x.Rect.X / scaleFactor.X),
					(int)(x.Rect.Y / scaleFactor.Y),
					(int)(x.Rect.Width / scaleFactor.X),
					(int)(x.Rect.Height / scaleFactor.Y))
				))
				.ToArray();
		}

		private static Rect Prediction2Box(
			Span<float> bboxPred,
			int x, int y, int stride,
			Size inputSize,
			int regCount)
		{
			Size2f ct = new((x + 0.5) * stride, (y + 0.5) * stride);
			float[] disPred = new float[4];

			for (int i = 0; i < 4; ++i)
			{
				float dis = 0;
				float[] afterSoftmax = Softmax(bboxPred.Slice(i * regCount, regCount));
				for (int j = 0; j < regCount; ++j)
				{
					dis += j * afterSoftmax[j];
				}
				dis *= stride;
				disPred[i] = dis;
			}

			return Rect.FromLTRB(
				(int)Math.Max(ct.Width - disPred[0], 0),
				(int)Math.Max(ct.Height - disPred[1], 0),
				(int)Math.Min(ct.Width + disPred[2], inputSize.Width),
				(int)Math.Min(ct.Height + disPred[3], inputSize.Height));
		}

		private static float[] Softmax(Span<float> src)
		{
			float alpha = float.MinValue;
			for (int i = 0; i < src.Length; ++i)
			{
				if (src[i] > alpha)
				{
					alpha = src[i];
				}
			}

			float[] dest = new float[src.Length];
			float denominator = 0;
			for (int i = 0; i < src.Length; ++i)
			{
				dest[i] = MathUtil.Exp(src[i] - alpha);
				denominator += dest[i];
			}

			return dest.Select(x => x / denominator).ToArray();
		}

		public static Mat Visualize(Mat src, IEnumerable<DetectionResult> results, int labelCount)
		{
			Mat dest = src.Clone();
			Scalar[] colors = GenerateColorMapToScalar().Take(labelCount).ToArray();
			foreach (DetectionResult r in results)
			{
				if (r.Confidence < 0.5f) continue;
				Scalar roiColor = colors[r.LabelId];

				if (r.IsRBox)
				{
					for (int k = 0; k < 4; ++k)
					{
						Point pt1 = new(r.RectArray[(k * 2) % 8], r.RectArray[(k * 2 + 1) % 8]);
						Point pt2 = new(r.RectArray[(k * 2 + 2) % 8], r.RectArray[(k * 2 + 3) % 8]);
						dest.Line(pt1, pt2, roiColor, thickness: 2);
					}
				}
				else
				{
					dest.Rectangle(r.Rect, roiColor, thickness: 2);
				}

				string text = $"{r.LabelName}:{r.Confidence:F2}";
				HersheyFonts fontFace = HersheyFonts.HersheyComplexSmall;
				double fontScale = 0.8;
				int thinkness = 1;
				Size textSize = Cv2.GetTextSize(text, fontFace, fontScale, thinkness, out int _);

				Point topLeft = new(r.RectArray[0], r.RectArray[1]);
				Rect textBack = new(topLeft.X, topLeft.Y - textSize.Height, textSize.Width, textSize.Height);
				dest.Rectangle(textBack, roiColor, thickness: -1);
				dest.PutText(text, topLeft, fontFace, fontScale, Scalar.White, thinkness);
			}
			return dest;

			static IEnumerable<Scalar> GenerateColorMapToScalar()
			{
				for (int i = 0; ; ++i)
				{
					int j = 0;
					int lab = i;
					Vec3i r = new();
					while (lab != 0)
					{
						r.Item0 |= (((lab >> 0) & 1) << (7 - j));
						r.Item1 |= (((lab >> 1) & 1) << (7 - j));
						r.Item2 |= (((lab >> 2) & 1) << (7 - j));
						++j;
						lab >>= 3;
					}

					yield return new Scalar(r.Item0, r.Item1, r.Item2);
				}
			}
		}
	}
}
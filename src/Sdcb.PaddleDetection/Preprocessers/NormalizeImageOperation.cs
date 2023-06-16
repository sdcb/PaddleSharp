using OpenCvSharp;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection.Preprocesses;

internal class NormalizeImageOperation : PreprocessOperation
	{
		public override string Name => PreprocessOperation.NormalizeImage;
		public float[] Means { get; set; }
		public float[] Scales { get; set; }
		public bool IsScale { get; set; }

		public NormalizeImageOperation(YamlMappingNode operationNode)
		{
			Means = ReadFloat3(operationNode, "mean");
			Scales = ReadFloat3(operationNode, "std");
			IsScale = bool.Parse(operationNode["is_scale"].ToString());
		}

		public override void Run(Mat src, ImageProcessContext data)
		{
			src.ConvertTo(src, MatType.CV_32FC3, IsScale ? 1.0 / 255 : 1);

			Mat[] bgr = src.Split();
			for (int i = 0; i < bgr.Length; ++i)
			{
				bgr[i].ConvertTo(bgr[i], MatType.CV_32FC1, 1 / Scales[i], (0.0 - Means[i]) / Scales[i]);
			}

			Cv2.Merge(bgr, src);

			foreach (Mat channel in bgr)
			{
				channel.Dispose();
			}
		}
	}
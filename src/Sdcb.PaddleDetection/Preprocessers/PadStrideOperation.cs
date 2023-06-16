using OpenCvSharp;
using System;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection.Preprocesses;

internal class PadStrideOperation : PreprocessOperation
	{
		public int Stride { get; set; }
		public override string Name => PreprocessOperation.PadStride;

		public PadStrideOperation(YamlMappingNode operationNode)
		{
			Stride = int.Parse(operationNode["stride"].ToString());
		}

		public override void Run(Mat src, ImageProcessContext data)
		{
			if (Stride <= 0) return;

			Size srcSize = src.Size();
			Size newSize = new(
				Stride * Math.Ceiling(1.0 * srcSize.Width / Stride),
				Stride * Math.Ceiling(1.0 * srcSize.Height / Stride));
			Cv2.CopyMakeBorder(src, src,
				0, newSize.Height - srcSize.Height,
				0, newSize.Width - srcSize.Width,
				BorderTypes.Constant, Scalar.Black);
			data.NetMat = src.Clone();
			data.NetShape = new Size2f(src.Width, src.Height);
		}
	}
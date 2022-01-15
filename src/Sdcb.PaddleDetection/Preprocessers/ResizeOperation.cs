using OpenCvSharp;
using System;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection.Preprocesses
{
    internal class ResizeOperation : PreprocessOperation
	{
		public override string Name => PreprocessOperation.Resize;
		public InterpolationFlags Interpolation { get; }
		public bool KeepRatio { get; }
		public Size TargetSize { get; }
		public Size NetShape { get; }

		public ResizeOperation(YamlMappingNode operationNode)
		{
			Interpolation = (InterpolationFlags)int.Parse(operationNode["interp"].ToString());
			KeepRatio = bool.Parse(operationNode["keep_ratio"].ToString());
			TargetSize = ReadSize(operationNode, "target_size");
		}

		private Point2f GenerateScale(Mat src)
		{
			Size size = src.Size();
			Point2f scaleRate = new(
				(float)TargetSize.Width / size.Width,
				(float)TargetSize.Height / size.Height);

			if (KeepRatio)
			{
				float min = Math.Min(scaleRate.X, scaleRate.Y);
				return new Point2f(min, min);
			}
			else
			{
				return scaleRate;
			}
		}

		public override void Run(Mat src, ImageProcessContext data)
		{
			Point2f scale = GenerateScale(src);
			Size size = src.Size();
			data.NetShape = new Size2f(size.Width * scale.X, size.Height * scale.Y);

			Cv2.Resize(src, src, Size.Zero, scale.X, scale.Y, Interpolation);
			data.Shape = src.Size();
			data.ScaleFactor = scale;
		}
	}

}
using OpenCvSharp;
using System;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection.Preprocesses
{
    internal class LetterBoxResizeOperation : PreprocessOperation
	{
		public Size TargetSize { get; set; }
		public Size NetShape { get; set; }
		public override string Name => PreprocessOperation.LetterBoxResize;

		public LetterBoxResizeOperation(YamlMappingNode operationNode)
		{
			TargetSize = ReadSize(operationNode, "target_size");
		}

		private float GenerateScale(Size srcSize)
		{
			float scaleX = (float)TargetSize.Width / srcSize.Width;
			float scaleY = (float)TargetSize.Height / srcSize.Height;
			return Math.Min(scaleX, scaleY);
		}

		public override void Run(Mat src, ImageProcessContext data)
		{
			Size srcSize = src.Size();
			float scale = GenerateScale(srcSize);
			Size newSize = new(
				Math.Round(srcSize.Width * scale),
				Math.Round(srcSize.Height * scale));
			data.Shape = newSize;
			Cv2.Resize(src, src, newSize, 0, 0, InterpolationFlags.Area);

			Size2f pad = new(
				(TargetSize.Width - newSize.Width) / 2.0f,
				(TargetSize.Height - newSize.Height) / 2.0f);
			Rect padRect = Rect.FromLTRB(
				(int)Math.Round(pad.Width - 0.1f),
				(int)Math.Round(pad.Height - 0.1f),
				(int)Math.Round(pad.Width + 0.1f),
				(int)Math.Round(pad.Height + 0.1f));
			Cv2.CopyMakeBorder(
				src, src,
				padRect.Top, padRect.Bottom, padRect.Left, padRect.Right,
				BorderTypes.Constant,
				new Scalar(127.5));
			Size finalSize = src.Size();
			data.NetShape = new Size2f(finalSize.Width, finalSize.Height);
			data.ScaleFactor = new Point2f(scale, scale);
		}
	}

}
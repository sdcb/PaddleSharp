using OpenCvSharp;

namespace Sdcb.PaddleDetection.Preprocesses
{
    internal class InitializeOperation : PreprocessOperation
	{
		public override string Name => PreprocessOperation.InitInfo;

		public override void Run(Mat src, ImageProcessContext desc)
		{
			Size size = src.Size();
			desc.Shape = size;
			desc.ScaleFactor = new Point2f(1.0f, 1.0f);
			desc.NetShape = new Size2f(size.Width, size.Height);
		}
	}

}
using OpenCvSharp;

namespace Sdcb.PaddleDetection;

internal class ImageProcessContext
	{
		public Size Shape;
		public float[] Data;
		public Size2f NetShape;
		public Point2f ScaleFactor;
		public Mat NetMat;
	}
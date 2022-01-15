using OpenCvSharp;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection.Preprocesses
{
    internal class TopDownEvalAffineOperation : PreprocessOperation
	{
		public override string Name => PreprocessOperation.TopDownEvalAffine;
		public InterpolationFlags Interpolation { get; set; } = InterpolationFlags.Linear;
		public Size TrainSize { get; }

		public TopDownEvalAffineOperation(YamlMappingNode operationNode)
		{
			TrainSize = ReadSize(operationNode, "trainsize");
		}

		public override void Run(Mat src, ImageProcessContext data)
		{
			Cv2.Resize(src, src, TrainSize, 0, 0, Interpolation);
			data.NetShape = new Size2f(TrainSize.Width, TrainSize.Height);
		}
	}

}
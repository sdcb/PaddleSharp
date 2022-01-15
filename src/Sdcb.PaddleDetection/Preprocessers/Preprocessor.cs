using OpenCvSharp;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection.Preprocesses
{
    internal class Preprocessor
	{
		private readonly Dictionary<string, PreprocessOperation> Operations = new();
		private readonly InitializeOperation InitializeOperation;

		public IEnumerable<PreprocessOperation> GetOperations()
		{
			yield return InitializeOperation;
			foreach (string runOrder in RunOrder)
			{
				if (Operations.TryGetValue(runOrder, out PreprocessOperation value))
				{
					yield return value;
				}
			}
		}

		public void Run(Mat src, ImageProcessContext ctx)
		{
			using Mat bgr = src.CvtColor(ColorConversionCodes.BGR2RGB);
			foreach (PreprocessOperation op in GetOperations())
			{
				op.Run(bgr, ctx);
			}
		}

		public Preprocessor(YamlSequenceNode preprocessNode)
		{
			InitializeOperation = new InitializeOperation();
			Operations = preprocessNode.AllNodes
				.OfType<YamlMappingNode>()
				.ToDictionary(
					x => x["type"].ToString(),
					v => PreprocessOperation.CreateOperation((string)v["type"], v));
		}

		public static string[] RunOrder = new string[]
		{
		PreprocessOperation.InitInfo,
		PreprocessOperation.TopDownEvalAffine,
		PreprocessOperation.Resize,
		PreprocessOperation.LetterBoxResize,
		PreprocessOperation.NormalizeImage,
		PreprocessOperation.PadStride,
		PreprocessOperation.Permute
		};
	}

}
using OpenCvSharp;
using System;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Sdcb.PaddleDetection.Preprocesses
{
    internal abstract class PreprocessOperation
	{
		public abstract string Name { get; }

		public abstract void Run(Mat src, ImageProcessContext data);

		internal static Size ReadSize(YamlMappingNode node, string tagName)
		{
			int[] arr = ((YamlSequenceNode)node[tagName])
				.Select(x => int.Parse(x.ToString()))
				.ToArray();
			if (arr.Length != 2)
			{
				throw new Exception($"config yml {tagName}.length != 2");
			}

			return new Size(arr[1], arr[0]);
		}

		protected static float[] ReadFloat3(YamlMappingNode node, string tagName)
		{
			float[] arr = ((YamlSequenceNode)node[tagName])
				.Select(x => float.Parse(x.ToString()))
				.ToArray();
			if (arr.Length != 3)
			{
				throw new Exception($"config yml {tagName}.length != 3");
			}

			return arr;
		}

		public static PreprocessOperation CreateOperation(string operationName, YamlMappingNode operationNode)
		{
			return operationName switch
			{
				InitInfo => new InitializeOperation(),
				TopDownEvalAffine => new TopDownEvalAffineOperation(operationNode),
				Resize => new ResizeOperation(operationNode),
				LetterBoxResize => new LetterBoxResizeOperation(operationNode),
				NormalizeImage => new NormalizeImageOperation(operationNode),
				PadStride => new PadStrideOperation(operationNode),
				Permute => new PermuteOperation(),
				_ => throw new NotImplementedException()
			};
		}

		public const string InitInfo = nameof(InitInfo);
		public const string TopDownEvalAffine = nameof(TopDownEvalAffine);
		public const string Resize = nameof(Resize);
		public const string LetterBoxResize = nameof(LetterBoxResize);
		public const string NormalizeImage = nameof(NormalizeImage);
		public const string PadStride = nameof(PadStride);
		public const string Permute = nameof(Permute);
	}

}
using OpenCvSharp;
using System;

namespace Sdcb.PaddleDetection
{
    public record DetectionResult
	{
		public int[] RectArray { get; }
		public int LabelId { get; }
		public string LabelName { get; }
		public float Confidence { get; }

		public bool IsRBox => RectArray.Length == 8;
		public Rect Rect => !IsRBox ? Rect.FromLTRB(RectArray[0], RectArray[1], RectArray[2], RectArray[3]) : throw new NotSupportedException();

		public DetectionResult WithRect(Rect rect) => new DetectionResult(rect, LabelId, LabelName, Confidence);

		public DetectionResult(int[] rectArray, int labelId, string labelName, float confidence)
		{
			RectArray = rectArray;
			LabelId = labelId;
			LabelName = labelName;
			Confidence = confidence;
		}

		public DetectionResult(Rect rect, int labelId, string labelName, float confidence) :
			this(new int[] { rect.Left, rect.Top, rect.Right, rect.Bottom }, labelId, labelName, confidence)
		{
		}
	}

}
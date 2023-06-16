using OpenCvSharp;
using System;

namespace Sdcb.PaddleDetection;

/// <summary>
/// Represents the result of an detection operation.
/// </summary>
public record DetectionResult
{
    /// <summary>
    /// Gets the coordinates of the box around the detected object.
    /// </summary>
    public int[] RectArray { get; }

    /// <summary>
    /// Gets the ID of the label for the detected object.
    /// </summary>
    public int LabelId { get; }

    /// <summary>
    /// Gets the name of the label for the detected object.
    /// </summary>
    public string LabelName { get; }

    /// <summary>
    /// Gets the confidence score of the detection.
    /// </summary>
    public float Confidence { get; }

    /// <summary>
    /// Determines whether the detected object is in RBox format.
    /// </summary>
    public bool IsRBox => RectArray.Length == 8;

    /// <summary>
    /// Gets the rectangle of the detected object in normal format.
    /// </summary>
    public Rect Rect => !IsRBox ? Rect.FromLTRB(RectArray[0], RectArray[1], RectArray[2], RectArray[3]) : throw new NotSupportedException();

    /// <summary>
    /// Creates a new instance of the DetectionResult class with the specified rectangle.
    /// </summary>
    /// <param name="rect">The rectangle of the detected object.</param>
    /// <returns>A new instance of the DetectionResult class with the specified rectangle.</returns>
    public DetectionResult WithRect(Rect rect) => new(rect, LabelId, LabelName, Confidence);

    /// <summary>
    /// Creates a new instance of the DetectionResult class with the specified parameters.
    /// </summary>
    /// <param name="rectArray">The coordinates of the box around the detected object.</param>
    /// <param name="labelId">The ID of the label for the detected object.</param>
    /// <param name="labelName">The name of the label for the detected object.</param>
    /// <param name="confidence">The confidence score of the detection.</param>
    public DetectionResult(int[] rectArray, int labelId, string labelName, float confidence)
    {
        RectArray = rectArray;
        LabelId = labelId;
        LabelName = labelName;
        Confidence = confidence;
    }

    /// <summary>
    /// Creates a new instance of the DetectionResult class with the specified rectangle and parameters.
    /// </summary>
    /// <param name="rect">The rectangle of the detected object.</param>
    /// <param name="labelId">The ID of the label for the detected object.</param>
    /// <param name="labelName">The name of the label for the detected object.</param>
    /// <param name="confidence">The confidence score of the detection.</param>
    public DetectionResult(Rect rect, int labelId, string labelName, float confidence) :
        this(new int[] { rect.Left, rect.Top, rect.Right, rect.Bottom }, labelId, labelName, confidence)
    {
    }
}

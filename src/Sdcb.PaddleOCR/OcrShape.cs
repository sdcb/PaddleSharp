namespace Sdcb.PaddleOCR;

/// <summary>
/// Represents the shape of an OCR input.
/// </summary>
public readonly record struct OcrShape
{
    /// <summary>
    /// The number of channels in the shape.
    /// </summary>
    public int Channel { get; }

    /// <summary>
    /// The height of the shape.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The width of the shape.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OcrShape"/> struct with the given channel, height and width.
    /// </summary>
    /// <param name="channel">The number of channels in the shape.</param>
    /// <param name="width">The width of the shape.</param>
    /// <param name="height">The height of the shape.</param>
    public OcrShape(int channel, int width, int height)
    {
        Channel = channel;
        Height = height;
        Width = width;
    }
}

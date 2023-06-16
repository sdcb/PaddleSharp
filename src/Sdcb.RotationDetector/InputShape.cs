namespace Sdcb.RotationDetector;

/// <summary>
/// Represents the shape of input data for a rotation detection model.
/// </summary>
public readonly record struct InputShape
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputShape"/> struct.
    /// </summary>
    /// <param name="channel">The number of color channels in the input image.</param>
    /// <param name="width">The width of the input image in pixels.</param>
    /// <param name="height">The height of the input image in pixels.</param>
    public InputShape(int channel, int width, int height)
    {
        Channel = channel;
        Height = height;
        Width = width;
    }

    /// <summary>
    /// Gets the number of color channels in the input image.
    /// </summary>
    public int Channel { get; init; }

    /// <summary>
    /// Gets the height of the input image in pixels.
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    /// Gets the width of the input image in pixels.
    /// </summary>
    public int Width { get; init; }
}

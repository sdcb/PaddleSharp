using OpenCvSharp;
using System;

namespace Sdcb.RotationDetector;

/// <summary>
/// Represents a rotation result containing the <see cref="Rotation"/> degree and the confidence <see cref="Score"/> of the rotation prediction.
/// </summary>
public record RotationResult(RotationDegree Rotation, float Score)
{
    /// <summary>
    /// Implicitly converts the <see cref="RotationResult"/> to a <see cref="RotationDegree"/>.
    /// </summary>
    /// <param name="r">The <see cref="RotationResult"/> to convert.</param>
    /// <returns>The <see cref="RotationDegree"/> value of the <paramref name="r"/> parameter.</returns>
    public static implicit operator RotationDegree(RotationResult r) => r.Rotation;

    /// <summary>
    /// Restores the image the <paramref name="src"/> parameter is pointing to to its original non-rotated state based on the <see cref="Rotation"/> property of the <see cref="RotationResult"/>.
    /// </summary>
    /// <param name="src">The image to restore.</param>
    /// <returns>The original reference to the <paramref name="src"/> parameter.</returns>
    public Mat RestoreRotationInPlace(Mat src)
    {
        if (src.Empty())
        {
            throw new ArgumentException("src size should not be 0, wrong input picture provided?");
        }

        if (Rotation == RotationDegree._90)
        {
            Cv2.Rotate(src, src, RotateFlags.Rotate90Counterclockwise);
            return src;
        }
        else if (Rotation == RotationDegree._180)
        {
            Cv2.Rotate(src, src, RotateFlags.Rotate180);
        }
        else if (Rotation == RotationDegree._270)
        {
            Cv2.Rotate(src, src, RotateFlags.Rotate90Clockwise);
        }

        return src;
    }
}

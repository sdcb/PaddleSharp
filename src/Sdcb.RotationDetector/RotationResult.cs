using OpenCvSharp;
using System;

namespace Sdcb.RotationDetector
{
    public record RotationResult(RotationDegree Rotation, float Score)
    {
        public static implicit operator RotationDegree(RotationResult r) => r.Rotation;

        /// <summary>
        /// Restore src back to non-rotated based on <see cref="Rotation"/>
        /// </summary>
        /// <param name="src">Note: src will changed if it's rotated.</param>
        /// <returns>The original reference to src parameter.</returns>
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
}
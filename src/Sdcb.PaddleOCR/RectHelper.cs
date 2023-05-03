using OpenCvSharp;
using System;

namespace Sdcb.PaddleOCR;

internal static class RectHelper
{
    public static float Distance(in Rect box1, in Rect box2)
    {
        int x1_1 = box1.X;
        int y1_1 = box1.Y;
        int x2_1 = box1.Right;
        int y2_1 = box1.Bottom;

        int x1_2 = box2.X;
        int y1_2 = box2.Y;
        int x2_2 = box2.Right;
        int y2_2 = box2.Bottom;

        float dis = Math.Abs(x1_2 - x1_1) + Math.Abs(y1_2 - y1_1) + Math.Abs(x2_2 - x2_1) + Math.Abs(y2_2 - y2_1);
        float dis_2 = Math.Abs(x1_2 - x1_1) + Math.Abs(y1_2 - y1_1);
        float dis_3 = Math.Abs(x2_2 - x2_1) + Math.Abs(y2_2 - y2_1);
        return dis + Math.Min(dis_2, dis_3);
    }

    public static float IntersectionOverUnion(in Rect box1, in Rect box2)
    {
        int x1 = Math.Max(box1.X, box2.X);
        int y1 = Math.Max(box1.Y, box2.Y);
        int x2 = Math.Min(box1.Right, box2.Right);
        int y2 = Math.Min(box1.Bottom, box2.Bottom);

        if (y1 >= y2 || x1 >= x2)
        {
            return 0.0f;
        }

        int intersectArea = (x2 - x1) * (y2 - y1);
        int box1Area = box1.Width * box1.Height;
        int box2Area = box2.Width * box2.Height;
        int unionArea = box1Area + box2Area - intersectArea;

        return (float)intersectArea / unionArea;
    }

    public static Rect Extend(in Rect rect, int extendLength)
    {
        return Rect.FromLTRB(rect.Left - extendLength, rect.Top - extendLength, rect.Right + extendLength, rect.Bottom + extendLength);
    }
}

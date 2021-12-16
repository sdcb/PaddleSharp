using OpenCvSharp;

namespace Sdcb.PaddleOCR
{
    public record struct PaddleOcrResultRegion(Rect Rect, string Text, float Score);
}

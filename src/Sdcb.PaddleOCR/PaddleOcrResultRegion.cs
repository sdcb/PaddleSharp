using OpenCvSharp;

namespace Sdcb.PaddleOCR;

public record struct PaddleOcrResultRegion(RotatedRect Rect, string Text, float Score);

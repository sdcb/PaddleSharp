using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.PaddleOCR;

public record TableCellBox(IReadOnlyList<int> Data)
{
    internal IEnumerable<Point> Contours => Data.Count switch
    {
        8 => Data.Chunk(2).Select(x => new Point(x[0], x[1])),
        4 => new Point[] { new Point(Data[0], Data[1]), new Point(Data[2], Data[1]), new Point(Data[2], Data[3]), new Point(Data[0], Data[3]) },
        _ => throw new NotSupportedException(OutOfRangeMessage),
    };

    public Rect Rect => Data.Count switch
    {
        8 => Xyxyxyxy2xyxy(Data),
        4 => Rect.FromLTRB(Data[0], Data[1], Data[2], Data[3]),
        _ => throw new NotSupportedException(OutOfRangeMessage),
    };

    private string OutOfRangeMessage => $"{nameof(TableCellBox)} length = {Data.Count} is not supported, supported length: 4 or 8.";

    private static Rect Xyxyxyxy2xyxy(IReadOnlyList<int> data)
    {
        if (data == null || data.Count != 8) throw new ArgumentException($"data.length must be 8");
        int[] xs = { data[0], data[2], data[4], data[6] };
        int[] ys = { data[1], data[3], data[5], data[7] };
        return Rect.FromLTRB(xs.Min(), ys.Min(), xs.Max(), ys.Max());
    }
}

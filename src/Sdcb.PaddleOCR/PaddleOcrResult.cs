using System.Linq;

namespace Sdcb.PaddleOCR
{
    public record PaddleOcrResult
    {
        public PaddleOcrResult(PaddleOcrResultRegion[] Regions)
        {
            this.Regions = Regions;
        }

        public PaddleOcrResultRegion[] Regions { get; }

        public string Text => string.Join("\n", Regions
            .OrderBy(x => x.Rect.Center.Y)
            .ThenBy(x => x.Rect.Center.X)
            .Select(x => x.Text));
    }
}

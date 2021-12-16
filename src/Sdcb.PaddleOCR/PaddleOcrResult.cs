using System.Linq;

namespace Sdcb.PaddleOCR
{
    public record PaddleOcrResult(PaddleOcrResultRegion[] Regions)
	{
		public string Text => string.Join("\n", Regions
			.OrderBy(x => x.Rect.Y)
			.ThenBy(x => x.Rect.X)
			.Select(x => x.Text));
	}
}

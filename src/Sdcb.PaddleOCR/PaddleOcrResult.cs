using System.Linq;

namespace Sdcb.PaddleOCR
{
#if NET5_0
	public record PaddleOcrResult(PaddleOcrResultRegion[] Regions)
	{
		public string Text => string.Join("\n", Regions
			.OrderBy(x => x.Rect.Y)
			.ThenBy(x => x.Rect.X)
			.Select(x => x.Text));
	}
#else
	public record PaddleOcrResult
	{
		public PaddleOcrResult(PaddleOcrResultRegion[] Regions)
        {
			this.Regions = Regions;
        }

		public PaddleOcrResultRegion[] Regions { get; }

		public string Text => string.Join("\n", Regions
			.OrderBy(x => x.Rect.Y)
			.ThenBy(x => x.Rect.X)
			.Select(x => x.Text));
	}
#endif
}

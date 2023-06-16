using System.Linq;

namespace Sdcb.PaddleOCR;

/// <summary>
/// Represents the OCR result of a paddle object detection model.
/// </summary>
public record PaddleOcrResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrResult"/> class with the specified <paramref name="Regions"/>.
    /// </summary>
    /// <param name="Regions">An array of <see cref="PaddleOcrResultRegion"/> objects representing the detected text regions.</param>
    public PaddleOcrResult(PaddleOcrResultRegion[] Regions)
    {
        this.Regions = Regions;
    }

    /// <summary>
    /// Gets an array of <see cref="PaddleOcrResultRegion"/> objects representing the detected text regions.
    /// </summary>
    /// <value>An array of <see cref="PaddleOcrResultRegion"/> objects representing the detected text regions.</value>
    public PaddleOcrResultRegion[] Regions { get; }

    /// <summary>
    /// Concatenates the text from each <see cref="PaddleOcrResultRegion"/> object in <see cref="Regions"/> 
    /// and returns the resulting string, ordered by the region's center positions.
    /// </summary>
    /// <value>A string containing the concatenated text from each <see cref="PaddleOcrResultRegion"/> object 
    /// in <see cref="Regions"/>, ordered by the region's center positions.</value>
    public string Text => string.Join("\n", Regions
        .OrderBy(x => x.Rect.Center.Y)
        .ThenBy(x => x.Rect.Center.X)
        .Select(x => x.Text));
}

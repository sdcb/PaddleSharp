using OpenCvSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdcb.PaddleOCR;

/// <summary>
/// Represents the result of table detection.
/// </summary>
/// <param name="Score">score of the table detection.</param>
/// <param name="StructureBoxes">the structured boxes of the table.</param>
/// <param name="HtmlTags">the html tags of the table.</param>
public record TableDetectionResult(float Score, List<TableCellBox> StructureBoxes, List<string> HtmlTags)
{
    /// <summary>
    /// Rebuilds table using the detected text content.
    /// </summary>
    /// <param name="ocrResult">the OCR result.</param>
    /// <returns>the table content as a string.</returns>
    public string RebuildTable(PaddleOcrResult ocrResult)
    {
        List<string>[] matched = Enumerable.Range(0, StructureBoxes.Count)
            .Select(x => new List<string>())
            .ToArray();

        for (int i = 0; i < ocrResult.Regions.Length; ++i)
        {
            PaddleOcrResultRegion region = ocrResult.Regions[i];
            Rect ocrBox = RectHelper.Extend(region.Rect.BoundingRect(), 1);

            int matchedStructure = StructureBoxes
                .Select((x, si) =>
                {
                    Rect structureBox = x.Rect;
                    return new
                    {
                        IouScore = RectHelper.IntersectionOverUnion(ocrBox, structureBox),
                        DistanceScore = RectHelper.Distance(ocrBox, structureBox),
                        Index = si
                    };
                })
                .OrderByDescending(x => x.IouScore)
                .ThenBy(x => x.DistanceScore)
                .First()
                .Index;

            matched[matchedStructure].Add(region.Text);
        }

        StringBuilder sb = new();
        sb.Append("<table>");
        int tdTagIndex = 0;

        foreach (string tag in HtmlTags)
        {
            if (tag.Contains("</td>"))
            {
                if (tag.Contains("<td></td>"))
                {
                    sb.Append("<td>");
                }

                if (matched[tdTagIndex].Count > 0)
                {
                    bool bWith = false;
                    if (matched[tdTagIndex][0].Contains("<b>") && matched[tdTagIndex].Count > 1)
                    {
                        bWith = true;
                        sb.Append("<b>");
                    }

                    for (int j = 0; j < matched[tdTagIndex].Count; j++)
                    {
                        string content = matched[tdTagIndex][j];
                        if (matched[tdTagIndex].Count > 1)
                        {
                            content = content.TrimStart(' ').Replace("<b>", "").Replace("</b>", "");
                            if (content.Length == 0)
                            {
                                continue;
                            }

                            if (j != matched[tdTagIndex].Count - 1 && !content.EndsWith(" "))
                            {
                                content += " ";
                            }
                        }

                        sb.Append(content);
                    }

                    if (bWith)
                    {
                        sb.Append("</b>");
                    }
                }

                if (tag.Contains("<td></td>"))
                {
                    sb.Append("</td>");
                }
                else
                {
                    sb.Append(tag);
                }

                tdTagIndex++;
            }
            else
            {
                sb.Append(tag);
            }
        }
        sb.Append("</table>");

        return sb.ToString();
    }

    /// <summary>
    /// Visualizes the table structure on the image.
    /// </summary>
    /// <param name="src">the source image.</param>
    /// <param name="color">the color used to draw the structure.</param>
    /// <param name="thickness">the thickness of the structure line.</param>
    /// <returns>the image with table structure visualized.</returns>
    public Mat Visualize(Mat src, Scalar color, int thickness = 1)
    {
        Mat clone = src.Clone();
        clone.DrawContours(StructureBoxes.Select(box => box.Contours), -1, color, thickness);
        return clone;
    }
}

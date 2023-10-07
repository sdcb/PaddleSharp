using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sdcb.PaddleOCR.Models.Local")]

namespace Sdcb.PaddleOCR.Models.LocalV4;

/// <summary>
/// Contains known models for local version 4
/// </summary>
internal static class KnownModels
{
    /// <summary>
    /// HashSet containing all the model names
    /// </summary>
    public static HashSet<string> All = new(new[]
    {
        "arabic_PP-OCRv4_rec",
        "ch_PP-OCRv4_det",
        "ch_PP-OCRv4_rec",
        "devanagari_PP-OCRv4_rec",
        "en_PP-OCRv4_rec",
        "japan_PP-OCRv4_rec",
        "ka_PP-OCRv4_rec",
        "korean_PP-OCRv4_rec",
        "ta_PP-OCRv4_rec",
        "te_PP-OCRv4_rec",
    });
}

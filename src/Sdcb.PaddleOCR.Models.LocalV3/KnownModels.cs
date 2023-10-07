using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sdcb.PaddleOCR.Models.Local")]

namespace Sdcb.PaddleOCR.Models.LocalV3;

public static class KnownModels
{
    public static HashSet<string> All = new(new[]
    {
        "arabic_PP-OCRv3_rec",
        "ch_PP-OCRv3_det",
        "ch_PP-OCRv3_rec",
        "chinese_cht_PP-OCRv3_rec",
        "cyrillic_PP-OCRv3_rec",
        "devanagari_PP-OCRv3_rec",
        "en_PP-OCRv3_det",
        "en_PP-OCRv3_rec",
        "japan_PP-OCRv3_rec",
        "ka_PP-OCRv3_rec",
        "korean_PP-OCRv3_rec",
        "latin_PP-OCRv3_rec",
        "ml_PP-OCRv3_det",
        "ta_PP-OCRv3_rec",
        "te_PP-OCRv3_rec",
    });
}
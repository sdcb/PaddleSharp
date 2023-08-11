namespace Sdcb.PaddleOCR.Models.Local;

/// <summary>
/// Provides a collection of all available OCR models for PaddleOCR in local version 3.
/// </summary>
public static class LocalFullModels
{
    /// <summary>
    /// Chinese v4, also support English and digits.
    /// </summary>
    public static FullOcrModel ChineseV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.ChineseV4);

    // delete because too large(>100MB)
    ///// <summary>
    ///// Chinese server v4, also support English and digits.
    ///// </summary>
    //public static FullOcrModel ChineseServerV4 => new(LocalDetectionModel.ChineseServerV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.ChineseV4);

    /// <summary>
    /// English v4 version.
    /// </summary>
    public static FullOcrModel EnglishV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.EnglishV4);

    /// <summary>
    /// The Korean V4 version.
    /// </summary>
    public static FullOcrModel KoreanV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.KoreanV4);

    /// <summary>
    /// The Japan V4 version.
    /// </summary>
    public static FullOcrModel JapanV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.JapanV4);

    /// <summary>
    /// The Telugu V4 version.
    /// </summary>
    public static FullOcrModel TeluguV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.TeluguV4);

    /// <summary>
    /// The Kannada V4 version.
    /// </summary>
    public static FullOcrModel KannadaV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.KannadaV4);

    /// <summary>
    /// The Tamil V4 version.
    /// </summary>
    public static FullOcrModel TamilV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.TamilV4);

    /// <summary>
    /// The Arabic V4 version.
    /// </summary>
    public static FullOcrModel ArabicV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.ArabicV4);

    /// <summary>
    /// The Devanagari V4 version.
    /// </summary>
    public static FullOcrModel DevanagariV4 => new(LocalDetectionModel.ChineseV4, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.DevanagariV4);

    /// <summary>
    /// Provides the OCR model for Chinese language.
    /// </summary>
    public static FullOcrModel ChineseV3 => new(LocalDetectionModel.ChineseV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.ChineseV3);

    /// <summary>
    /// Provides the OCR model for English language.
    /// </summary>
    public static FullOcrModel EnglishV3 => new(LocalDetectionModel.EnglishV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.EnglishV3);

    /// <summary>
    /// Provides the OCR model for Korean language.
    /// </summary>
    public static FullOcrModel KoreanV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.KoreanV3);

    /// <summary>
    /// Provides the OCR model for Japanese language.
    /// </summary>
    public static FullOcrModel JapanV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.JapanV3);

    /// <summary>
    /// Provides the OCR model for Traditional Chinese language.
    /// </summary>
    public static FullOcrModel TraditionalChineseV3 => new(LocalDetectionModel.ChineseV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.TraditionalChineseV3);

    /// <summary>
    /// Provides the OCR model for Telugu language.
    /// </summary>
    public static FullOcrModel TeluguV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.TeluguV3);

    /// <summary>
    /// Provides the OCR model for Kannada language.
    /// </summary>
    public static FullOcrModel KannadaV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.KannadaV3);

    /// <summary>
    /// Provides the OCR model for Tamil language.
    /// </summary>
    public static FullOcrModel TamilV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.TamilV3);

    /// <summary>
    /// Provides the OCR model for Latin language.
    /// </summary>
    public static FullOcrModel LatinV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.LatinV3);

    /// <summary>
    /// Provides the OCR model for Arabic language.
    /// </summary>
    public static FullOcrModel ArabicV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.ArabicV3);

    /// <summary>
    /// Provides the OCR model for Cyrillic language.
    /// </summary>
    public static FullOcrModel CyrillicV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.CyrillicV3);

    /// <summary>
    /// Provides the OCR model for Devanagari language.
    /// </summary>
    public static FullOcrModel DevanagariV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.DevanagariV3);

    /// <summary>
    /// Provides an array of all available OCR models for PaddleOCR
    /// </summary>
    public static FullOcrModel[] All => new[]
    {
        ChineseV3,
        EnglishV3,
        KoreanV3,
        JapanV3,
        TraditionalChineseV3,
        TeluguV3,
        KannadaV3,
        TamilV3,
        LatinV3,
        ArabicV3,
        CyrillicV3,
        DevanagariV3,
    };
}

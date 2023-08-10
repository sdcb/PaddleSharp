namespace Sdcb.PaddleOCR.Models.Local;

/// <summary>
/// Provides a collection of all available OCR models for PaddleOCR in local version 3.
/// </summary>
public static class LocalFullModels
{
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
    public static FullOcrModel TeluguV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.TeluguV3);

    /// <summary>
    /// Provides the OCR model for Kannada language.
    /// </summary>
    public static FullOcrModel KannadaV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.KannadaV3);

    /// <summary>
    /// Provides the OCR model for Tamil language.
    /// </summary>
    public static FullOcrModel TamilV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.TamilV3);

    /// <summary>
    /// Provides the OCR model for Latin language.
    /// </summary>
    public static FullOcrModel LatinV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.LatinV3);

    /// <summary>
    /// Provides the OCR model for Arabic language.
    /// </summary>
    public static FullOcrModel ArabicV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.ArabicV3);

    /// <summary>
    /// Provides the OCR model for Cyrillic language.
    /// </summary>
    public static FullOcrModel CyrillicV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.CyrillicV3);

    /// <summary>
    /// Provides the OCR model for Devanagari language.
    /// </summary>
    public static FullOcrModel DevanagariV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.DevanagariV3);

    /// <summary>
    /// Provides an array of all available OCR models for PaddleOCR in local version 3.
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

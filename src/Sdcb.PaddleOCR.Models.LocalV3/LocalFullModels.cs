namespace Sdcb.PaddleOCR.Models.LocalV3;

public static class LocalFullModels
{
    public static FullOcrModel ChineseV3 => new(LocalDetectionModel.ChineseV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.ChineseV3);

    public static FullOcrModel EnglishV3 => new(LocalDetectionModel.EnglishV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.EnglishV3);

    public static FullOcrModel KoreanV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.KoreanV3);

    public static FullOcrModel JapanV3 => new(LocalDetectionModel.MultiLanguageV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.JapanV3);

    public static FullOcrModel TraditionalChineseV3 => new(LocalDetectionModel.ChineseV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.TraditionalChineseV3);

    public static FullOcrModel TeluguV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.TeluguV3);

    public static FullOcrModel KannadaV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.KannadaV3);

    public static FullOcrModel TamilV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.TamilV3);

    public static FullOcrModel LatinV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.LatinV3);

    public static FullOcrModel ArabicV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.ArabicV3);

    public static FullOcrModel CyrillicV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.CyrillicV3);

    public static FullOcrModel DevanagariV3 => new(LocalDetectionModel.MultiLanguageV3, null, LocalRecognizationModel.DevanagariV3);

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

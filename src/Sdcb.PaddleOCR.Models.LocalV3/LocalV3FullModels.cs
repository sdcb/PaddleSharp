namespace Sdcb.PaddleOCR.Models.LocalV3
{
    public static class LocalV3FullModels
    {
        public readonly static FullOcrModel ChineseV3 = new(LocalDetectionModel.ChineseV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.ChineseV3);

        public readonly static FullOcrModel ChineseV3Slim = new (LocalDetectionModel.ChineseV3Slim, LocalClassificationModel.ChineseMobileSlimV2, LocalRecognizationModel.ChineseV3Slim);

        public readonly static FullOcrModel EnglishV3 = new(LocalDetectionModel.EnglishV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.EnglishV3);

        public readonly static FullOcrModel TranditionalChinseV3 = new(LocalDetectionModel.ChineseV3, LocalClassificationModel.ChineseMobileV2, LocalRecognizationModel.TranditionalChineseV3);
    }
}

using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.LocalV3.Details;

namespace Sdcb.PaddleOCR.Models.LocalV3
{
    public class LocalDetectionModel : DetectionModel
    {
        public string Name { get; }

        public ModelVersion Version { get; }

        public LocalDetectionModel(string name, ModelVersion version)
        {
            Name = name;
            Version = version;
        }

        public override PaddleConfig CreateConfig() => Utils.LoadLocalModel(Name);

        /// <summary>
        /// [New] slim quantization with distillation lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 1.1M)
        /// </summary>
        public static LocalDetectionModel ChineseV3Slim => new("ch_PP-OCRv3_det_slim", ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 3.8M)
        /// </summary>
        public static LocalDetectionModel ChineseV3 => new("ch_PP-OCRv3_det", ModelVersion.V3);

        /// <summary>
        /// [New] Slim qunatization with distillation lightweight detection model, supporting English
        /// (Size: 1.1M)
        /// </summary>
        public static LocalDetectionModel EnglishV3Slim => new("en_PP-OCRv3_det_slim", ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight detection model, supporting English
        /// (Size: 3.8M)
        /// </summary>
        public static LocalDetectionModel EnglishV3 => new("en_PP-OCRv3_det", ModelVersion.V3);

        /// <summary>
        /// [New] Slim qunatization with distillation lightweight detection model, supporting English
        /// (Size: 1.1M)
        /// </summary>
        public static LocalDetectionModel MultiLanguageV3Slim => new("ml_PP-OCRv3_det_slim", ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight detection model, supporting English
        /// (Size: 3.8M)
        /// </summary>
        public static LocalDetectionModel MultiLanguageV3 => new("ml_PP-OCRv3_det", ModelVersion.V3);

        public static LocalDetectionModel[] All => new[]
        {
            ChineseV3Slim,
            ChineseV3,
            EnglishV3Slim,
            EnglishV3,
            MultiLanguageV3Slim,
            MultiLanguageV3,
        };
    }
}

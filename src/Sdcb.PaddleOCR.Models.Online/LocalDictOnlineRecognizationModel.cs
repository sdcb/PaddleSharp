using Sdcb.PaddleOCR.Models.Online.Details;
using Sdcb.PaddleOCR.Models.Shared;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online;

/// <summary>
/// Class representing a local online recognition model with a custom dictionary.
/// </summary>
/// <remarks>
/// Used for downloading and extracting a model from a url, and creating a new StreamDictFileRecognizationModel with the downloaded contents.
/// </remarks>
public record LocalDictOnlineRecognizationModel(string Name, string DictName, Uri Uri, ModelVersion Version)
{
    /// <summary>
    /// Gets or sets the root directory for the downloaded models.
    /// </summary>
    public string RootDirectory = Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Downloads and extracts a RecognizationModel asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The downloaded and extracted RecognizationModel.</returns>
    public async Task<RecognizationModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken);

        if (Version == ModelVersion.V5)
        {
            return RecognizationModel.FromDirectory(RootDirectory, "", Version);
        }
        else
        {
            return new StreamDictFileRecognizationModel(RootDirectory, SharedUtils.LoadDicts(DictName), Version);
        }
    }

    /// <summary>
    /// v5 model for Chinese recognition
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseV5 => new("PP-OCRv5_mobile_rec_infer", "", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv5_mobile_rec_infer.tar"), ModelVersion.V5);

    /// <summary>
    /// v5 server model for Chinese recognition
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseServerV5 => new("PP-OCRv5_server_rec_infer.tar", "", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv5_server_rec_infer.tar"), ModelVersion.V5);

    /// <summary>
    /// v4 model for Chinese recognition
    /// (Size: 10.46MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseV4 => new("ch_PP-OCRv4_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/chinese/ch_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for English recognition
    /// (Size: 9.76MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel EnglishV4 => new("en_PP-OCRv4_rec", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/english/en_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for Korean recognition
    /// (Size: 23.25MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel KoreanV4 => new("korean_PP-OCRv4_rec", "korean_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/multilingual/korean_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for Japanese recognition
    /// (Size: 9.51MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel JapanV4 => new("japan_PP-OCRv4_rec", "japan_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/multilingual/japan_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for Telugu recognition
    /// (Size: 21.62MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel TeluguV4 => new("te_PP-OCRv4_rec", "te_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/multilingual/te_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for Kannada recognition
    /// (Size: 7.56MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel KannadaV4 => new("ka_PP-OCRv4_rec", "ka_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/multilingual/ka_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for Tamil recognition
    /// (Size: 21.60MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel TamilV4 => new("ta_PP-OCRv4_rec", "ta_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/multilingual/ta_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for Arabic recognition
    /// (Size: 7.55MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ArabicV4 => new("arabic_PP-OCRv4_rec", "arabic_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/multilingual/arabic_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// v4 model for Devanagari recognition
    /// (Size: 7.56MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel DevanagariV4 => new("devanagari_PP-OCRv4_rec", "devanagari_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/multilingual/devanagari_PP-OCRv4_rec_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// Slim qunatization with distillation lightweight model, supporting Chinese, English text recognition
    /// (Size: 4.9MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseV3Slim => new("ch_PP-OCRv3_rec_slim", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_rec_slim_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Original lightweight model, supporting Chinese, English, multilingual text recognition
    /// (Size: 12.4MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseV3 => new("ch_PP-OCRv3_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Slim qunatization with distillation lightweight model, supporting Chinese, English text recognition
    /// (Size: 9MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseV2Slim => new("ch_PP-OCRv2_rec_slim", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_rec_slim_quant_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Original lightweight model, supporting Chinese, English, multilingual text recognition
    /// (Size: 8.5MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseV2 => new("ch_PP-OCRv2_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_rec_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Slim pruned and quantized lightweight model, supporting Chinese, English and number recognition
    /// (Size: 6MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseMobileSlimV2 => new("ch_ppocr_mobile_slim_v2.0_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_rec_slim_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Original lightweight model, supporting Chinese, English and number recognition
    /// (Size: 5.2MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseMobileV2 => new("ch_ppocr_mobile_v2.0_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_rec_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// General model, supporting Chinese, English and number recognition
    /// (Size: 94.8MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ChineseServerV2 => new("ch_ppocr_server_v2.0_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_rec_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Slim qunatization with distillation lightweight model, supporting english, English text recognition
    /// (Size: 3.2MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel EnglishV3Slim => new("en_PP-OCRv3_rec_slim", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_rec_slim_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Original lightweight model, supporting english, English, multilingual text recognition
    /// (Size: 9.6MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel EnglishV3 => new("en_PP-OCRv3_rec", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Slim pruned and quantized lightweight model, supporting English and number recognition
    /// (Size: 2.7MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel EnglishNumberMobileSlimV2 => new("en_number_mobile_slim_v2.0_rec", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/en/en_number_mobile_v2.0_rec_slim_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Original lightweight model, supporting English and number recognition
    /// (Size: 2.6MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel EnglishNumberMobileV2 => new("en_number_mobile_v2.0_rec", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/multilingual/en_number_mobile_v2.0_rec_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Lightweight model for Korean recognition
    /// (Size: 11MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel KoreanV3 => new("korean_PP-OCRv3_rec", "korean_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/korean_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Japanese recognition
    /// (Size: 11MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel JapanV3 => new("japan_PP-OCRv3_rec", "japan_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/japan_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for chinese cht
    /// (Size: 12MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel TraditionalChineseV3 => new("chinese_cht_PP-OCRv3_rec", "chinese_cht_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/chinese_cht_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Telugu recognition
    /// (Size: 9.6MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel TeluguV3 => new("te_PP-OCRv3_rec", "te_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/te_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Kannada recognition
    /// (Size: 9.9MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel KannadaV3 => new("ka_PP-OCRv3_rec", "ka_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/ka_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Tamil recognition
    /// (Size: 9.6MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel TamilV3 => new("ta_PP-OCRv3_rec", "ta_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/ta_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for latin recognition
    /// (Size: 9.7MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel LatinV3 => new("latin_PP-OCRv3_rec", "latin_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/latin_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for arabic recognition
    /// (Size: 9.6MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel ArabicV3 => new("arabic_PP-OCRv3_rec", "arabic_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/arabic_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for cyrillic recognition
    /// (Size: 9.6MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel CyrillicV3 => new("cyrillic_PP-OCRv3_rec", "cyrillic_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/cyrillic_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Lightweight model for devanagari recognition
    /// (Size: 9.9MB)
    /// </summary>
    public static LocalDictOnlineRecognizationModel DevanagariV3 => new("devanagari_PP-OCRv3_rec", "devanagari_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/devanagari_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Provides an array of all available recognizer models, including for Chinese, English, Korean, Japanese, Arabic, Cyrillic and so on.
    /// </summary>
    public static LocalDictOnlineRecognizationModel[] All => new[]
    {
        ChineseV5,
        ChineseServerV5,
        ChineseV4,
        EnglishV4,
        KoreanV4,
        JapanV4,
        TeluguV4,
        KannadaV4,
        TamilV4,
        ArabicV4,
        DevanagariV4,
        ChineseV3Slim,
        ChineseV3,
        ChineseV2Slim,
        ChineseV2,
        ChineseMobileSlimV2,
        ChineseMobileV2,
        ChineseServerV2,
        EnglishV3Slim,
        EnglishV3,
        EnglishNumberMobileSlimV2,
        EnglishNumberMobileV2,
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

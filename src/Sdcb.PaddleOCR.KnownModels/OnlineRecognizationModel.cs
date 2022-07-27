using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online
{
    public record LocalDictOnlineRecognizationModel(string name, string dictName, Uri uri, ModelVersion version)
    {
        public string RootDirectory = Path.Combine(Settings.GlobalModelDirectory, name);

        public static string DictRootDirectory = Path.Combine(Settings.GlobalModelDirectory, "dicts");

        public async Task<FileRecognizationModel> DownloadAsync(CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(RootDirectory);

            string paramsFile = Path.Combine(RootDirectory, "inference.pdiparams");
            if (!File.Exists(paramsFile))
            {
                string localTarFile = Path.Combine(RootDirectory, uri.Segments.Last());
                if (!File.Exists(localTarFile))
                {
                    Console.WriteLine($"Downloading {name} model from {uri}");
                    await Utils.DownloadFile(uri, localTarFile, cancellationToken);
                }

                Console.WriteLine($"Extracting {localTarFile} to {RootDirectory}");
                using (IArchive archive = ArchiveFactory.Open(localTarFile))
                {
                    archive.WriteToDirectory(RootDirectory);
                }

                File.Delete(localTarFile);
            }

            string keyFile = Path.Combine(DictRootDirectory, Path.GetFileName(dictName));
            if (!File.Exists(keyFile))
            {
                Directory.CreateDirectory(DictRootDirectory);
                using FileStream localFile = File.OpenWrite(keyFile);
                using Stream resource = GetType().Assembly.GetManifestResourceStream($"Sdcb.PaddleOCR.KnownModels.dicts.{dictName}");
                await resource.CopyToAsync(localFile, 81920, cancellationToken);
            }

            return new FileRecognizationModel(RootDirectory, keyFile, version);
        }

        internal static Stream GetDictStreamByName(string dictName)
        {
            Type type = typeof(LocalDictOnlineRecognizationModel);
            string ns = type.Namespace;
            
            return type.Assembly.GetManifestResourceStream($"{ns}.dicts.{dictName}");
        }

        /// <summary>
        /// [New] Slim qunatization with distillation lightweight model, supporting Chinese, English text recognition
        /// (Size: 4.9M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ChineseV3Slim = new("ch_PP-OCRv3_rec_slim", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_rec_slim_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight model, supporting Chinese, English, multilingual text recognition
        /// (Size: 12.4M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ChineseV3 = new("ch_PP-OCRv3_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Slim qunatization with distillation lightweight model, supporting Chinese, English text recognition
        /// (Size: 9M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ChineseV2Slim = new("ch_PP-OCRv2_rec_slim", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_rec_slim_quant_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Original lightweight model, supporting Chinese, English, multilingual text recognition
        /// (Size: 8.5M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ChineseV2 = new("ch_PP-OCRv2_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_rec_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Slim pruned and quantized lightweight model, supporting Chinese, English and number recognition
        /// (Size: 6M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ChineseMobileSlimV2 = new("ch_ppocr_mobile_slim_v2.0_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_rec_slim_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Original lightweight model, supporting Chinese, English and number recognition
        /// (Size: 5.2M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ChineseMobileV2 = new("ch_ppocr_mobile_v2.0_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_rec_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// General model, supporting Chinese, English and number recognition
        /// (Size: 94.8M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ChineseServerV2 = new("ch_ppocr_server_v2.0_rec", "ppocr_keys_v1.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_rec_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// [New] Slim qunatization with distillation lightweight model, supporting english, English text recognition
        /// (Size: 3.2M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel EnglishV3Slim = new("en_PP-OCRv3_rec_slim", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_rec_slim_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight model, supporting english, English, multilingual text recognition
        /// (Size: 9.6M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel EnglishV3 = new("en_PP-OCRv3_rec", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Slim pruned and quantized lightweight model, supporting English and number recognition
        /// (Size: 2.7M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel EnglishNumberMobileSlimV2 = new("en_number_mobile_slim_v2.0_rec", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/en/en_number_mobile_v2.0_rec_slim_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Original lightweight model, supporting English and number recognition
        /// (Size: 2.6M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel EnglishNumberMobileV2 = new("en_number_mobile_v2.0_rec", "en_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/multilingual/en_number_mobile_v2.0_rec_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Lightweight model for Korean recognition
        /// (Size: 11M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel KoreanV3 = new("korean_PP-OCRv3_rec", "korean_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/korean_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for Japanese recognition
        /// (Size: 11M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel JapanV3 = new("japan_PP-OCRv3_rec", "japan_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/japan_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for chinese cht
        /// (Size: 12M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel TranditionalChineseV3 = new("chinese_cht_PP-OCRv3_rec", "chinese_cht_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/chinese_cht_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for Telugu recognition
        /// (Size: 9.6M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel TeluguV3 = new("te_PP-OCRv3_rec", "te_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/te_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for Kannada recognition
        /// (Size: 9.9M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel KannadaV3 = new("ka_PP-OCRv3_rec", "ka_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/ka_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for Tamil recognition
        /// (Size: 9.6M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel TamilV3 = new("ta_PP-OCRv3_rec", "ta_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/ta_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for latin recognition
        /// (Size: 9.7M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel LatinV3 = new("latin_PP-OCRv3_rec", "latin_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/latin_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for arabic recognition
        /// (Size: 9.6M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel ArabicV3 = new("arabic_PP-OCRv3_rec", "arabic_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/arabic_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for cyrillic recognition
        /// (Size: 9.6M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel CyrillicV3 = new("cyrillic_PP-OCRv3_rec", "cyrillic_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/cyrillic_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// Lightweight model for devanagari recognition
        /// (Size: 9.9M)
        /// </summary>
        public readonly static LocalDictOnlineRecognizationModel DevanagariV3 = new("devanagari_PP-OCRv3_rec", "devanagari_dict.txt", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/devanagari_PP-OCRv3_rec_infer.tar"), ModelVersion.V3);
    }
}

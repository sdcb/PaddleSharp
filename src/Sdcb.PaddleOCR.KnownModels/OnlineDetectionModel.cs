using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using Sdcb.PaddleOCR.Models.Online.Details;
using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online
{
    public record OnlineDetectionModel(string name, Uri uri, ModelVersion version)
    {
        public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, name);

        public async Task<FileDetectionModel> DownloadAsync(CancellationToken cancellationToken = default)
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

            return new FileDetectionModel(RootDirectory);
        }

        /// <summary>
        /// [New] slim quantization with distillation lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 1.1M)
        /// </summary>
        public static OnlineDetectionModel ChineseV3Slim => new("ch_PP-OCRv3_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_det_slim_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 3.8M)
        /// </summary>
        public static OnlineDetectionModel ChineseV3 => new("ch_PP-OCRv3_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_det_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// [New] slim quantization with distillation lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 3M)
        /// </summary>
        public static OnlineDetectionModel ChineseV2Slim => new("ch_PP-OCRv2_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_det_slim_quant_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// [New] Original lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 3M)
        /// </summary>
        public static OnlineDetectionModel ChineseV2 => new("ch_PP-OCRv2_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_det_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Slim pruned lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 2.6M)
        /// </summary>
        public static OnlineDetectionModel ChineseMobileSlimV2 => new("ch_ppocr_mobile_slim_v2.0_det", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/slim/ch_ppocr_mobile_v2.0_det_prune_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// Original lightweight model, supporting Chinese, English, multilingual text detection
        /// (Size: 3M)
        /// </summary>
        public static OnlineDetectionModel ChineseMobileV2 => new("ch_ppocr_mobile_v2.0_det", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_det_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// General model, which is larger than the lightweight model, but achieved better performance
        /// (Size: 47M)
        /// </summary>
        public static OnlineDetectionModel ChineseServerV2 => new("ch_ppocr_server_v2.0_det", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_det_infer.tar"), ModelVersion.V2);

        /// <summary>
        /// [New] Slim qunatization with distillation lightweight detection model, supporting English
        /// (Size: 1.1M)
        /// </summary>
        public static OnlineDetectionModel EnglishV3Slim => new("en_PP-OCRv3_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_det_slim_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight detection model, supporting English
        /// (Size: 3.8M)
        /// </summary>
        public static OnlineDetectionModel EnglishV3 => new("en_PP-OCRv3_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_det_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// [New] Slim qunatization with distillation lightweight detection model, supporting English
        /// (Size: 1.1M)
        /// </summary>
        public static OnlineDetectionModel MultiLanguageV3Slim => new("ml_PP-OCRv3_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/Multilingual_PP-OCRv3_det_slim_infer.tar"), ModelVersion.V3);

        /// <summary>
        /// [New] Original lightweight detection model, supporting English
        /// (Size: 3.8M)
        /// </summary>
        public static OnlineDetectionModel MultiLanguageV3 => new("ml_PP-OCRv3_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/Multilingual_PP-OCRv3_det_infer.tar"), ModelVersion.V3);

        public static OnlineDetectionModel[] All => new[]
        {
            ChineseV3Slim,
            ChineseV3,
            ChineseV2Slim,
            ChineseV2,
            ChineseMobileSlimV2,
            ChineseMobileV2,
            ChineseServerV2,
            EnglishV3Slim,
            EnglishV3,
            MultiLanguageV3Slim,
            MultiLanguageV3,
        };
    }
}

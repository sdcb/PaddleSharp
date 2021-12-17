using SharpCompress.Archives;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.KnownModels
{
    public class KnownOCRModel
    {
        private static readonly Uri ChineseKeyUri = new Uri(@"https://raw.githubusercontent.com/PaddlePaddle/PaddleOCR/release/2.3/ppocr/utils/ppocr_keys_v1.txt");
        private static readonly Uri EnglishKeyUri = new Uri(@"https://raw.githubusercontent.com/PaddlePaddle/PaddleOCR/release/2.3/ppocr/utils/en_dict.txt");

        public static OCRModel PPOcrV2 = new OCRModel("ppocr-v2",
            new Uri(@"https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_det_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_rec_infer.tar"),
            ChineseKeyUri);

        public static OCRModel PPOcrMobileV2 = new OCRModel("ppocr-mobile-v2",
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_det_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_rec_infer.tar"),
            ChineseKeyUri);

        public static OCRModel EnPPOcrMobileV2 = new OCRModel("en-ppocr-mobile-v2",
            PPOcrV2.DetectionModelUri,
            PPOcrV2.ClassifierModelUri,
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/multilingual/en_number_mobile_v2.0_rec_infer.tar"),
            EnglishKeyUri);

        public static OCRModel PPOcrServerV2 = new OCRModel("ppocr-server-v2",
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_det_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_rec_infer.tar"),
            ChineseKeyUri);
    }
}

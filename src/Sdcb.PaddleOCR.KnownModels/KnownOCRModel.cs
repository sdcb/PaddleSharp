using System;

namespace Sdcb.PaddleOCR.KnownModels
{
    public class KnownOCRModel
    {
        private static readonly Uri[] ChineseKeyUris = new[]
        {
            new Uri(@"https://gitee.com/paddlepaddle/PaddleOCR/raw/release/2.3/ppocr/utils/ppocr_keys_v1.txt"), /* Gitee */
            new Uri(@"https://raw.githubusercontent.com/PaddlePaddle/PaddleOCR/release/2.3/ppocr/utils/ppocr_keys_v1.txt"), /* Github */
        };
        private static readonly Uri[] EnglishKeyUris = new[]
        {
            new Uri(@"https://raw.githubusercontent.com/PaddlePaddle/PaddleOCR/release/2.3/ppocr/utils/en_dict.txt"), /* Github */
            new Uri(@"https://gitee.com/paddlepaddle/PaddleOCR/raw/release/2.3/ppocr/utils/en_dict.txt"), /* Gitee */
        };
        private static readonly Uri[] ChineseTranditionalKeyUris = new[]
        {
            new Uri(@"https://raw.githubusercontent.com/PaddlePaddle/PaddleOCR/release/2.4/ppocr/utils/dict/chinese_cht_dict.txt"), /* Github */
            new Uri(@"https://gitee.com/paddlepaddle/PaddleOCR/raw/release/2.4/ppocr/utils/dict/chinese_cht_dict.txt"), /* Gitee */
        };

        public static OCRModel PPOcrV2 = new OCRModel("ppocr-v2",
            new Uri(@"https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_det_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_rec_infer.tar"),
            ChineseKeyUris);

        // this model not correct.
        //public static OCRModel PPOcrMobileV2 = new OCRModel("ppocr-mobile-v2",
        //    new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_det_infer.tar"),
        //    new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"),
        //    new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_rec_infer.tar"),
        //    ChineseKeyUris);

        public static OCRModel EnglishMobileV2 = new OCRModel("en-ppocr-mobile-v2",
            PPOcrV2.DetectionModelUris,
            PPOcrV2.ClassifierModelUris,
            new[] { new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/multilingual/en_number_mobile_v2.0_rec_infer.tar") },
            EnglishKeyUris);

        public static OCRModel ChineseTranditionalMobileV2 = new OCRModel("chinese-cht-mobile-v2",
            PPOcrV2.DetectionModelUris,
            PPOcrV2.ClassifierModelUris,
            new[] { new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/multilingual/chinese_cht_mobile_v2.0_rec_infer.tar") },
            ChineseTranditionalKeyUris);

        public static OCRModel PPOcrServerV2 = new OCRModel("ppocr-server-v2",
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_det_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"),
            new Uri(@"https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_rec_infer.tar"),
            ChineseKeyUris);
    }
}

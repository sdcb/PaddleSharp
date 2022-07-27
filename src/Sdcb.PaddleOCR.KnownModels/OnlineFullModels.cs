using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online
{
    public record class OnlineFullModels(OnlineDetectionModel detModel, OnlineClassificationModel? clsModel, LocalDictOnlineRecognizationModel recModel)
    {
        public async Task<FullOcrModel> DownloadAsync(CancellationToken cancellationToken = default)
        {
            FileDetectionModel localDetModel = await detModel.DownloadAsync(cancellationToken);
            FileClassificationModel? localClsModel = clsModel != null ? await clsModel.DownloadAsync(cancellationToken) : null;
            FileRecognizationModel? localRecModel = await recModel.DownloadAsync(cancellationToken);
            return new FullOcrModel(localDetModel, localClsModel, localRecModel);
        }

        public readonly static OnlineFullModels ChineseV2 = new(OnlineDetectionModel.ChineseV2, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseV3);

        public readonly static OnlineFullModels ChineseServerV2 = new(OnlineDetectionModel.ChineseServerV2, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseServerV2);

        public readonly static OnlineFullModels ChineseV3 = new(OnlineDetectionModel.ChineseV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseV3);

        public readonly static OnlineFullModels ChineseV3Slim = new(OnlineDetectionModel.ChineseV3Slim, OnlineClassificationModel.ChineseMobileSlimV2, LocalDictOnlineRecognizationModel.ChineseV3Slim);

        public readonly static OnlineFullModels EnglishV3 = new(OnlineDetectionModel.EnglishV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.EnglishV3);

        public readonly static OnlineFullModels TranditionalChinseV3 = new(OnlineDetectionModel.ChineseV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TranditionalChineseV3);

        // free to make any other combinations
    }
}

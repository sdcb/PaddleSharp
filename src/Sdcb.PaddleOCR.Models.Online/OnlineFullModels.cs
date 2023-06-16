using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online;

public record class OnlineFullModels(OnlineDetectionModel detModel, OnlineClassificationModel? clsModel, LocalDictOnlineRecognizationModel recModel)
{
    public async Task<FullOcrModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        FileDetectionModel localDetModel = await detModel.DownloadAsync(cancellationToken);
        FileClassificationModel? localClsModel = clsModel != null ? await clsModel.DownloadAsync(cancellationToken) : null;
        RecognizationModel localRecModel = await recModel.DownloadAsync(cancellationToken);
        return new FullOcrModel(localDetModel, localClsModel, localRecModel);
    }

    public readonly static OnlineFullModels ChineseV2 = new(OnlineDetectionModel.ChineseV2, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseV3);

    public readonly static OnlineFullModels ChineseServerV2 = new(OnlineDetectionModel.ChineseServerV2, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseServerV2);

    public readonly static OnlineFullModels ChineseV3 = new(OnlineDetectionModel.ChineseV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseV3);

    public readonly static OnlineFullModels ChineseV3Slim = new(OnlineDetectionModel.ChineseV3Slim, OnlineClassificationModel.ChineseMobileSlimV2, LocalDictOnlineRecognizationModel.ChineseV3Slim);

    public readonly static OnlineFullModels EnglishV3 = new(OnlineDetectionModel.EnglishV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.EnglishV3);

    public readonly static OnlineFullModels EnglishV3Slim = new(OnlineDetectionModel.EnglishV3Slim, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.EnglishV3Slim);

    public readonly static OnlineFullModels KoreanV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.KoreanV3);

    public readonly static OnlineFullModels JapanV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.JapanV3);

    public readonly static OnlineFullModels TraditionalChineseV3 = new(OnlineDetectionModel.ChineseV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TraditionalChineseV3);

    public readonly static OnlineFullModels TeluguV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TeluguV3);

    public readonly static OnlineFullModels KannadaV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.KannadaV3);

    public readonly static OnlineFullModels TamilV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TamilV3);

    public readonly static OnlineFullModels LatinV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.LatinV3);

    public readonly static OnlineFullModels ArabicV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ArabicV3);

    public readonly static OnlineFullModels CyrillicV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.CyrillicV3);

    public readonly static OnlineFullModels DevanagariV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.DevanagariV3);
}

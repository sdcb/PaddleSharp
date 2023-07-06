using Sdcb.PaddleOCR.Models.Details;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online;

/// <summary>
/// Class for online full ocr models.
/// </summary>
/// <remarks>
/// Contains static instances for commonly used models and a method for downloading a full OCR model.
/// </remarks>
public record class OnlineFullModels(OnlineDetectionModel DetModel, OnlineClassificationModel? ClsModel, LocalDictOnlineRecognizationModel RecModel)
{
    /// <summary>
    /// Downloads a full OCR model asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="FullOcrModel"/> instance.</returns>
    public async Task<FullOcrModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        FileDetectionModel localDetModel = await DetModel.DownloadAsync(cancellationToken);
        FileClassificationModel? localClsModel = ClsModel != null ? await ClsModel.DownloadAsync(cancellationToken) : null;
        RecognizationModel localRecModel = await RecModel.DownloadAsync(cancellationToken);
        return new FullOcrModel(localDetModel, localClsModel, localRecModel);
    }

    /// <summary>
    /// The Chinese V4 version.
    /// </summary>
    public readonly static OnlineFullModels ChineseV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseV4);

    /// <summary>
    /// The English V4 version.
    /// </summary>
    public readonly static OnlineFullModels EnglishV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.EnglishV4);

    /// <summary>
    /// The Korean V4 version.
    /// </summary>
    public readonly static OnlineFullModels KoreanV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.KoreanV4);

    /// <summary>
    /// The Japan V4 version.
    /// </summary>
    public readonly static OnlineFullModels JapanV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.JapanV4);

    /// <summary>
    /// The Telugu V4 version.
    /// </summary>
    public readonly static OnlineFullModels TeluguV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TeluguV4);

    /// <summary>
    /// The Kannada V4 version.
    /// </summary>
    public readonly static OnlineFullModels KannadaV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.KannadaV4);

    /// <summary>
    /// The Tamil V4 version.
    /// </summary>
    public readonly static OnlineFullModels TamilV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TamilV4);

    /// <summary>
    /// The Arabic V4 version.
    /// </summary>
    public readonly static OnlineFullModels ArabicV4 = new(OnlineDetectionModel.ChineseV4, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ArabicV4);



    /// <summary>
    /// The Chinese Server version 2.
    /// </summary>
    public readonly static OnlineFullModels ChineseServerV2 = new(OnlineDetectionModel.ChineseServerV2, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseServerV2);

    /// <summary>
    /// The Chinese V2 version.
    /// </summary>
    public readonly static OnlineFullModels ChineseV2 = new(OnlineDetectionModel.ChineseV2, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseV3);

    /// <summary>
    /// The Chinese V3 version.
    /// </summary>
    public readonly static OnlineFullModels ChineseV3 = new(OnlineDetectionModel.ChineseV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ChineseV3);

    /// <summary>
    /// The slim version of Chinese V3.
    /// </summary>
    public readonly static OnlineFullModels ChineseV3Slim = new(OnlineDetectionModel.ChineseV3Slim, OnlineClassificationModel.ChineseMobileSlimV2, LocalDictOnlineRecognizationModel.ChineseV3Slim);

    /// <summary>
    /// The English V3 version.
    /// </summary>
    public readonly static OnlineFullModels EnglishV3 = new(OnlineDetectionModel.EnglishV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.EnglishV3);

    /// <summary>
    /// The slim version of English V3.
    /// </summary>
    public readonly static OnlineFullModels EnglishV3Slim = new(OnlineDetectionModel.EnglishV3Slim, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.EnglishV3Slim);

    /// <summary>
    /// The Korean V3 version.
    /// </summary>
    public readonly static OnlineFullModels KoreanV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.KoreanV3);

    /// <summary>
    /// The Japan V3 version.
    /// </summary>
    public readonly static OnlineFullModels JapanV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.JapanV3);

    /// <summary>
    /// The Traditional Chinese V3 version.
    /// </summary>
    public readonly static OnlineFullModels TraditionalChineseV3 = new(OnlineDetectionModel.ChineseV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TraditionalChineseV3);

    /// <summary>
    /// The Telugu V3 version.
    /// </summary>
    public readonly static OnlineFullModels TeluguV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TeluguV3);

    /// <summary>
    /// The Kannada V3 version.
    /// </summary>
    public readonly static OnlineFullModels KannadaV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.KannadaV3);

    /// <summary>
    /// The Tamil V3 version.
    /// </summary>
    public readonly static OnlineFullModels TamilV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.TamilV3);

    /// <summary>
    /// The Latin V3 version.
    /// </summary>
    public readonly static OnlineFullModels LatinV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.LatinV3);

    /// <summary>
    /// The Arabic V3 version.
    /// </summary>
    public readonly static OnlineFullModels ArabicV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.ArabicV3);

    /// <summary>
    /// The Cyrillic V3 version.
    /// </summary>
    public readonly static OnlineFullModels CyrillicV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.CyrillicV3);

    /// <summary>
    /// The Devanagari V3 version.
    /// </summary>
    public readonly static OnlineFullModels DevanagariV3 = new(OnlineDetectionModel.MultiLanguageV3, OnlineClassificationModel.ChineseMobileV2, LocalDictOnlineRecognizationModel.DevanagariV3);
}

using Sdcb.PaddleOCR.Models.Details;
using Sdcb.PaddleOCR.Models.Online.Details;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.PaddleOCR.Models.Online;

/// <summary>
/// Represents an online classification model that can be downloaded and used for text angle classification.
/// </summary>
public record OnlineClassificationModel(string Name, Uri Uri, ModelVersion Version)
{
    /// <summary>
    /// Gets the root directory of the model.
    /// </summary>
    public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Downloads and extracts the model asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="FileClassificationModel"/> that represent the downloaded model.</returns>
    public async Task<FileClassificationModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken);
        return new FileClassificationModel(RootDirectory, Version);
    }

    /// <summary>
    /// Gets an online classification model for slim quantized text angle classification (Size: 2.1M).
    /// </summary>
    public static OnlineClassificationModel ChineseMobileSlimV2 => new("ch_ppocr_mobile_slim_v2.0_cls", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_slim_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Gets an online classification model for original text angle classification (Size: 1.38M).
    /// </summary>
    public static OnlineClassificationModel ChineseMobileV2 => new("ch_ppocr_mobile_v2.0_cls", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_cls_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Gets an array of all available online classification models.
    /// </summary>
    public static OnlineClassificationModel[] All => new[]
    {
        ChineseMobileSlimV2,
        ChineseMobileV2,
    };
}

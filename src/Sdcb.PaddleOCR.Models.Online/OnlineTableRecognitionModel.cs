using Sdcb.PaddleOCR.Models.Online.Details;
using System;
using System.Threading.Tasks;
using System.Threading;
using Sdcb.PaddleOCR.Models.Online;
using System.IO;

namespace Sdcb.PaddleOCR.Models.LocalV3;

/// <summary>
/// This class represents an online table recognition model.
/// </summary>
public class OnlineTableRecognitionModel
{
    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the download URI for the model.
    /// </summary>
    public Uri ModelDownloadUri { get; }

    /// <summary>
    /// Gets the name of the dictionary.
    /// </summary>
    public string DictName { get; }

    /// <summary>
    /// Gets the root directory of the model.
    /// </summary>
    public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineTableRecognitionModel"/> class.
    /// </summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="modelDownloadUri">The download URI for the model.</param>
    /// <param name="dictName">The name of the dictionary.</param>
    public OnlineTableRecognitionModel(string name, Uri modelDownloadUri, string dictName)
    {
        Name = name;
        ModelDownloadUri = modelDownloadUri;
        DictName = dictName;
    }

    /// <summary>
    /// Downloads the model asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>The downloaded table recognition model.</returns>
    public async Task<TableRecognitionModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        await Utils.DownloadAndExtractAsync(Name, ModelDownloadUri, RootDirectory, cancellationToken);
        return new StreamDictTableRecognizationModel(RootDirectory, Utils.LoadDicts(DictName));
    }

    /// <summary>
    /// Gets the English mobile version 2 SLANet table recognition model.
    /// </summary>
    public static OnlineTableRecognitionModel EnglishMobileV2_SLANET => new(
        "en_ppstructure_mobile_v2.0_SLANet",
        new Uri("https://paddleocr.bj.bcebos.com/ppstructure/models/slanet/en_ppstructure_mobile_v2.0_SLANet_infer.tar"),
        "table_structure_dict.txt");

    /// <summary>
    /// Gets the Chinese mobile version 2 SLANet table recognition model.
    /// </summary>
    public static OnlineTableRecognitionModel ChineseMobileV2_SLANET => new(
        "ch_ppstructure_mobile_v2.0_SLANet",
        new Uri("https://paddleocr.bj.bcebos.com/ppstructure/models/slanet/ch_ppstructure_mobile_v2.0_SLANet_infer.tar"),
        "table_structure_dict_ch.txt");

    /// <summary>
    /// Gets an array of all online table recognition models.
    /// </summary>
    public static OnlineTableRecognitionModel[] All => new[]
    {
        EnglishMobileV2_SLANET,
        ChineseMobileV2_SLANET,
    };
}

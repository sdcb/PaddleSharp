using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.LocalV3.Details;

namespace Sdcb.PaddleOCR.Models.LocalV3;

/// <summary>
/// Represents a local table recognition model.
/// </summary>
public class LocalTableRecognitionModel : TableRecognitionModel
{
    /// <summary>
    /// Gets the name of the model.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Constructor for creating a local table recognition model.
    /// </summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="dictName">The name of the dictionary.</param>
    public LocalTableRecognitionModel(string name, string dictName) : base(Utils.LoadDicts(dictName))
    {
        Name = name;
    }

    /// <inheritdoc/>
    public override PaddleConfig CreateConfig()
    {
        PaddleConfig config = Utils.LoadLocalModel(Name);
        ConfigPostProcess(config);
        return config;
    }

    /// <summary>
    /// Represents an English mobile V2 SLANET.
    /// </summary>
    public static LocalTableRecognitionModel EnglishMobileV2_SLANET => new("en_ppstructure_mobile_v2.0_SLANet", "table_structure_dict.txt");

    /// <summary>
    /// Represents a Chinese mobile V2 SLANET.
    /// </summary>
    public static LocalTableRecognitionModel ChineseMobileV2_SLANET => new("ch_ppstructure_mobile_v2.0_SLANet", "table_structure_dict_ch.txt");

    /// <summary>
    /// Gets all the local table recognition models.
    /// </summary>
    public static LocalTableRecognitionModel[] All => new[]
    {
        EnglishMobileV2_SLANET,
        ChineseMobileV2_SLANET,
    };
}

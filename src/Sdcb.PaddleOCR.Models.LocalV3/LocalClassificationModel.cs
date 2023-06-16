using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.LocalV3.Details;

namespace Sdcb.PaddleOCR.Models.LocalV3;

public class LocalClassificationModel : ClassificationModel
{
    public string Name { get; }
    public ModelVersion Version { get; }

    public override OcrShape Shape => DefaultShape;

    public LocalClassificationModel(string name, ModelVersion version)
    {
        Name = name;
        Version = version;
    }

    public override PaddleConfig CreateConfig() => Utils.LoadLocalModel(Name);

    /// <summary>
    /// Original model for text angle classification
    /// (Size: 1.38M)
    /// </summary>
    public static LocalClassificationModel ChineseMobileV2 => new("ch_ppocr_mobile_v2.0_cls", ModelVersion.V2);

    public static LocalClassificationModel[] All => new[]
    {
        ChineseMobileV2,
    };
}

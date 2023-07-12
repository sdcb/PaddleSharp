using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.LocalV3.Details;
using System.Collections.Generic;

namespace Sdcb.PaddleOCR.Models.LocalV3;

/// <summary>
/// Provides a local implementation of PaddleOCR model with the ability to recognize various languages such as Chinese, English, Korean, Japanese, Telugu and Devanagari
/// </summary>
public class LocalRecognizationModel : RecognizationModel
{
    /// <summary>
    /// The name of the model.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// A list of labels for recognition.
    /// </summary>
    public IReadOnlyList<string> Labels { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalRecognizationModel"/> class.
    /// </summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="dictName">The dictionary name of the model.</param>
    /// <param name="version">The version of the model.</param>
    public LocalRecognizationModel(string name, string dictName, ModelVersion version) : base(version)
    {
        Name = name;
        Labels = Utils.LoadDicts(dictName);
    }

    /// <summary>
    /// Gets label by index for Labels.
    /// </summary>
    /// <param name="i">The index of the label to get</param>
    /// <returns>The specified label</returns>
    public override string GetLabelByIndex(int i) => GetLabelByIndex(i, Labels);

    /// <summary>
    /// Creates and returns a PaddleConfig instance based on the Name property.
    /// </summary>
    /// <returns>A new instance of PaddleConfig</returns>
    public override PaddleConfig CreateConfig()
    {
        return Utils.LoadLocalModel(Name);
    }

    /// <summary>
    /// [New] Original lightweight model, supporting Chinese, English, multilingual text recognition
    /// (Size: 12.4M)
    /// </summary>
    public static LocalRecognizationModel ChineseV3 => new("ch_PP-OCRv3_rec", "ppocr_keys_v1.txt", ModelVersion.V3);

    /// <summary>
    /// [New] Original lightweight model, supporting English, multilingual text recognition
    /// (Size: 9.6M)
    /// </summary>
    public static LocalRecognizationModel EnglishV3 => new("en_PP-OCRv3_rec", "en_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Korean recognition
    /// (Size: 11M)
    /// </summary>
    public static LocalRecognizationModel KoreanV3 => new("korean_PP-OCRv3_rec", "korean_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Japanese recognition
    /// (Size: 11M)
    /// </summary>
    public static LocalRecognizationModel JapanV3 => new("japan_PP-OCRv3_rec", "japan_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for TraditionalChinese recognition
    /// (Size: 12M)
    /// </summary>
    public static LocalRecognizationModel TraditionalChineseV3 => new("chinese_cht_PP-OCRv3_rec", "chinese_cht_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Telugu recognition
    /// (Size: 9.6M)
    /// </summary>
    public static LocalRecognizationModel TeluguV3 => new("te_PP-OCRv3_rec", "te_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Kannada recognition
    /// (Size: 9.9M)
    /// </summary>
    public static LocalRecognizationModel KannadaV3 => new("ka_PP-OCRv3_rec", "ka_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for Tamil recognition
    /// (Size: 9.6M)
    /// </summary>
    public static LocalRecognizationModel TamilV3 => new("ta_PP-OCRv3_rec", "ta_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for latin recognition
    /// (Size: 9.7M)
    /// </summary>
    public static LocalRecognizationModel LatinV3 => new("latin_PP-OCRv3_rec", "latin_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for arabic recognition
    /// (Size: 9.6M)
    /// </summary>
    public static LocalRecognizationModel ArabicV3 => new("arabic_PP-OCRv3_rec", "arabic_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for cyrillic recognition
    /// (Size: 9.6M)
    /// </summary>
    public static LocalRecognizationModel CyrillicV3 => new("cyrillic_PP-OCRv3_rec", "cyrillic_dict.txt", ModelVersion.V3);

    /// <summary>
    /// Lightweight model for devanagari recognition
    /// (Size: 9.9M)
    /// </summary>
    public static LocalRecognizationModel DevanagariV3 => new("devanagari_PP-OCRv3_rec", "devanagari_dict.txt", ModelVersion.V3);

    /// <summary>
    /// An array containing all instances of the LocalRecognizationModel
    /// </summary>
    public static LocalRecognizationModel[] All => new[]
    {
        ChineseV3,
        EnglishV3,
        KoreanV3,
        JapanV3,
        TraditionalChineseV3,
        TeluguV3,
        KannadaV3,
        TamilV3,
        LatinV3,
        ArabicV3,
        CyrillicV3,
        DevanagariV3,
    };
}

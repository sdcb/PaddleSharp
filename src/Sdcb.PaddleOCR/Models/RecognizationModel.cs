using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;

namespace Sdcb.PaddleOCR.Models;

public abstract class RecognizationModel
{
    public abstract string GetLabelByIndex(int i);

    public abstract PaddleConfig CreateConfig();

    public abstract OcrShape Shape { get; }

    public static RecognizationModel FromDirectory(string directoryPath, string labelPath, ModelVersion version) => new FileRecognizationModel(directoryPath, labelPath, version);
}

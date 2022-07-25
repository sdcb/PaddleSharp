using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;

namespace Sdcb.PaddleOCR.Models
{
    public abstract class ClassificationModel
    {
        public abstract OcrShape Shape { get; }
        public abstract PaddleConfig CreateConfig();

        public static OcrShape DefaultShape = new(3, 48, 192);

        public static ClassificationModel FromDirectory(string directoryPath) => new FileClassificationModel(directoryPath);
    }
}

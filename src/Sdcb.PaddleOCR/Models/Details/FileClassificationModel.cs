using Sdcb.PaddleInference;

namespace Sdcb.PaddleOCR.Models.Details
{
    public class FileClassificationModel : ClassificationModel
    {
        public string DirectoryPath { get; init; }

        public FileClassificationModel(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public override OcrShape Shape => DefaultShape;

        public override PaddleConfig CreateConfig() => PaddleConfig.FromModelDir(DirectoryPath);
    }
}

using Sdcb.PaddleInference;
using System.IO;

namespace Sdcb.PaddleOCR.Models.Details
{
    public class FileTableRecognizationModel : TableRecognitionModel
    {
        public string DirectoryPath { get; }
        public string LabelFilePath { get; }

        public FileTableRecognizationModel(string directoryPath, string labelFilePath) : base(File.ReadAllLines(labelFilePath))
        {
            DirectoryPath = directoryPath;
            LabelFilePath = labelFilePath;
        }

        public override PaddleConfig CreateConfig()
        {
            return PaddleConfig.FromModelDir(DirectoryPath);
        }
    }
}

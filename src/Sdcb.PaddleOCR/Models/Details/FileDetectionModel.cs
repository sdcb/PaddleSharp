using Sdcb.PaddleInference;

namespace Sdcb.PaddleOCR.Models.Details;

public class FileDetectionModel : DetectionModel
{
    public string DirectoryPath { get; init; }

    public FileDetectionModel(string directoryPath)
    {
        DirectoryPath = directoryPath;
    }

    public override PaddleConfig CreateConfig() => PaddleConfig.FromModelDir(DirectoryPath);
}

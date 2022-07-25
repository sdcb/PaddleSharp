using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;

namespace Sdcb.PaddleOCR.Models
{
    public abstract class DetectionModel
    {
        public abstract PaddleConfig CreateConfig();

        public static DetectionModel FromDirectory(string directoryPath) => new FileDetectionModel(directoryPath);
    }
}

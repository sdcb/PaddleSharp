using Sdcb.RotationDetector;
using Sdcb.PaddleInference;

namespace Sdcb.RotationDetector
{
    internal class FileRotationDetectionModel : RotationDetectionModel
    {
        private readonly string _directoryPath;

        public FileRotationDetectionModel(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public override InputShape Shape => DefaultShape;

        public override PaddleConfig CreateConfig()
        {
            return PaddleConfig.FromModelDir(_directoryPath);
        }
    }
}

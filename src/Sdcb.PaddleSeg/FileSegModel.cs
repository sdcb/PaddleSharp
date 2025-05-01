using Sdcb.PaddleInference;

namespace Sdcb.PaddleSeg
{
    public class FileSegModel : SegModel
    {
        /// <summary>
        /// Gets the directory path containing the model files.
        /// </summary>
        public string DirectoryPath { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDetectionModel"/> class with the specified directory path.
        /// </summary>
        /// <param name="directoryPath">The directory path containing the model files.</param>
        /// <param name="version">The version of detection model.</param>
        public FileSegModel(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        /// <summary>
        /// Creates a PaddleConfig object using the model directory path.
        /// </summary>
        /// <returns>A <see cref="PaddleConfig"/> object created using <see cref="DirectoryPath"/>.</returns>
        public override PaddleConfig CreateConfig() => PaddleConfig.FromModelDir(DirectoryPath);
    }
}
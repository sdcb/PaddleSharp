using Sdcb.PaddleInference;
using System.Collections.Generic;

namespace Sdcb.PaddleOCR.Models.Online.Details
{
    internal class StreamDictTableRecognizationModel : TableRecognitionModel
    {
        public string DirectoryPath { get; }

        public StreamDictTableRecognizationModel(string directoryPath, IReadOnlyList<string> dict) : base(dict)
        {
            DirectoryPath = directoryPath;
        }

        public override PaddleConfig CreateConfig()
        {
            PaddleConfig config = PaddleConfig.FromModelDir(DirectoryPath);
            ConfigPostProcess(config);
            return config;
        }
    }
}

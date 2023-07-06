using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Details;
using System.Collections.Generic;

namespace Sdcb.PaddleOCR.Models.Online.Details;

internal class StreamDictFileRecognizationModel : RecognizationModel
{
    private readonly IReadOnlyList<string> _labels;

    public string DirectoryPath { get; }

    public StreamDictFileRecognizationModel(string directoryPath, IReadOnlyList<string> dict, ModelVersion version) : base(version)
    {
        DirectoryPath = directoryPath;
        _labels = dict;
    }

    public override PaddleConfig CreateConfig()
    {
        PaddleConfig config = PaddleConfig.FromModelDir(DirectoryPath);
        ConfigPostProcess(config);
        return config;
    }

    public override string GetLabelByIndex(int i) => GetLabelByIndex(i, _labels);
}

using Sdcb.PaddleOCR.Models.Details;
using Sdcb.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.PaddleInference.Tests;

public class IOInfoTests
{
    private readonly ITestOutputHelper _console;

    public IOInfoTests(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task CanDumpIOInfo()
    {
        FileDetectionModel model = await OnlineDetectionModel.ChineseV4.DownloadAsync();
        using PaddleConfig c = PaddleConfig.FromModelDir(model.DirectoryPath);
        using PaddlePredictor p = c.CreatePredictorNoDelete();

        _console.WriteLine("Input infos:");
        PaddleIOInfo[] inputInfos = p.InputInfos;
        foreach (PaddleIOInfo info in inputInfos)
        {
            _console.WriteLine(info.ToString());
        }

        _console.WriteLine("Output infos:");
        PaddleIOInfo[] outputInfos = p.OutputInfos;
        foreach (PaddleIOInfo info in outputInfos)
        {
            _console.WriteLine(info.ToString());
        }
    }
}

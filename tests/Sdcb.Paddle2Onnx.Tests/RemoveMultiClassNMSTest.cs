using Sdcb.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.Paddle2Onnx.Tests;

public class RemoveMultiClassNMSTest
{
    private readonly ITestOutputHelper _console;

    public RemoveMultiClassNMSTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task NormalOCRDetectionModel_Should_CanExport()
    {
        // Arrange
        await LocalDictOnlineRecognizationModel.ChineseV3.DownloadAsync();
        string dir = Path.Combine(Settings.GlobalModelDirectory, LocalDictOnlineRecognizationModel.ChineseV3.Name);
        string modelFile = Path.Combine(dir, "inference.pdmodel");
        string paramsFile = Path.Combine(dir, "inference.pdiparams");
        byte[] onnxModel = Paddle2OnnxConverter.ConvertToOnnx(modelFile, paramsFile);

        // Act
        byte[] newOnnxModel = Paddle2OnnxConverter.RemoveOnnxMultiClassNMS(onnxModel);

        // Assert
        Assert.NotEmpty(newOnnxModel);
        _console.WriteLine($"onnxModel: {onnxModel.Length} bytes");
        _console.WriteLine($"newOnnxModel: {newOnnxModel.Length} bytes");
    }
}
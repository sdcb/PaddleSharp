using Sdcb.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.Paddle2Onnx.Tests;

[Trait("Category", "LinuxExclude")]
public class DescribeOnnxTest
{
    private readonly ITestOutputHelper _console;

    public DescribeOnnxTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task OnnxModel_CanBe_Described()
    {
        // Arrange
        await LocalDictOnlineRecognizationModel.ChineseV3.DownloadAsync();
        string dir = Path.Combine(Settings.GlobalModelDirectory, LocalDictOnlineRecognizationModel.ChineseV3.Name);
        string modelFile = Path.Combine(dir, "inference.pdmodel");
        string paramsFile = Path.Combine(dir, "inference.pdiparams");
        byte[] onnxModel = Paddle2OnnxConverter.ConvertToOnnx(modelFile, paramsFile);

        // Act
        OnnxModelInfo info = Paddle2OnnxConverter.DescribeOnnxModel(onnxModel);

        // Assert
        Assert.NotNull(info);
        Assert.NotEmpty(info.Inputs);
        Assert.NotEmpty(info.Outputs);
    }
}

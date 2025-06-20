using Sdcb.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.Paddle2Onnx.Tests;

[Trait("Category", "WindowsOnly")]
public class DescribePaddleTest
{
    private readonly ITestOutputHelper _console;

    public DescribePaddleTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task PaddleModel_CanBe_Described()
    {
        // Arrange
        await LocalDictOnlineRecognizationModel.ChineseV3.DownloadAsync();
        string dir = Path.Combine(Settings.GlobalModelDirectory, LocalDictOnlineRecognizationModel.ChineseV3.Name);
        string modelFile = Path.Combine(dir, "inference.pdmodel");
        byte[] modelBuffer = File.ReadAllBytes(modelFile);

        // Act
        PaddleModelInfo info = Paddle2OnnxConverter.DescribePaddleModel(modelBuffer);

        // Assert
        Assert.NotNull(info);
        Assert.NotEmpty(info.InputNames);
        Assert.NotEmpty(info.OutputNames);
        _console.WriteLine($"InputNames: {string.Join(", ", info.InputNames)}");
        _console.WriteLine($"OutputNames: {string.Join(", ", info.OutputNames)}");
    }

    [Fact(Skip = "Paddle2Onnx is calling std::about() when parse failed.")]
    public async Task WrongPaddleModel_CanNotBe_Described()
    {
        // Arrange
        await LocalDictOnlineRecognizationModel.ChineseV3.DownloadAsync();
        string dir = Path.Combine(Settings.GlobalModelDirectory, LocalDictOnlineRecognizationModel.ChineseV3.Name);
        string modelFile = Path.Combine(dir, "inference.pdiparams");
        byte[] modelBuffer = File.ReadAllBytes(modelFile);

        // Act & Assert
        Assert.Throws<Exception>(() => Paddle2OnnxConverter.DescribePaddleModel(modelBuffer));
    }
}

using Sdcb.PaddleOCR.Models.Details;
using Sdcb.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.Paddle2Onnx.Tests;

[Trait("Category", "WindowsOnly")]
public class TestCanConvert
{
#pragma warning disable IDE0052 // 删除未读的私有成员
    private readonly ITestOutputHelper _console;
#pragma warning restore IDE0052 // 删除未读的私有成员

    public TestCanConvert(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task NormalOCRDetectionModel_Should_CanConvert()
    {
        // Arrange
        FileDetectionModel fd = await OnlineDetectionModel.ChineseV3.DownloadAsync();
        string modelFile = Path.Combine(fd.DirectoryPath, "inference.pdmodel");
        string paramsFile = Path.Combine(fd.DirectoryPath, "inference.pdiparams");

        // Act
        bool can = Paddle2OnnxConverter.CanConvert(modelFile, paramsFile);

        // Assert
        Assert.True(can);
    }

    [Fact]
    public async Task NormalOCRDetectionModelBuffer_Should_CanConvert()
    {
        // Arrange
        FileDetectionModel fd = await OnlineDetectionModel.ChineseV3.DownloadAsync();
        string modelFile = Path.Combine(fd.DirectoryPath, "inference.pdmodel");
        string paramsFile = Path.Combine(fd.DirectoryPath, "inference.pdiparams");
        byte[] modelBuffer = File.ReadAllBytes(modelFile);
        byte[] paramsBuffer = File.ReadAllBytes(paramsFile);

        // Act
        bool can = Paddle2OnnxConverter.CanConvert(modelBuffer, paramsBuffer);

        // Assert
        Assert.True(can);
    }

    [Fact]
    public async Task ModelWithCustomOps_Should_CanConvert()
    {
        // Arrange
        FileDetectionModel fd = await OnlineDetectionModel.ChineseV3.DownloadAsync();
        string modelFile = Path.Combine(fd.DirectoryPath, "inference.pdmodel");
        string paramsFile = Path.Combine(fd.DirectoryPath, "inference.pdiparams");

        CustomOp[] ops = new[]
        {
            new CustomOp("fc", "FC"),
            new CustomOp("softmax", "Softmax")
        };

        // Act
        bool can = Paddle2OnnxConverter.CanConvert(modelFile, paramsFile, customOps: ops);

        // Assert
        Assert.True(can);
    }

    [Fact]
    public async Task TensorRtBackend_Should_BeValid()
    {
        // Arrange
        FileDetectionModel fd = await OnlineDetectionModel.ChineseV3.DownloadAsync();
        string modelFile = Path.Combine(fd.DirectoryPath, "inference.pdmodel");
        string paramsFile = Path.Combine(fd.DirectoryPath, "inference.pdiparams");

        // Act
        bool can = Paddle2OnnxConverter.CanConvert(modelFile, paramsFile, deployBackend: "tensorrt");

        // Assert
        Assert.True(can);
    }
}
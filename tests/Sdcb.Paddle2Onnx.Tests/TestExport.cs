using Sdcb.PaddleOCR.Models.Details;
using Sdcb.PaddleOCR.Models.Online;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Sdcb.Paddle2Onnx.Tests;

[Trait("Category", "WindowsOnly")]
public class TestExport
{
    private readonly ITestOutputHelper _console;

    public TestExport(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task NormalOCRDetectionModel_Should_CanExport()
    {
        // Arrange
        FileDetectionModel fd = await OnlineDetectionModel.ChineseV3.DownloadAsync();
        string modelFile = Path.Combine(fd.DirectoryPath, "inference.pdmodel");
        string paramsFile = Path.Combine(fd.DirectoryPath, "inference.pdiparams");

        // Act
        byte[] onnxModel = Paddle2OnnxConverter.ConvertToOnnx(modelFile, paramsFile);

        // Assert
        Assert.NotEmpty(onnxModel);
    }

    [Fact]
    public async Task NormalOCRDetectionModelBuffer_Should_CanExport()
    {
        // Arrange
        FileDetectionModel fd = await OnlineDetectionModel.ChineseV3.DownloadAsync();
        string modelFile = Path.Combine(fd.DirectoryPath, "inference.pdmodel");
        string paramsFile = Path.Combine(fd.DirectoryPath, "inference.pdiparams");
        byte[] modelBuffer = File.ReadAllBytes(modelFile);
        byte[] paramsBuffer = File.ReadAllBytes(paramsFile);

        // Act
        byte[] onnxModel = Paddle2OnnxConverter.ConvertToOnnx(modelBuffer, paramsBuffer);

        // Assert
        Assert.NotEmpty(onnxModel);
    }

    [Fact(
        Skip = "Memory leak test"
        )]
    public async Task ExportMemory_Should_NoLeak()
    {
        // Arrange
        FileDetectionModel fd = await OnlineDetectionModel.ChineseV3.DownloadAsync();
        string modelFile = Path.Combine(fd.DirectoryPath, "inference.pdmodel");
        string paramsFile = Path.Combine(fd.DirectoryPath, "inference.pdiparams");
        byte[] modelBuffer = File.ReadAllBytes(modelFile);
        byte[] paramsBuffer = File.ReadAllBytes(paramsFile);

        // Act
        for (int i = 0; i < 100; ++i)
        {
            byte[] onnxModel = Paddle2OnnxConverter.ConvertToOnnx(modelBuffer, paramsBuffer);
            GC.Collect();
            _console.WriteLine($"{i}, onnx size: {onnxModel.Length}, memory: {Process.GetCurrentProcess().PrivateMemorySize64}");
        }
    }
}
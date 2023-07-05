using OpenCvSharp;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.LocalV3;

namespace Sdcb.PaddleOCR.Tests;

public class TableTest
{
    [Theory]
    [InlineData("en_ppstructure_mobile_v2.0_SLANet")]
    [InlineData("ch_ppstructure_mobile_v2.0_SLANet")]
    public void LocalV3TableTest(string modelName)
    {
        using PaddleOcrTableRecognizer tableRec = new(LocalTableRecognitionModel.All.Single(x => x.Name == modelName));
        using Mat src = Cv2.ImRead("samples/table.jpg");
        TableDetectionResult result = tableRec.Run(src);
        Assert.NotNull(result);
        Assert.NotEmpty(result.StructureBoxes);
        Assert.NotEmpty(result.HtmlTags);
        Assert.True(result.Score > 0.9f);
        //using Mat visualized = result.Visualize(src, Scalar.LightGreen);
        //Cv2.ImWrite("table-visualized.jpg", visualized);
    }

    [Theory]
    [InlineData("en_ppstructure_mobile_v2.0_SLANet", "<table><thead><tr><td>Methods</td><td>R</td><td>P</td><td>F</td><td>FPS</td></tr></thead><tbody><tr>")]
    [InlineData("ch_ppstructure_mobile_v2.0_SLANet", "<table><tbody><tr><td>Methods</td><td>R</td><td>P</td><td>F</td><td>FPS</td></tr><tr><td>SegLink[26]")]
    public async void LocalV3TableRebuild(string modelName, string expectedHtmlStart)
    {
        using Mat src = Cv2.ImRead("samples/table.jpg");
        Task<TableDetectionResult> tableResultTask = Task.Run(() =>
        {
            using PaddleOcrTableRecognizer tableRec = new(LocalTableRecognitionModel.All.Single(x => x.Name == modelName));
            return tableRec.Run(src);
        });

        Task<PaddleOcrResult> ocrResultTask = Task.Run(() =>
        {
            using PaddleOcrAll all = new(LocalFullModels.ChineseV3);
            all.Detector.UnclipRatio = 1.2f;
            return all.Run(src);
        });

        TableDetectionResult tableResult = await tableResultTask;
        PaddleOcrResult ocrResult = await ocrResultTask;
        //tableResult.Visualize(src, Scalar.LightGreen).ImWrite(modelName + ".jpg");

        string html = tableResult.RebuildTable(ocrResult);
        Assert.StartsWith(expectedHtmlStart, html);
    }

    [Theory]
    [InlineData("en_ppstructure_mobile_v2.0_SLANet", "<table><thead><tr><td>Methods</td><td>R</td><td>P</td><td>F</td><td>FPS</td></tr></thead><tbody><tr>")]
    [InlineData("ch_ppstructure_mobile_v2.0_SLANet", "<table><tbody><tr><td>Methods</td><td>R</td><td>P</td><td>F</td><td>FPS</td></tr><tr><td>SegLink[26]")]
    public async Task OnlineV3TableRebuild(string modelName, string expectedHtmlStart)
    {
        using Mat src = Cv2.ImRead("samples/table.jpg");

        Task<PaddleOcrResult> ocrResultTask = Task.Run(() =>
        {
            using PaddleOcrAll all = new(LocalFullModels.ChineseV3);
            all.Detector.UnclipRatio = 1.2f;
            return all.Run(src);
        });

        Task<TableDetectionResult> tableResultTask = Task.Run(async () =>
        {
            OnlineTableRecognitionModel tableOnlineModel = OnlineTableRecognitionModel.All.Single(x => x.Name == modelName);
            TableRecognitionModel tableModel = await tableOnlineModel.DownloadAsync();
            using PaddleOcrTableRecognizer tableRec = new(tableModel);
            return tableRec.Run(src);
        });

        PaddleOcrResult ocrResult = await ocrResultTask;
        TableDetectionResult tableResult = await tableResultTask;

        string html = tableResult.RebuildTable(ocrResult);
        Assert.StartsWith(expectedHtmlStart, html);
    }

    [Theory]
    [InlineData("en_ppstructure_mobile_v2.0_SLANet")]
    [InlineData("ch_ppstructure_mobile_v2.0_SLANet")]
    public async Task OnlineV3TableTest(string modelName)
    {
        OnlineTableRecognitionModel tableOnlineModel = OnlineTableRecognitionModel.All.Single(x => x.Name == modelName);
        TableRecognitionModel tableModel = await tableOnlineModel.DownloadAsync();
        using PaddleOcrTableRecognizer tableRec = new(tableModel);
        using Mat src = Cv2.ImRead("samples/table.jpg");
        TableDetectionResult result = tableRec.Run(src);
        Assert.NotNull(result);
        Assert.NotEmpty(result.StructureBoxes);
        Assert.NotEmpty(result.HtmlTags);
        Assert.True(result.Score > 0.9f);
        //using Mat visualized = result.Visualize(src, Scalar.LightGreen);
        //Cv2.ImWrite("table-visualized.jpg", visualized);
    }
}

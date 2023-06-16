using OpenCvSharp;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.PaddleOCR.Tests;

public class OnlineModelsTest
{
    private readonly ITestOutputHelper _console;

    public OnlineModelsTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task FastCheckOCR()
    {
        FullOcrModel model = await OnlineFullModels.EnglishV3.DownloadAsync();

        byte[] sampleImageData;
        string sampleImageUrl = @"https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png";
        using (HttpClient http = new HttpClient())
        {
            _console.WriteLine("Download sample image from: " + sampleImageUrl);
            sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
        }

        using (PaddleOcrAll all = new PaddleOcrAll(model)
        {
            AllowRotateDetection = true, /* 允许识别有角度的文字 */
            Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
        })
        {
            // Load local file by following code:
            // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
            using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
            {
                PaddleOcrResult result = all.Run(src);
                _console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    _console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                }
            }
        }
    }
}
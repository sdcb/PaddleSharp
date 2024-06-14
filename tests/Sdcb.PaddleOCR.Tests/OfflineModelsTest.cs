using OpenCvSharp;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Local;
using Xunit.Abstractions;

namespace Sdcb.PaddleOCR.Tests;

public class OfflineModelsTest
{
    private readonly ITestOutputHelper _console;

    public OfflineModelsTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public void FastCheckOCREnglishV3()
    {
        FullOcrModel model = LocalFullModels.EnglishV3;
        FastCheck(model);
    }

    [Fact]
    public void FastCheckOCREnglishV4()
    {
        FullOcrModel model = LocalFullModels.EnglishV4;
        FastCheck(model);
    }

    [Fact]
    public void FastCheckOCRChineseV4()
    {
        FullOcrModel model = LocalFullModels.ChineseV4;
        FastCheck(model);
    }

    private void FastCheck(FullOcrModel model)
    {
        // from: https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png
        byte[] sampleImageData = File.ReadAllBytes(@"./samples/vsext.png");

        using (PaddleOcrAll all = new(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = false,
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

    [Fact]
    public async Task QueuedOCR()
    {
        FullOcrModel model = LocalFullModels.EnglishV3;

        // from: https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png
        byte[] sampleImageData = File.ReadAllBytes(@"./samples/vsext.png");

        using QueuedPaddleOcrAll all = new(() => new PaddleOcrAll(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = false,
        }, consumerCount: 1, boundedCapacity: 64);

        {
            // Load local file by following code:
            // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
            using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
            {
                PaddleOcrResult result = await all.Run(src);
                _console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    _console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                }
            }
        }
    }

    [Fact(Skip = "Too slow")]
    public void TwoThreadsInferAtSameTime()
    {
        FullOcrModel model = LocalFullModels.EnglishV3;

        // from: https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png
        byte[] sampleImageData = File.ReadAllBytes(@"./samples/vsext.png");

        using PaddleOcrAll all = new(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = false,
        };

        // Load local file by following code:
        // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
        using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
        {
            int threadCount = 2;
            Thread[] threads = new Thread[threadCount];
            PaddleOcrResult[] results = new PaddleOcrResult[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                int index = i;
                threads[i] = new(() =>
                {
                    results[index] = all.Run(src);
                });
                threads[i].Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            for (int i = 0; i < threadCount; ++i)
            {
                _console.WriteLine($"Detected all texts in thread {i}: \n" + results[i].Text);
            }
        }
    }
}
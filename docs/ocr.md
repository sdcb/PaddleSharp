# Usage

## How to integrate Sdcb.PaddleOCR to ASP.NET Core:

Please refer to this demo website, it contains a tutorial: [https://github.com/sdcb/paddlesharp-ocr-aspnetcore-demo](https://github.com/sdcb/paddlesharp-ocr-aspnetcore-demo)

In your service builder code, register a QueuedPaddleOcrAll Singleton:
```csharp
builder.Services.AddSingleton(s =>
{
    Action<PaddleConfig> device = builder.Configuration["PaddleDevice"] == "GPU" ? PaddleDevice.Gpu() : PaddleDevice.Mkldnn();
    return new QueuedPaddleOcrAll(() => new PaddleOcrAll(LocalFullModels.ChineseV3, device)
    {
        Enable180Classification = true,
        AllowRotateDetection = true,
    }, consumerCount: 1);
});
```

In your controller, use the registered `QueuedPaddleOcrAll` singleton:
```csharp
public class OcrController : Controller
{
    private readonly QueuedPaddleOcrAll _ocr;

    public OcrController(QueuedPaddleOcrAll ocr) { _ocr = ocr; }

    [Route("ocr")]
    public async Task<OcrResponse> Ocr(IFormFile file)
    {
        using MemoryStream ms = new();
        using Stream stream = file.OpenReadStream();
        stream.CopyTo(ms);
        using Mat src = Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
        double scale = 1;
        using Mat scaled = src.Resize(Size.Zero, scale, scale);

        Stopwatch sw = Stopwatch.StartNew();
        string textResult = (await _ocr.Run(scaled)).Text;
        sw.Stop();

        return new OcrResponse(textResult, sw.ElapsedMilliseconds);
    }
}
```

## How to migrate previous old version to latest 2.6.0.1?
![image](https://user-images.githubusercontent.com/1317141/206610787-4d31057f-9d7f-4235-a2c4-433322e21bb6.png)

## Windows(Local model): Detection and Recognition(All)
1. Install NuGet Packages:
   ```
   Sdcb.PaddleInference
   Sdcb.PaddleOCR
   Sdcb.PaddleOCR.Models.LocalV3
   Sdcb.PaddleInference.runtime.win64.mkl
   OpenCvSharp4.runtime.win
   ```

2. Using following C# code to get result:
   ```csharp
   FullOcrModel model = LocalFullModels.ChineseV3;
   
   byte[] sampleImageData;
   string sampleImageUrl = @"https://www.tp-link.com.cn/content/images2017/gallery/4288_1920.jpg";
   using (HttpClient http = new HttpClient())
   {
       Console.WriteLine("Download sample image from: " + sampleImageUrl);
       sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
   }
   
   using (PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
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
           Console.WriteLine("Detected all texts: \n" + result.Text);
           foreach (PaddleOcrResultRegion region in result.Regions)
           {
               Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
           }
       }
   }
   ```

## Windows(Online model): Detection and Recognition(All)
1. Install NuGet Packages:
   ```
   Sdcb.PaddleInference
   Sdcb.PaddleOCR
   Sdcb.PaddleOCR.Models.Online
   Sdcb.PaddleInference.runtime.win64.mkl
   OpenCvSharp4.runtime.win
   ```

2. Using following C# code to get result:
   ```csharp
   FullOcrModel model = await OnlineFullModels.EnglishV3.DownloadAsync();
   
   byte[] sampleImageData;
   string sampleImageUrl = @"https://www.tp-link.com.cn/content/images2017/gallery/4288_1920.jpg";
   using (HttpClient http = new HttpClient())
   {
       Console.WriteLine("Download sample image from: " + sampleImageUrl);
       sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
   }
   
   using (PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
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
           Console.WriteLine("Detected all texts: \n" + result.Text);
           foreach (PaddleOcrResultRegion region in result.Regions)
           {
               Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
           }
       }
   }
   ```

## Linux(Ubuntu 20.04): Detection and Recognition(All)
1. Use `sdflysha/sdflysha/dotnet6-paddle:2.3.0-ubuntu20` to replace `mcr.microsoft.com/dotnet/aspnet:6.0` in `Dockerfile` as docker base image.

The build steps for `sdflysha/dotnet6-paddle:2.3.0-ubuntu20` was described [here](./build/docker/dotnet6-paddle/Dockerfile).

2. Install NuGet Packages:
```ps
dotnet add package Sdcb.PaddleOCR.Models.LocalV3
```

Please aware in `Linux`, the native binding library is not required, instead, you should compile your own `OpenCV`/`PaddleInference` library, or just use the `Docker` image.

3. write following C# code to get result(also can be exactly the same as windows):
```csharp
FullOcrModel model = LocalFullModels.ChineseV3;
using (PaddleOcrAll all = new PaddleOcrAll(model, PaddleDevice.Mkldnn()))
// Load in-memory data by following code:
// using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
using (Mat src = Cv2.ImRead(@"/app/test.jpg"))
{
    Console.WriteLine(all.Run(src).Text);
}
```

## Detection Only
```csharp
// Install following packages:
// Sdcb.PaddleInference
// Sdcb.PaddleOCR
// Sdcb.PaddleOCR.Models.LocalV3
// Sdcb.PaddleInference.runtime.win64.mkl (required in Windows, linux using docker)
// OpenCvSharp4.runtime.win (required in Windows, linux using docker)
byte[] sampleImageData;
string sampleImageUrl = @"https://www.tp-link.com.cn/content/images2017/gallery/4288_1920.jpg";
using (HttpClient http = new HttpClient())
{
    Console.WriteLine("Download sample image from: " + sampleImageUrl);
    sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
}

using (PaddleOcrDetector detector = new PaddleOcrDetector(LocalDetectionModel.ChineseV3, PaddleDevice.Mkldnn()))
using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
{
    RotatedRect[] rects = detector.Run(src);
    using (Mat visualized = PaddleOcrDetector.Visualize(src, rects, Scalar.Red, thickness: 2))
    {
        string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "output.jpg");
        Console.WriteLine("OutputFile: " + outputFile);
        visualized.ImWrite(outputFile);
    }
}
```

# Language supports

Please refer to https://github.com/PaddlePaddle/PaddleOCR/blob/release/2.5/doc/doc_en/models_list_en.md to check language support models.

Just replace the `.ChineseV3` in demo code with your speicific language, then you can use the language.

# Paddle Devices

* Mkldnn - `PaddleDevice.Mkldnn()`
  
  Generally fast

* Openblas - `PaddleDevice.Openblas()`

  Much slower, but binary file smaller and consume lesser memory

* Gpu - `PaddleDevice.Gpu()`

  Much faster but relies on NVIDIA GPU and CUDA

  If you wants to use GPU, you should refer to FAQ `How to enable GPU?` section, CUDA/cuDNN/TensorRT need to be installed manually.

* TensorRT - `PaddleDevice.Gpu().And(PaddleDevice.TensorRt(dynamicMapCacheKey))`

  Even faster than raw Gpu but need install TensorRT environment.

# Technical details

There is 3 steps to do OCR:
1. Detection - Detect text's position, angle and area (`PaddleOCRDetector`)
2. Classification - Determin whether text should rotate 180 degreee.
3. Recognization - Recognize the area into text

# Optimize parameters and performance hints
## PaddleConfig.MkldnnCacheCapacity
Default value: `1`

This value has a positive correlation to the peak of memory usage that used by `mkldnn` and a negative correlation to the performance when providing different images.

To figure out each value corresponding to the peak memory usage, you should run the detection for various images(using the same image will not increase memory usage) continuously till the memory usage get stable within a variation of 1GB.

For more details please check the [pr #46](https://github.com/sdcb/PaddleSharp/pull/46) that decreases the default value and the [Paddle](https://github.com/PaddlePaddle/docs/blob/63362b7443c77a324f58a045bcc8d03bb59637fa/docs/design/mkldnn/caching/caching.md) document for `MkldnnCacheCapacity`.

## PaddleOcrAll.Enable180Classification
Default value: `false`

This directly effect the step 2, set to `false` can skip this step, which will unable to detect text from right to left(which should be acceptable because most text direction is from left to right).

Close this option can make the full process about  `~10%` faster.


## PaddleOcrAll.AllowRotateDetection
Default value: `true`

This allows detect any rotated texts. If your subject is 0 degree text (like scaned table or screenshot), you can set this parameter to `false`, which will improve OCR accurancy and little bit performance.


## PaddleOcrAll.Detector.MaxSize
Default value: `1536`

This effect the the max size of step #1, lower this value can improve performance and reduce memory usage, but will also lower the accurancy.

You can also set this value to `null`, in that case, images will not scale-down to detect, performance will drop and memory will high, but should able to get better accurancy.


## How can I improve performance?
Please review the `Technical details` section and read the `Optimize parameters and performance hints` section, or UseGpu.

# PaddleSharp

💗.NET Wrapper for `PaddleInference` C API, include `PaddleOCR`, support **Windows**(x64) and **Linux**(Ubuntu-20.04 x64).

## NuGet Packages/Docker Images

| NuGet Package                          | Version                                                                                                                                                  | Description                                                |
| -------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| Sdcb.PaddleInference                   | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.svg)](https://nuget.org/packages/Sdcb.PaddleInference)                                     | Paddle Inference C API .NET binding                        |
| Sdcb.PaddleOCR                         | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.svg)](https://nuget.org/packages/Sdcb.PaddleOCR)                                                 | PaddleOCR library(based on Sdcb.PaddleInference)           |
| Sdcb.PaddleOCR.KnownModels             | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.KnownModels.svg)](https://nuget.org/packages/Sdcb.PaddleOCR.KnownModels)                         | Helper to download PaddleOCR models                        |
| Sdcb.PaddleInference.runtime.win64.mkl | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.mkl.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.mkl) | Paddle Inference C API Windows x64(mkl-dnn) Native binding |

**Note**: Linux does not need a native binding `NuGet` package like windows(`Sdcb.PaddleInference.runtime.win64.mkl`), instead, you can/should based from a [Dockerfile](https://hub.docker.com/r/sdflysha/ubuntu20-dotnet6-paddleocr2.2.1) to development:

| Docker Images                               | Version                                                                                | Description                                                                  |
| ------------------------------------------- | -------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- |
| sdflysha/ubuntu20-dotnet6-paddleocr2.2.1    | [![Docker](https://img.shields.io/docker/v/sdflysha/ubuntu20-dotnet6-paddleocr2.2.1)](https://hub.docker.com/r/sdflysha/ubuntu20-dotnet6-paddleocr2.2.1)    | PaddleOCR 2.2.1, OpenCV 4.5.3, based on official Ubuntu 20.04 .NET 6 Runtime |
| sdflysha/ubuntu20-dotnet6sdk-paddleocr2.2.1 | [![Docker](https://img.shields.io/docker/v/sdflysha/ubuntu20-dotnet6sdk-paddleocr2.2.1)](https://hub.docker.com/r/sdflysha/ubuntu20-dotnet6sdk-paddleocr2.2.1) | PaddleOCR 2.2.1, OpenCV 4.5.3, based on official Ubuntu 20.04 .NET 6 SDK     |

# Usage

## Windows: Detection and Recognition(All)
1. Pre-condition

Please ensure the [latest Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe) was installed in `Windows`(typically it should automatically installed if you have `Visual Studio` installed)
Otherwise, it will failed with following error(Windows only):
```
DllNotFoundException: Unable to load DLL 'paddle_inference_c' or one of its dependencies (0x8007007E)
```

2. Install NuGet Packages:
```ps
dotnet add package Sdcb.PaddleInference
dotnet add package Sdcb.PaddleInference.runtime.win64.mkl
dotnet add package Sdcb.PaddleOCR
dotnet add package Sdcb.PaddleOCR.KnownModels
dotnet add package OpenCvSharp4
dotnet add package OpenCvSharp4.runtime.win
```

3. Using following C# code to get result:
```csharp
OCRModel model = KnownOCRModel.PPOcrV2;
await model.EnsureAll();

byte[] sampleImageData;
string sampleImageUrl = @"https://www.tp-link.com.cn/content/images/detail/2164/TL-XDR5450易展Turbo版-3840px_03.jpg";
using (HttpClient http = new HttpClient())
{
    Console.WriteLine("Download sample image from: " + sampleImageUrl);
    sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
}

using (PaddleOcrAll all = new PaddleOcrAll(model.RootDirectory, model.KeyPath))
{
    // Load local file by following code:
    // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
    using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
    {
        PaddleOcrResult result = all.Run(src);
        Console.WriteLine("Detected all texts: \n" + result.Text);
        foreach (PaddleOcrResultRegion region in result.Regions)
        {
            Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize: {region.Rect.Size}, Angle: {region.Rect.Angle}");
        }
    }
}
```

## Linux(Ubuntu 20.04): Detection and Recognition(All)
1. Use `sdflysha/ubuntu20-dotnet6-paddleocr2.2.1:20211223` to replace `mcr.microsoft.com/dotnet/aspnet:6.0` in `Dockerfile` as docker base image.

The build steps for `ubuntu20-dotnet6-paddleocr` was described [here](./build/docker/ubuntu20-dotnet6-paddleocr2.2.1/Dockerfile).

And also, we also provided another dotnet6-sdk `Dockerfile`, described [here](./build/docker/ubuntu20-dotnet6sdk-paddleocr2.2.1/Dockerfile).

2. Install NuGet Packages:
```ps
dotnet add package Sdcb.PaddleInference
dotnet add package Sdcb.PaddleOCR
dotnet add package Sdcb.PaddleOCR.KnownModels
dotnet add package OpenCvSharp4
dotnet add package OpenCvSharp4.runtime.ubuntu.18.04-x64
```

Please aware in `Linux`, the native binding library is not required, instead, you should compile your own `OpenCV`/`PaddleInference` library, or just use the `Docker` image.

3. write following C# code to get result(also can be exactly the same as windows):
```csharp
OCRModel model = KnownOCRModel.PPOcrV2;
await model.EnsureAll();
using (PaddleOcrAll all = new PaddleOcrAll(model.RootDirectory, model.KeyPath))
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
// Sdcb.PaddleInference.runtime.win64.mkl (required in windows)
// Sdcb.PaddleOCR
// Sdcb.PaddleOCR.KnownModels
// OpenCvSharp4
// OpenCvSharp4.runtime.win
string inputFile = @"C:\Users\ZhouJie\Pictures\xdr5480.jpg";
OCRModel model = KnownOCRModel.PPOcrV2;
await model.EnsureAll();
using (PaddleOcrDetector detector = new PaddleOcrDetector(model.DetectionDirectory))
using (Mat src = Cv2.ImRead(inputFile))
{
    Rect[] rects = detector.Run(src);
    using (Mat clone = src.Clone())
    {
        foreach (Rect rect in rects)
        {
            clone.Rectangle(rect, Scalar.Red, thickness: 2);
        }
        string outputFile = Path.Combine(Path.GetDirectoryName(inputFile), "output.jpg");
        clone.ImWrite(outputFile);
    }
}

```

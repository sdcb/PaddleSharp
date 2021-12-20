# PaddleSharp

.NET Wrapper for PaddleInference C API, include PaddleOCR

## NuGet Packages

| NuGet Package                            | Version                                                                                                                                                  | Description                                                |
| ---------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| `Sdcb.PaddleInference.runtime.win64.mkl` | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.mkl.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.mkl) | Paddle Inference C API Windows x64(mkl-dnn) Native binding |
| `Sdcb.PaddleInference`                   | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.svg)](https://nuget.org/packages/Sdcb.PaddleInference)                                     | Paddle Inference C API .NET binding                        |
| `Sdcb.PaddleOCR`                         | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.svg)](https://nuget.org/packages/Sdcb.PaddleOCR)                                                 | PaddleOCR library(based on Sdcb.PaddleInference)           |
| `Sdcb.PaddleOCR.KnownModels`             | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.KnownModels.svg)](https://nuget.org/packages/Sdcb.PaddleOCR.KnownModels)                         | Helper to download PaddleOCR models                        |

# Usage

## Detection and Recognition(All)
1. Install NuGet Packages:
```ps
dotnet add package Sdcb.PaddleInference
dotnet add package Sdcb.PaddleInference.runtime.win64.mkl
dotnet add package Sdcb.PaddleOCR
dotnet add package Sdcb.PaddleOCR.KnownModels
dotnet add package OpenCvSharp4
dotnet add package OpenCvSharp4.runtime.win
```

2. write following C# code to get result:
```csharp
await KnownOCRModel.PPOcrV2.EnsureAll();
using (PaddleOcrAll all = new PaddleOcrAll(KnownOCRModel.PPOcrV2.RootDirectory, KnownOCRModel.PPOcrV2.KeyPath))
using (Mat src = Cv2.ImRead(@"C:\Users\ZhouJie\Pictures\xdr5480.jpg"))
{
    Console.WriteLine(all.Run(src).Text);
}
```

## Detection Only
```csharp
// Install following packages:
// Sdcb.PaddleInference
// Sdcb.PaddleInference.runtime.win64.mkl
// Sdcb.PaddleOCR
// Sdcb.PaddleOCR.KnownModels
// OpenCvSharp4
// OpenCvSharp4.runtime.win
string inputFile = @"C:\Users\ZhouJie\Pictures\xdr5480.jpg";
await KnownOCRModel.PPOcrV2.EnsureAll(QueryCancelToken);
using (PaddleOcrDetector detector = new PaddleOcrDetector(KnownOCRModel.PPOcrV2.DetectionDirectory))
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

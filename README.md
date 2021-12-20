# PaddleSharp

.NET Wrapper for PaddleInference C API, include PaddleOCR

## NuGet Packages

| NuGet Package                            | Version                                                                                                                                                  | Description                                                |
| ---------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| `Sdcb.PaddleInference.runtime.win64.mkl` | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.mkl.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.mkl) | Paddle Inference C API Windows x64(mkl-dnn) Native binding |

## OCR Usage
1. Install NuGet Packages:
```ps
dotnet add package Sdcb.PaddleInference
dotnet add package Sdcb.PaddleInference.runtime.win64.mkl
dotnet add package Sdcb.PaddleOCR
dotnet add package Sdcb.PaddleOCR.KnownModels
dotnet add OpenCvSharp4
dotnet add OpenCvSharp4.runtime.win
```

2. write following C# code to get result:
```csharp
await KnownOCRModel.PPOcrV2.EnsureAll(QueryCancelToken);
using PaddleOcrAll all = new (KnownOCRModel.PPOcrV2.RootDirectory, KnownOCRModel.PPOcrV2.KeyPath);
using Mat src = Cv2.ImRead(@"C:\Users\ZhouJie\Pictures\xdr5480.jpg");
Console.WriteLine(all.Run(src).Text);
```

# PaddleSharp [![QQ](https://img.shields.io/badge/QQ_Group-579060605-52B6EF?style=social&logo=tencent-qq&logoColor=000&logoWidth=20)](https://jq.qq.com/?_wv=1027&k=K4fBqpyQ)

💗.NET Wrapper for `PaddleInference` C API, include [PaddleOCR](./docs/ocr.md), [PaddleDetection](./docs/detection.md), support **Windows**(x64), NVIDIA GPU and **Linux**(Ubuntu-20.04 x64).

[PaddleOCR](./docs/ocr.md) support 14 OCR languages model download on-demand, allow rotated text angle detection, 180 degree text detection.

[PaddleDetection](./docs/detection.md) support normal detection model and Yolo model.

## NuGet Packages/Docker Images

| NuGet Package                                        | Version                                                                                                                                                                                  | Description                                                                         |
| ---------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------- |
| Sdcb.PaddleInference                                 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.svg)](https://nuget.org/packages/Sdcb.PaddleInference)                                                                     | Paddle Inference C API .NET binding                                                 |
| Sdcb.PaddleOCR                                       | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.svg)](https://nuget.org/packages/Sdcb.PaddleOCR)                                                                                 | PaddleOCR library(based on Sdcb.PaddleInference)                                    |
| Sdcb.PaddleOCR.KnownModels                           | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.KnownModels.svg)](https://nuget.org/packages/Sdcb.PaddleOCR.KnownModels)                                                         | Helper to download PaddleOCR models                                                 |
| Sdcb.PaddleInference.runtime.win64.mkl               | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.mkl.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.mkl)                                 | Paddle Inference C API Windows x64(mkl-dnn) Native binding                          |
| Sdcb.PaddleInference.runtime.win64.cuda11_cudnn8_tr7 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda11_cudnn8_tr7.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda11_cudnn8_tr7.mkl) | Paddle Inference C API Windows x64(GPU CUDA 11/cuDNN 8.0/TensorRT 7) Native binding |
| Sdcb.PaddleDetection                                 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleDetection.svg)](https://nuget.org/packages/Sdcb.PaddleDetection)                                                                     | PaddleDetection library(based on Sdcb.PaddleInference)                              |

**Note**: Linux does not need a native binding `NuGet` package like windows(`Sdcb.PaddleInference.runtime.win64.mkl`), instead, you can/should based from a [Dockerfile](https://hub.docker.com/r/sdflysha/ubuntu20-dotnet6-paddleocr2.2.1) to development:

| Docker Images                               | Version                                                                                                                                                        | Description                                                                        |
| ------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| sdflysha/ubuntu20-dotnet6-paddleocr2.2.1    | [![Docker](https://img.shields.io/docker/v/sdflysha/ubuntu20-dotnet6-paddleocr2.2.1)](https://hub.docker.com/r/sdflysha/ubuntu20-dotnet6-paddleocr2.2.1)       | PaddleInference 2.2.1, OpenCV 4.5.3, based on official Ubuntu 20.04 .NET 6 Runtime |
| sdflysha/ubuntu20-dotnet6sdk-paddleocr2.2.1 | [![Docker](https://img.shields.io/docker/v/sdflysha/ubuntu20-dotnet6sdk-paddleocr2.2.1)](https://hub.docker.com/r/sdflysha/ubuntu20-dotnet6sdk-paddleocr2.2.1) | PaddleInference 2.2.1, OpenCV 4.5.3, based on official Ubuntu 20.04 .NET 6 SDK     |

# Usage
* PaddleOCR: [PaddleOCR](./docs/ocr.md)
* PaddleDetection: [PaddleDetection](./docs/detection.md)


# Contact
QQ group of C#/.NET computer vision technical communicate(C#/.NET计算机视觉技术交流群): **579060605**
![](./assets/qq.png)

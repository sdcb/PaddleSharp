# PaddleSharp [![main](https://github.com/sdcb/PaddleSharp/actions/workflows/main.yml/badge.svg)](https://github.com/sdcb/PaddleSharp/actions/workflows/main.yml) [![QQ](https://img.shields.io/badge/QQ_Group-579060605-52B6EF?style=social&logo=tencent-qq&logoColor=000&logoWidth=20)](https://jq.qq.com/?_wv=1027&k=K4fBqpyQ)

💗.NET Wrapper for `PaddleInference` C API, include [PaddleOCR](./docs/ocr.md), [PaddleDetection](./docs/detection.md), [Rotation Detector](./docs/rotation-detection.md) support **Windows**(x64), NVIDIA GPU and **Linux**(Ubuntu-20.04 x64).

[PaddleOCR](./docs/ocr.md) support 14 OCR languages model download on-demand, allow rotated text angle detection, 180 degree text detection.

[PaddleDetection](./docs/detection.md) support PPYolo detection model and PicoDet model.

## NuGet Packages/Docker Images

### Infrastructure packages

| NuGet Package                               | Version                                                                                                                                                            | Description                                          |
| ------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ---------------------------------------------------- |
| Sdcb.PaddleInference                        | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.svg)](https://nuget.org/packages/Sdcb.PaddleInference)                                               | Paddle Inference C API .NET binding                  |
| Sdcb.PaddleInference.runtime.win64.openblas | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.openblas.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.openblas) | Paddle Inference native windows-x64-openblas binding |
| Sdcb.PaddleInference.runtime.win64.mkl      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.mkl.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.mkl)           | Paddle Inference native windows-x64-mkldnn binding   |

**Note**: Linux does not need a native binding `NuGet` package like windows(`Sdcb.PaddleInference.runtime.win64.mkl`), instead, you can/should based from a [Dockerfile](https://hub.docker.com/r/sdflysha/dotnet6-focal-paddle2.2.2) to development:

| Docker Images              | Version                                                                                                                      | Description                                                                        |
| -------------------------- | ---------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| sdflysha/dotnet6-paddle    | [![Docker](https://img.shields.io/docker/v/sdflysha/dotnet6-paddle)](https://hub.docker.com/r/sdflysha/dotnet6-focal-paddle) | PaddleInference 2.3.0, OpenCV 4.6.0, based on official Ubuntu 20.04 .NET 6 Runtime |
| sdflysha/dotnet6sdk-paddle | [![Docker](https://img.shields.io/docker/v/sdflysha/dotnet6sdk-paddle)](https://hub.docker.com/r/sdflysha/dotnet6sdk-paddle) | PaddleInference 2.3.0, OpenCV 4.6.0, based on official Ubuntu 20.04 .NET 6 SDK     |

### Paddle Inference GPU package
Since GPU package are too large(>1.5GB), I cannot publish a NuGet package to nuget.org, there is a limitation of 250MB when upload to Github, there is some related issues to this:

* https://github.com/PaddlePaddle/Paddle/issues/43874
* https://github.com/NuGet/Home/issues/11706#issuecomment-1167305006

However you're good to build your own GPU nuget package using `01-build-native.linq`.

There is 2 old version GPU package here, might unable to use(not very large at that time):

<details>

| NuGet Package                                        | Version                                                                                                                                                                                  | Description                                                               |
| ---------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| Sdcb.PaddleInference.runtime.win64.cuda10_cudnn7     | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda10_cudnn7.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda10_cudnn7.mkl)         | Paddle Inference native windows-x64(CUDA 10/cuDNN 7.x) binding            |
| Sdcb.PaddleInference.runtime.win64.cuda11_cudnn8_tr7 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda11_cudnn8_tr7.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda11_cudnn8_tr7.mkl) | Paddle Inference native windows-x64(CUDA 11/cuDNN 8.0/TensorRT 7) binding |  |
</details>

Here is the GPU package that I compiled(not from baidu):
| NuGet Package                                             | Version                                                                                                                                                                                            | Description                                                                                      |
| --------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| Sdcb.PaddleInference.runtime.win64.cuda101_cudnn76_sm61   | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda101_cudnn76_sm61.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda101_cudnn76_sm61)         | Paddle Inference native windows-x64(CUDA 10.1/cuDNN 7.6) SM61 binding                            |
| Sdcb.PaddleInference.runtime.win64.cuda102_cudnn85_pascal | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn85_pascal.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn85_pascal.mkl) | Paddle Inference native windows-x64(CUDA 10.2/cuDNN 8.5) Pascal binding                          |  |
| Sdcb.PaddleInference.runtime.win64.cuda117_cudnn85_ampere | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda117_cudnn85_ampere.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda117_cudnn85_ampere.mkl) | Paddle Inference native windows-x64(CUDA 11.7/cuDNN 8.5) Ampere binding                          |  |
| Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84        | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84.mkl)               | Paddle Inference native windows-x64(CUDA 11.6/cuDNN 8.4) SM61/SM75/SM86 binding with mkldnn/ONNX |  |


### PaddleOCR packages

| NuGet Package                 | Version                                                                                                                                | Description                                             |
| ----------------------------- | -------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| Sdcb.PaddleOCR                | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.svg)](https://nuget.org/packages/Sdcb.PaddleOCR)                               | PaddleOCR library(based on Sdcb.PaddleInference)        |
| Sdcb.PaddleOCR.Models.Online  | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.Models.Online.svg)](https://nuget.org/packages/Sdcb.PaddleOCR.Models.Online)   | Online PaddleOCR models, will download when first using |
| Sdcb.PaddleOCR.Models.LocalV3 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.Models.LocalV3.svg)](https://nuget.org/packages/Sdcb.PaddleOCR.Models.LocalV3) | Full local v3 models, include multiple language(~130MB) |
| Sdcb.PaddleOCR.KnownModels    | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleOCR.KnownModels.svg)](https://nuget.org/packages/Sdcb.PaddleOCR.KnownModels)       | Old online model download helper, *deprecated*          |

### Rotation Detection packages(part of PaddleClass)
| NuGet Package         | Version                                                                                                                | Description                                             |
| --------------------- | ---------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| Sdcb.RotationDetector | [![NuGet](https://img.shields.io/nuget/v/Sdcb.RotationDetector.svg)](https://nuget.org/packages/Sdcb.RotationDetector) | RotationDetector library(based on Sdcb.PaddleInference) |

### PaddleDetection packages
<details>

| NuGet Package        | Version                                                                                                              | Description                                            |
| -------------------- | -------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------ |
| Sdcb.PaddleDetection | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleDetection.svg)](https://nuget.org/packages/Sdcb.PaddleDetection) | PaddleDetection library(based on Sdcb.PaddleInference) |
</details>

# Usage
* PaddleOCR: [PaddleOCR](./docs/ocr.md)
* PaddleDetection: [PaddleDetection](./docs/detection.md)

# FAQ
## Why my code runs good in my windows machine, but DllNotFoundException in other machine:
1. Please ensure the [latest Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe) was installed in `Windows`(typically it should automatically installed if you have `Visual Studio` installed)
Otherwise, it will failed with following error(Windows only):
   ```
   DllNotFoundException: Unable to load DLL 'paddle_inference_c' or one of its dependencies (0x8007007E)
   ```
   
   If it's Unable to load DLL OpenCvSharpExtern.dll or one of its dependencies, then most likely the Media Foundation is not installed in windows server 2012 R2 machine: <img width="830" alt="image" src="https://user-images.githubusercontent.com/1317141/193706883-6a71ea83-65d9-448b-afee-2d25660430a1.png">

2. Many old CPUs does not support AVX instructions, please ensure your CPU supports AVX, or download the x64-noavx-openblas dlls and disable Mkldnn: `PaddleConfig.Defaults.UseMkldnn = false;`

3. If you're using **Win7-x64**, and your CPU do support AVX2, then you might also need to extract following 3 dlls into `C:\Windows\System32` folder to make it run:
   * api-ms-win-core-libraryloader-l1-2-0.dll
   * api-ms-win-core-processtopology-obsolete-l1-1-0.dll
   * API-MS-Win-Eventing-Provider-L1-1-0.dll
   
   You can download these 3 dlls here: [win7-x64-onnxruntime-missing-dlls.zip](https://github.com/sdcb/PaddleSharp/files/10110622/win7-x64-onnxruntime-missing-dlls.zip)

## How to enable GPU?
Enable GPU support can significantly improve the throughput and lower the CPU usage.

Steps to use GPU in windows:
1. (for windows) Install the package: `Sdcb.PaddleInference.runtime.win64.cuda11_cudnn8_tr7` instead of `Sdcb.PaddleInference.runtime.win64.mkl`, **do not** install both.
2. Install CUDA from NVIDIA, and configure environment variables to `PATH` or `LD_LIBRARY_PATH`(linux)
3. Install cuDNN from NVIDIA, and configure environment variables to `PATH` or `LD_LIBRARY_PATH`(linux)
4. Install TensorRT from NVIDIA, and configure environment variables to `PATH` or `LD_LIBRARY_PATH`(linux)

You can refer this blog page for GPU in Windows: [关于PaddleSharp GPU使用 常见问题记录](https://www.cnblogs.com/cuichaohui/p/15766519.html)

If you're using Linux, you need to compile your own OpenCvSharp4 environment following the [docker build scripts](./build/docker/ubuntu20-dotnet6-paddleocr2.2.1/Dockerfile) follow the CUDA/cuDNN/TensorRT configuration tasks.

After these steps completed, you can try specify `PaddleConfig.Defaults.UseGpu = true` in begin of your code and then enjoy😁.

# Thanks & Sponsors
* 深圳-钱文松
* iNeuOS工业互联网操作系统：http://www.ineuos.net

# Contact
QQ group of C#/.NET computer vision technical communicate(C#/.NET计算机视觉技术交流群): **579060605**
![](./assets/qq.png)

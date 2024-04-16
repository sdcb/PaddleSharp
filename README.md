# PaddleSharp 🌟 [![main](https://github.com/sdcb/PaddleSharp/actions/workflows/main.yml/badge.svg)](https://github.com/sdcb/PaddleSharp/actions/workflows/main.yml) [![QQ](https://img.shields.io/badge/QQ_Group-579060605-52B6EF?style=social&logo=tencent-qq&logoColor=000&logoWidth=20)](https://jq.qq.com/?_wv=1027&k=K4fBqpyQ)

**English** | **[简体中文](README_CN.md)**

💗 .NET Wrapper for `PaddleInference` C API, support **Windows**(x64) 💻, NVIDIA Cuda 10.2+ based GPU 🎮 and **Linux**(Ubuntu-22.04 x64) 🐧, currently contained following main components:

* [PaddleOCR 📖](./docs/ocr.md) support 14 OCR languages model download on-demand, allow rotated text angle detection, 180 degree text detection, also support table recognition 📊.
* [PaddleDetection 🎯](./docs/detection.md) support PPYolo detection model and PicoDet model 🏹.
* [RotationDetection 🔄](./docs/rotation-detection.md) use Baidu's official `text_image_orientation_infer` model to detect text picture's rotation angle(`0, 90, 180, 270`).
* [PaddleNLP ChineseSegmenter 📚](./docs/paddlenlp-lac.md) support `PaddleNLP` Lac Chinese segmenter model, supports tagging/customized words.
* [Paddle2Onnx 🔄](./docs/paddle2onnx.md) Allow user export `ONNX` model using `C#`.

## NuGet Packages/Docker Images 📦

### Release notes 📝
Please checkout [this page 📄](https://github.com/sdcb/PaddleSharp/releases).

### Infrastructure packages 🏗️

| NuGet Package 💼      | Version 📌                                                                                                            | Description 📚                         |
| -------------------- | -------------------------------------------------------------------------------------------------------------------- | ------------------------------------- |
| Sdcb.PaddleInference | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.svg)](https://nuget.org/packages/Sdcb.PaddleInference) | Paddle Inference C API .NET binding ⚙️ |

### Native packages 🏗️

| Package                                                         | Version 📌                                                                                                                                                                                                  | Description                                      |
| --------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| Sdcb.PaddleInference.runtime.win64.mkl                          | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.mkl.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.mkl)                                                   | win64+mkldnn                                     |
| Sdcb.PaddleInference.runtime.win64.openblas                     | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.openblas.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.openblas)                                         | win64+openblas                                   |
| Sdcb.PaddleInference.runtime.win64.openblas-noavx               | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.openblas-noavx.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.openblas-noavx)                             | win64+openblas(no AVX, for old CPUs)             |
| Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_tr72_sm61_75 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_tr72_sm61_75.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_tr72_sm61_75) | win64/CUDA 10.2/cuDNN 7.6/TensorRT 7.2/sm61+sm75 |
| Sdcb.PaddleInference.runtime.win64.cuda118_cudnn86_tr85_sm86_89 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda118_cudnn86_tr85_sm86_89.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda118_cudnn86_tr85_sm86_89) | win64/CUDA 11.8/cuDNN 8.6/TensorRT 8.5/sm86+sm89 |

Linux OS packages(preview):

| Package                                                         | Version 📌                                                                                                                                                                                                  | Description                                      |
| --------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| Sdcb.PaddleInference.runtime.linux-loongarch64                  | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.linux-loongarch64.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.linux-loongarch64)                                   | Loongnix GCC 8.2 Loongarch64                     |
| Sdcb.PaddleInference.runtime.linux64.mkl.gcc82                  | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.linux64.mkl.gcc82.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.linux64.mkl.gcc82)                                   | Linux-x64 GCC 8.2(tested in Ubuntu 22.04)        |

Be aware, as the Linux operating system cannot modify the value of `LD_LIBRARY_PATH` at runtime. If dependent dynamic libraries (such as libcommon.so) are loaded before the main dynamic library (such as libpaddle_inference_c.so), and also due to protobuf errors reported: https://github.com/PaddlePaddle/Paddle/issues/62670

Therefore, all NuGet packages for Linux operating systems are in a preview state, and I'm unable to resolve this issue. Currently, if you are using the NuGet package on Linux, you need to manually specify the `LD_LIBRARY_PATH` environment variable before running the program, using the following commands:

* For x64 CPUs:
`export LD_LIBRARY_PATH=/<program directory>/bin/Debug/net8.0/runtimes/linux-x64/native:$LD_LIBRARY_PATH`

* For Loongson 5000 or above CPUs (linux-loongarch64):
`export LD_LIBRARY_PATH=/<program directory>/bin/Debug/net8.0/runtimes/linux-loongarch64/native:$LD_LIBRARY_PATH`

Some of packages already deprecated(Version <= 2.5.0):
| Package                                                         | Version 📌                                                                                                                                                                                                  | Description                                      |
| --------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| Sdcb.PaddleInference.runtime.win64.cuda117_cudnn84_tr84_sm86    | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda117_cudnn84_tr84_sm86.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda117_cudnn84_tr84_sm86)       | win64/CUDA 11.7/cuDNN 8.4/TensorRT 8.4/sm86      |
| Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_sm61_75      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_sm61_75.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_sm61_75)           | win64/CUDA 10.2/cuDNN 7.6/sm61+sm75              |
| Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84_sm86_onnx    | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84_sm86_onnx.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84_sm86_onnx)       | win64/CUDA 11.6/cuDNN 8.4/sm86/onnx              |

Any other packages that starts with `Sdcb.PaddleInference.runtime` might deprecated.

Baidu packages were downloaded from here: https://www.paddlepaddle.org.cn/inference/master/guides/install/download_lib.html#windows

All Windows packages were compiled manually by me.

Baidu official GPU packages are too large(>1.5GB) to publish to nuget.org, there is a limitation of 250MB when upload to Github, there is some related issues to this:

* https://github.com/PaddlePaddle/Paddle/issues/43874 ❌
* https://github.com/NuGet/Home/issues/11706#issuecomment-1167305006 ❌

But You're good to build your own GPU nuget package using `01-build-native.linq` 🛠️.


## Paddle Devices

* Mkldnn - `PaddleDevice.Mkldnn()`
  
  Based on [Mkldnn](https://github.com/oneapi-src/oneDNN), generally fast

* Openblas - `PaddleDevice.Openblas()`

  Based on [openblas](https://www.openblas.net/), slower, but dependencies file smaller and consume lesser memory

* Onnx - `PaddleDevice.Onnx()`

  Based on [onnxruntime](https://github.com/microsoft/onnxruntime), is also pretty fast and consume less memory

* Gpu - `PaddleDevice.Gpu()`

  Much faster but relies on NVIDIA GPU and CUDA

  If you wants to use GPU, you should refer to FAQ `How to enable GPU?` section, CUDA/cuDNN/TensorRT need to be installed manually.

* TensorRT - `PaddleDevice.Gpu().And(PaddleDevice.TensorRt("shape-info.txt"))`

  Even faster than raw Gpu but need install TensorRT environment.

  Please refer to [tensorrt](#tensorrt) section for more details

## FAQ ❓
### Why my code runs good in my windows machine, but DllNotFoundException in other machine: 💻
1. Please ensure the [latest Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe) was installed in `Windows` (typically it should automatically installed if you have `Visual Studio` installed) 🛠️
Otherwise, it will fail with the following error (Windows only):
   ```
   DllNotFoundException: Unable to load DLL 'paddle_inference_c' or one of its dependencies (0x8007007E)
   ```
   
   If it's Unable to load DLL OpenCvSharpExtern.dll or one of its dependencies, then most likely the Media Foundation is not installed in the Windows Server 2012 R2 machine: <img width="830" alt="image" src="https://user-images.githubusercontent.com/1317141/193706883-6a71ea83-65d9-448b-afee-2d25660430a1.png">

2. Many old CPUs do not support AVX instructions, please ensure your CPU supports AVX, or download the x64-noavx-openblas DLLs and disable Mkldnn: `PaddleDevice.Openblas()` 🚀

3. If you're using **Win7-x64**, and your CPU does support AVX2, then you might also need to extract the following 3 DLLs into `C:\Windows\System32` folder to make it run: 💾
   * api-ms-win-core-libraryloader-l1-2-0.dll
   * api-ms-win-core-processtopology-obsolete-l1-1-0.dll
   * API-MS-Win-Eventing-Provider-L1-1-0.dll
   
   You can download these 3 DLLs here: [win7-x64-onnxruntime-missing-dlls.zip](https://github.com/sdcb/PaddleSharp/files/10110622/win7-x64-onnxruntime-missing-dlls.zip) ⬇️

### How to enable GPU? 🎮
Enable GPU support can significantly improve the throughput and lower the CPU usage. 🚀

Steps to use GPU in Windows:
1. (for Windows) Install the package: `Sdcb.PaddleInference.runtime.win64.cuda*` instead of `Sdcb.PaddleInference.runtime.win64.mkl`, **do not** install both. 📦
2. Install CUDA from NVIDIA, and configure environment variables to `PATH` or `LD_LIBRARY_PATH` (Linux) 🔧
3. Install cuDNN from NVIDIA, and configure environment variables to `PATH` or `LD_LIBRARY_PATH` (Linux) 🛠️
4. Install TensorRT from NVIDIA, and configure environment variables to `PATH` or `LD_LIBRARY_PATH` (Linux) ⚙️

You can refer to this blog page for GPU in Windows: [关于PaddleSharp GPU使用 常见问题记录](https://www.cnblogs.com/cuichaohui/p/15766519.html) 📝

If you're using Linux, you need to compile your own OpenCvSharp4 environment following the [docker build scripts](./build/docker/dotnet6sdk-paddle/Dockerfile) and the CUDA/cuDNN/TensorRT configuration tasks. 🐧

After these steps are completed, you can try specifying `PaddleDevice.Gpu()` in the paddle device configuration parameter, then enjoy the performance boost! 🎉

### TensorRT 🚄

To use TensorRT, just specify `PaddleDevice.Gpu().And(PaddleDevice.TensorRt("shape-info.txt"))` instead of `PaddleDevice.Gpu()` to make it work. 💡

Please be aware, this shape info text file `**.txt` is bound to your model. **Different models have different shape info**, so if you're using a complex model like `Sdcb.PaddleOCR`, you should use different shapes for different models like this:
```csharp
using PaddleOcrAll all = new(model,
   PaddleDevice.Gpu().And(PaddleDevice.TensorRt("det.txt")),
   PaddleDevice.Gpu().And(PaddleDevice.TensorRt("cls.txt")),
   PaddleDevice.Gpu().And(PaddleDevice.TensorRt("rec.txt")))
{
   Enable180Classification = true,
   AllowRotateDetection = true,
};
```

In this case:
* `DetectionModel` will use `det.txt` 🔍
* `180DegreeClassificationModel` will use `cls.txt` 🔃
* `RecognitionModel` will use `rec.txt` 🔡

**NOTE 📝:**

The first round of `TensorRT` running will generate a shape info `**.txt` file in this folder: `%AppData%\Sdcb.PaddleInference\TensorRtCache`. It will take around 100 seconds to finish TensorRT cache generation. After that, it should be faster than the general `GPU`. 🚀

In this case, if something strange happens (for example, you mistakenly create the same `shape-info.txt` file for different models), you can delete this folder to generate TensorRT cache again: `%AppData%\Sdcb.PaddleInference\TensorRtCache`. 🗑️

## Thanks & Sponsors 🙏
* 崔亮  https://github.com/cuiliang
* 梁现伟
* 深圳-钱文松
* iNeuOS工业互联网操作系统：http://www.ineuos.net

## Contact 📞
QQ group of C#/.NET computer vision technical communication (C#/.NET计算机视觉技术交流群): **579060605**
![](./assets/qq.png)

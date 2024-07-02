# PaddleSharp 🌟 [![main](https://github.com/sdcb/PaddleSharp/actions/workflows/main.yml/badge.svg)](https://github.com/sdcb/PaddleSharp/actions/workflows/main.yml) [![QQ](https://img.shields.io/badge/QQ_Group-579060605-52B6EF?style=social&logo=tencent-qq&logoColor=000&logoWidth=20)](https://jq.qq.com/?_wv=1027&k=K4fBqpyQ)

**[English](README.md)** | **简体中文**

💗 为 `PaddleInference` C API 提供的 .NET 包装，支持 **Windows**(x64) 💻，基于NVIDIA Cuda 10.2+ 的 GPU 🎮 和 **Linux**(Ubuntu-22.04 x64) 🐧，当前包含以下主要组件：

* [PaddleOCR 📖](./docs/ocr.md) 支持14种OCR语言模型的按需下载，允许旋转文本角度检测，180度文本检测，同时也支持表格识别📊。
* [PaddleDetection 🎯](./docs/detection.md) 支持PPYolo检测模型和PicoDet模型🏹。
* [RotationDetection 🔄](./docs/rotation-detection.md) 使用百度官方的 `text_image_orientation_infer` 模型来检测文本图片的旋转角度(`0, 90, 180, 270`)。
* [PaddleNLP中文分词 📚](./docs/paddlenlp-lac.md) 支持`PaddleNLP`LAC中文分词模型，支持词性标注、自定义词典等功能。
* [Paddle2Onnx 🔄](./docs/paddle2onnx.md) 允许用户使用 `C#` 导出 `ONNX` 模型。

## NuGet 包/Docker 镜像 📦

### 发布说明 📝
请查看 [此页面 📄](https://github.com/sdcb/PaddleSharp/releases)。

### 基础设施包 🏗️

| NuGet 包 💼           | 版本 📌                                                                                                               | 描述 📚                             |
| -------------------- | -------------------------------------------------------------------------------------------------------------------- | ---------------------------------- |
| Sdcb.PaddleInference | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.svg)](https://nuget.org/packages/Sdcb.PaddleInference) | Paddle Inference C API .NET 绑定 ⚙️ |

### 本地动态库包 🏗️

| 包名称                                            | 版本 📌                                                                                                                                                                         | 描述                  |
| ------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------------------- |
| Sdcb.PaddleInference.runtime.win64.mkl            | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.mkl.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.mkl)                       | win64+mkldnn          |
| Sdcb.PaddleInference.runtime.win64.openblas       | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.openblas.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.openblas)             | win64+openblas        |
| Sdcb.PaddleInference.runtime.win64.openblas-noavx | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.openblas-noavx.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.openblas-noavx) | 不含AVX，适用于老CPU  |
| Sdcb.PaddleInference.runtime.win64.cu120-sm86-89  | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cu120-sm86-89.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cu120-sm86-89)   | 适用于英伟达30/40系列 |
| Sdcb.PaddleInference.runtime.win64.cu120-sm80  | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cu120-sm80.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cu120-sm80)   | 适用于NVIDIA A100/A10 |
| Sdcb.PaddleInference.runtime.win64.cu120-sm61-75  | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cu120-sm61-75.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cu120-sm61-75)   | 适用于英伟达10/20系列 |

注意：`cu120`指CUDA 12.0，它编译于CUDA 12.0.1/cuDNN 8.9.7.29/Tensor RT 8.6.1.6的环境。

Linux操作系统NuGet包(预览):

| Package                                        | Version 📌                                                                                                                                                                | Description                               |
| ---------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------- |
| Sdcb.PaddleInference.runtime.linux-loongarch64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.linux-loongarch64.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.linux-loongarch64) | Loongnix GCC 8.2(龙芯3A5000以上)          |
| Sdcb.PaddleInference.runtime.linux64.mkl.gcc82 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.linux64.mkl.gcc82.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.linux64.mkl.gcc82) | Linux-x64 GCC 8.2(tested in Ubuntu 22.04) |

注意，因为Linux操作系统无法在运行时修改`LD_LIBRARY_PATH`的值，如果按先加载被依赖动态库（如libcommon.so）再加载主要动态库（如libpaddle_inference_c.so）的话，而且也由于会报protobuf错误：https://github.com/PaddlePaddle/Paddle/issues/62670

所以所有Linux操作系统的NuGet包都是处于preview的状态，我无法解决这个问题，目前如果您使用Linux的NuGet包的话，程序运行前需要手动使用下列命令指定LD_LIBRARY_PATH环境变量：

* 在x64 CPU中：
```
export LD_LIBRARY_PATH=/<程序目录>/bin/Debug/net8.0/runtimes/linux-x64/native:$LD_LIBRARY_PATH
```

* 龙芯5000或以上的CPU（linux-loongarch64）：
```
export LD_LIBRARY_PATH=/<程序目录>/bin/Debug/net8.0/runtimes/linux-loongarch64/native:$LD_LIBRARY_PATH
```


一些旧的包已经被废弃了(版本 <= 2.5.0):
| 包名称                                                          | 版本 📌                                                                                                                                                                                                     | 描述                                             |
| --------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_tr72_sm61_75 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_tr72_sm61_75.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_tr72_sm61_75) | win64/CUDA 10.2/cuDNN 7.6/TensorRT 7.2/sm61+sm75 |
| Sdcb.PaddleInference.runtime.win64.cuda118_cudnn86_tr85_sm86_89 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda118_cudnn86_tr85_sm86_89.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda118_cudnn86_tr85_sm86_89) | win64/CUDA 11.8/cuDNN 8.6/TensorRT 8.5/sm86+sm89 |
| Sdcb.PaddleInference.runtime.win64.cuda117_cudnn84_tr84_sm86    | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda117_cudnn84_tr84_sm86.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda117_cudnn84_tr84_sm86)       | win64/CUDA 11.7/cuDNN 8.4/TensorRT 8.4/sm86      |
| Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_sm61_75      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_sm61_75.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda102_cudnn76_sm61_75)           | win64/CUDA 10.2/cuDNN 7.6/sm61+sm75              |
| Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84_sm86_onnx    | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84_sm86_onnx.svg)](https://nuget.org/packages/Sdcb.PaddleInference.runtime.win64.cuda116_cudnn84_sm86_onnx)       | win64/CUDA 11.6/cuDNN 8.4/sm86/onnx              |

以 `Sdcb.PaddleInference.runtime` 开头的其他包也可能已经被废弃。

百度的包可以从这里下载：https://www.paddlepaddle.org.cn/inference/master/guides/install/download_lib.html#windows

我的 Sdcb 包是自编译的。

百度官方的 GPU 包过大(>1.5GB)，无法发布到 nuget.org，上传到 Github 时大小限制为 250MB，相关问题如下：

* https://github.com/PaddlePaddle/Paddle/issues/43874 ❌
* https://github.com/NuGet/Home/issues/11706#issuecomment-1167305006 ❌

但是，你可以使用 `01-build-native.linq` 🛠️构建你自己的 GPU nuget 包。

## Paddle 设备

* Mkldnn - `PaddleDevice.Mkldnn()`
  
  基于 [Mkldnn](https://github.com/oneapi-src/oneDNN)，一般来说很快

* Blas - `PaddleDevice.Blas()`

  基于 [openblas](https://www.openblas.net/)，或者mkldnn blas，较慢，但依赖文件更小，内存消耗更少

* Onnx - `PaddleDevice.Onnx()`

  基于 [onnxruntime](https://github.com/microsoft/onnxruntime)，也很快，内存消耗更少

* Gpu - `PaddleDevice.Gpu()`

  更快，但依赖 NVIDIA GPU 和 CUDA

  如果你想使用 GPU，你应该参考 FAQ 中的 `如何启用 GPU?` 部分，CUDA/cuDNN/TensorRT 需要手动安装。

* TensorRT - `PaddleDevice.Gpu().And(PaddleDevice.TensorRt("shape-info.txt"))`

  比原生 Gpu 还要快，但需要安装 TensorRT 环境。

  请参考 [tensorrt](#tensorrt) 部分了解更多细节

## 常见问题 ❓
### 为什么我的代码在我自己的 windows 机器上运行良好，但在其他机器上出现 DllNotFoundException: 💻
1. 请确保 `Windows` 上已安装[最新的 Visual C++ 运行库](https://aka.ms/vs/17/release/vc_redist.x64.exe) (如果你已经安装了 `Visual Studio`，通常这会自动安装) 🛠️
否则，会出现以下错误（仅限 Windows）：
   ```
   DllNotFoundException: 无法加载 DLL 'paddle_inference_c' 或其依赖项之一 (0x8007007E)
   ```
   
   如果遇到无法加载 DLL OpenCvSharpExtern.dll 或其依赖项之一的问题，那么可能是 Windows Server 2012 R2 机器上没有安装 Media Foundation：<img width="830" alt="image" src="https://user-images.githubusercontent.com/1317141/193706883-6a71ea83-65d9-448b-afee-2d25660430a1.png">

2. 许多旧的 CPU 不支持 AVX 指令集，请确保你的 CPU 支持 AVX，或者下载 x64-noavx-openblas DLLs 并禁用 Mkldnn: `PaddleDevice.Openblas()` 🚀

3. 如果你正在使用 **Win7-x64**，并且你的 CPU 支持 AVX2，那么你可能还需要将以下3个 DLLs 提取到 `C:\Windows\System32` 文件夹中才能运行：💾
   * api-ms-win-core-libraryloader-l1-2-0.dll
   * api-ms-win-core-processtopology-obsolete-l1-1-0.dll
   * API-MS-Win-Eventing-Provider-L1-1-0.dll
   
   你可以在这里下载这3个 DLLs：[win7-x64-onnxruntime-missing-dlls.zip](https://github.com/sdcb/PaddleSharp/files/10110622/win7-x64-onnxruntime-missing-dlls.zip) ⬇️

### 如何启用 GPU? 🎮
启用 GPU 支持可以显著提高吞吐量并降低 CPU 使用率。🚀

在 Windows 中使用 GPU 的步骤：
1. （对于 Windows）安装包：`Sdcb.PaddleInference.runtime.win64.cu120*` 代替 `Sdcb.PaddleInference.runtime.win64.mkl`，**不要** 同时安装。📦
2. 从 NVIDIA 安装 CUDA，并将环境变量配置到 `PATH` 或 `LD_LIBRARY_PATH` (Linux) 🔧
3. 从 NVIDIA 安装 cuDNN，并将环境变量配置到 `PATH` 或 `LD_LIBRARY_PATH` (Linux) 🛠️
4. 从 NVIDIA 安装 TensorRT，并将环境变量配置到 `PATH` 或 `LD_LIBRARY_PATH` (Linux) ⚙️

你可以参考这个博客页面了解在 Windows 中使用 GPU：[关于PaddleSharp GPU使用 常见问题记录](https://www.cnblogs.com/cuichaohui/p/15766519.html) 📝

如果你正在使用 Linux，你需要根据 [docker 构建脚本](./build/docker/dotnet6sdk-paddle/Dockerfile) 编译自己的 OpenCvSharp4 环境，并完成 CUDA/cuDNN/TensorRT 的配置任务。🐧

完成这些步骤后，你可以尝试在 paddle device 配置参数中指定 `PaddleDevice.Gpu()`，然后享受性能提升！🎉

## 感谢 & 赞助商 🙏
* 崔亮  https://github.com/cuiliang
* 梁现伟
* 深圳-钱文松
* iNeuOS工业互联网操作系统：http://www.ineuos.net

## 联系方式 📞
C#/.NET 计算机视觉技术交流群：**579060605**
![](./assets/qq.png)

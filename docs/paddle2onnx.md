# Sdcb.Paddle2Onnx

Sdcb.Paddle2Onnx is an open-source project that provides C#/.NET wrapper for Paddle2Onnx, a tool to convert PaddlePaddle models to ONNX models. It allows users to convert PaddlePaddle models to ONNX models and use them in various deep learning frameworks.

*NOTE*: This project is only supported in Windows.

## Installation

To use Sdcb.Paddle2Onnx, you need to install .NET standard 2.0 or later.

To install Sdcb.Paddle2Onnx, you can use the following command in the Package Manager Console:

```powershell
Install-Package Sdcb.Paddle2Onnx
Install-Package Sdcb.Paddle2Onnx.runtime.win64
```

### Packages

| NuGet Package 💼                | Version 📌                                                                                                                                | Description 📚                    |
| ------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------- |
| Sdcb.Paddle2Onnx               | [![NuGet](https://img.shields.io/nuget/v/Sdcb.Paddle2Onnx.svg)](https://nuget.org/packages/Sdcb.Paddle2Onnx)                             | Paddle2Onnx C++ API .NET binding |
| Sdcb.Paddle2Onnx.runtime.win64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.Paddle2Onnx.runtime.win64.svg)](https://nuget.org/packages/Sdcb.Paddle2Onnx.runtime.win64) | wi-x64 Paddle2Onnx runtime       |

## Usage

### Checking If a Model Can Be Converted

To check if a PaddlePaddle model can be converted to an ONNX model, you can use the `Paddle2OnnxConverter.CanConvert` method. You can provide either the file paths or the byte arrays of the model and the parameter files.

```csharp
string modelFile = "model.pdmodel";
string paramsFile = "model.pdiparams";
bool can = Paddle2OnnxConverter.CanConvert(modelFile, paramsFile);
```

### Converting PaddlePaddle Models to ONNX Models

To convert a PaddlePaddle model to an ONNX model, you can use the `Paddle2OnnxConverter.ConvertToOnnx` method. You can provide either the file paths or the byte arrays of the model and the parameter files.

```csharp
byte[] modelBuffer = File.ReadAllBytes("model.pdmodel");
byte[] paramsBuffer = File.ReadAllBytes("model.pdiparams");
byte[] onnxModel = Paddle2OnnxConverter.ConvertToOnnx(modelBuffer, paramsBuffer);
```

### Describing a PaddlePaddle Model

To describe a PaddlePaddle model, you can use the `Paddle2OnnxConverter.DescribePaddleModel` method. You need to provide the byte array of the model file.

```csharp
byte[] modelBuffer = File.ReadAllBytes("model.pdmodel");
PaddleModelInfo info = Paddle2OnnxConverter.DescribePaddleModel(modelBuffer);
Console.WriteLine($"Input shapes: {string.Join(", ", info.InputShapes)}");
Console.WriteLine($"Output shapes: {string.Join(", ", info.OutputShapes)}");
```

### Describing an ONNX Model

To describe an ONNX model, you can use the `Paddle2OnnxConverter.DescribeOnnxModel` method. You need to provide the byte array of the ONNX model.

```csharp
OnnxModelInfo info = Paddle2OnnxConverter.DescribeOnnxModel(onnxModel);
Console.WriteLine("Input shapes:");
Console.WriteLine(string.Join("\n", info.InputShapes));
Console.WriteLine():
Console.WriteLine("Output shapes:");
Console.WriteLine(string.Join("\n", info.OutputShapes));
```

### Using Custom Operators

To use custom operators when converting a PaddlePaddle model to an ONNX model, you can provide an array of `CustomOp` objects to the `Paddle2OnnxConverter.CanConvert` method.

```csharp
CustomOp[] ops = new[]
{
    new CustomOp("fc", "FC"),
    new CustomOp("softmax", "Softmax")
};

bool can = Paddle2OnnxConverter.CanConvert(modelFile, paramsFile, customOps: ops);
```

### Using a Different Backend

To use a different backend when converting a PaddlePaddle model to an ONNX model, you can provide the name of the backend to the `Paddle2OnnxConverter.CanConvert` method. Currently, the only supported backend is TensorRT.

```csharp
bool can = Paddle2OnnxConverter.CanConvert(modelFile, paramsFile, deployBackend: "tensorrt");
```
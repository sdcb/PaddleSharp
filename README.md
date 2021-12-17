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
```
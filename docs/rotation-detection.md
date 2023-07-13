# Sdcb.RotationDetector usage

Sdcb.RotationDetector is part of [PaddleClas](https://github.com/PaddlePaddle/PaddleClas), it's based on [PULC Classification Model of Text Image Orientation](https://github.com/PaddlePaddle/PaddleClas/blob/release/2.5/docs/en/PULC/PULC_text_image_orientation_en.md)

## NuGet Packages:

### Rotation Detection packages (part of PaddleCls) 🔄
| NuGet Package 💼       | Version 📌                                                                                                              | Description 📚                                             |
| --------------------- | ---------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------- |
| Sdcb.RotationDetector | [![NuGet](https://img.shields.io/nuget/v/Sdcb.RotationDetector.svg)](https://nuget.org/packages/Sdcb.RotationDetector) | RotationDetector library(based on Sdcb.PaddleInference) ⚙️ |

## Usage:
1. Install NuGet Packages:
   ```
   Sdcb.PaddleInference.runtime.win64.mkl
   Sdcb.RotationDetector
   OpenCvSharp4.runtime.win
   ```

2. Using following C# code to get result:
   ```csharp
   using PaddleRotationDetector detector = new PaddleRotationDetector(RotationDetectionModel.EmbeddedDefault);
   using Mat src = Cv2.ImRead(@"C:\your-local-file-here.jpg");
   RotationResult r = detector.Run(src);
   Console.WriteLine(r.Rotation); // _0, _90, _180, _270

   // Restore to non-rotated:
   // r.RestoreRotationInPlace(src);
   ```

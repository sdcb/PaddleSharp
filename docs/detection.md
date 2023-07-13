# Sdcb.PaddleDetection

## NuGet packages 🎯

| NuGet Package 💼      | Version 📌                                                                                                            | Description 📚                                            |
| -------------------- | -------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------- |
| Sdcb.PaddleDetection | [![NuGet](https://img.shields.io/nuget/v/Sdcb.PaddleDetection.svg)](https://nuget.org/packages/Sdcb.PaddleDetection) | PaddleDetection library(based on Sdcb.PaddleInference) ⚙️ |

## Usage

1. Get your PaddleDetection models ready, or just download a model from [my exported models](https://pan.baidu.com/s/1pYuvUIU3HyEpu4gG0qSeMg?pwd=r90c) (which is exported from official PaddlePaddle [PicoDet](https://github.com/PaddlePaddle/PaddleDetection/tree/release/2.3/configs/picodet) and [PPYolo](https://github.com/PaddlePaddle/PaddleDetection/tree/release/2.3/configs/ppyolo)).
2. Check your models
   
   Note: PaddleDetection inference model **must** should like following format:
   ```
   model_dir
   -> infer_cfg.yml
   -> model.pdiparams
   -> model.pdiparams.info
   -> model.pdmodel
   ```
   If your model filename is `xxx.pdparams`, you must export to inference model, you can refer to [this document](https://github.com/PaddlePaddle/PaddleDetection/blob/release/2.3/deploy/README_en.md#11-the-export-model) to export a inference model.
3. Install NuGet Packages:
   ```
   Sdcb.PaddleInference
   Sdcb.PaddleInference.runtime.win64.mkl
   Sdcb.PaddleDetection
   OpenCvSharp4
   OpenCvSharp4.runtime.win
   ```
4. Using following C# code to get result:
   ```csharp
   string modelDir = DetectionLocalModel.PicoDets.L_416_coco.Directory; // your model directory here
   using (PaddleDetector detector = new PaddleDetector(modelDir, Path.Combine(modelDir, "infer_cfg.yml"), PaddleDevice.Mkldnn()))
   using (VideoCapture vc = new VideoCapture())
   {
       vc.Open(0);
       while (true)
       {
           using (Mat mat = vc.RetrieveMat())
           {
               DetectionResult[] results = detector.Run(mat);
   
               using (Mat dest = PaddleDetector.Visualize(mat, 
                   results.Where(x => x.Confidence > 0.5f), 
                   detector.Config.LabelList.Length))
               {
                   Cv2.ImShow("test", dest);
               }
           }
           Cv2.WaitKey(1);
       }
   }
   ```

running effect(for image):
![image](https://user-images.githubusercontent.com/1317141/222453236-fc7c25b8-c5ca-41b7-bf93-ac2708bb8c62.png)

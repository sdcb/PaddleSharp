# Usage

## How to migrate previous old version to latest 2.6.0.1?
![image](https://user-images.githubusercontent.com/1317141/206610787-4d31057f-9d7f-4235-a2c4-433322e21bb6.png)

## Windows(Local model): Detection and Recognition(All)
1. Install NuGet Packages:
   ```
   Sdcb.PaddleOCR.Models.LocalV3
   Sdcb.PaddleInference.runtime.win64.mkl
   OpenCvSharp4.runtime.win
   ```

2. Using following C# code to get result:
   ```csharp
   FullOcrModel model = LocalFullModels.ChineseV3;
   
   byte[] sampleImageData;
   string sampleImageUrl = @"https://www.tp-link.com.cn/content/images2017/gallery/4288_1920.jpg";
   using (HttpClient http = new HttpClient())
   {
       Console.WriteLine("Download sample image from: " + sampleImageUrl);
       sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
   }
   
   using (PaddleOcrAll all = new PaddleOcrAll(model)
   {
       AllowRotateDetection = true, /* 允许识别有角度的文字 */ 
       Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
   })
   {
       // Load local file by following code:
       // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
       using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
       {
           PaddleOcrResult result = all.Run(src);
           Console.WriteLine("Detected all texts: \n" + result.Text);
           foreach (PaddleOcrResultRegion region in result.Regions)
           {
               Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
           }
       }
   }
   ```

## Windows(Online model): Detection and Recognition(All)
1. Install NuGet Packages:
   ```
   Sdcb.PaddleOCR.Models.Online
   Sdcb.PaddleInference.runtime.win64.mkl
   OpenCvSharp4.runtime.win
   ```

2. Using following C# code to get result:
   ```csharp
   FullOcrModel model = await OnlineFullModels.EnglishV3.DownloadAsync();
   
   byte[] sampleImageData;
   string sampleImageUrl = @"https://www.tp-link.com.cn/content/images2017/gallery/4288_1920.jpg";
   using (HttpClient http = new HttpClient())
   {
       Console.WriteLine("Download sample image from: " + sampleImageUrl);
       sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
   }
   
   using (PaddleOcrAll all = new PaddleOcrAll(model)
   {
       AllowRotateDetection = true, /* 允许识别有角度的文字 */ 
       Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
   })
   {
       // Load local file by following code:
       // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
       using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
       {
           PaddleOcrResult result = all.Run(src);
           Console.WriteLine("Detected all texts: \n" + result.Text);
           foreach (PaddleOcrResultRegion region in result.Regions)
           {
               Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
           }
       }
   }
   ```

## Linux(Ubuntu 20.04): Detection and Recognition(All)
1. Use `sdflysha/sdflysha/dotnet6-paddle:2.3.0-ubuntu20` to replace `mcr.microsoft.com/dotnet/aspnet:6.0` in `Dockerfile` as docker base image.

The build steps for `sdflysha/dotnet6-paddle:2.3.0-ubuntu20` was described [here](./build/docker/dotnet6-paddle/Dockerfile).

2. Install NuGet Packages:
```ps
dotnet add package Sdcb.PaddleOCR.Models.LocalV3
```

Please aware in `Linux`, the native binding library is not required, instead, you should compile your own `OpenCV`/`PaddleInference` library, or just use the `Docker` image.

3. write following C# code to get result(also can be exactly the same as windows):
```csharp
FullOcrModel model = LocalFullModels.ChineseV3;
using (PaddleOcrAll all = new PaddleOcrAll(model))
// Load in-memory data by following code:
// using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
using (Mat src = Cv2.ImRead(@"/app/test.jpg"))
{
    Console.WriteLine(all.Run(src).Text);
}
```

## Detection Only
```csharp
// Install following packages:
// Sdcb.PaddleOCR.Models.LocalV3
// Sdcb.PaddleInference.runtime.win64.mkl (required in Windows, linux using docker)
// OpenCvSharp4.runtime.win (required in Windows, linux using docker)
byte[] sampleImageData;
string sampleImageUrl = @"https://www.tp-link.com.cn/content/images2017/gallery/4288_1920.jpg";
using (HttpClient http = new HttpClient())
{
    Console.WriteLine("Download sample image from: " + sampleImageUrl);
    sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
}

using (PaddleOcrDetector detector = new PaddleOcrDetector(LocalDetectionModel.ChineseV3))
using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
{
    RotatedRect[] rects = detector.Run(src);
    using (Mat visualized = PaddleOcrDetector.Visualize(src, rects, Scalar.Red, thickness: 2))
    {
        string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "output.jpg");
        Console.WriteLine("OutputFile: " + outputFile);
        visualized.ImWrite(outputFile);
    }
}
```

# Language supports

Please refer to https://github.com/PaddlePaddle/PaddleOCR/blob/release/2.5/doc/doc_en/models_list_en.md to check language support models.

Just replace the `.ChineseV3` in demo code with your speicific language, then you can use the language.

# Technical details

There is 3 steps to do OCR:
1. Detection - Detect text's position, angle and area (`PaddleOCRDetector`)
2. Classification - Determin whether text should rotate 180 degreee.
3. Recognization - Recognize the area into text

# Optimize parameters and performance hints
## PaddleOcrAll.Enable180Classification
Default value: `false`

This directly effect the step 2, set to `false` can skip this step, which will unable to detect text from right to left(which should be acceptable because most text direction is from left to right).

Close this option can make the full process about  `~10%` faster.


## PaddleOcrAll.AllowRotateDetection
Default value: `true`

This allows detect any rotated texts. If your subject is 0 degree text (like scaned table or screenshot), you can set this parameter to `false`, which will improve OCR accurancy and little bit performance.


## PaddleOcrAll.Detector.MaxSize
Default value: `2048`

This effect the the max size of step #1, lower this value can improve performance and reduce memory usage, but will also lower the accurancy.

You can also set this value to `null`, in that case, images will not scale-down to detect, performance will drop and memory will high, but should able to get better accurancy.

## PaddleConfig.Defaults.UseGpu
Default value: `false`

If you wants to use GPU, you should refer to FAQ `How to enable GPU?` section, CUDA/cuDNN/TensorRT need to be installed manually.

## How can I improve performance?
Please review the `Technical details` section and read the `Optimize parameters and performance hints` section, or UseGpu.

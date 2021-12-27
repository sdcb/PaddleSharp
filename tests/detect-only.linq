<Query Kind="Statements">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleInference</NuGetReference>
  <NuGetReference>Sdcb.PaddleInference.runtime.win64.mkl</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR</NuGetReference>
  <NuGetReference>Sdcb.PaddleOCR.KnownModels</NuGetReference>
  <Namespace>OpenCvSharp</Namespace>
  <Namespace>Sdcb.PaddleOCR</Namespace>
  <Namespace>Sdcb.PaddleOCR.KnownModels</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

// Install following packages:
// Sdcb.PaddleInference
// Sdcb.PaddleInference.runtime.win64.mkl (required in Windows)
// Sdcb.PaddleOCR
// Sdcb.PaddleOCR.KnownModels
// OpenCvSharp4
// OpenCvSharp4.runtime.win (required in Windows)
// OpenCvSharp4.runtime.linux18.04 (required in Linux)
byte[] sampleImageData;
string sampleImageUrl = @"https://www.tp-link.com.cn/content/images/detail/2164/TL-XDR5450易展Turbo版-3840px_03.jpg";
using (HttpClient http = new HttpClient())
{
    Console.WriteLine("Download sample image from: " + sampleImageUrl);
    sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
}

OCRModel model = KnownOCRModel.PPOcrV2;
await model.EnsureAll();
using (PaddleOcrDetector detector = new PaddleOcrDetector(model.DetectionDirectory))
using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
{
    RotatedRect[] rects = detector.Run(src);
    using (Mat visualized = PaddleOcrDetector.Visualize(src, rects, Scalar.Red, thickness: 2))
    {
        string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "output.jpg");
        visualized.ImWrite(outputFile);
    }
}
<Query Kind="Statements">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference>Sdcb.PaddleInference</NuGetReference>
  <NuGetReference>Sdcb.PaddleInference.runtime.win64.mkl</NuGetReference>
  <NuGetReference>Sdcb.PaddleOCR</NuGetReference>
  <NuGetReference>Sdcb.PaddleOCR.KnownModels</NuGetReference>
  <Namespace>OpenCvSharp</Namespace>
  <Namespace>Sdcb.PaddleInference</Namespace>
  <Namespace>Sdcb.PaddleOCR</Namespace>
  <Namespace>Sdcb.PaddleOCR.KnownModels</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
  <Namespace>System.Net.Http</Namespace>
</Query>

OCRModel model = KnownOCRModel.PPOcrV2;
await model.EnsureAll();

byte[] sampleImageData;
string sampleImageUrl = @"https://www.tp-link.com.cn/content/images/detail/2164/TL-XDR5450易展Turbo版-3840px_03.jpg";
using (HttpClient http = new HttpClient())
{
    Console.WriteLine("Download sample image from: " + sampleImageUrl);
    sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
}

using (PaddleOcrAll all = new PaddleOcrAll(model.RootDirectory, model.KeyPath))
{
    // Load local file by following code:
    // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
    using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
    {
        PaddleOcrResult result = all.Run(src);
        Console.WriteLine("Detected all texts: \n" + result.Text);
        foreach (PaddleOcrResultRegion region in result.Regions)
        {
            Console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize: {region.Rect.Size}, Angle: {region.Rect.Angle}");
        }
    }
}
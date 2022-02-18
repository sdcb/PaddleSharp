<Query Kind="Statements">
  <NuGetReference>OpenCvSharp4</NuGetReference>
  <NuGetReference>OpenCvSharp4.runtime.win</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleInference</NuGetReference>
  <NuGetReference>Sdcb.PaddleInference.runtime.win64.mkl</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR</NuGetReference>
  <NuGetReference Prerelease="true">Sdcb.PaddleOCR.KnownModels</NuGetReference>
  <Namespace>OpenCvSharp</Namespace>
  <Namespace>Sdcb.PaddleInference</Namespace>
  <Namespace>Sdcb.PaddleOCR</Namespace>
  <Namespace>Sdcb.PaddleOCR.KnownModels</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

IOCRModel model = await KnownOCRModel.PPOcrV2.EnsureAll(QueryCancelToken);
PaddleConfig.Defaults.UseGpu = false;
PaddleConfig.Defaults.UseMkldnn = true;

using Mat src = Cv2.ImRead(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "xdr5480.jpg"));
//using Mat src = Cv2.ImDecode(GetClipboardImage(), ImreadModes.Color);

using PaddleOcrAll all = new(model.DetectionDirectory, model.ClassifierDirectory, model.RecognitionDirectory, model.KeyPath)
{
   Enable180Classification = true,
   AllowRotateDetection = true,
};
//var predictor = PaddleConfig.FromModelDir(model.DetectionDirectory);
//predictor.Dump();

using Mat scaled = src.Resize(Size.Zero, 1, 1);
var sw = Stopwatch.StartNew();
PaddleOcrResult result = all.Run(scaled);
sw.ElapsedMilliseconds.Dump("elapsed");

Util.HorizontalRun(false, Image(PaddleOcrDetector.Visualize(scaled, result.Regions.Select(x => x.Rect).ToArray(), Scalar.Red, thickness: 2)), result.Regions
	.OrderBy(x => x.Rect.Center.Y)
	.Select(x => x.Text)).Dump();

byte[] GetClipboardImage()
{
	using var ms = new MemoryStream();
	Clipboard.GetImage().Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
	return ms.ToArray();
}

object Image(Mat src) => Util.Image(src.ToBytes(), Util.ScaleMode.Unscaled);